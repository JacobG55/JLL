using System;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components.Filters
{
    public abstract class JFilter<T> : MonoBehaviour, IJFilter
    {
        [Header("JFilter")]
        public UnityEvent<T> filteredEvent = new UnityEvent<T>();
        public UnityEvent<T> failedEvent = new UnityEvent<T>();

        [HideInInspector]
        private UnityEvent<bool> FilteredResult = new UnityEvent<bool>();

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

        public virtual void FilterDefault()
        {

        }

        public ref UnityEvent<bool> GetResultEvent()
        {
            return ref FilteredResult;
        }
    }

    public interface IJFilter
    {
        public abstract void FilterDefault();
        public abstract ref UnityEvent<bool> GetResultEvent();
    }


    [Serializable]
    public abstract class JFilterProperty<T>
    {
        public bool shouldCheck = false;
        public T value;

        public abstract bool CheckValue(T val);
    }

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
        public bool caseSensitive = false;
        public CompareFilter compareMethod = CompareFilter.Equal;

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
}
