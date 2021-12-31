using System.Threading.Tasks;
using EeveeTools.Database;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Tofu.Common;
using SystemFile = System.IO.File;

namespace Tofu.OsuWeb.Controllers {
    public class ReplayController : Controller {
        [HttpGet]
        [Route("/web/osu-getreplay.php")]
        public async Task<ActionResult> Index([FromQuery(Name = "c")] int scoreId) {
            string replayPath = $"replays/{scoreId}";

            if (SystemFile.Exists(replayPath)) {
                byte[] replayBytes = await SystemFile.ReadAllBytesAsync(replayPath);

                const string incrementCountSql = "UPDATE tofu.scores SET scores.watch_count = scores.watch_count + 1 WHERE scores.score_id = @scoreid";

                MySqlParameter[] incrementCountParams = new[] {
                    new MySqlParameter("@scoreid", scoreId)
                };

                await MySqlDatabaseHandler.MySqlNonQueryAsync(CommonGlobal.DatabaseContext, incrementCountSql, incrementCountParams);

                return this.File(replayBytes, "application/octet-stream");
            }

            return this.NotFound("Replay not found.");
        }
    }
}
