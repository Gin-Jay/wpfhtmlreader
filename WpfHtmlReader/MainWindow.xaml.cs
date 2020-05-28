using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;

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

        private void buttonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            buttonStartSearching.IsEnabled = false;
            dataGridUrsWithCount.ItemsSource = null;
            textBlockFilePath.Text = "";

            filename = Worker.FileOpener();

            if (filename == null)
            {
                MessageBox.Show("Ошибка или отмена операции.");
                return;
            }

            textBlockFilePath.Text = filename;

            listOfUrls = Worker.UrlExtractor(filename);

            if (listOfUrls.Count == 0)
            {
                MessageBox.Show("В выбранном файле не найдено ссылок.");
                return;
            }

            progressBarWork.Value = 0;
            progressBarWork.Maximum = listOfUrls.Count*2;
            textBoxSearching.IsEnabled = true;
            buttonStartSearching.IsEnabled = true;
        }

        private async void buttonStartSearching_Click(object sender, RoutedEventArgs e)
        {
            buttonCancel.IsEnabled = true;

            cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;

            //убрать или заменить проверку на filename
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

            var resultArray = await new Worker().HtmlWordsCounter(textBoxSearching.Text, listOfUrls, token, progressBarWork);

            if (!cancelTokenSource.IsCancellationRequested)
            {
                int maxCounter = resultArray.Max();
                int maxIndex = resultArray.ToList().IndexOf(maxCounter);

                List<FinalDataModel> finalDataList = new List<FinalDataModel>();
                for (int i = 0; i < listOfUrls.Count; i++)
                {
                    finalDataList.Add(new FinalDataModel() {Id = i+1, Url = listOfUrls[i], Repeats = resultArray[i] });
                }

                dataGridUrsWithCount.ItemsSource = finalDataList;
                dataGridUrsWithCount.SelectedIndex = maxIndex;
                dataGridUrsWithCount.Focus();
                dataGridUrsWithCount.ScrollIntoView(dataGridUrsWithCount.SelectedItem);

                buttonCancel.IsEnabled = false;

                MessageBox.Show($"Максимальное значение = {maxCounter} строка: {maxIndex + 1}");
            }

        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            cancelTokenSource.Cancel();

            buttonCancel.IsEnabled = false;
        }
    }
}