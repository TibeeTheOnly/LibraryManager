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


namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for UserListPage.xaml
    /// </summary>
    public partial class UserListPage : Page
    {
        private User LoggedInUser;
        private List<User> Users;
        public UserListPage(User loggedInUser)
        {
            InitializeComponent();
            LoggedInUser = loggedInUser;

            Users = File.ReadAllLines("users.txt").Select(User.FromString).ToList();
            UserListBox.ItemsSource = Users.Select(u => $"{u.Username} - {u.Role}");
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (LoggedInUser.Role != "Admin")
            {
                MessageBox.Show("Nincs jogosultságod más felhasználók módosítására",
                    "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (UserListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Kérlek válassz egy felhasználót!",
                    "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selectedUser = Users[UserListBox.SelectedIndex];
            selectedUser.Role = selectedUser.Role == "User" ? "Admin" : "User";

            if (LoggedInUser.Username == selectedUser.Username)
            {
                LoggedInUser.Role = selectedUser.Role;
            }

            File.WriteAllLines("users.txt", Users.Select(u => u.ToString()));
            UserListBox.ItemsSource = Users.Select(u => $"{u.Username} - {u.Role}");
            MessageBox.Show("Jogosultság módosítva!", "Siker", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void UserListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserListBox.SelectedItem == null)
            {
                return;
            }

            if (LoggedInUser.Role != "Admin")
            {
                MessageBox.Show("Nincs jogosultságod más felhasználók módosítására",
                    "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                UserListBox.SelectedItem = null;
            }
        }
    }
}
