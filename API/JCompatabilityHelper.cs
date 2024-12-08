using BepInEx.Bootstrap;

namespace JLL.API
{
    public class JCompatabilityHelper
    {
        public static ModCompatCheck IsModLoaded = new ModCompatCheck();

        public class ModCompatCheck
        {
            public bool SimpleCommands = false;
            public bool WeatherRegistry = false;
            public bool LLL = false;
            public bool LethalLib = false;
            public bool ReservedSlotCore = false;
            public bool LethalConfig = false;
            public bool MoreCompany = false;
            public bool LittleCompany = false;
            public bool WesleyMoons = false;
            public DiversityModules Diversity = new DiversityModules();
            public class DiversityModules
            {
                public bool Core = false;
                public bool Frailty = false;
                public bool Cutscene = false;
            }
            public StarlancerModules Starlancer = new StarlancerModules();
            public class StarlancerModules
            {
                public bool AIFix = false;
                public bool EnemyEscape = false;
            }
            public bool Mirage = false;
            public bool SellBodies = false;
            public bool FacilityMeltdown = false;

            internal void SetValues()
            {
                foreach (var plugin in Chainloader.PluginInfos)
                {
                    switch (plugin.Key)
                    {
                        case "mrov.WeatherRegistry":
                            IsModLoaded.WeatherRegistry = true;
                            break;
                        case "JacobG5.SimpleCommands":
                            IsModLoaded.SimpleCommands = true;
                            break;
                        case "imabatby.lethallevelloader":
                            IsModLoaded.LLL = true;
                            break;
                        case "evaisa.lethallib":
                            IsModLoaded.LethalLib = true;
                            break;
                        case "FlipMods.ReservedItemSlotCore":
                            IsModLoaded.ReservedSlotCore = true;
                            break;
                        case "ainavt.lc.lethalconfig":
                            IsModLoaded.LethalConfig = true;
                            break;
                        case "Chaos.Diversity":
                            IsModLoaded.Diversity.Core = true;
                            break;
                        case "Chaos.Frailty":
                            IsModLoaded.Diversity.Frailty = true;
                            break;
                        case "Chaos.LCCutscene":
                            IsModLoaded.Diversity.Cutscene = true;
                            break;
                        case "me.swipez.melonloader.morecompany":
                            IsModLoaded.MoreCompany = true;
                            break;
                        case "Toybox.LittleCompany":
                            IsModLoaded.LittleCompany = true;
                            break;
                        case "AudioKnight.StarlancerAIFix":
                            IsModLoaded.Starlancer.AIFix = true;
                            break;
                        case "AudioKnight.StarlancerEnemyEscape":
                            IsModLoaded.Starlancer.EnemyEscape = true;
                            break;
                        case "qwbarch.Mirage":
                            IsModLoaded.Mirage = true;
                            break;
                        case "Entity378.sellbodies":
                            IsModLoaded.SellBodies = true;
                            break;
                        case "me.loaforc.facilitymeltdown":
                            IsModLoaded.FacilityMeltdown = true;
                            break; 
                        case "JacobG5.WesleyMoons":
                            IsModLoaded.WesleyMoons = true;
                            break;
                        default: break;
                    }
                }
            }
        }

        internal static void Init()
        {
            IsModLoaded.SetValues();
        }
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
        }

        public static bool IsLoaded(CachedMods mod)
        {
            return mod switch
            {
                CachedMods.SimpleCommands => IsModLoaded.SimpleCommands,
                CachedMods.WeatherRegistry => IsModLoaded.WeatherRegistry,
                CachedMods.LethalLevelLoader => IsModLoaded.LLL,
                CachedMods.LethalLib => IsModLoaded.LethalLib,
                CachedMods.ReservedSlotCore => IsModLoaded.ReservedSlotCore,
                CachedMods.LethalConfig => IsModLoaded.LethalConfig,
                CachedMods.MoreCompany => IsModLoaded.MoreCompany,
                CachedMods.LittleCompany => IsModLoaded.LittleCompany,
                CachedMods.Diversity => IsModLoaded.Diversity.Core,
                CachedMods.Frailty => IsModLoaded.Diversity.Frailty,
                CachedMods.LCCutscene => IsModLoaded.Diversity.Cutscene,
                CachedMods.StarlancerAIFix => IsModLoaded.Starlancer.AIFix,
                CachedMods.StarlancerEnemyFix => IsModLoaded.Starlancer.EnemyEscape,
                CachedMods.Mirage => IsModLoaded.Mirage,
                CachedMods.SellBodies => IsModLoaded.SellBodies,
                CachedMods.FacilityMeltdown => IsModLoaded.FacilityMeltdown,
                _ => false,
            };
        }

        public static bool IsLoaded(string guid)
        {
            return Chainloader.PluginInfos.ContainsKey(guid);
        }
    }
}
