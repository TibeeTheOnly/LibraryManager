using System.IO;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenRegisterForm(object sender, RoutedEventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.Show();
            this.Close();
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (File.Exists("users.txt"))
            {
                var users = File.ReadAllLines("users.txt")
                                .Select(line => User.FromString(line))
                                .ToList();

                var matchedUser = users.FirstOrDefault(user => user.Username == username
                && user.Password == password);

                if (matchedUser != null)
                {
                    MessageBox.Show($"Sikeres bejelentkezés!\nÜdvözölünk, {matchedUser.Name}",
                        "Bejelentkezés", MessageBoxButton.OK, MessageBoxImage.Information);
                    //TODO: tovább a következő ablakra
                    MainDashboardWindow mainDashboardWindow = new MainDashboardWindow(matchedUser);
                    mainDashboardWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Hibás felhasználónév vagy jelszó",
                        "Bejelentkezés", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Nincs regisztrált felhasználó",
                        "Bejelentkezés", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}