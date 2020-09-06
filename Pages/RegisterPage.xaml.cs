using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskWpf.Helpers;
using TaskWpf.Properties;
using TaskWpf.ViewModels;
using TaskWpf.Windows;

namespace TaskWpf.Pages
{
    /// <summary>
    /// RegisterPage.xaml 的互動邏輯
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
            Change_To_Login_Button.Background = Brushes.Transparent;
            FocusOnAccountTextBox();
        }

        private void FocusOnAccountTextBox()
        {
            UserNameTextBox.Focus();
        }

        private async Task RegisterAction()
        {
            if (UserNameTextBox.Text == "" || passwordBoxConfirm.Password == "")
            {
                MessageBox.Show("username and password is required");
                return;
            }

            string loginUrl = ConfigurationManager.AppSettings["HostApiUrl"].ToString() + "register";
            var contentBody = JsonConvert.SerializeObject(new LoginViewModel
            {
                UserName = UserNameTextBox.Text,
                Password = passwordBoxConfirm.Password
            });

            var response = await WebApiHelper.SendRequestAsync(HttpMethod.Post, loginUrl, contentBody);

            if (response.StatusCode.ToString() == "OK")
            {
                UserSettings.Default.UserName = UserNameTextBox.Text;
                UserSettings.Default.Save();

                WebApiHelper.UpdateJwtToken(await response.Content.ReadAsStringAsync());
                new MainWindow().Show();
                Window mainWindow = Application.Current.MainWindow;
                mainWindow.Close();
            }
            else
            {
                MessageBox.Show("Register failed.");
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                RegisterAction();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RegisterAction();
        }

        private void Create_Button_Click(object sender, RoutedEventArgs e)
        {
            LoginPage loginPage = new LoginPage();
            this.Content = new Frame()
            {
                Content = loginPage
            };
        }

    }
}
