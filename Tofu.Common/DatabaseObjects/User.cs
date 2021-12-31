using System;
using System.Threading.Tasks;
using EeveeTools.Database;
using EeveeTools.Helpers;
using MySqlConnector;
using Tofu.Bancho.Helpers;
using Tofu.Common;

namespace Tofu.Bancho.DatabaseObjects {
    public class User {
        public long     Id { get; set; }
        public string   Username { get; set; }
        public string   Password { get; set; }
        public long     Privileges { get; set; }
        public string   EmailAdress { get; set; }
        public DateTime SilencedUntil { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastActivity { get; set; }
        public bool     Banned { get; set; }
        public string   Location { get; set; }
        public string   Userpage { get; set; }

        public UserStats OsuStats;
        public UserStats TaikoStats;
        public UserStats CatchStats;
        public UserStats ManiaStats;

        public User() {

        }

        public static (bool success, string error, User resultUser) Create(string username, string password, string email = "", bool passwordIsMd5 = false) {
            try {
                //Check for duplicate username
                const string duplicateUsernameCheck = "SELECT COUNT(*) AS 'count' FROM tofu.users WHERE users.username = @username";

                MySqlParameter[] duplicateUsernameParams = new[] {
                    new MySqlParameter("@username", username)
                };

                long usernameCount = (long) DatabaseHelper.MySqlQueryOne(CommonGlobal.DatabaseContext, duplicateUsernameCheck, duplicateUsernameParams)["count"];

                if (usernameCount != 0)
                    return (false, "Username already exists!", null);

                //Check for duplicate email/username
                const string duplicateEmailCheck = "SELECT COUNT(*) AS 'count' FROM tofu.users WHERE users.email_adress = @email";

                MySqlParameter[] duplicateEmailParams = new[] {
                    new MySqlParameter("@email", email)
                };

                long emailCount = (long) DatabaseHelper.MySqlQueryOne(CommonGlobal.DatabaseContext, duplicateEmailCheck, duplicateEmailParams)["count"];

                if (emailCount != 0)
                    return (false, "This mail adress has already been registered!", null);

                if (!passwordIsMd5)
                    password = CryptoHelper.HashMd5(password);

                int privileges = 0;

                const string insertUserSql = "INSERT INTO tofu.users (username, password, privileges, email_adress, silenced_until, user_page) VALUES (@username, @password, @privileges, @email, CURRENT_TIMESTAMP(), @userpage)";

                MySqlParameter[] insertUserParams = new[] {
                    new MySqlParameter("@username", username), new MySqlParameter("@password", password), new MySqlParameter("@privileges", privileges), new MySqlParameter("@email", email), new MySqlParameter("@userpage", ""),
                };

                MySqlDatabaseHandler.MySqlNonQuery(CommonGlobal.DatabaseContext, insertUserSql, insertUserParams);

                const string getUserIdSql = "SELECT users.id FROM tofu.users WHERE users.username = @username";

                //We can just reuse the params from earlier, no need to create em again
                long userId = (long) DatabaseHelper.MySqlQueryOne(CommonGlobal.DatabaseContext, getUserIdSql, duplicateUsernameParams)["id"];

                const string insertStatsStatsSql = "INSERT INTO tofu.stats (user_id, mode) VALUES (@userid, 0), (@userid, 1), (@userid, 2), (@userid, 3)";

                MySqlParameter[] insertStatsParams = new[] {
                    new MySqlParameter("@userid", userId)
                };

                MySqlDatabaseHandler.MySqlNonQuery(CommonGlobal.DatabaseContext, insertStatsStatsSql, insertStatsParams);

                return (true, "Registered successfully!", FromDatabase(userId));
            }
            catch (Exception e) {
#if DEBUG
                return (false, e.ToString(), null);
#else
                return (false, "A error occured internally during registration.", null);
#endif
            }
        }

        public static async Task<(bool success, string error, User resultUser)> CreateAsync(string username, string password, string email = "", bool passwordIsMd5 = false) {
            try {
                //Check for duplicate username
                const string duplicateUsernameCheck = "SELECT COUNT(*) AS 'count' FROM tofu.users WHERE users.username = @username";

                MySqlParameter[] duplicateUsernameParams = new[] {
                    new MySqlParameter("@username", username)
                };

                long usernameCount = (long) (await DatabaseHelper.MySqlQueryOneAsync(CommonGlobal.DatabaseContext, duplicateUsernameCheck, duplicateUsernameParams))["count"];

                if (usernameCount != 0)
                    return (false, "Username already exists!", null);

                //Check for duplicate email/username
                const string duplicateEmailCheck = "SELECT COUNT(*) AS 'count' FROM tofu.users WHERE users.email_adress = @email";

                MySqlParameter[] duplicateEmailParams = new[] {
                    new MySqlParameter("@email", email)
                };

                long emailCount = (long) (await DatabaseHelper.MySqlQueryOneAsync(CommonGlobal.DatabaseContext, duplicateEmailCheck, duplicateEmailParams))["count"];

                if (emailCount != 0)
                    return (false, "This mail adress has already been registered!", null);

                if (!passwordIsMd5)
                    password = CryptoHelper.HashMd5(password);

                int privileges = 0;

                const string insertUserSql = "INSERT INTO tofu.users (username, password, privileges, email_adress, silenced_until, user_page) VALUES (@username, @password, @privileges, @email, CURRENT_TIMESTAMP(), @userpage)";

                MySqlParameter[] insertUserParams = new[] {
                    new MySqlParameter("@username", username), new MySqlParameter("@password", password), new MySqlParameter("@privileges", privileges), new MySqlParameter("@email", email), new MySqlParameter("@userpage", ""),
                };

                await MySqlDatabaseHandler.MySqlNonQueryAsync(CommonGlobal.DatabaseContext, insertUserSql, insertUserParams);

                const string getUserIdSql = "SELECT users.id FROM tofu.users WHERE users.username = @username";

                //We can just reuse the params from earlier, no need to create em again
                long userId = (long) (await DatabaseHelper.MySqlQueryOneAsync(CommonGlobal.DatabaseContext, getUserIdSql, duplicateUsernameParams))["id"];

                const string insertStatsStatsSql = "INSERT INTO tofu.stats (user_id, mode) VALUES (@userid, 0), (@userid, 1), (@userid, 2), (@userid, 3)";

                MySqlParameter[] insertStatsParams = new[] {
                    new MySqlParameter("@userid", userId)
                };

                await MySqlDatabaseHandler.MySqlNonQueryAsync(CommonGlobal.DatabaseContext, insertStatsStatsSql, insertStatsParams);

                return (true, "Registered successfully!", FromDatabase(userId));
            }
            catch (Exception e) {
#if DEBUG
                return (false, e.ToString(), null);
#else
                return (false, "A error occured internally during registration.", null);
#endif
            }
        }

        public static User FromDatabase(long userId) {
            try {
                User user = new User();

                const string userFetchSql = "SELECT * FROM tofu.users WHERE id=@userid";

                MySqlParameter[] userFetchParams = new[] {
                    new MySqlParameter("@userid", userId)
                };

                var result = DatabaseHelper.MySqlQueryOne(CommonGlobal.DatabaseContext, userFetchSql, userFetchParams);

                if (result == null)
                    return null;

                user.MapDatabaseResults(result);

                return user;
            }
            catch {
                return null;
            }
        }

        public static async Task<User> FromDatabaseAsync(long userId) {
            try {
                User user = new User();

                const string userFetchSql = "SELECT * FROM tofu.users WHERE id=@userid";

                MySqlParameter[] userFetchParams = new[] {
                    new MySqlParameter("@userid", userId)
                };

                var result = await DatabaseHelper.MySqlQueryOneAsync(CommonGlobal.DatabaseContext, userFetchSql, userFetchParams);

                if (result == null)
                    return null;

                user.MapDatabaseResults(result);

                return user;
            }
            catch {
                return null;
            }
        }

        public static User FromDatabase(string username) {
            try {
                User user = new User();

                const string userFetchSql = "SELECT * FROM tofu.users WHERE username=@username";

                MySqlParameter[] userFetchParams = new[] {
                    new MySqlParameter("@username", username)
                };

                var result = DatabaseHelper.MySqlQueryOne(CommonGlobal.DatabaseContext, userFetchSql, userFetchParams);

                if (result == null)
                    return null;

                user.MapDatabaseResults(result);

                return user;
            }
            catch {
                return null;
            }
        }

        public static async Task<User> FromDatabaseAsync(string username) {
            try {
                User user = new User();

                const string userFetchSql = "SELECT * FROM tofu.users WHERE username=@username";

                MySqlParameter[] userFetchParams = new[] {
                    new MySqlParameter("@username", username)
                };

                var result = await DatabaseHelper.MySqlQueryOneAsync(CommonGlobal.DatabaseContext, userFetchSql, userFetchParams);

                if (result == null)
                    return null;

                user.MapDatabaseResults(result);

                return user;
            }
            catch {
                return null;
            }
        }

        public void FetchAllStats() {
            this.FetchStats(0);
            this.FetchStats(1);
            this.FetchStats(2);
            this.FetchStats(3);
        }

        public void FetchStats(byte mode) {
            const string fetchSql = "SELECT * FROM tofu.stats WHERE stats.user_id=@userid AND stats.mode=@mode";

            MySqlParameter[] fetchParams = new [] {
                new MySqlParameter("@userid", this.Id),
                new MySqlParameter("@mode", mode)
            };

            var fetchResults = MySqlDatabaseHandler.MySqlQuery(CommonGlobal.DatabaseContext, fetchSql, fetchParams);

            UserStats stats = new UserStats();
            stats.MapDatabaseResults(fetchResults[0]);

            switch (mode) {
                case 0:
                    this.OsuStats = stats;
                    break;
                case 1:
                    this.TaikoStats = stats;
                    break;
                case 2:
                    this.CatchStats = stats;
                    break;
                case 3:
                    this.ManiaStats = stats;
                    break;
            }
        }

        public async Task FetchStatsAsync(byte mode) {
            const string fetchSql = "SELECT * FROM tofu.stats WHERE stats.user_id=@userid AND stats.mode=@mode";

            MySqlParameter[] fetchParams = new [] {
                new MySqlParameter("@userid", this.Id),
                new MySqlParameter("@mode",   mode)
            };

            var fetchResults = await MySqlDatabaseHandler.MySqlQueryAsync(CommonGlobal.DatabaseContext, fetchSql, fetchParams);

            UserStats stats = new UserStats();
            stats.MapDatabaseResults(fetchResults[0]);

            switch (mode) {
                case 0:
                    this.OsuStats = stats;
                    break;
                case 1:
                    this.TaikoStats = stats;
                    break;
                case 2:
                    this.CatchStats = stats;
                    break;
                case 3:
                    this.ManiaStats = stats;
                    break;
            }
        }
    }
}
