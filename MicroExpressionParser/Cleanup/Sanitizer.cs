using System.Collections.Generic;
using RPGEngine.Core;
using RPGEngine.Language;
using RPGEngine.Parser;
using RPGEngine.Entities;

namespace RPGEngine.Cleanup
{
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
            result[0] = new Token(LConstants.GET_PROP_F);
            result[1] = new Token(char.ToString(LConstants.LEFT_PAREN));
            result[2] = new Token(entity.Key);
            result[3] = token;
            result[4] = new Token(char.ToString(LConstants.RIGHT_PAREN));
            return result;
        }

        public Token[] ReplaceEntities(Token[] tokens, Entity caster, Entity target)
        {
            List<Token> result = new List<Token>();
            foreach (Token token in tokens)
            {
                if (token.Type == TokenType.Variable && token.Value.StartsWith(char.ToString(LConstants.SPECIAL_CHAR)))
                {
                    if(token.Value.Equals(LConstants.TargetKeyword))
                        result.Add(new Token(target.Key));
                    else if (token.Value.Equals(LConstants.SourceKeyword))
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
            string[] lines = skill.Split(LConstants.FUNCTION_SEPARATOR);
            MeVariable var = null;
            foreach (string expr in lines)
            {
                Token[] tokens = Tokenizer.Tokenize(expr);
                Token[] sanitizedTokens = ReplaceEntities(tokens, caster, target);
                var = TreeResolver.Resolve(sanitizedTokens, _engine).Value;
            }

            return var;
        }


        public static MeNode ReplaceTargetAndSource(MeNode tree, Entity caster, Entity target)
        {
            List<MeNode> leaves = new List<MeNode>();
            foreach(MeNode leaf in tree.Leaves)
                leaves.Add(ReplaceTargetAndSource(leaf,caster,target));
            MeNode node = null;
            if(tree.Value.Type == VariableType.PlaceHolder)
            {
                if (tree.Value.ToPlaceholder() == LConstants.TargetKeyword)
                {
                    node = new MeNode(target);
                }
                else if (tree.Value.ToPlaceholder() == LConstants.SourceKeyword)
                {
                    node = new MeNode(caster);
                }
                else
                {
                    node = new MeNode(tree.Value);
                }
            }
            else
            {
                node = new MeNode(tree.Value);
            }
            node.Leaves.Clear();
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
            return TreeResolver.Resolve(sanitizedTokens,_engine).Value.ToDouble();

        }

        public MeNode[] SplitAndConvert(string expression)
        {
            string[] lines = expression.Split(LConstants.FUNCTION_SEPARATOR);
            List<MeNode> result = new List<MeNode>();
            foreach (string expr in lines)
            {
                if (expr.Length != 0)
                {
                    Token[] tokens = Tokenizer.Tokenize(expr);
                    result.Add(TreeConverter.Build(tokens, _engine));
                }
            }

            return result.ToArray();
        }

        public void ReplaceNumericPlaceholders(MeNode tree,Dictionary<string,double> valueMap)
        {
            foreach (MeNode leaf in tree.Leaves)
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

        public void ReplaceNumericPlaceholders(MeNode tree, double[] values)
        {
            if(values!=null)
                ReplaceNumericPlaceholders(tree,GetNumericValueMap(values));
        }
        public MeVariable ResolveStatus(MeNode tree, double[] values)
        {
            Dictionary<string, double> valueMap = GetNumericValueMap(values);
            ReplaceNumericPlaceholders(tree, valueMap);
            return TreeResolver.Resolve(tree).Value;
        }

        public static MeNode SanitizeMitigation(MeNode tree,Entity target, Entity caster, double amount)
        {
            List<MeNode> leaves = new List<MeNode>();
            foreach (MeNode leaf in tree.Leaves)
                leaves.Add(SanitizeMitigation(leaf, caster, target,amount));
            MeNode node = null;
            if (tree.Value.Type == VariableType.PlaceHolder)
            {
                if (tree.Value.ToPlaceholder().Equals(LConstants.TargetKeyword))
                {
                    node = new MeNode(target);
                }
                else if (tree.Value.ToPlaceholder().Equals(LConstants.SourceKeyword))
                {
                    node = new MeNode(caster);
                }
                else if(tree.Value.ToPlaceholder().Equals( LConstants.ValueKeyword))
                {
                    node = new MeNode(amount);
                }
            }
            else
            {
                node = new MeNode(tree.Value);
            }
            node.Leaves.AddRange(leaves);
            return node;
        }

        public void SetHarmsToPeriodic(MeNode tree)
        {
            foreach (MeNode leaf in tree.Leaves)
               SetHarmsToPeriodic(leaf);
            MeNode node = new MeNode(tree.Value); 

            if (tree.Value.Type == VariableType.Function && tree.Value.ToFunction().Key.Equals(LConstants.HARM_F))
            {
                MeNode periodic = new MeNode(true);
                periodic.Parent = tree;
                tree.Leaves.Add(periodic);
            }
        }

        public static MeNode ReplacePropeties(MeNode tree,Entity origin)
        {
            List<MeNode> leaves = new List<MeNode>();
            foreach (MeNode leaf in tree.Leaves)
                leaves.Add(ReplacePropeties(leaf,origin));
            MeNode node = null;
            if (tree.Value.Type == VariableType.String && origin.HasProperty(tree.Value.ToMeString()))
            {
                MeNode[] nodeLeaves = new MeNode[2]{new MeNode(origin), tree};
                Operator prop = Definer.Instance().Operators[LConstants.PROP_OP];
                node = new MeNode(new MeVariable(){Type = VariableType.Operator,Value = prop});
                node.Leaves.AddRange(nodeLeaves);
            }
            else
            {
                node = new MeNode(tree.Value);
            }
            node.Leaves.AddRange(leaves);
            return node;

        }

        public static MeNode ReplaceExpValues(MeNode tree, double prev, long level)
        {
            List<MeNode> leaves = new List<MeNode>();
            foreach (MeNode leaf in tree.Leaves)
                leaves.Add(ReplaceExpValues(leaf, prev,level));
            MeNode node;
            if (tree.Value.Type == VariableType.PlaceHolder)
            {
                if (tree.Value.ToPlaceholder().Equals(LConstants.ExpPrevKeyword))
                {
                    node = new MeNode(prev);
                }
                else if (tree.Value.ToPlaceholder().Equals(LConstants.LevelKeyword))
                {
                    node = new MeNode(level);
                }
                else
                {
                    node = new MeNode(tree.Value);
                }
            }
            else
            {
                node = new MeNode(tree.Value);
            }
            node.Leaves.AddRange(leaves);

            return node;
        }
    }
}
