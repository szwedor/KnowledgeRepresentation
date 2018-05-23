using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Stories.Information
{
    public static class InformationsDefinitions
    {
        public static readonly List<InformationEntry> definitions;

        static InformationsDefinitions()
        {
            definitions = new List<InformationEntry>();

            var attributesValues = typeof(InformationsDefinitions).GetFields(BindingFlags.Static | BindingFlags.NonPublic)
                .Where(p => p.FieldType == typeof(InformationEntry) && p.GetCustomAttribute(typeof(InformationDefinition)) != null).
                Select(a => a.GetValue(null)).ToList(); //POTWOREK :)
            foreach (var value in attributesValues)
            {
                definitions.Add((InformationEntry)value);
            }
        }

        [InformationDefinition]
        private static readonly InformationEntry IntroductionEntry = new InformationEntry()
        {
            InformationName = "Wstęp",
            HtmlDescription = Properties.Resources.Introduction
        };

        [InformationDefinition]
        private static readonly InformationEntry GuiEntry = new InformationEntry()
        {
            InformationName = "Graficzny interfejs użytkownika",
            HtmlDescription = Properties.Resources.GraphicalInterface
        };

        [InformationDefinition]
        private static readonly InformationEntry LanguageDefinitionEntry = new InformationEntry()
        {
            InformationName = "Opis języka",
            HtmlDescription = Properties.Resources.LanguageDefinition
        };

        [InformationDefinition]
        private static readonly InformationEntry KeyWordsEntry = new InformationEntry()
        {
            InformationName = "Słowa kluczowe",
            HtmlDescription = Properties.Resources.KeyWords
        };

        [InformationDefinition]
        private static readonly InformationEntry SyntaxEntry = new InformationEntry()
        {
            InformationName = "Syntaktyka",
            HtmlDescription = Properties.Resources.Syntax
        };

        [InformationDefinition]
        private static readonly InformationEntry LogicFormulasEntry = new InformationEntry()
        {
            InformationName = "Formuły logiczne",
            HtmlDescription = Properties.Resources.LogicFormulas
        };

        [InformationDefinition]
        private static readonly InformationEntry AfterEntry = new InformationEntry()
        {
            InformationName = "Instrukcja wartości: after",
            HtmlDescription = Properties.Resources.After
        };

        [InformationDefinition]
        private static readonly InformationEntry CausesEntry = new InformationEntry()
        {
            InformationName = "Instrukcja efektu: causes, impossible, typically",
            HtmlDescription = Properties.Resources.Causes
        }; 
    }
}
