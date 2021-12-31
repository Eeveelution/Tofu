using System.Collections.Generic;
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
    }
}
