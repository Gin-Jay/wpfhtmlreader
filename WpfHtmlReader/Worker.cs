using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHtmlReader
{
    class Worker
    {
        public static string FileOpener()
        {
            string filename;
            Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            openDialog.DefaultExt = ".txt";
            openDialog.Filter = "TXT Files (*.txt)|*.txt|All Files (*.*)|*.*";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = openDialog.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                filename = openDialog.FileName;
                return filename;
            }
            else
            {
                return result.ToString();
            }
        }

        public static List<string> UrlExtractor(string filename)
        {
            List<string> listOfStringsFromFile = UrlValidation.ValidationCheck(File.ReadLines(filename).ToList());

            return listOfStringsFromFile;
        }

        public static async Task<int[]> HtmlWordsCouter (string searchingWord, List<string> listOfUrls, CancellationToken token)
        {
            string[] htmlCodes = new string[listOfUrls.Count];

            for (int i = 0; i < listOfUrls.Count; i++)
            {
                htmlCodes[i] = await Request.GetAsync(listOfUrls[i]);
                if (token.IsCancellationRequested)
                {
                    MessageBox.Show("Операция поиска прервана");
                    return null;
                }
            }

            int[] Counter = new int[listOfUrls.Count];

            for (int i = 0; i < listOfUrls.Count; i++)
            {
                int count = 0, n = 0;

                while ((n = htmlCodes[i].IndexOf(searchingWord, n)) != -1)
                {
                    n += searchingWord.Length;
                    ++count;

                    if (token.IsCancellationRequested)
                    {
                        MessageBox.Show("Операция поиска прервана");
                        return null;
                    }
                }
                Counter[i] = count;
            }
            return await Task.FromResult(Counter);
        }
    }
}
