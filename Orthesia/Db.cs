using Discord;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Orthesia
{
    public class Db
    {
        public Db()
        {
            R = RethinkDB.R;
        }

        public async Task InitAsync(string dbName = "Orthesia")
        {
            this.dbName = dbName;
            conn = await R.Connection().ConnectAsync();
            if (!await R.DbList().Contains(dbName).RunAsync<bool>(conn))
                await R.DbCreate(dbName).RunAsync(conn);
            if (!await R.Db(dbName).TableList().Contains("Supports").RunAsync<bool>(conn))
                await R.Db(dbName).TableCreate("Supports").RunAsync(conn);
            if (!await R.Db(dbName).TableList().Contains("Users").RunAsync<bool>(conn))
                await R.Db(dbName).TableCreate("Users").RunAsync(conn);
        }

        public async Task<bool> IsLastMoreThan10Minutes(ulong userId)
        {
            string userIdStr = userId.ToString();
            if (await R.Db(dbName).Table("Users").GetAll(userIdStr).Count().Eq(0).RunAsync<bool>(conn))
                return true;
            dynamic json = await R.Db(dbName).Table("Users").Get(userId.ToString()).RunAsync(conn);
            return DateTime.ParseExact((string)json.Timer, "yyMMddHHmmss", CultureInfo.InvariantCulture).AddMinutes(5).CompareTo(DateTime.Now) < 0;
        }

        public async Task DeleteChannel(ulong chanId)
        {
            foreach (var json in await R.Db(dbName).Table("Supports").RunAsync(conn))
            {
                if (json.Channel == chanId.ToString())
                {
                    await UpdateTimer((string)json.id);
                    return;
                }
            }
        }

        public async Task UpdateTimer(ulong userId)
            => await UpdateTimer(userId.ToString());

        public async Task UpdateTimer(string userIdStr)
        {
            if (await R.Db(dbName).Table("Users").GetAll(userIdStr).Count().Eq(0).RunAsync<bool>(conn))
                await R.Db(dbName).Table("Users").Insert(R.HashMap("id", userIdStr)
                    .With("Timer", DateTime.Now.ToString("yyMMddHHmmss"))
                    ).RunAsync(conn);
            else
                await R.Db(dbName).Table("Users").Update(R.HashMap("id", userIdStr)
                    .With("Timer", DateTime.Now.ToString("yyMMddHHmmss"))
                    ).RunAsync(conn);
        }

        public async Task AddTicket(ulong userId, ulong chanId, ulong introMsgId, ulong menuMsgId, string randomId, ulong chanFromId)
        {
            string userIdStr = userId.ToString();
            await R.Db(dbName).Table("Supports").Insert(R.HashMap("id", userIdStr)
                .With("Channel", chanId.ToString())
                .With("IntroMsg", introMsgId.ToString())
                .With("MenuMsg", menuMsgId.ToString())
                .With("RandomId", randomId)
                .With("CloseMsg", 0)
                .With("ChanFromId", chanFromId.ToString())
                ).RunAsync(conn);
        }

        public async Task DeleteTicket(ulong userId, IGuild guild)
        {
            string userIdStr = userId.ToString();
            if (await R.Db(dbName).Table("Supports").GetAll(userIdStr).Count().Eq(0).RunAsync<bool>(conn))
                return;
            dynamic json = await R.Db(dbName).Table("Supports").Get(userId.ToString()).RunAsync(conn);
            await (await guild.GetTextChannelAsync(ulong.Parse((string)json.Channel))).DeleteAsync();
            await (await (await guild.GetTextChannelAsync(ulong.Parse((string)json.ChanFromId))).GetMessageAsync(ulong.Parse((string)json.IntroMsg))).DeleteAsync();
            await R.Db(dbName).Table("Supports").Get(userIdStr).Delete().RunAsync(conn);
            await UpdateTimer(userId);
        }

        public async Task<bool> DoesTicketExist(ulong userId, IGuild guild)
        {
            string userIdStr = userId.ToString();
            if (await R.Db(dbName).Table("Supports").GetAll(userIdStr).Count().Eq(0).RunAsync<bool>(conn))
                return false;
            dynamic json = await R.Db(dbName).Table("Supports").Get(userId.ToString()).RunAsync(conn);
            if (await guild.GetTextChannelAsync(ulong.Parse((string)json.Channel)) == null)
            {
                await R.Db(dbName).Table("Supports").Get(userIdStr).Delete().RunAsync(conn);
                return false;
            }
            return true;
        }

        public async Task<IMessage> GetCloseMessage(ulong userId, IMessageChannel chan)
        {
            string closeMsg = await GetCloseMessageId(userId, chan);
            if (closeMsg == "0")
                return null;
            return await chan.GetMessageAsync(ulong.Parse(closeMsg));
        }

        public async Task<string> GetCloseMessageId(ulong userId, IMessageChannel chan)
        {
            string userIdStr = userId.ToString();
            if (await R.Db(dbName).Table("Supports").GetAll(userIdStr).Count().Eq(0).RunAsync<bool>(conn))
                return "0";
            dynamic json = await R.Db(dbName).Table("Supports").Get(userId.ToString()).RunAsync(conn);
            return json.CloseMsg;
        }

        public async Task<string> GetMenuMessageId(ulong userId, IMessageChannel chan)
        {
            string userIdStr = userId.ToString();
            if (await R.Db(dbName).Table("Supports").GetAll(userIdStr).Count().Eq(0).RunAsync<bool>(conn))
                return "0";
            dynamic json = await R.Db(dbName).Table("Supports").Get(userId.ToString()).RunAsync(conn);
            return json.MenuMsg;
        }

        public async Task DeleteCloseMsg(ulong userId, IMessageChannel chan)
        {
            IMessage msg = await GetCloseMessage(userId, chan);
            if (msg != null)
            {
                await R.Db(dbName).Table("Supports").Update(R.HashMap("id", userId.ToString())
                    .With("CloseMsg", "0")
                    ).RunAsync(conn);
                await msg.DeleteAsync();
            }
        }

        public async Task UpdateCloseMsg(ulong userId, ulong msgId)
        {
            string userIdStr = userId.ToString();
            await R.Db(dbName).Table("Supports").Update(R.HashMap("id", userIdStr)
                .With("CloseMsg", msgId.ToString())
                ).RunAsync(conn);
        }

        private RethinkDB R;
        private Connection conn;
        private string dbName;
    }
}
