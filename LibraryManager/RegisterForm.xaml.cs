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
using System.Windows.Shapes;

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for RegisterForm.xaml
    /// </summary>
    public partial class RegisterForm : Window
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void RegisterUserClick(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string name = NameTextBox.Text;
            string email = EmailTextBox.Text;
            bool termsAccepted = TermsCheckBox.IsChecked == true;

            //minden mező ki van e töltve

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)
                || string.IsNullOrWhiteSpace(confirmPassword) || string.IsNullOrWhiteSpace(name)
                || string.IsNullOrWhiteSpace(email) || !termsAccepted)
            {
                MessageBox.Show("Minden mezőt ki kell tölteni és el kell fogadni a felhasználói feltételeket",
                    "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("A jelszavak nem egyeznek meg!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsPasswordStrong(password))
            {
                MessageBox.Show("A jelszónak legalább 8 karakterből kell állnia," +
                    " tartalmaznia kell számot és speciális karektert",
                    "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            User user = new User(username, password, name, email);

            //Felhasználó mentése fájlba
            if (File.Exists("users.txt"))
            {                
                if (new FileInfo("users.txt").Length != 0)
                {
                    using (StreamWriter writer = new StreamWriter("users.txt", true))
                    {
                        writer.WriteLine(user.ToString());
                    }
                }
                else
                {
                    user.Role = "Admin";
                    using (StreamWriter writer = new StreamWriter("users.txt", true))
                    {
                        writer.WriteLine(user.ToString());
                    }
                }
            }
            else
            {
                user.Role = "Admin";
                using (StreamWriter writer = new StreamWriter("users.txt", true))
                {
                    writer.WriteLine(user.ToString());
                }
            }

            MessageBox.Show("Sikeres regisztráció!", "Regisztráció", MessageBoxButton.OK,
                MessageBoxImage.Information);

            //Visszatérés a főoldalra
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        //8 vagy annál több karakter, szám legyen benne és speciális karakter
        private bool IsPasswordStrong(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => !char.IsLetterOrDigit(ch));
        }
    }
}
