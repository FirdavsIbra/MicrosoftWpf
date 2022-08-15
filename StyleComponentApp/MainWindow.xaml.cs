using Newtonsoft.Json;
using StyleComponentApp.Models;
using StyleComponentApp.UI.Controls;
using StyleComponentApp.UI.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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

namespace StyleComponentApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private IEnumerable<User> allUsers;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread( async () =>
            {
                this.Dispatcher.Invoke(() => UsersList.Items.Clear());

                allUsers = await GetAllUsers();

                await LoadUsers(allUsers);
            });

            thread.Start();

        }

        private async Task<IEnumerable<User>> GetAllUsers()
        {
            string username = "admin";
            string password = "12345";
            string encoded = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encoded);
            var response = await httpClient.GetAsync("https://talabamiz.uz/api/students");
            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IEnumerable<User>>(content);
        }

        private async Task LoadUsers(IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                await this.Dispatcher.InvokeAsync(() =>
                {
                    PrivateChat privateChat = new PrivateChat();
                    privateChat.NameTxt.Text = user.FirstName + " " + user.LastName;
                    privateChat.LastMsgTxt.Text = user.LastMessage;
                    privateChat.NewMsgCountTxt.Text = (new Random().Next(1, 10)).ToString();
                    privateChat.DateTimeTxt.Text = DateTime.UtcNow.ToString("HH:mm");
                    privateChat.UserImg.ImageSource = user.Image is not null
                        ? new BitmapImage(new Uri("https://talabamiz.uz/" + user.Image.path))
                        : new BitmapImage(new Uri("https://talabamiz.uz/Images//99daf8ac38de4433aa36a61baf4c9c4d.png"));

                    UsersList.Items.Add(privateChat);
                });
            }
        }

        private Thread thread;
        private void SearchTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            UsersList.Items.Clear();

            string text = SearchTxt.Text.ToLower();

            var users = allUsers.Where(p => p.FirstName.ToLower().Contains(text)
                || p.LastName.ToLower().Contains(text)
                || p.LastMessage.ToLower().Contains(text));

            thread = new Thread(async () =>
            {
                await LoadUsers(users);
            });
            thread.Start();
        }
    }
}
