using System.Globalization;
using RPGEngine.Game;

namespace RPGEngine.Core
{
    using System;

    using RPGEngine.Language;
    using Entities;

    public class MeException : Exception
    {
        public MeException(string message) : base(message) { }
    }
    public enum VariableType
    {
        Invalid,
        Function,
        NumericValue,
        Entity,
        EntityProperty,
        Operator,
        DamageType,
        Array,
        String,
        Boolean,
        Status,
        StatModifier,
        PlaceHolder,
        Property
    }

    public class MeVariable
    {
        public VariableType Type { get; set; }
        public object Value { get; set; }

        public MeVariable()
        {
            Value = null;
            Type = VariableType.Invalid;
        }
        public MeVariable(MeVariable other)
        {
            Type = other.Type;
            Value = other.Value;
        }

        public string GetString()
        {
            switch (Type)
            {
                case VariableType.Function:

                    return ToFunction().Key;

                case VariableType.Operator:
                    return ToOperator().Key;

                case VariableType.String:
                    return ToMeString();

                case VariableType.Entity:
                    return ToEntity().Key;
            }
            return "";
        }

        public static implicit operator MeVariable(double value)
        {
            return new MeVariable() { Type = VariableType.NumericValue, Value = value };
        }

        public static implicit operator MeVariable(Entity value)
        {
            return new MeVariable() { Type = VariableType.Entity, Value = value };
        }

        public static implicit operator MeVariable(bool value)
        {
            return new MeVariable() { Type = VariableType.Boolean, Value = value };
        }

        public virtual MeVariable Execute()
        {
            return this;
        }

        protected void ValidateType(VariableType type)
        {
            if (Type != type)
                throw new MeException($"Exception when converting variable of type {Type}({GetString()}) to {type}.");
        }

        public long ToLong()
        {
            return (long)ToDouble();
        }
        public double ToDouble()
        {
            if (Type == VariableType.Property)
                return ((Property)Value).Value;

            ValidateType(VariableType.NumericValue);
            return (double)Value;
        }

        public Property ToProperty()
        {
            ValidateType(VariableType.Property);
            return (Property)Value;
        }

        public DamageTypeTemplate ToDamageType()
        {
            ValidateType(VariableType.DamageType);
            return (DamageTypeTemplate)Value;
        }

        public Entity ToEntity()
        {
            ValidateType(VariableType.Entity);
            return (Entity)Value;
        }

        public Operator ToOperator()
        {
            ValidateType(VariableType.Operator);
            return (Operator)Value;
        }

        public Function ToFunction()
        {
            ValidateType(VariableType.Function);
            return (Function)Value;
        }

        public MeVariable[] ToArray()
        {
            if(Type == VariableType.Array)
                return (MeVariable[])Value;

            return  new MeVariable[1] {this};
        }

        public EntityAttribute ToEntityProperty()
        {
            ValidateType(VariableType.EntityProperty);
            return (EntityAttribute)Value;
        }

        public string ToMeString()
        {
            ValidateType(VariableType.String);
            return (string)Value;
        }

        public bool ToBoolean()
        {
            ValidateType(VariableType.Boolean);
            return (bool)Value;
        }

        public StatusTemplate ToStatus()
        {
            ValidateType(VariableType.Status);
            return (StatusTemplate)Value;
        }

        public StatModifier ToModifier()
        {
            ValidateType(VariableType.StatModifier);
            return (StatModifier)Value;
        }

        public string ToPlaceholder()
        {
            ValidateType(VariableType.PlaceHolder);
            return (string)Value;
        }

        public static double[] ToDoubleArray(MeVariable[] array)
        {
            double[] result = new double[array.Length];
            for (int i = 0; i < array.Length; ++i)
            {
                result[i] = array[i].ToDouble();
            }

            return result;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case VariableType.Invalid:
                    break;
                case VariableType.Function:
                    return $"func:{ToFunction().Key}";
                case VariableType.NumericValue:
                    return ToDouble().ToString(CultureInfo.InvariantCulture);
                case VariableType.Entity:
                    return $"entity:{ToEntity().Name}";
                case VariableType.EntityProperty:
                    return $"prop:{ToEntityProperty().Value}";
                case VariableType.Operator:
                    return $"op:{ToOperator().Key}";
                case VariableType.DamageType:
                    return $"dmgT:{ToDamageType().Key}";
                case VariableType.Array:
                    return "[]";
                case VariableType.String:
                    return ToMeString();
                case VariableType.Boolean:
                    return ToBoolean().ToString();
                case VariableType.Status:
                    return $"status:{ToDamageType().Key}";
                case VariableType.StatModifier:
                    return $"mod:{ToModifier().StatKey}";
                case VariableType.PlaceHolder:
                    return $"$:{ToPlaceholder()}";
                case VariableType.Property:
                    return $"prop:{ToProperty().Value}";
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return "";
        }
    }

    public class MeFunction : MeVariable
    {
        public MeVariable[] SubVariables { get; set; }

        public MeFunction(MeVariable baseVar, MeVariable[] parameters)
        {
            Value = baseVar.Value;
            Type = baseVar.Type;
            SubVariables = parameters;
        }

        public MeFunction(MeFunction other)
        {
            SubVariables = new MeVariable[other.SubVariables.Length];
            Array.Copy(other.SubVariables, SubVariables.Length, SubVariables, 0, SubVariables.Length);
        }

        public override MeVariable Execute()
        {
            if (Type == VariableType.Function)
                return ToFunction().Execute(SubVariables);
            else if (Type == VariableType.Operator)
                return ToOperator().Execute(SubVariables);
            return base.Execute();
        }
    }
}
