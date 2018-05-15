using System;
using System.Collections.Generic;
using System.Linq;

namespace Stories.Parser.Conditions
{
    public static class ConditionExtensionMethods
    {

        public static IEnumerable<KeyValuePair<string, bool>> ExtractFluentsValues(this ConditionExpression condition)
        {
            switch (condition)
            {
                case ConditionConstant constant: return new Dictionary<string, bool>();
                case ConditionNegation negation: return ExtractFluentsValues(negation.Expression).Select(x => new KeyValuePair<string, bool>(x.Key, !x.Value));
                case ConditionOperation operation:
                    var left = ExtractFluentsValues(operation.Left);
                    var right = ExtractFluentsValues(operation.Right);

                    return left.Concat(right);
                case ConditionVariable variable:
                    return new Dictionary<string, bool> { { variable.Label, true } };

                default: throw new InvalidOperationException();
            }
        }
    }
}
