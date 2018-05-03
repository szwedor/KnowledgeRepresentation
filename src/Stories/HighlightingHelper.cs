using System;

namespace Stories
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Highlighting.Xshd;
    using Stories.Parser.Parsers;

    public static class HighlightingHelper
    {
        private static IHighlightingDefinition basicDefinition;
        private static HighlightingRule[] basicRules;
        static HighlightingHelper()
        {
            using (var reader = new XmlTextReader(new StringReader(Properties.Resources.AdaSyntaxt)))
            {
                basicDefinition = HighlightingLoader.Load(reader,
                    HighlightingManager.Instance);

                basicRules = basicDefinition.MainRuleSet.Rules.ToArray();
            }
        }

        internal static void ResetHighlighting(this TextEditor editor)
        {
            editor.SyntaxHighlighting = basicDefinition;
            basicDefinition.MainRuleSet.Rules.Clear();
            Array.ForEach(basicRules,
                rule=>
                basicDefinition.MainRuleSet.Rules.Add(rule));
            editor.RefreshHighlighting();
        }

        public static void AddHighlighting(this TextEditor editor, IEnumerable<string> strings, Color color)
        {
            var stringsList = strings.ToList();
            if (!stringsList.Any())
                return;
            var highlightingRule = new HighlightingRule();
            highlightingRule.Color = new HighlightingColor
            {
                Foreground = new SimpleHighlightingBrush(color),
                FontWeight = FontWeights.Bold
            };

            var regex = string.Format(@"\b({0})\b", string.Join("|", strings));
            highlightingRule.Regex = new Regex(regex);
            editor.SyntaxHighlighting.MainRuleSet.Rules.Add(highlightingRule);
        }

        public static void RefreshHighlighting(this TextEditor editor)
        {
            var syntax = editor.SyntaxHighlighting;
            editor.SyntaxHighlighting = null;
            editor.SyntaxHighlighting = syntax;
        }
    }
}
