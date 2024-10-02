using JLL.API;
using JLL.Components.Filters;
using System;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine;

namespace JLL.Components
{
    public class JModConfigGrabber : MonoBehaviour
    {
        public string modAuthor = "";
        public string modName = "";

        private string modGUID;
        private CustomConfigEntry? modEntry;

        [FormerlySerializedAs("checkAllOnAwake")]
        public bool checkAllOnEnable = true;

        [Header("Mod Properties")]
        public PropertyCheck<bool, CheckFilter>[] BoolChecks = new PropertyCheck<bool, CheckFilter>[0];
        public PropertyCheck<int, IntFilter>[] IntChecks = new PropertyCheck<int, IntFilter>[0];
        public PropertyCheck<float, NumericFilter>[] FloatChecks = new PropertyCheck<float, NumericFilter>[0];
        public PropertyCheck<string, NameFilter>[] StringChecks = new PropertyCheck<string, NameFilter>[0];

        [Header("Check All Properties Result")]
        public UnityEvent CheckSuccess = new UnityEvent();
        public UnityEvent CheckFailure = new UnityEvent();

        [Serializable]
        public class PropertyCheck<T, F> where F : JFilterProperty<T>
        {
            public string propertyName;
            [Tooltip("When filter is disabled the Success event gets triggered")]
            public F Filter;
            [FormerlySerializedAs("Event")]
            public UnityEvent<T> Success = new UnityEvent<T>();
            public UnityEvent<T> Failed = new UnityEvent<T>();

            public bool Check(T value)
            {
                JLogHelper.LogInfo($"{propertyName} = {value}");
                if (Filter.shouldCheck && !Filter.CheckValue(value))
                {
                    Failed.Invoke(value);
                    return false;
                }
                Success.Invoke(value);
                return true;
            }
        }

        void Awake()
        {
            if (modEntry == null)
            {
                modGUID = $"{modAuthor}.{modName}";
                if (CustomConfigRegistry.Mods.ContainsKey(modGUID))
                {
                    modEntry = CustomConfigRegistry.Mods[modGUID];
                    modEntry.LogInfo("Found JsonMod!");
                }
                else
                {
                    JLogHelper.LogWarning($"{modGUID} JSON mod could not be found in registry.");
                }
            }
        }

        void OnEnable()
        {
            if (checkAllOnEnable)
            {
                CheckAll();
            }
        }

        public void CheckAll()
        {
            if (modEntry == null) return;

            bool success = true;

            foreach (var check in BoolChecks)
            {
                success &= check.Check(modEntry.GetBool(check.propertyName));
            }

            foreach (var check in IntChecks)
            {
                success &= check.Check(modEntry.GetInt(check.propertyName));
            }

            foreach (var check in FloatChecks)
            {
                success &= check.Check(modEntry.GetFloat(check.propertyName));
            }

            foreach (var check in StringChecks)
            {
                success &= check.Check(modEntry.GetString(check.propertyName));
            }

            if (success)
            {
                CheckSuccess.Invoke();
            }
            else
            {
                CheckFailure.Invoke();
            }
        }

        public void GetPropertyValue(string name)
        {
            if (modEntry == null)
            {
                return;
            }

            if (modEntry.Configs.ContainsKey(name))
            {
                var pair = modEntry.Configs[name];

                switch (pair.Value.type)
                {
                    case CustomConfigEntry.EntryType.Bool:
                        for (int i = 0; i < BoolChecks.Length; i++)
                        {
                            if (BoolChecks[i].propertyName == name)
                            {
                                BoolChecks[i].Check(modEntry.GetBool(name));
                                break;
                            }
                        }
                        break;
                    case CustomConfigEntry.EntryType.Int:
                        for (int i = 0; i < IntChecks.Length; i++)
                        {
                            if (IntChecks[i].propertyName == name)
                            {
                                IntChecks[i].Check(modEntry.GetInt(name));
                                break;
                            }
                        }
                        break;
                    case CustomConfigEntry.EntryType.Float:
                        for (int i = 0; i < FloatChecks.Length; i++)
                        {
                            if (FloatChecks[i].propertyName == name)
                            {
                                FloatChecks[i].Check(modEntry.GetFloat(name));
                                break;
                            }
                        }
                        break;
                    case CustomConfigEntry.EntryType.String:
                        for (int i = 0; i < StringChecks.Length; i++)
                        {
                            if (StringChecks[i].propertyName == name)
                            {
                                StringChecks[i].Check(modEntry.GetString(name));
                                break;
                            }
                        }
                        break;
                }
            }
        }

        public void LogInfo(string message)
        {
            modEntry?.LogInfo(message);
        }

        public void LogWarning(string message)
        {
            modEntry?.LogWarning(message);
        }
    }
}
