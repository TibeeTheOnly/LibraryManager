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
using System.Windows.Shapes;

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for MainDashboardWindow.xaml
    /// </summary>
    public partial class MainDashboardWindow : Window
    {
        private User LoggedInUser;
        public MainDashboardWindow(User loggedInUser)
        {
            InitializeComponent();
            LoggedInUser = loggedInUser;
            MainFrame.Content = new ProfilePage(LoggedInUser);
            CheckIfAdmin();
        }

        private void NavigateToProfilePageClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ProfilePage(LoggedInUser);
        }

        private void NavigateToUsersListPageClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new UserListPage(LoggedInUser);
        }

        private void LogoutClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void NavigateToBookListPageClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new BookListPage(LoggedInUser);
        }

        public void CheckIfAdmin()
        {
            if (LoggedInUser != null)
            {
                if (LoggedInUser.Role == "Admin")
                {
                    UserListPageButton.IsEnabled = true;
                }
            }
        }
    }
}
