using JLL.API.LevelProperties;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class JTerminalController : NetworkBehaviour
    {
        public int transactionAmount = 200;
        public UnityEvent TransactionSuccess = new UnityEvent();
        public UnityEvent TransactionFailed = new UnityEvent();

        public void AwardCredits(int amount)
        {
            AwardCreditsServerRpc(amount);
        }

        [ServerRpc(RequireOwnership = false)]
        private void AwardCreditsServerRpc(int amount)
        {
            AwardCreditsClientRpc(amount);
        }

        [ClientRpc]
        private void AwardCreditsClientRpc(int amount)
        {
            JLevelPropertyRegistry.GetTerminal().groupCredits += amount;
        }

        public void CreditTransaction(int amount)
        {
            CreditTransactionServerRpc(amount);
        }

        [ServerRpc(RequireOwnership = false)]
        private void CreditTransactionServerRpc(int amount)
        {
            CreditTransactionClientRpc(amount);
        }

        [ClientRpc]
        private void CreditTransactionClientRpc(int amount)
        {
            Terminal terminal = JLevelPropertyRegistry.GetTerminal();
            if (terminal.groupCredits >= amount)
            {
                terminal.groupCredits -= amount;
                TransactionSuccess.Invoke();
            }
            else
            {
                TransactionFailed.Invoke();
            }
        }

        public void TriggerSignalTranslator(string message)
        {
            SignalTranslator signalTranslator = UnityEngine.Object.FindObjectOfType<SignalTranslator>();
            if (signalTranslator == null || Time.realtimeSinceStartup - signalTranslator.timeLastUsingSignalTranslator < 8f)
            {
                return;
            }
            TransmitMessage(message);
        }

        public void TransmitMessage(string message)
        {
            HUDManager.Instance.UseSignalTranslatorServerRpc(message);
        }
    }
}
