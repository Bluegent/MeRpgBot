using System;
using System.Collections.Generic;
using RPGEngine.Core;
using RPGEngine.GameConfigReader;
using RPGEngine.Language;

namespace RPGEngine.Game
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
        public List<MeNode> HpMods { get; private set; }
        public List<MeNode> Modifiers { get; private set; }
        public MeNode Interval { get; set; }
        public MeNode MaxStacks { get; set; }
        public StackingType Type { get; set; }

        public StatusTemplate(MeNode[] formulas)
        {
            HpMods = new List<MeNode>();
            Modifiers = new List<MeNode>();         
            foreach (MeNode node in formulas)
            {
                if (node.Value.Type == VariableType.Function)
                {
                    if (node.Value.GetString() == LConstants.HARM_F || node.Value.GetString() == LConstants.HEAL_F)
                    {
                        HpMods.Add(node);
                    }
                    if (node.Value.GetString() == LConstants.ADD_MOD_F)
                    {
                        Modifiers.Add(node);
                    }
                }
            }
        }
        public static StackingType FromString(string str)
        {
            foreach(StackingType type in Enum.GetValues(typeof(StackingType)))
                if (str.Equals(type.ToString().ToLower()))
                    return type;
            return StackingType.None;
        }
    }
}
