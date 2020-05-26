using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfHtmlReader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string filename;
        List<string> listOfUrls = new List<string>();
        public static CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        CancellationToken token = cancelTokenSource.Token;

        private void buttonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            buttonStartSearching.IsEnabled = false;

            filename = Worker.FileOpener();
            if (filename == "false")
            {
                MessageBox.Show("Ошибка открытия файла.");
                return;
            }

            textBlockFilePath.Text = filename;

            listOfUrls = Worker.UrlExtractor(filename);
            if (listOfUrls.Count == 0)
            {
                MessageBox.Show("В выбранном файле не найдено ссылок.");
                return;
            }
            listBoxWithUrls.ItemsSource = listOfUrls;

            buttonStartSearching.IsEnabled = true;
        }

        private async void buttonStartSearching_Click(object sender, RoutedEventArgs e)
        {
            
            if (String.IsNullOrEmpty(textBlockFilePath.Text))
            {
                MessageBox.Show("Не выбран файл для работы.");
                return;
            }

            if (String.IsNullOrWhiteSpace(textBoxSearching.Text))
            {
                MessageBox.Show("Нет введённого текста для поиска.");
                return;
            }

            //string textFromSearchingTextBox = textBoxSearching.Text;

            //string[] htmlCodes = new string[listOfUrls.Count];

            //for (int i = 0; i < listOfUrls.Count; i++)
            //{
            //    htmlCodes[i] = await Request.GetAsync(listOfUrls[i]);
            //}

            //int[] Counter = new int[listOfUrls.Count];

            //for (int i = 0; i < listOfUrls.Count; i++)
            //{

            //    int count = 0, n = 0;

            //    while ((n = htmlCodes[i].IndexOf(textFromSearchingTextBox, n)) != -1)
            //    {
            //        n += textFromSearchingTextBox.Length;
            //        ++count;
            //    }
            //    Counter[i] = count;
            //}
            //var Counter = await Worker.HtmlWordsCouter(textBoxSearching.Text, listOfUrls);
            listBoxWithCounts.ItemsSource = await Worker.HtmlWordsCouter(textBoxSearching.Text, listOfUrls, token);
            //string htmlCode = await Request.GetAsync(listOfUrls[0]);
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            cancelTokenSource.Cancel();
        }
    }
}