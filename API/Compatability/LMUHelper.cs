using LethalMoonUnlocks;
using System.Linq;

namespace JLL.API.Compatability
{
    public class LMUHelper
    {
        public static void ReleaseLock(string numberlessPlanetName) => UnlockManager.TryReleaseStoryLock(numberlessPlanetName);

        public static void SubscribeStoryLocks(params string[] numberlessPlanetNames)
        {
            UnlockManager.OnCollectStoryLockedMoons += () => numberlessPlanetNames.ToList();
        }
    }
}
