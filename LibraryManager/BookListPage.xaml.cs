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
            if(BookListBox.SelectedItem == null)
            {
                MessageBox.Show("Kérjük, válasszon ki egy könyvet a szerkesztéshez.");
                return;
            }
            
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

            Window inputWindow = new Window
            {
                Title = "Könyv Hozzáadása",
                Width = 350,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F0F0"))
            };

            StackPanel stackPanel = new StackPanel { Margin = new Thickness(20) };

            Style textBlockStyle = new Style(typeof(TextBlock))
            {
                Setters = {
                    new Setter(TextBlock.FontSizeProperty, 14d),
                    new Setter(TextBlock.FontWeightProperty, FontWeights.SemiBold),
                    new Setter(TextBlock.MarginProperty, new Thickness(0, 10, 0, 5))
                }
            };

            Style textBoxStyle = new Style(typeof(TextBox))
            {
                Setters = {
                    new Setter(TextBox.FontSizeProperty, 14d),
                    new Setter(TextBox.PaddingProperty, new Thickness(5)),
                    new Setter(TextBox.MarginProperty, new Thickness(0, 0, 0, 10))
                }
            };

            Style comboBoxStyle = new Style(typeof(ComboBox))
            {
                Setters = {
                    new Setter(ComboBox.FontSizeProperty, 14d),
                    new Setter(ComboBox.PaddingProperty, new Thickness(5)),
                    new Setter(ComboBox.MarginProperty, new Thickness(0, 0, 0, 10))
                }
            };

            Style buttonStyle = new Style(typeof(Button))
            {
                Setters = {
                    new Setter(Button.FontSizeProperty, 14d),
                    new Setter(Button.FontWeightProperty, FontWeights.SemiBold),
                    new Setter(Button.PaddingProperty, new Thickness(15, 10, 15, 10)),
                    new Setter(Button.BackgroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007ACC"))),
                    new Setter(Button.ForegroundProperty, Brushes.White),
                    new Setter(Button.BorderThicknessProperty, new Thickness(0)),
                    new Setter(Button.CursorProperty, Cursors.Hand),
                    new Setter(Button.MarginProperty, new Thickness(0, 20, 0, 0))
                }
            };

            TextBox titleTextBox = new TextBox { Style = textBoxStyle, Text = bookToEdit.Title };
            TextBox authorTextBox = new TextBox { Style = textBoxStyle, Text = bookToEdit.Author };
            TextBox yearTextBox = new TextBox { Style = textBoxStyle, Text = bookToEdit.Year.ToString() };

            stackPanel.Children.Add(new TextBlock { Text = "Könyvcím *", Style = textBlockStyle });
            stackPanel.Children.Add(titleTextBox);

            stackPanel.Children.Add(new TextBlock { Text = "Szerző neve *", Style = textBlockStyle });
            stackPanel.Children.Add(authorTextBox);

            stackPanel.Children.Add(new TextBlock { Text = "Kiadási év *", Style = textBlockStyle });
            stackPanel.Children.Add(yearTextBox);

            stackPanel.Children.Add(new TextBlock { Text = "Kategória *", Style = textBlockStyle });
            ComboBox categoryComboBox = new ComboBox { Style = comboBoxStyle };
            categoryComboBox.Items.Add("regény");
            categoryComboBox.Items.Add("tudományos");
            categoryComboBox.Items.Add("ismeretterjesztő");
            categoryComboBox.Items.Add("egyéb");
            stackPanel.Children.Add(categoryComboBox);

            // Create a button to submit the data
            Button submitButton = new Button { Content = "Szerkesztés", Style = buttonStyle };
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
                    DeleteBook(selectedBook); // Delete the old book
                    EditBook(new Book(titleTextBox.Text, authorTextBox.Text, categoryComboBox.SelectedItem.ToString(), year));
                }

                inputWindow.Close();
            };

            stackPanel.Children.Add(submitButton);

            inputWindow.Content = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(20),
                Child = stackPanel
            };

            inputWindow.ShowDialog();
        }

        private void DeleteBookButtonClick(object sender, RoutedEventArgs e)
        {
            if(BookListBox.SelectedItem == null)
            {
                MessageBox.Show("Kérjük, válasszon ki egy könyvet a törléshez.");
                return;
            }

            string selectedBook = BookListBox.SelectedItem.ToString();

            DeleteBook(selectedBook);
            RefreshBookList();
        }

        private void DeleteBook(string selectedBook)
        {
            foreach (Book book in Books)
            {
                if (selectedBook.Contains(book.Title))
                {
                    Books.Remove(book);
                    break;
                }
            }
            RefreshBookList();
        }

        private void AddBookButtonClick(object sender, RoutedEventArgs e)
        {
            Window inputWindow = new Window
            {
                Title = "Könyv Hozzáadása",
                Width = 350,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F0F0"))
            };

            StackPanel stackPanel = new StackPanel { Margin = new Thickness(20) };

            Style textBlockStyle = new Style(typeof(TextBlock))
            {
                Setters = {
                    new Setter(TextBlock.FontSizeProperty, 14d),
                    new Setter(TextBlock.FontWeightProperty, FontWeights.SemiBold),
                    new Setter(TextBlock.MarginProperty, new Thickness(0, 10, 0, 5))
                }
            };

            Style textBoxStyle = new Style(typeof(TextBox))
            {
                Setters = {
                    new Setter(TextBox.FontSizeProperty, 14d),
                    new Setter(TextBox.PaddingProperty, new Thickness(5)),
                    new Setter(TextBox.MarginProperty, new Thickness(0, 0, 0, 10))
                }
            };

            Style comboBoxStyle = new Style(typeof(ComboBox))
            {
                Setters = {
                    new Setter(ComboBox.FontSizeProperty, 14d),
                    new Setter(ComboBox.PaddingProperty, new Thickness(5)),
                    new Setter(ComboBox.MarginProperty, new Thickness(0, 0, 0, 10))
                }
            };

            Style buttonStyle = new Style(typeof(Button))
            {
                Setters = {
                    new Setter(Button.FontSizeProperty, 14d),
                    new Setter(Button.FontWeightProperty, FontWeights.SemiBold),
                    new Setter(Button.PaddingProperty, new Thickness(15, 10, 15, 10)),
                    new Setter(Button.BackgroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007ACC"))),
                    new Setter(Button.ForegroundProperty, Brushes.White),
                    new Setter(Button.BorderThicknessProperty, new Thickness(0)),
                    new Setter(Button.CursorProperty, Cursors.Hand),
                    new Setter(Button.MarginProperty, new Thickness(0, 20, 0, 0))
                }
            };

            TextBox titleTextBox = new TextBox { Style = textBoxStyle };
            TextBox authorTextBox = new TextBox { Style = textBoxStyle };
            TextBox yearTextBox = new TextBox { Style = textBoxStyle };

            stackPanel.Children.Add(new TextBlock { Text = "Könyvcím *", Style = textBlockStyle });
            stackPanel.Children.Add(titleTextBox);

            stackPanel.Children.Add(new TextBlock { Text = "Szerző neve *", Style = textBlockStyle });
            stackPanel.Children.Add(authorTextBox);

            stackPanel.Children.Add(new TextBlock { Text = "Kiadási év *", Style = textBlockStyle });
            stackPanel.Children.Add(yearTextBox);

            stackPanel.Children.Add(new TextBlock { Text = "Kategória *", Style = textBlockStyle });
            ComboBox categoryComboBox = new ComboBox { Style = comboBoxStyle };
            categoryComboBox.Items.Add("regény");
            categoryComboBox.Items.Add("tudományos");
            categoryComboBox.Items.Add("ismeretterjesztő");
            categoryComboBox.Items.Add("egyéb");
            stackPanel.Children.Add(categoryComboBox);

            Button submitButton = new Button { Content = "Hozzáadás", Style = buttonStyle };
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

                inputWindow.Close();
            };
            stackPanel.Children.Add(submitButton);

            inputWindow.Content = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(20),
                Child = stackPanel
            };

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

            MessageBox.Show("Könyv szerkesztve: " + book.Title);

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

            BookListBox.Items.Clear();
            Books.Clear();

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
