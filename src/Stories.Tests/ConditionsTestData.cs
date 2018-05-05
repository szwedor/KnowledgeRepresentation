namespace Stories.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Stories.Parser.Conditions;
    using Stories.Parser.Parsers;

    public static class ConditionsTestData
    {
        public static readonly List<object[]> Data = new List<object[]>
        {
            new object[] {"BilboLives", new ConditionVariable("BilboLives")},
            new object[] {"not BilboLives", new ConditionNegation(new ConditionVariable("BilboLives"))},
            new object[]
            {
                "BilboLives and FrodoLives", new ConditionOperation
                {
                    Left = new ConditionVariable("BilboLives"),
                    Operation = OperationType.And,
                    Right = new ConditionVariable("FrodoLives")
                }
            },
            new object[]
            {
                "BilboLives then FrodoLives", new ConditionOperation
                {
                    Left = new ConditionVariable("BilboLives"),
                    Operation = OperationType.Consequence,
                    Right = new ConditionVariable("FrodoLives")
                }
            },
            new object[]
            {
                "BilboLives iff FrodoLives", new ConditionOperation
                {
                    Left = new ConditionVariable("BilboLives"),
                    Operation = OperationType.Iff,
                    Right = new ConditionVariable("FrodoLives")
                }
            }
        };

        public static readonly List<object[]> ObservableData =
            Data.Select(
                    p => p.Concat(new object[] {false}).ToArray()).Concat(
                    Data.Select(p => p.Concat(new object[] {true}).ToArray()))
                .ToList();

        public static readonly List<object[]> EffectAndPrecondition =
            Data.Join(Data, p => 1, p => 1, (a, b) => a.Concat(b).ToArray()).ToList();

        public static readonly List<object[]> EffectAndPreconditionTypically =
            EffectAndPrecondition.Select(
                    p => p.Concat(new object[] {false}).ToArray()).Concat(
                    EffectAndPrecondition.Select(p => p.Concat(new object[] {true}).ToArray()))
                .ToList();

 
    }
}