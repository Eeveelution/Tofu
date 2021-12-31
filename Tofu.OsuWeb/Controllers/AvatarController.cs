using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Tofu.OsuWeb.Controllers {
    public class AvatarController : Controller {
        [HttpGet]
        [Route("/forum/download.php")]
        public async Task<ActionResult> Index([FromQuery] string avatar) {
            //Path traversal
            if (avatar.Contains(".") || avatar.Contains("/") || avatar.Contains("\\"))
                return this.BadRequest("Sincerly, fuck off.");

            byte[] avatarFile;

            string avatarLocationPng = $"avatars/{avatar}.png";
            string avatarLocationJpg = $"avatars/{avatar}.jpg";

            if (System.IO.File.Exists(avatarLocationJpg)) {
                avatarFile = await System.IO.File.ReadAllBytesAsync(avatarLocationJpg);

                return File(avatarFile, "image/jpeg");
            }

            if (System.IO.File.Exists(avatarLocationPng)) {
                avatarFile = await System.IO.File.ReadAllBytesAsync(avatarLocationPng);

                return File(avatarFile, "image/png");
            }

            return this.NotFound("Avatar not found.");
        }
    }
}
