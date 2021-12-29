using System.Threading;

namespace Tofu.Bancho.Helpers {
    public class ThreadHelper {
        public static void SpawnThread(ThreadStart action) {
            new Thread(action).Start();
        }
    }
}
