using System.Threading;

namespace Tofu.Bancho.Helpers {
    public class ThreadHelper {
        /// <summary>
        /// Quickly Creates a temporary Thread
        /// </summary>
        /// <param name="action"></param>
        public static void SpawnThread(ThreadStart action) {
            new Thread(action).Start();
        }
    }
}
