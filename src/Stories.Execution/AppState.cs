using System;
using System.Collections.Generic;
using System.Linq;
using Stories.Parser.Conditions;

namespace Stories.Execution
{
    public class AppState
    {
        private Dictionary<string, bool> values;

        private AppState(string[] fluents, bool[] values)
        {
            this.values = fluents
                .Zip(values, (key, value) => new { key, value })
                .ToDictionary(v => v.key, v => v.value);
        }

        public static IEnumerable<AppState> ValidStates(List<ConditionExpression> alwaysConditions, string[] fluents)
        {
            bool[] values = new bool[fluents.Length];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = false;
            }

            while (true)
            {
                var state = new AppState(fluents, values);

                if (alwaysConditions.All(cnd => state.EvaluateCondition(cnd)))
                {
                    yield return state;
                }

                int j;
                for (j = 0; j < values.Length && values[j]; j++)
                {
                    values[j] = false;
                }
                if (j >= values.Length)
                {
                    break;
                }
                values[j] = true;
            }

        }

        public bool GetVariable(string label)
        {
            return values[label];
        }

        public bool EvaluateCondition(ConditionExpression expression)
        {
            switch (expression)
            {
                case ConditionConstant constant: return constant.Value;
                case ConditionNegation negation: return !EvaluateCondition(negation.Expression);
                case ConditionOperation operation:
                    switch (operation.Operation)
                    {
                        case OperationType.And: return EvaluateCondition(operation.Left) && EvaluateCondition(operation.Right);
                        case OperationType.Or: return EvaluateCondition(operation.Left) || EvaluateCondition(operation.Right);
                        case OperationType.Consequence: return !EvaluateCondition(operation.Left) || EvaluateCondition(operation.Right);
                        case OperationType.Iff: return EvaluateCondition(operation.Left) == EvaluateCondition(operation.Right);
                        default: throw new InvalidOperationException();
                    }
                case ConditionVariable variable: return GetVariable(variable.Label);
                default: throw new InvalidOperationException();
            }
        }
    }
}
