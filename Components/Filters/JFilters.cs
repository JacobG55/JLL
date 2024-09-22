using System;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components.Filters
{
    public abstract class JFilter<T> : MonoBehaviour
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
        }
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
            ModuloZero
        }

        public bool CheckNum(float check, float current)
        {
            switch (operation)
            {
                case FilterOpperand.EqualTo:
                    return check == current;
                case FilterOpperand.GreaterThan:
                    return check > current;
                case FilterOpperand.LessThan:
                    return check < current;
                case FilterOpperand.GreaterThanOrEqual:
                    return check >= current;
                case FilterOpperand.LessThanOrEqual:
                    return check <= current;
                case FilterOpperand.ModuloZero:
                    return check % current == 0;
                default:
                    break;
            }
            return false;
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

        public override bool CheckValue(string val)
        {
            if (caseSensitive)
            {
                return val == value;
            }
            return val.ToLower() == value.ToLower();
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
