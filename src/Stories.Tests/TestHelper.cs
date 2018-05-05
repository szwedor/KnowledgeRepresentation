namespace Stories.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using FluentAssertions;
    using Stories.Execution;
    using Stories.Graph.Model;
    using Stories.Parser;
    using Stories.Parser.Conditions;
    using Stories.Parser.Statements;

    public static class TestHelper
    {
        public static HistoryStatement GetHistory(this string[] storyString) =>
            Parsing.GetHistory(string.Join(Environment.NewLine, storyString));

        public static HistoryStatement GetHistory(this string storyString) =>
            Parsing.GetHistory(storyString);

        public static void ShouldEqualCondition(this ConditionExpression expression,
            ConditionExpression expressionToCompare)
            => expression.Should().BeEquivalentTo(expressionToCompare, opts => opts.IncludingAllRuntimeProperties());

        public static Vertex ShouldHaveEdges(this Vertex vertex, string actor, params AppState[] states)
        {
             vertex.EdgesOutgoing.Where(p => p.Actor == actor).Select(p => p.To.State).Should().BeEquivalentTo(states);
            return vertex;
        }
    };

}
