using System;
using System.Collections.Generic;

namespace WpfHtmlReader
{
    class UrlValidation
    {
        public static List<string> ValidationCheck(List<string> fileLines)
        {
            List<string> urlList = new List<string>();
            Uri uriResult;
            foreach (var line in fileLines)
            {
                bool result = Uri.TryCreate(line, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                if (result == true)
                {
                    urlList.Add(line);
                }
            }
            
            return urlList;
        }
    }
}
