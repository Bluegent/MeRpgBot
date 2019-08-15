using System;
using RPGEngine.GameConfigReader;

namespace RPGEngine.Core
{
    public class StatModifier
    {
        public string StatKey { get; set; }
        public double Amount { get; set; }
    }

    public enum StackingType
    {
        None, //The buff does not stack, so when it is applied while another stack of it exists on the target, nothing happens.
        Refresh, //The duration of existing effect is refreshed when applied again. The formula IS NOT recalculated!
        Independent, //Status stacks have independent durations, so applying a new one will not affect the others.

    }
    public class StatusTemplate : BaseObject
    {
        public MeNode[] ComponentFormulas { get; set; }
        public MeNode Interval { get; set; }
        public MeNode MaxStacks { get; set; }
        public StackingType Type { get; set; }

        public static StackingType FromString(string str)
        {
            foreach(StackingType type in Enum.GetValues(typeof(StackingType)))
                if (str.Equals(type.ToString().ToLower()))
                    return type;
            return StackingType.None;
        }
    }
}
