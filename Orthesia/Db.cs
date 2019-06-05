using RethinkDb.Driver;
using RethinkDb.Driver.Net;
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
            if (!await R.Db(dbName).TableList().Contains("Support").RunAsync<bool>(conn))
                await R.Db(dbName).TableCreate("Support").RunAsync(conn);
        }

        private RethinkDB R;
        private Connection conn;
        private string dbName;
    }
}
