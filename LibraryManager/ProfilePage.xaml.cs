using System;
using System.Collections.Generic;
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
using System.IO;

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        private User LoggedInUser;
        public ProfilePage(User loggedInUser)
        {
            InitializeComponent();
            LoggedInUser = loggedInUser;
            UsernameTextBox.Text = LoggedInUser.Username;
            NameTextBox.Text = LoggedInUser.Name;
            EmailTextBox.Text = LoggedInUser.Email;
            RoleTextBox.Text = LoggedInUser.Role;
        }

        private void SaveProfile(object sender, RoutedEventArgs e)
        {
            LoggedInUser.Name = NameTextBox.Text;
            LoggedInUser.Email = EmailTextBox.Text;

            //felhasználó adatainak frissítése a fájlban
            var users = File.ReadAllLines("users.txt").Select(User.FromString).ToList();
            var userIndex = users.FindIndex(u => u.Username == LoggedInUser.Username);
            if (userIndex != -1)
            {
                users[userIndex] = LoggedInUser;
                File.WriteAllLines("users.txt", users.Select(u => u.ToString()));
                MessageBox.Show("Adatok mentve!", "Siker", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
    }
}
