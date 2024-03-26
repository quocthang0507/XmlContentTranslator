namespace XmlContentTranslator
{
    public static class StringUtils
    {
        public static string Max50(string text)
        {
            return text.Length > 50 ? text.Substring(0, 50).Trim() + "..." : text;
        }
    }
}
