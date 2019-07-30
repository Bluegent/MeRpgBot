﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroExpressionParser
{
    using System.Threading;

    public class MeException : Exception
    {
        public MeException(String message) : base(message) { }
    }
    public enum VariableType
    {
        Function,
        NumericValue,
        Entity,
        EntityProperty,
        Operator,
        DamageType,
        Array,
        String
    }

    public class DamageType
    {
        public string Key { get; set; }
        public string MitigationFormula { get; set; }

    }
    public class MeVariable
    {
        public VariableType Type { get; set; }
        public object Value { get; set; }

        public static implicit operator MeVariable(double value)
        {
            return new MeVariable(){Type = VariableType.NumericValue, Value =  value};
        }

        public static implicit operator MeVariable(Entity value)
        {
            return new MeVariable() { Type = VariableType.Entity, Value = value };
        }

        public void ValidateType(VariableType type)
        {
            if (Type != type)
                throw new MeException($"Exception when converting variable of type {Type} to {type}.");
        }

        public double ToDouble()
        {
            ValidateType(VariableType.NumericValue);
            return (double)Value;
        }

        public DamageType ToDamageType()
        {
            ValidateType(VariableType.DamageType);
            return (DamageType)Value;
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

        public AbstractFunction ToFunction()
        {
            ValidateType(VariableType.Function);
            return (AbstractFunction)Value;
        }

        public MeVariable[] ToArray()
        {
            ValidateType(VariableType.Array);
            return (MeVariable[])Value;
        }

        public EntityProperty ToEntityProperty()
        {
            ValidateType(VariableType.EntityProperty);
            return (EntityProperty)Value;
        }

        public new string ToString()
        {
            ValidateType(VariableType.String);
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
    }
    class Base
    {
    }
}
