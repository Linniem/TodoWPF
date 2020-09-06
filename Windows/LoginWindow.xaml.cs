using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TaskWpf.Helpers;
using TaskWpf.Pages;
using TaskWpf.Properties;

namespace TaskWpf.Windows
{
    /// <summary>
    /// LoginPage.xaml 的互動邏輯
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            CkeckIsAlreadyLoginAsync().GetAwaiter();
            InitializeComponent();

            LoadIcon();
            LoadLoginPage();
        }

        private async Task CkeckIsAlreadyLoginAsync()
        {
            string loginUrl = ConfigurationManager.AppSettings["HostApiUrl"].ToString() + "username";
            var response = await WebApiHelper.SendRequestAsync(HttpMethod.Get, loginUrl);

            var username = await response.Content.ReadAsStringAsync();
            UserSettings.Default.UserName = username;
            UserSettings.Default.Save();

            if (response.StatusCode.ToString() == "OK")
            {
                new MainWindow().Show();
                Window mainWindow = Application.Current.MainWindow;
                mainWindow.Close();
            }
        }

        private void LoadIcon()
        {
            FileStream fstream = new FileStream(@"../../../Images/Icon246.jpg", FileMode.Open);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = fstream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            fstream.Close();

            Icon.BeginInit();
            Icon.Source = bitmap;
            Icon.EndInit();
        }

        private void LoadLoginPage()
        {
            LoginPage loginPage = new LoginPage();
            Login_Register_Content.Content = new Frame()
            {
                Content = loginPage
            };
        }
    }
}
