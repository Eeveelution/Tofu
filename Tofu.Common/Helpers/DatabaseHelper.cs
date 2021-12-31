using System.Collections.Generic;
using System.Threading.Tasks;
using EeveeTools.Database;
using MySqlConnector;

namespace Tofu.Bancho.Helpers {
    public static class DatabaseHelper {
        public static IReadOnlyDictionary<string, object> MySqlQueryOne(DatabaseContext ctx, string query, MySqlParameter[] parameters = null) {
            try {
                return MySqlDatabaseHandler.MySqlQuery(ctx, query, parameters)[0];
            }
            catch {
                return null;
            }
        }

        public static async Task<IReadOnlyDictionary<string, object>> MySqlQueryOneAsync(DatabaseContext ctx, string query, MySqlParameter[] parameters = null) {
            try {
                return (await MySqlDatabaseHandler.MySqlQueryAsync(ctx, query, parameters)) [0];
            }
            catch {
                return null;
            }
        }
    }
}
