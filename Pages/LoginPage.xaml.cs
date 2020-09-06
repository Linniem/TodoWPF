using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
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
using TaskWpf.Helpers;
using TaskWpf.Properties;
using TaskWpf.ViewModels;
using TaskWpf.Windows;

namespace TaskWpf.Pages
{
    /// <summary>
    /// LoginPage.xaml 的互動邏輯
    /// </summary>
    public partial class LoginPage : Page
    {


        public LoginPage()
        {
            InitializeComponent();
            Create_Button.Background = Brushes.Transparent;
            FocusOnAccountTextBox();
        }

        private void FocusOnAccountTextBox()
        {
            accountTextBox.Focus();
        }

        private async Task LoginAction()
        {
            if (accountTextBox.Text == "" || passwordBoxConfirm.Password == "")
            {
                MessageBox.Show("username and password is required");
                return;
            }

            string loginUrl = ConfigurationManager.AppSettings["HostApiUrl"].ToString() + "signin";
            var contentBody = JsonConvert.SerializeObject(new LoginViewModel
            {
                UserName = accountTextBox.Text,
                Password = passwordBoxConfirm.Password
            });

            var response = await WebApiHelper.SendRequestAsync(HttpMethod.Post, loginUrl, contentBody);

            if (response.StatusCode.ToString() == "OK")
            {
                UserSettings.Default.UserName = accountTextBox.Text;
                UserSettings.Default.Save();

                WebApiHelper.UpdateJwtToken(await response.Content.ReadAsStringAsync());
                new MainWindow().Show();
                Window mainWindow = Application.Current.MainWindow;
                mainWindow.Close();
            }
            else
            {
                MessageBox.Show("Log in failed.");
            }

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await LoginAction();
        }

        private async void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                await LoginAction();
            }
        }

        private void Create_Button_Click(object sender, RoutedEventArgs e)
        {
            RegisterPage registerPage = new RegisterPage();
            this.Content = new Frame()
            {
                Content = registerPage
            };
        }
    }
}
