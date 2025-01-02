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
    /// Interaction logic for BookListPage.xaml
    /// </summary>
    public partial class BookListPage : Page
    {
        private User LoggedInUser;
        private List<User> Users;
        internal List<Book> Books;
        public BookListPage(User loggedInUser)
        {
            InitializeComponent();
            LoggedInUser = loggedInUser;
            Books = new List<Book>();
            CheckIfAdmin();
            LoadBooks();
        }

        public void CheckIfAdmin()
        {
            if (LoggedInUser != null)
            {
                if (LoggedInUser.Role == "Admin")
                {
                    EditBookButton.IsEnabled = true;
                    DeleteBookButton.IsEnabled = true;
                    ExportBooksButton.IsEnabled = true;
                    ImportBooksButton.IsEnabled = true;
                }
            }
        }

        private void ChangeBookButtonClick(object sender, RoutedEventArgs e)
        {
            string selectedBook = BookListBox.SelectedItem.ToString();
            Book bookToEdit = null;
            foreach (Book book in Books)
            {
                if (selectedBook.Contains(book.Title))
                {
                    bookToEdit = book;
                    break;
                }
            }
            string filePath = "books.txt";

            // Create a new window for book input
            Window inputWindow = new Window
            {
                Title = "Könyv Szerkesztése",
                Width = 300,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            // Create a stack panel to hold the input fields
            StackPanel stackPanel = new StackPanel();

            // Create input fields

                TextBlock titleTextBlock = new TextBlock { Text = "Könyvcím *", Margin = new Thickness(10) };
                TextBox titleTextBox = new TextBox { Margin = new Thickness(10), Text = bookToEdit.Title };
                TextBlock authorTextBlock = new TextBlock { Text = "Szerző neve *", Margin = new Thickness(10) };
                TextBox authorTextBox = new TextBox { Margin = new Thickness(10), Text = bookToEdit.Author };
                TextBlock yearTextBlock = new TextBlock { Text = "Kiadási év *", Margin = new Thickness(10) };
                TextBox yearTextBox = new TextBox { Margin = new Thickness(10), Text = bookToEdit.Year.ToString() };
                TextBlock categoryTextBlock = new TextBlock { Text = "Kategória *", Margin = new Thickness(10) };
                ComboBox categoryComboBox = new ComboBox { Margin = new Thickness(10) };
                categoryComboBox.Items.Add("regény");
                categoryComboBox.Items.Add("tudományos");
                categoryComboBox.Items.Add("ismeretterjesztő");
                categoryComboBox.Items.Add("egyéb");
                categoryComboBox.SelectedItem = bookToEdit.Genre; // Set the selected category

            // Create a button to submit the data
            Button submitButton = new Button { Content = "Mentés", Margin = new Thickness(10) };
            submitButton.Click += (s, args) =>
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(titleTextBox.Text) || string.IsNullOrWhiteSpace(authorTextBox.Text))
                {
                    MessageBox.Show("Könyvcím és szerző neve kötelező mező.");
                    return;
                }
                else if (!int.TryParse(yearTextBox.Text, out int year) || year < 1800 || year > DateTime.Now.Year)
                {
                    MessageBox.Show("Kiadási év érvénytelen. Kérjük, adjon meg egy számot 1800 és az aktuális év között.");
                    return;
                }
                else if (categoryComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Kategória kötelező mező.");
                    return;
                }
                else
                {
                    DeleteBook(selectedBook, filePath); // Delete the old book
                    EditBook(new Book(titleTextBox.Text, authorTextBox.Text, categoryComboBox.SelectedItem.ToString(), year));
                }

                inputWindow.Close();
            };

            // Add controls to the stack panel
            stackPanel.Children.Add(titleTextBlock);
            stackPanel.Children.Add(titleTextBox);
            stackPanel.Children.Add(authorTextBlock);
            stackPanel.Children.Add(authorTextBox);
            stackPanel.Children.Add(yearTextBlock);
            stackPanel.Children.Add(yearTextBox);
            stackPanel.Children.Add(categoryTextBlock);
            stackPanel.Children.Add(categoryComboBox);
            stackPanel.Children.Add(submitButton);

            // Set the content of the window
            inputWindow.Content = stackPanel;

            // Show the input window
            inputWindow.ShowDialog();

            RefreshBookList();
        }

        private void DeleteBookButtonClick(object sender, RoutedEventArgs e)
        {
            string selectedBook = BookListBox.SelectedItem.ToString();
            string filePath = "books.txt";

            DeleteBook(selectedBook, filePath);
            RefreshBookList();
        }

        private void DeleteBook(string selectedBook, string filePath)
        {
            foreach (Book book in Books)
            {
                if (selectedBook.Contains(book.Title))
                {
                    Books.Remove(book);
                    break;
                }
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Book book in Books)
                {
                    writer.WriteLine($"{book.Title};{book.Author};{book.Genre};{book.Year}");
                }
            }
            RefreshBookList();
        }

        private void AddBookButtonClick(object sender, RoutedEventArgs e)
        {
            // Create a new window for book input
            Window inputWindow = new Window
            {
                Title = "Könyv Hozzáadása",
                Width = 300,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            // Create a stack panel to hold the input fields
            StackPanel stackPanel = new StackPanel();

            // Create input fields
            TextBlock titleTextBlock = new TextBlock { Text = "Könyvcím *", Margin = new Thickness(10) };
            TextBox titleTextBox = new TextBox { Margin = new Thickness(10) };
            TextBlock authorTextBlock = new TextBlock { Text = "Szerző neve *", Margin = new Thickness(10) };
            TextBox authorTextBox = new TextBox { Margin = new Thickness(10) };
            TextBlock yearTextBlock = new TextBlock { Text = "Kiadási év *", Margin = new Thickness(10) };
            TextBox yearTextBox = new TextBox { Margin = new Thickness(10) };
            TextBlock categoryTextBlock = new TextBlock { Text = "Kategória *", Margin = new Thickness(10) };
            ComboBox categoryComboBox = new ComboBox { Margin = new Thickness(10) };
            categoryComboBox.Items.Add("regény");
            categoryComboBox.Items.Add("tudományos");
            categoryComboBox.Items.Add("ismeretterjesztő");
            categoryComboBox.Items.Add("egyéb");

            // Create a button to submit the data
            Button submitButton = new Button { Content = "Hozzáadás", Margin = new Thickness(10) };
            submitButton.Click += (s, args) =>
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(titleTextBox.Text) || string.IsNullOrWhiteSpace(authorTextBox.Text))
                {
                    MessageBox.Show("Könyvcím és szerző neve kötelező mező.");
                    return;
                }
                else if (!int.TryParse(yearTextBox.Text, out int year) || year < 1800 || year > DateTime.Now.Year)
                {
                    MessageBox.Show("Kiadási év érvénytelen. Kérjük, adjon meg egy számot 1800 és az aktuális év között.");
                    return;
                }
                else if (categoryComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Kategória kötelező mező.");
                    return;
                }
                else
                {
                    AddBook(new Book(titleTextBox.Text, authorTextBox.Text, categoryComboBox.SelectedItem.ToString(), year));                    
                }

                // Here you can add the book to your data source
                // Example: AddBook(new Book(titleTextBox.Text, authorTextBox.Text, year, categoryComboBox.SelectedItem.ToString()));

                inputWindow.Close();
            };

            // Add controls to the stack panel
            stackPanel.Children.Add(titleTextBlock);
            stackPanel.Children.Add(titleTextBox);
            stackPanel.Children.Add(authorTextBlock);
            stackPanel.Children.Add(authorTextBox);
            stackPanel.Children.Add(yearTextBlock);
            stackPanel.Children.Add(yearTextBox);
            stackPanel.Children.Add(categoryTextBlock);
            stackPanel.Children.Add(categoryComboBox);
            stackPanel.Children.Add(submitButton);

            // Set the content of the window
            inputWindow.Content = stackPanel;

            // Show the input window
            inputWindow.ShowDialog();
        }
        
        void AddBook(Book book)
        {
            // Assuming you have a list to store books
            if (Books == null)
            {
                Books = new List<Book>();
            }
            Books.Add(book);

            // Write the book details to a text file
            string filePath = "books.txt";
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine($"{book.Title};{book.Author};{book.Genre};{book.Year}");
            }

            MessageBox.Show("Könyv hozzáadva: " + book.Title);

            RefreshBookList();
        }

        void EditBook(Book book)
        {
            // Assuming you have a list to store books
            if (Books == null)
            {
                Books = new List<Book>();
            }
            Books.Add(book);

            // Write the book details to a text file
            string filePath = "books.txt";
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine($"{book.Title};{book.Author};{book.Genre};{book.Year}");
            }

            MessageBox.Show("Könyv hozzáadva: " + book.Title);

            RefreshBookList();
        }

        public void RefreshBookList()
        {
            if (Books == null)
            {
                return;
            }

            BookListBox.Items.Clear();

            foreach (Book book in Books)
            {
                // Add the book to the list box
                BookListBox.Items.Add($"{book.Title} - {book.Author} - {book.Genre} - {book.Year}");
            }
        }

        public void LoadBooks()
        {
            // Read the books from the text file
            string filePath = "books.txt";
            if (!File.Exists(filePath))
            {
                return;
            }

            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(';');
                if (parts.Length == 4)
                {
                    Books.Add(new Book(parts[0], parts[1], parts[2], int.Parse(parts[3])));
                }
            }

            RefreshBookList();
        }

        public void ExportBooks()
        {
            string filePath = "books.txt";
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Book book in Books)
                {
                    writer.WriteLine($"{book.Title};{book.Author};{book.Genre};{book.Year}");
                }
            }
            MessageBox.Show("Könyvek exportálva " + filePath);
        }

        private void ImportBooksClick(object sender, RoutedEventArgs e)
        {
            LoadBooks();
        }

        private void ExportBooksClick(object sender, RoutedEventArgs e)
        {
            ExportBooks();
        }

        private void FilterBooksClick(object sender, RoutedEventArgs e)
        {
            string selectedCategory = CategoryComboBox.Text;
            if (selectedCategory == "mind")
            {
                RefreshBookList();
                return;
            }
            else
            {
                BookListBox.Items.Clear();
                foreach (Book book in Books)
                {
                    if (book.Genre == selectedCategory)
                    {
                        BookListBox.Items.Add($"{book.Title} - {book.Author} - {book.Genre} - {book.Year}");
                    }
                }
            }
        }
    }
}
