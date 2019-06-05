using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using System;
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
            if (!await R.Db(dbName).Table("Guilds").GetAll(userIdStr).Count().Eq(0).RunAsync<bool>(conn))
                return false;
            return true;
        }

        public async Task UpdateTimer(ulong userId)
        {
            string userIdStr = userId.ToString();
            if (!await R.Db(dbName).Table("Guilds").GetAll(userIdStr).Count().Eq(0).RunAsync<bool>(conn))
                await R.Db(dbName).Table("Guilds").Insert(R.HashMap("id", userIdStr)
                    .With("Timer", DateTime.Now.ToString("yyMMddHHmmss"))
                    ).RunAsync(conn);
            else
                await R.Db(dbName).Table("Guilds").Update(R.HashMap("id", userIdStr)
                    .With("Timer", DateTime.Now.ToString("yyMMddHHmmss"))
                    ).RunAsync(conn);
        }

        private RethinkDB R;
        private Connection conn;
        private string dbName;
    }
}
