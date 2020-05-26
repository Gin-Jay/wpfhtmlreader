using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        private void buttonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
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
                textBlockFilePath.Text = filename;
            }
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

            string textFromSearchingTextBox = textBoxSearching.Text;

            List<string> listOfStringsFromFile = new List<string>();
            listOfStringsFromFile = File.ReadLines(filename).ToList();
            listOfUrls = UrlValidation.ValidationCheck(listOfStringsFromFile);

            listBoxWithUrls.ItemsSource = listOfUrls;

            string[] htmlCodes = new string[listOfUrls.Count];

            for (int i = 0; i < listOfUrls.Count; i++)
            {
                htmlCodes[i] = await Request.GetAsync(listOfUrls[i]);
            }

            int[] Counter = new int[listOfUrls.Count];

            for (int i = 0; i < listOfUrls.Count; i++)
            {
                //string page = webBrowserFromGin.Document.Body.InnerText;

                int count = 0, n = 0;

                while ((n = htmlCodes[i].IndexOf(textFromSearchingTextBox, n)) != -1)
                {
                    n += textFromSearchingTextBox.Length;
                    ++count;
                }
                Counter[i] = count;
            }

            listBoxWithCounts.ItemsSource = Counter;
            //string htmlCode = await Request.GetAsync(listOfUrls[0]);
        }
    }
}