using System;
using System.Collections.Generic;

namespace Stories.Parser.Conditions
{
    public enum OperationType
    {
        And,
        Or,
        Not,
        Iff,
        Consequence
    }

    public abstract class ConditionExpression
    {
    }
}
