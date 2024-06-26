﻿using System;
using System.Text;

namespace XmlContentTranslator.Translator
{
    public class Json
    {
        public static string EncodeJsonText(string text)
        {
            var sb = new StringBuilder(text.Length);
            foreach (var c in text)
            {
                switch (c)
                {
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '"':
                        sb.Append("\\\"");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString().Replace(Environment.NewLine, "<br />");
        }

        public static string DecodeJsonText(string text)
        {
            text = text.Replace("<br />", Environment.NewLine);
            text = text.Replace("<br>", Environment.NewLine);
            text = text.Replace("<br/>", Environment.NewLine);
            text = text.Replace("\\n", Environment.NewLine);
            bool keepNext = false;
            var sb = new StringBuilder(text.Length);
            foreach (var c in text)
            {
                if (c == '\\' && !keepNext)
                {
                    keepNext = true;
                }
                else
                {
                    sb.Append(c);
                    keepNext = false;
                }
            }
            return sb.ToString();
        }


    }
}
