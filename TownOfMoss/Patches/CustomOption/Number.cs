using System;
using UnityEngine;

namespace TownOfUs.CustomOption
{
    public class CustomNumberOption : CustomOption
    {
        protected internal CustomNumberOption(int id, string name, float value, float min, float max, float increment,
            Func<object, string> format = null) : base(id, name, CustomOptionType.Number, value, format)
        {
            Min = min;
            Max = max;
            Increment = increment;
        }

        protected internal CustomNumberOption(bool indent, int id, string name, float value, float min, float max,
            float increment,
            Func<object, string> format = null) : this(id, name, value, min, max, increment, format)
        {
            Indent = indent;
        }

        protected float Min { get; set; }
        protected float Max { get; set; }
        protected float Increment { get; set; }

        protected internal float Get()
        {
            return (float) Value;
        }

       
        protected internal void Increase()
        {
            var increment = Input.GetKeyInt(KeyCode.LeftShift) ? Increment / 10.0f
                : Increment;
            if ((int)Get() == (int)Max) {
                Set(Min);
                return;
            }
            Set(Mathf.Clamp(Get() + increment, Min, Max));
        }

        protected internal void Decrease()
        {
            var increment = Input.GetKeyInt(KeyCode.LeftShift) ? Increment / 10.0f
                : Increment;
            if ((int)Get() == (int)Min) {
                Set(Max);
                return;
            }
            Set(Mathf.Clamp(Get() - increment, Min, Max));
        }

        public override void OptionCreated()
        {
            base.OptionCreated();
            var number = Setting.Cast<NumberOption>();

            number.TitleText.text = Name;
            number.ValidRange = new FloatRange(Min, Max);
            number.Increment = Increment;
            number.Value = number.oldValue = Get();
            number.ValueText.text = ToString();
        }
    }
}