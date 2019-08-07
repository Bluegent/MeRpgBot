using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        Refresh, //The duration of existing stacks is refreshed. The formula IS NOT recalculated!
        Independent, //Status stacks have independent durations, so applying a new one will not affect the others.
    }
    public class StatusTemplate
    {
        public MeNode[] ComponentFormulas { get; set; }
        public MeNode Interval { get; set; }
        public MeNode MaxStacks { get; set; }
        public StackingType Type { get; set; }
        public string Key { get; set; }
    }
}
