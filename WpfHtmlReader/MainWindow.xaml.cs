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
        public static CancellationTokenSource cancelTokenSource;
        //CancellationToken token = cancelTokenSource.Token;

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

            progressBarWork.Maximum = listOfUrls.Count;
            textBoxSearching.IsEnabled = true;
            buttonStartSearching.IsEnabled = true;
        }

        private async void buttonStartSearching_Click(object sender, RoutedEventArgs e)
        {
            buttonCancel.IsEnabled = true;
            cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;
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

            listBoxWithCounts.ItemsSource = await Worker.HtmlWordsCouter(textBoxSearching.Text, listOfUrls, token);
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            cancelTokenSource.Cancel();

            buttonCancel.IsEnabled = false;
        }
    }
}