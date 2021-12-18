using System;

namespace TownOfUs.CustomOption
{
    public class CustomToggleOption : CustomOption
    {
        protected internal CustomToggleOption(int id, string name, bool value = true) : base(id, name,
            CustomOptionType.Toggle,
            value)
        {
            Format = val => (bool) val ? "On" : "Off";
            onChange = null;
        }

        protected internal bool Get()
        {
            return (bool) Value;
        }

        protected internal void Toggle()
        {
            Set(!Get());
            onChange?.Invoke(Get());
        }

        public override void OptionCreated()
        {
            base.OptionCreated();
            Setting.Cast<ToggleOption>().TitleText.text = Name;
            Setting.Cast<ToggleOption>().CheckMark.enabled = Get();
        }
        
        public Action<bool> onChange;
    }
}