using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
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
using TaskWpf.Properties;
using TaskWpf.ViewModels;

namespace TaskWpf.Windows
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private IEnumerable<TaskUnitViewModel> allTasks;

        public MainWindow()
        {
            InitializeComponent();
            RenderUserName();
            TodayListViewItem.IsSelected = true;
            GetAllTasksThenRenderAsync().GetAwaiter();
        }

        private void RenderUserName()
        {
            var userName = UserSettings.Default.UserName;
            userNameText.Text = $"Welcome Back, {userName}";
        }

        private async Task GetAllTasksThenRenderAsync()
        {
            await LoadTasksFromServer();
            RenderAllTasks();
        }

        private async Task LoadTasksFromServer()
        {
            string addTaskUrl = ConfigurationManager.AppSettings["HostApiUrl"].ToString() + "api/task";

            var response = await WebApiHelper.SendRequestAsync(HttpMethod.Get, addTaskUrl);
            if (response.StatusCode.ToString() == "OK")
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                allTasks = JsonConvert.DeserializeObject<IEnumerable<TaskUnitViewModel>>(jsonString);
            }
        }

        private void RenderAllTasks()
        {
            TaskListView.Items.Clear();
            foreach (var taskUnit in allTasks)
            {
                var newListViewItem = new ListViewItem()
                {
                    DataContext = taskUnit.TaskUnitId
                };
                var newTextBlock = new TextBlock
                {
                    Text = taskUnit.TaskName,
                    FontSize = 18,
                    Foreground = new SolidColorBrush(Colors.White),
                    Height = 45,
                    Padding = new Thickness(20, 10, 20, 10)
                };
                if (taskUnit.IsComplete)
                {
                    newTextBlock.TextDecorations.Add(TextDecorations.Strikethrough);
                }
                newListViewItem.Content = newTextBlock;
                newListViewItem.AddHandler(ListViewItem.SelectedEvent, new RoutedEventHandler(TaskUnit_Click));
                TaskListView.Items.Add(newListViewItem);
            }

        }

        private async void TaskUnit_Click(object sender, RoutedEventArgs e)
        {
            ListViewItem listViewItem = e.Source as ListViewItem;
            int taskId = int.Parse(listViewItem.DataContext.ToString());

            string putTaskUrl = ConfigurationManager.AppSettings["HostApiUrl"].ToString() + "api/task";
            var taskUnit = allTasks.FirstOrDefault(x => x.TaskUnitId == taskId);
            taskUnit.IsComplete = !taskUnit.IsComplete;
            var contentBody = JsonConvert.SerializeObject(taskUnit);

            var response = await WebApiHelper.SendRequestAsync(HttpMethod.Put, putTaskUrl, contentBody);
            if (response.StatusCode.ToString() == "OK")
            {
                await GetAllTasksThenRenderAsync();
            }
        }

        private void ListViewItem_Today_Selected(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Today");
        }

        private void ListViewItem_Important_Selected(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Important");
        }

        private void ListViewItem_Planed_Selected(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Planed");
        }

        private void ListViewItem_AssignToMe_Selected(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("AssignToMe");
        }

        private void ListViewItem_NormalTask_Selected(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("NormalTask");
        }

        private void NewTaskTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (NewTaskTextBox.Text == "新增工作")
            {
                NewTaskTextBox.Text = "";
            }
        }

        private void NewTaskTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewTaskTextBox.Text))
            {
                NewTaskTextBox.Text = "新增工作";
            }
        }


        private async void NewTaskTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            string newTaskName = NewTaskTextBox.Text;
            if (e.Key == Key.Return && newTaskName != string.Empty)
            {
                string addTaskUrl = ConfigurationManager.AppSettings["HostApiUrl"].ToString() + "api/task";
                var contentBody = JsonConvert.SerializeObject(new
                {
                    TaskName = newTaskName
                });

                var response = await WebApiHelper.SendRequestAsync(HttpMethod.Post, addTaskUrl, contentBody);
                if (response.StatusCode.ToString() == "OK")
                {
                    NewTaskTextBox.Text = string.Empty;
                    await GetAllTasksThenRenderAsync();
                }

            }
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            UserSettings.Default.JwtToken = string.Empty;
            UserSettings.Default.Save();

            new LoginWindow().Show();
            Window mainWindow = Window.GetWindow(this);
            mainWindow.Close();
        }
    }
}
