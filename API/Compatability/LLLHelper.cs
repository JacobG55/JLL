using LethalLevelLoader;

namespace JLL.API.Compatability
{
    public class LLLHelper
    {
        public static void LockExtendedLevel(SelectableLevel level, bool isLocked = true)
        {
            for (int i = 0; i < PatchedContent.ExtendedLevels.Count; i++)
            {
                if (PatchedContent.ExtendedLevels[i].SelectableLevel == level)
                {
                    PatchedContent.ExtendedLevels[i].IsRouteLocked = isLocked;
                    JLL.Instance.mls.LogInfo($"{PatchedContent.ExtendedLevels[i]} Locked: {PatchedContent.ExtendedLevels[i].IsRouteLocked}");
                    return;
                }
            }
        }
        public static void HideExtendedLevel(SelectableLevel level, bool isHidden = true)
        {
            for (int i = 0; i < PatchedContent.ExtendedLevels.Count; i++)
            {
                if (PatchedContent.ExtendedLevels[i].SelectableLevel == level)
                {
                    PatchedContent.ExtendedLevels[i].IsRouteHidden = isHidden;
                    JLL.Instance.mls.LogInfo($"{PatchedContent.ExtendedLevels[i]} Hidden: {PatchedContent.ExtendedLevels[i].IsRouteHidden}");
                    return;
                }
            }
        }
    }
}
