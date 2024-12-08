using GameNetcodeStuff;
using LittleCompany.modifications;
using System;

namespace JLL.API.Compatability
{
    public class LittleCompanyHelper
    {
        public static void ApplyModification(PlayerControllerB target, int type, float multiplier = 1f, Action? onComplete = null)
        {
            try
            {
                Modification.ModificationType modType = Modification.ModificationType.Normalizing;
                if (Enum.IsDefined(typeof(Modification.ModificationType), type))
                {
                    modType = (Modification.ModificationType)type;
                }

                if (PlayerModification.CanApplyModificationTo(target, modType, target, multiplier))
                {
                    PlayerModification.ApplyModificationTo(target, modType, target, multiplier, onComplete);
                }
            }
            catch (Exception e)
            {
                JLogHelper.LogWarning($"LittleCompany modification failed.\n{e}");
            }
        }
    }
}
