using DiversityRemastered.Player;
using GameNetcodeStuff;
using System;
using UnityEngine;

namespace JLL.API.Compatability
{
    public class DiversityHelper
    {
        public static void DrawFocus(PlayerControllerB player, bool beingUsed, Vector3 pos)
        {
            try
            {
                if (player.TryGetComponent(out PlayerRevamp revamp))
                {
                    player.IsInspectingItem = beingUsed;
                    revamp.forceLook = beingUsed;
                    revamp.forceLookPosition = pos;
                }
            }
            catch (Exception e)
            {
                JLogHelper.LogWarning($"Diversity focus failed.\n{e}");
            }
        }
    }
}
