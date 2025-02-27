using BepInEx.Bootstrap;
using System;

namespace JLL.API
{
    public class JCompatabilityHelper
    {
        public enum CachedMods
        {
            Other = -1,
            SimpleCommands,
            WeatherRegistry,
            LethalLevelLoader,
            LethalLib,
            ReservedSlotCore,
            LethalConfig,
            MoreCompany,
            LittleCompany,
            Diversity,
            Frailty,
            LCCutscene,
            StarlancerAIFix,
            StarlancerEnemyFix,
            Mirage,
            SellBodies,
            FacilityMeltdown,
            WesleyMoons,
            LethalMoonUnlocks
        }

        public static bool IsLoaded(CachedMods mod)
        {
            return mod switch
            {
                CachedMods.SimpleCommands => IsLoaded("JacobG5.SimpleCommands"),
                CachedMods.WeatherRegistry => IsLoaded("mrov.WeatherRegistry"),
                CachedMods.LethalLevelLoader => IsLoaded("imabatby.lethallevelloader"),
                CachedMods.LethalLib => IsLoaded("evaisa.lethallib"),
                CachedMods.ReservedSlotCore => IsLoaded("FlipMods.ReservedItemSlotCore"),
                CachedMods.LethalConfig => IsLoaded("ainavt.lc.lethalconfig"),
                CachedMods.MoreCompany => IsLoaded("me.swipez.melonloader.morecompany"),
                CachedMods.LittleCompany => IsLoaded("Toybox.LittleCompany"),
                CachedMods.Diversity => IsLoaded("Chaos.Diversity"),
                CachedMods.Frailty => IsLoaded("Chaos.Frailty"),
                CachedMods.LCCutscene => IsLoaded("Chaos.LCCutscene"),
                CachedMods.StarlancerAIFix => IsLoaded("AudioKnight.StarlancerAIFix"),
                CachedMods.StarlancerEnemyFix => IsLoaded("AudioKnight.StarlancerEnemyEscape"),
                CachedMods.Mirage => IsLoaded("qwbarch.Mirage"),
                CachedMods.SellBodies => IsLoaded("Entity378.sellbodies"),
                CachedMods.FacilityMeltdown => IsLoaded("me.loaforc.facilitymeltdown"),
                CachedMods.WesleyMoons => IsLoaded("JacobG5.WesleyMoons"),
                CachedMods.LethalMoonUnlocks => IsLoaded("com.xmods.lethalmoonunlocks"),
                _ => false,
            };
        }

        public static bool IsLoaded(string guid, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            foreach (var plugin in Chainloader.PluginInfos)
            {
                if (plugin.Value.Metadata.GUID.Equals(guid, comparison)) return true;
            }
            return false;
        }
    }
}
