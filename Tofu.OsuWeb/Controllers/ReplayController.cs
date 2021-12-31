using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Tofu.OsuWeb.Controllers {
    public class ReplayController : Controller {
        [HttpGet]
        [Route("/web/osu-getreplay.php")]
        public async Task<ActionResult> Index([FromQuery] int scoreId) {


            return this.Accepted();
        }
    }
}
