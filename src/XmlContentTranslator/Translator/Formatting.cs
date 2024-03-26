﻿using System;
using System.Collections.Generic;
using System.Text;

namespace XmlContentTranslator.Translator
{
    public class Formatting
    {
        private bool Italic { get; set; }
        private bool ItalicTwoLines { get; set; }
        private string StartTags { get; set; }
        private bool AutoBreak { get; set; }
        private bool SquareBrackets { get; set; }
        private bool SquareBracketsUppercase { get; set; }

        public string SetTagsAndReturnTrimmed(string text, string source)
        {
            text = text.Trim();

            // SSA/ASS tags
            if (text.StartsWith("{\\"))
            {
                var endIndex = text.IndexOf('}');
                if (endIndex > 0)
                {
                    StartTags = text.Substring(0, endIndex + 1);
                    text = text.Remove(0, endIndex + 1).Trim();
                }
            }

            // Italic tags
            if (text.StartsWith("<i>", StringComparison.Ordinal) && text.EndsWith("</i>", StringComparison.Ordinal) && text.Contains("</i>" + Environment.NewLine + "<i>") && Utilities.GetNumberOfLines(text) == 2 && Utilities.CountTagInText(text, "<i>") == 1)
            {
                ItalicTwoLines = true;
                text = HtmlUtil.RemoveOpenCloseTags(text, HtmlUtil.TagItalic);
            }
            else if (text.StartsWith("<i>", StringComparison.Ordinal) && text.EndsWith("</i>", StringComparison.Ordinal) && Utilities.CountTagInText(text, "<i>") == 1)
            {
                Italic = true;
                text = text.Substring(3, text.Length - 7);
            }

            // Un-break line
            var allowedLanguages = new List<string> { "en", "da", "nl", "de", "sv", "nb", "fr", "it" };
            if (allowedLanguages.Contains(source))
            {
                var lines = HtmlUtil.RemoveHtmlTags(text).SplitToLines();
                if (lines.Length == 2 && !string.IsNullOrEmpty(lines[0]) && !string.IsNullOrEmpty(lines[1]) &&
                    char.IsLetterOrDigit(lines[0][lines[0].Length - 1]) &&
                    char.IsLower(lines[1][0]))
                {
                    text = string.Join(" ", text.SplitToLines()).Replace("  ", " ");
                    AutoBreak = true;
                }
            }

            // Square brackets
            if (text.StartsWith("[", StringComparison.Ordinal) && text.EndsWith("]", StringComparison.Ordinal) &&
                Utilities.GetNumberOfLines(text) == 1 && Utilities.CountTagInText(text, "[") == 1 &&
                Utilities.GetNumberOfLines(text) == 1 && Utilities.CountTagInText(text, "]") == 1)
            {
                if (text == text.ToUpperInvariant())
                    SquareBracketsUppercase = true;
                else
                    SquareBrackets = true;
                text = text.Replace("[", string.Empty).Replace("]", string.Empty);
            }

            return text.Trim();
        }

        public string ReAddFormatting(string text)
        {
            // Auto-break line
            if (AutoBreak)
            {
                text = Utilities.AutoBreakLine(text);
            }

            // Square brackets
            if (SquareBracketsUppercase)
            {
                text = "[" + text.ToUpperInvariant().Trim() + "]";
            }
            else if (SquareBrackets)
            {
                text = "[" + text.Trim() + "]";
            }

            // Italic tags
            if (ItalicTwoLines)
            {
                var sb = new StringBuilder();
                foreach (var line in text.SplitToLines())
                {
                    sb.AppendLine("<i>" + line + "</i>");
                }
                text = sb.ToString().Trim();
            }
            else if (Italic)
            {
                text = "<i>" + text + "</i>";
            }

            // SSA/ASS tags
            text = StartTags + text;

            return text;
        }


        private int NumberOfLines { get; set; }

        public string Unbreak(string text, string source)
        {
            NumberOfLines = source.SplitToLines().Length;
            return text.Replace(Environment.NewLine, " ").Replace("  ", " ");
        }

        public string Rebreak(string text)
        {
            return NumberOfLines == 1 ? text : Utilities.AutoBreakLine(text);
        }

    }
}
