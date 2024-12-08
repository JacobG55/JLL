using GameNetcodeStuff;
using JLL.API;
using JLL.API.Compatability;
using UnityEngine;

namespace JLL.Components.Compats
{
    public class DiversityFocusPoint : MonoBehaviour, CompatibilityComponent
    {
        public float maxDistance = 4f;
        private PlayerControllerB? player = null;
        private bool beingUsed = false;

        public bool IsModLoaded()
        {
            return JCompatabilityHelper.IsModLoaded.Diversity.Core;
        }

        void Update()
        {
            if (player != null && Vector3.Distance(transform.position, player.transform.position) >= maxDistance)
            {
                DrawFocus(player);
            }
        }

        public void DrawFocus(PlayerControllerB player)
        {
            if (IsModLoaded())
            {
                beingUsed = !beingUsed;
                this.player = beingUsed ? player : null;
                DiversityHelper.DrawFocus(player, beingUsed, transform.position);
            }
        }
    }
}
