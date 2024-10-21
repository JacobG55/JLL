using JLL.API.Events;
using System;
using UnityEngine;

namespace JLL.Components.Helpers
{
    public class MathHelper : MonoBehaviour
    {
        public MathOperation[] Operations = new MathOperation[0];
        public FloatEvent Value = new FloatEvent();
        public IntEvent RoundedValue = new IntEvent();

        [Serializable]
        public class MathOperation
        {
            public Operation operation;
            public float value;

            public float Perform(float number)
            {
                return operation switch
                {
                    Operation.Add => number + value,
                    Operation.Subtract => number - value,
                    Operation.Multiply => number * value,
                    Operation.Divide => number / value,
                    Operation.Min => Mathf.Min(number, value),
                    Operation.Max => Mathf.Max(number, value),
                    Operation.AbsoluteValue => Mathf.Abs(number),
                    Operation.Power => Mathf.Pow(number, value),
                    _ => number,
                };
            }
        }

        public enum Operation
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Min,
            Max,
            AbsoluteValue,
            Power,
        }

        public void DoMath(float value)
        {
            for (int i = 0; i < Operations.Length; i++)
            {
                value = Operations[i].Perform(value);
            }
            Value.Invoke(value);
            RoundedValue.Invoke(Mathf.RoundToInt(value));
        }

        public void DoMath(int value)
        {
            DoMath((float)value);
        }
    }
}
