using JLL.API.Compatability;
using JLL.API;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components.Filters
{
    public abstract class JFilter<T> : JFilter
    {
        [Header("JFilter")]
        public UnityEvent<T> filteredEvent = new UnityEvent<T>();
        public UnityEvent<T> failedEvent = new UnityEvent<T>();

        public abstract void Filter(T input);

        public void Result(T input, bool success = false)
        {
            if (success)
            {
                filteredEvent.Invoke(input);
            }
            else
            {
                failedEvent.Invoke(input);
            }
            FilteredResult.Invoke(success);
        }

        public override void FilterDefault()
        {

        }
    }

    public abstract class JFilter : MonoBehaviour
    {
        [HideInInspector]
        public UnityEvent<bool> FilteredResult = new UnityEvent<bool>();
        public abstract void FilterDefault();
    }

    [Serializable]
    public abstract class JFilterProperty<T, F>
    {
        public bool shouldCheck = false;
        public F value;

        public bool Check(T val)
        {
            if (shouldCheck && !CheckValue(val)) return false;
            return true;
        }

        public abstract bool CheckValue(T val);
    }

    [Serializable]
    public abstract class JFilterProperty<T> : JFilterProperty<T, T> { }

    [Serializable]
    public abstract class NumFilter<T> : JFilterProperty<T>
    {
        public FilterOpperand operation = FilterOpperand.EqualTo;

        public enum FilterOpperand
        {
            GreaterThan,
            LessThan,
            EqualTo,
            GreaterThanOrEqual,
            LessThanOrEqual,
            ModuloZero,
            NotEqualTo,
        }

        public bool CheckNum(float check, float current)
        {
            return operation switch
            {
                FilterOpperand.EqualTo => check == current,
                FilterOpperand.GreaterThan => check > current,
                FilterOpperand.LessThan => check < current,
                FilterOpperand.GreaterThanOrEqual => check >= current,
                FilterOpperand.LessThanOrEqual => check <= current,
                FilterOpperand.ModuloZero => check % current == 0,
                FilterOpperand.NotEqualTo => check != current,
                _ => false,
            };
        }
    }

    [Serializable]
    public class NumericFilter : NumFilter<float>
    {
        public override bool CheckValue(float val)
        {
            return CheckNum(val, value);
        }
    }

    [Serializable]
    public class IntFilter : NumFilter<int>
    {
        public override bool CheckValue(int val)
        {
            return CheckNum(val, value);
        }
    }

    [Serializable]
    public class NameFilter : JFilterProperty<string>
    {
        public CompareFilter compareMethod = CompareFilter.Equal;
        public bool caseSensitive = false;

        public enum CompareFilter
        {
            Equal,
            Contains,
            StartsWith,
            EndsWith,
        }

        public override bool CheckValue(string val)
        {
            string compare = caseSensitive ? value : value.ToLower();
            if (!caseSensitive) val = val.ToLower();

            return compareMethod switch
            {
                CompareFilter.Equal => val == compare,
                CompareFilter.Contains => val.Contains(compare),
                CompareFilter.StartsWith => val.StartsWith(compare),
                CompareFilter.EndsWith => val.EndsWith(compare),
                _ => false,
            };
        }
    }

    [Serializable]
    public class CheckFilter : JFilterProperty<bool>
    {
        public override bool CheckValue(bool val)
        {
            return val == value;
        }
    }

    [Serializable]
    public class ItemFilter : JFilterProperty<GrabbableObject, ItemFilter.Properties>
    {
        public override bool CheckValue(GrabbableObject item)
        {
            return value.Check(item);
        }

        [Serializable]
        public class Properties
        {
            public NameFilter itemName = new NameFilter();
            public CheckFilter twoHanded = new CheckFilter();
            public CheckFilter isScrap = new CheckFilter();
            public NumericFilter scrapValue = new NumericFilter() { value = 100, operation = NumFilter<float>.FilterOpperand.GreaterThan };
            public NumericFilter itemCharge = new NumericFilter() { value = 20, operation = NumFilter<float>.FilterOpperand.LessThan };
            public CheckFilter insideFactory = new CheckFilter();
            public string[] contentTags = new string[0];
            public bool mustHaveAllTags = true;

            public bool Check(GrabbableObject item)
            {
                if (item == null) return false;
                if (itemName.shouldCheck && !itemName.CheckValue(item.itemProperties.itemName))
                {
                    return false;
                }
                if (twoHanded.shouldCheck && !twoHanded.CheckValue(item.itemProperties.twoHanded))
                {
                    return false;
                }
                if (isScrap.shouldCheck && !isScrap.CheckValue(item.itemProperties.isScrap))
                {
                    return false;
                }
                if (scrapValue.shouldCheck && !scrapValue.CheckValue(item.scrapValue))
                {
                    return false;
                }
                if (itemCharge.shouldCheck && item.insertedBattery != null && !itemCharge.CheckValue(item.insertedBattery.charge))
                {
                    return false;
                }
                if (insideFactory.shouldCheck && !insideFactory.CheckValue(item.isInFactory))
                {
                    return false;
                }
                if (JCompatabilityHelper.IsModLoaded.LLL && contentTags.Length > 0)
                {
                    if (!LLLHelper.ItemTagFilter(item.itemProperties, contentTags, mustHaveAllTags))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
