﻿using System.Globalization;
using RPGEngine.Game;

namespace RPGEngine.Core
{
    using System;
    using System.Net;
    using System.Runtime.Remoting.Messaging;
    using System.Windows.Markup;

    using RPGEngine.Language;
    using Entities;

    using RPGEngine.Templates;

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
        EntityAttribute,
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

        public T Get<T>()
        {
            return (T)Value;
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
                throw new MeException($"Exception when converting variable of type {Type}({ToString()}) to {type}.");
        }

        public long ToLong()
        {
            return (long)ToDouble();
        }

        public static explicit operator long(MeVariable var) => var.ToLong();

        public double ToDouble()
        {
            if (Type == VariableType.Property)
                return ((Property)Value).Value;

            ValidateType(VariableType.NumericValue);
            return (double)Value;
        }

        public static explicit operator double(MeVariable var) => var.ToDouble();

        public Property ToProperty()
        {
            ValidateType(VariableType.Property);
            return (Property)Value;
        }

        public static explicit operator Property(MeVariable var) => var.ToProperty();

        public DamageTypeTemplate ToDamageType()
        {
            ValidateType(VariableType.DamageType);
            return (DamageTypeTemplate)Value;
        }

        public static explicit operator DamageTypeTemplate(MeVariable var) => var.ToDamageType();

        public Entity ToEntity()
        {
            ValidateType(VariableType.Entity);
            return (Entity)Value;
        }

        public static explicit operator Entity(MeVariable var) => var.ToEntity();

        public Operator ToOperator()
        {
            ValidateType(VariableType.Operator);
            return (Operator)Value;
        }
        public static explicit operator Operator(MeVariable var) => var.ToOperator();

        public Function ToFunction()
        {
            ValidateType(VariableType.Function);
            return (Function)Value;
        }

        public static explicit operator Function(MeVariable var) => var.ToFunction();

        public MeVariable[] ToArray()
        {
            if (Type == VariableType.Array)
                return (MeVariable[])Value;

            return new MeVariable[1] { this };
        }

        public static explicit operator MeVariable[] (MeVariable var) => var.ToArray();

        public EntityAttribute ToAttribute()
        {
            ValidateType(VariableType.EntityAttribute);
            return (EntityAttribute)Value;
        }

        public static explicit operator EntityAttribute(MeVariable var) => var.ToAttribute();

        public string ToMeString()
        {
            ValidateType(VariableType.String);
            return (string)Value;
        }

        public static explicit operator string(MeVariable var) => var.ToMeString();

        public bool ToBoolean()
        {
            ValidateType(VariableType.Boolean);
            return (bool)Value;
        }

        public static explicit operator bool(MeVariable var) => var.ToBoolean();

        public StatusTemplate ToStatus()
        {
            ValidateType(VariableType.Status);
            return (StatusTemplate)Value;
        }

        public static explicit operator StatusTemplate(MeVariable var) => var.ToStatus();

        public StatModifier ToModifier()
        {
            ValidateType(VariableType.StatModifier);
            return (StatModifier)Value;
        }

        public static explicit operator StatModifier(MeVariable var) => var.ToModifier();

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
                case VariableType.EntityAttribute:
                    return $"prop:{ToAttribute().Value}";
                case VariableType.Operator:
                    return $"op:{ToOperator().Key}";
                case VariableType.DamageType:
                    return $"dmgT:{ToDamageType().Key}";
                case VariableType.Array:
                    return "[]:";
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
