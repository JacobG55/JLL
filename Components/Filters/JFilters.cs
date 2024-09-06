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

        public void Result(bool success, T input)
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
    public class NumericFilter : JFilterProperty<float>
    {
        public FilterOpperand operation = FilterOpperand.EqualTo;

        public enum FilterOpperand
        {
            GreaterThan,
            LessThan,
            EqualTo
        }

        public override bool CheckValue(float val)
        {
            switch (operation)
            {
                case FilterOpperand.EqualTo:
                    return val == value;
                case FilterOpperand.GreaterThan:
                    return val > value;
                case FilterOpperand.LessThan:
                    return val < value;
                default:
                    break;
            }
            return false;
        }
    }

    [Serializable]
    public class NameFilter : JFilterProperty<string>
    {
        public override bool CheckValue(string val)
        {
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
