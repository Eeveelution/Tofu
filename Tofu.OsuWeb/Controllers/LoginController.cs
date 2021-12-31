using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tofu.Common.DatabaseObjects;

namespace Tofu.OsuWeb.Controllers {
    public class LoginController : Controller {
        [HttpGet]
        [Route("/web/osu-login.php")]
        public async Task<ActionResult> LoginGet([FromQuery] string username, [FromQuery] string password) {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return this.BadRequest("0");

            User user = await Common.DatabaseObjects.User.FromDatabaseAsync(username);

            if (user == null)
                return this.Ok("0");

            if (user.Banned)
                return this.Ok("0");

            if(user.Password != password)
                return this.Ok("0");

            return this.Ok("1");
        }
    }
}
