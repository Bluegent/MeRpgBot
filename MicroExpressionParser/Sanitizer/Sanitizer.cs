using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroExpressionParser
{
    using System.Diagnostics.Eventing.Reader;

    using MicroExpressionParser.Core;
    using System.Runtime.CompilerServices;

    public class Sanitizer
    {
        private IGameEngine _engine;

        public Sanitizer(IGameEngine engine)
        {
            _engine = engine;
        }


        public Token[] ConvertToGetProp(Token token, Entity entity)
        {
            Token[] result = new Token[5];
            result[0] = new Token(StringConstants.GET_PROP_F);
            result[1] = new Token(char.ToString(StringConstants.LEFT_PAREN));
            result[2] = new Token(entity.Key);
            result[3] = token;
            result[4] = new Token(char.ToString(StringConstants.RIGHT_PAREN));
            return result;
        }

        public Token[] ReplaceEntities(Token[] tokens, Entity caster, Entity target)
        {
            List<Token> result = new List<Token>();
            foreach (Token token in tokens)
            {
                if (token.Type == TokenType.Variable && token.Value.StartsWith(char.ToString(StringConstants.SPECIAL_CHAR)))
                {
                    if(token.Value.Equals(StringConstants.TargetKeyword))
                        result.Add(new Token(target.Key));
                    else if (token.Value.Equals(StringConstants.SourceKeyword))
                        result.Add(new Token(caster.Key));
                }
                else
                {
                    result.Add(token);
                }
            }
            return result.ToArray();
        }

        public MeVariable SanitizeSkill(string skill, Entity caster, Entity target)
        {
            string[] lines = skill.Split(StringConstants.FUNCTION_SEPARATOR);
            MeVariable var = null;
            foreach (string expr in lines)
            {
                Token[] tokens = Tokenizer.Tokenize(expr);
                Token[] sanitizedTokens = ReplaceEntities(tokens, caster, target);
                var = FunctionalTreeConverter.ResolveTree(sanitizedTokens, _engine).Value;
            }

            return var;
        }


        public FunctionalNode SanitizeSkillEntities(FunctionalNode tree, Entity caster, Entity target)
        {
            List<FunctionalNode> leaves = new List<FunctionalNode>();
            foreach(FunctionalNode leaf in tree.Leaves)
                leaves.Add(SanitizeSkillEntities(leaf,caster,target));
            FunctionalNode node = null;
            if(tree.Value.Type == VariableType.PlaceHolder)
            {
                if (tree.Value.ToPlaceholder() == StringConstants.TargetKeyword)
                {
                    node = new FunctionalNode(target);
                }
                else if (tree.Value.ToPlaceholder() == StringConstants.SourceKeyword)
                {
                    node = new FunctionalNode(caster);
                }
                else
                {
                    node = new FunctionalNode(tree.Value);
                }
            }
            else
            {
                node = new FunctionalNode(tree.Value);
            }
            node.Leaves.AddRange(leaves);
            return node;
        }

        public Token[] ReplaceProperties(Token[] tokens, Entity entity)
        {
            List<Token> result = new List<Token>();
            foreach (Token token in tokens)
            {
                if (token.Type == TokenType.Variable)
                {
                    if (entity.GetProperty(token.Value) != null)
                    {
                        result.AddRange(ConvertToGetProp(token,entity));
                    }
                    else
                    {
                        result.Add(token);
                    }
                }
                else
                {
                    result.Add(token);
                }
            }
            return result.ToArray();
        }

        public double SanitizeCompoundStat(Entity entity, string expression)
        {
            Token[] tokens = Tokenizer.Tokenize(expression);
            Token[] sanitizedTokens = ReplaceProperties(tokens, entity);
            return FunctionalTreeConverter.ResolveTree(sanitizedTokens,_engine).Value.ToDouble();

        }

        public FunctionalNode[] SplitStatus(string expression)
        {
            string[] lines = expression.Split(StringConstants.FUNCTION_SEPARATOR);
            List<FunctionalNode> result = new List<FunctionalNode>();
            foreach (string expr in lines)
            {
                Token[] tokens = Tokenizer.Tokenize(expr);
                result.Add(FunctionalTreeConverter.BuildTree(tokens, _engine));
            }

            return result.ToArray();
        }

        public void ReplaceNumericPlaceholders(FunctionalNode tree,Dictionary<string,double> valueMap)
        {
            foreach (FunctionalNode leaf in tree.Leaves)
                ReplaceNumericPlaceholders(leaf, valueMap);
            if (tree.Value.Type == VariableType.PlaceHolder)
                tree.Value = new MeVariable() { Value = valueMap[tree.Value.ToPlaceholder()], Type = VariableType.NumericValue };
        }

        private Dictionary<string, double> GetNumericValueMap(double[] values)
        {
            Dictionary<string, double> valueMap = new Dictionary<string, double>();
            for (int i = 0; i < values.Length; ++i)
            {
                valueMap.Add($"${i}", values[i]);
            }
            return valueMap;
        }

        public void ReplaceNumericPlaceholders(FunctionalNode tree, double[] values)
        {
            if(values!=null)
                ReplaceNumericPlaceholders(tree,GetNumericValueMap(values));
        }
        public MeVariable ResolveStatus(FunctionalNode tree, double[] values)
        {
            Dictionary<string, double> valueMap = GetNumericValueMap(values);
            ReplaceNumericPlaceholders(tree, valueMap);
            return FunctionalTreeConverter.ResolveNode(tree,0).Value;
        }
    }
}
