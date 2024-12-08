using GameNetcodeStuff;
using JLL.API;
using JLL.API.Compatability;
using Unity.Netcode;

namespace JLL.Components.Compats
{
    public class LittleCompanyScaleModifier : NetworkBehaviour, CompatibilityComponent
    {
        public ScaleModificationType ModificationType = ScaleModificationType.Shrinking;
        public float multiplayer = 1f;

        public InteractEvent ScaledSuccess = new InteractEvent();

        public enum ScaleModificationType
        {
            Shrinking = 1,
            Growing = 2,
            Normalizing = 0
        }

        public bool IsModLoaded()
        {
            return JCompatabilityHelper.IsModLoaded.LittleCompany;
        }

        public void Modify(PlayerControllerB target)
        {
            ModifyPlayerServerRpc((int)target.actualClientId);
        }


        [ServerRpc(RequireOwnership = false)]
        private void ModifyPlayerServerRpc(int playerTarget)
        {
            if (!IsModLoaded()) return;
            ModifyPlayerClientRpc(playerTarget);
        }

        [ClientRpc]
        private void ModifyPlayerClientRpc(int playerTarget)
        {
            if (!IsModLoaded()) return;
            PlayerControllerB player = RoundManager.Instance.playersManager.allPlayerScripts[playerTarget];
            LittleCompanyHelper.ApplyModification(player, (int)ModificationType, multiplayer, () => OnSuccess(player));
        }

        private void OnSuccess(PlayerControllerB player)
        {
            ScaledSuccess.Invoke(player);
        }
    }
}
