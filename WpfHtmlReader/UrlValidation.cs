using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WpfHtmlReader
{
    class UrlValidation
    {
        public static List<string> ValidationCheck(List<string> fileLines)
        {
            List<string> urlList = new List<string>();
            string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";

            foreach (var line in fileLines)
            {
                Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (Rgx.IsMatch(line))
                {
                    if (!line.StartsWith("http") && !line.StartsWith("https"))
                        urlList.Add($"http://{line}");
                    else
                        urlList.Add(line);

                }
            }

            return urlList;
        }
    }
}
