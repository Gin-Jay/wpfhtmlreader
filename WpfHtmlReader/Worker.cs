using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfHtmlReader
{

    class Worker
    {
        delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);

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
            if (result.HasValue && result.Value )
            {
                // Open document 
                filename = openDialog.FileName;
                return filename;
            }

            return null;
        }

        public static List<string> UrlExtractor(string filename)
        {
            List<string> listOfStringsFromFile = UrlValidation.ValidationCheck(File.ReadLines(filename).ToList());

            return listOfStringsFromFile;
        }
             
        public async Task<int[]> HtmlWordsCounter (string searchingWord, List<string> listOfUrls, CancellationToken token, ProgressBar progressBar)
        {
            string[] htmlCodes = new string[listOfUrls.Count];

            UpdateProgressBarDelegate updProgress = new UpdateProgressBarDelegate(progressBar.SetValue);

            double value = 0;

            for (int i = 0; i < listOfUrls.Count; i++)
            {
                htmlCodes[i] = await Request.GetAsync(listOfUrls[i]);

                Application.Current.Dispatcher.Invoke(updProgress, new object[] { ProgressBar.ValueProperty, ++value });

                if (token.IsCancellationRequested)
                {
                    MessageBox.Show("Операция поиска прервана");
                    return null;
                }
            }

            int[] counter = new int[listOfUrls.Count];

            for (int i = 0; i < listOfUrls.Count; i++)
            {
                int count = 0, n = 0;

                Application.Current.Dispatcher.Invoke(updProgress, new object[] { ProgressBar.ValueProperty, ++value });
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
                counter[i] = count;
            }
            return await Task.FromResult(counter);
        }

    }
}
