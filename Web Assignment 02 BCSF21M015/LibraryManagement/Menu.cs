using LibraryDAL;
using System;
// Presentation Layer
namespace LibraryManagement
{
    public class Menu
    {
        private readonly DataAccess _dataAccess;
        public Menu(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public void DisplayMenu()
        {
            Console.WriteLine("\n\tHello! Welcome to Library Management System");
            bool flag = false;

            while (flag == false)
            {
                Console.WriteLine("\n\t1.  Add a new book");
                Console.WriteLine("\t2.  Remove a book");
                Console.WriteLine("\t3.  Update a book");
                Console.WriteLine("\t4.  Register a new borrower");
                Console.WriteLine("\t5.  Update a borrower");
                Console.WriteLine("\t6.  Delete a borrower");
                Console.WriteLine("\t7.  Borrow a book");
                Console.WriteLine("\t8.  Return a book");
                Console.WriteLine("\t9.  Retrieves a specific book by ID");
                Console.WriteLine("\t10. Search for books by title, author, or genre");
                Console.WriteLine("\t11. View all books");
                Console.WriteLine("\t12. View borrowed books by a specific borrower");
                Console.WriteLine("\t13. View all borrowers");
                Console.WriteLine("\t14. View all transactions");
                Console.WriteLine("\t15. Exit the application");
                Console.Write("\n\tSelect: ");
                // string indicates that the input variable can hold a string or a null value
                string input = Console.ReadLine();
                if (input == null)
                {
                    Console.WriteLine("\n\tInput cannot be null. Please enter a valid input.");
                    return;
                }

                int choice;
                bool isValid = int.TryParse(input, out choice);

                if (!isValid || choice <= 0 || choice > 16)
                {
                    flag = false;
                    Console.WriteLine("\n\tInvalid choice. Please enter a number between 1 and 13.");
                }
                else
                {
                    switch (choice)
                    {
                        case 1:
                            addbook();
                            break;
                        case 2:
                            RemoveBook();
                            break;
                        case 3:
                            UpdateBook();
                            break;
                        case 4:
                            registerNewBorrower();
                            break;
                        case 5:
                            UpdateBorrower();
                            break;
                        case 6:
                            deleteBorrower();
                            break;
                        case 7:
                            borrowBook();
                            break;
                        case 8:
                            ReturnBook();
                            break;
                        case 9:
                            GetBookById();
                            break;
                        case 10:
                            SearchBooks();
                            break;
                        case 11:
                            _dataAccess.ViewAllBooks();
                            break;
                        case 12:
                            ViewBorrowedBooksByBorrower();
                            break;
                        case 13:
                            _dataAccess.GetAllBorrowers();
                            break;
                        case 14:
                            _dataAccess.ViewAllTransactions();
                            break;
                        case 15:
                            Console.WriteLine("\n\tApplication exit successfully.");
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("\tInvalid option. Try again.");
                            break;
                    }
                }
            }
        }
        private void addbook()
        {
            //int bookId = 0;
            string title, genre, author;

            // Input title
            Console.Write("\tEnter title: ");
            title = Console.ReadLine();
            while (string.IsNullOrEmpty(title))
            {
                Console.Write("\tEnter valid title: ");
                title = Console.ReadLine();
            }

            // Input genre
            Console.Write("\tEnter genre: ");
            genre = Console.ReadLine();
            while (string.IsNullOrEmpty(genre))
            {
                Console.Write("\tEnter valid genre: ");
                genre = Console.ReadLine();
            }

            // Input author name
            Console.Write("\tEnter author name: ");
            author = Console.ReadLine();
            while (string.IsNullOrEmpty(author))
            {
                Console.Write("\tEnter valid author name: ");
                author = Console.ReadLine();
            }

            Book book = new Book
            {
                //BookId = bookId,
                Title = title,
                Author = author,
                Genre = genre,
                IsAvailable = true
            };
            _dataAccess.AddBook(book);
        }
        private void GetBookById()
        {
            int bookId;
            Console.Write("\n\tEnter search id (number): ");
            while (!int.TryParse(Console.ReadLine(), out bookId) || bookId <= 0)
            {
                Console.Write("\n\tEnter valid book id (number): ");
            }

            var book = _dataAccess.GetBookById(bookId);
            if (book != null)
            {
                Console.WriteLine($"\tID: {book.BookId}, Title: {book.Title}, Author: {book.Author}, Genre: {book.Genre}, Available: {book.IsAvailable}");
            }
            else
            {
                Console.WriteLine("\tNo book found.");
            }
        }
        private void RemoveBook()
        {
            int bookid = 0;
            Console.Write("\n\tEnter book id (number) to remove a book: ");
            while (!int.TryParse(Console.ReadLine(), out bookid) || bookid <= 0)
            {
                Console.Write("\n\tEnter valid book id (number): ");
            }
            var book = _dataAccess.GetBookById(bookid);
            if (book == null || book.BookId != bookid)
            {
                Console.WriteLine("\n\tBook not found");
            }
            else
            {
                _dataAccess.RemoveBook(bookid);
            }
        }
        private void SearchBooks()
        {
            Console.Write("\n\tEnter search query (title, author, or genre): ");
            string query = null;
            query = Console.ReadLine();
            // Ensure query is not null or empty
            if (string.IsNullOrEmpty(query))
            {
                Console.WriteLine("\tSearch query cannot be empty.");
                return;
            }
            var books = _dataAccess.SearchBooks(query);
            if (books.Count > 0)
            {
                foreach (var book in books)
                {
                    Console.WriteLine($"\tID: {book.BookId}, Title: {book.Title}, Genre: {book.Genre}, Author: {book.Author}, Available: {book.IsAvailable}");
                }
            }
            else
            {
                Console.WriteLine("\tNo books found.");
            }
        }
        private void UpdateBook()
        {
            Console.Write("\n\tEnter book id (number): ");
            int bookId;
            while (!int.TryParse(Console.ReadLine(), out bookId) || bookId <= 0)
            {
                Console.Write("\n\tEnter valid book id (number): ");
            }

            var book = _dataAccess.GetBookById(bookId);
            if (book != null)
            {
                Console.Write("\tEnter new Title: ");
                string title = Console.ReadLine();
                Console.Write("\tEnter new Genre: ");
                string genre = Console.ReadLine();
                Console.Write("\tEnter new Author: ");
                string author = Console.ReadLine();
                if (!string.IsNullOrEmpty(title)) book.Title = title;
                if (!string.IsNullOrEmpty(author)) book.Author = author;
                if (!string.IsNullOrEmpty(genre)) book.Genre = genre;

                _dataAccess.UpdateBook(book);
                Console.WriteLine("\n\tBook updated successfully!");
            }
            else
            {
                Console.WriteLine("\n\tBook not found.");
            }
        }
        private void registerNewBorrower()
        {
            string name, email;

            // Input name
            Console.Write("\tEnter name: ");
            name = Console.ReadLine();
            while (string.IsNullOrEmpty(name))
            {
                Console.Write("\tEnter valid name: ");
                name = Console.ReadLine();
            }

            // Input email
            Console.Write("\tEnter email: ");
            email = Console.ReadLine();
            while (string.IsNullOrEmpty(email))
            {
                Console.Write("\tEnter valid email: ");
                email = Console.ReadLine();
            }

            Borrower borrower = new Borrower
            {
                Name = name,
                Email = email
            };
            _dataAccess.RegisterBorrower(borrower);
        }
        private void UpdateBorrower()
        {
            Console.Write("\n\tEnter Borrower ID to update: ");
            int borrowerId;
            while (!int.TryParse(Console.ReadLine(), out borrowerId) || borrowerId <= 0)
            {
                Console.Write("\n\tEnter a valid borrower id (number): ");
            }
            var borrower = _dataAccess.GetBorrowerById(borrowerId);
            if (borrower != null)
            {
                Console.Write("\tEnter new Name: ");
                string name = Console.ReadLine();
                if (!string.IsNullOrEmpty(name)) borrower.Name = name;

                string email;
                do
                {
                    Console.Write("\tEnter new Email: ");
                    email = Console.ReadLine();
                } while (string.IsNullOrEmpty(email) || !_dataAccess.IsValidEmail(email));

                borrower.Email = email;

                _dataAccess.UpdateBorrower(borrower);
                Console.WriteLine("\n\tBorrower updated successfully!");
            }
            else
            {
                Console.WriteLine("\n\tBorrower not found.");
            }
        }

        private void deleteBorrower()
        {
            Console.Write("\n\tEnter borrower id (number) to remove: ");
            int borrowerId;
            while (!int.TryParse(Console.ReadLine(), out borrowerId) || borrowerId <= 0)
            {
                Console.Write("\n\tEnter valid borrower id (number): ");
            }

            var borrower = _dataAccess.GetBorrowerById(borrowerId);
            if (borrower == null || borrower.BorrowerId != borrowerId)
            {
                Console.WriteLine("\n\tBorrower not found.");
            }
            else
            {
                _dataAccess.DeleteBorrower(borrowerId);
            }
        }

        private bool IsBookIDAvailable(int bookId)
        {
            var book = _dataAccess.GetBookById(bookId);
            return book != null && book.IsAvailable;
        }

        private void borrowBook()
        {

            Console.Write("\tEnter Book ID: ");
            int bookId = int.Parse(Console.ReadLine() ?? "0");
            while (bookId <= 0)
            {
                Console.Write("\n\tEnter valid book id (number): ");
                bookId = int.Parse(Console.ReadLine() ?? "0");
            }

            // Check if the book is available
            if (!IsBookIDAvailable(bookId))
            {
                Console.WriteLine("\n\tBook not available in library");
                return;
            }

            Console.Write("\tEnter Borrower ID: ");
            int borrowerId = int.Parse(Console.ReadLine() ?? "0");
            while (borrowerId <= 0)
            {
                Console.Write("\n\tEnter valid borrower id (number): ");
                borrowerId = int.Parse(Console.ReadLine() ?? "0");
            }

            if (!_dataAccess.IsBorrowerRegistered(borrowerId))
            {
                Console.WriteLine("\n\tThis borrower id is not registered here!!");
                Console.WriteLine("\tHe/She must be registered here firstly!");
                registerNewBorrower();
            }
            var book = _dataAccess.GetBookById(bookId);
            if (book != null && book.IsAvailable)
            {
                var transaction = new Transaction
                {
                    //TransactionId = transactionId,
                    BookId = bookId,
                    BorrowerId = borrowerId,
                    Date = DateTime.Now,
                    IsBorrowed = true
                };

                book.IsAvailable = false;
                _dataAccess.UpdateBook(book);
                _dataAccess.RecordTransaction(transaction);
                Console.WriteLine("\n\tBook borrowed successfully!");
            }
            else
            {
                Console.WriteLine("\n\tBook not available or not found.");
            }
        }
        private void ReturnBook()
        {
            Console.Write("\tEnter Book ID: ");
            int bookId = int.Parse(Console.ReadLine() ?? "0");
            while (bookId <= 0)
            {
                Console.Write("\n\tEnter valid book id (number): ");
                bookId = int.Parse(Console.ReadLine() ?? "0");
            }
            Console.Write("\tEnter Borrower ID: ");
            int borrowerId = int.Parse(Console.ReadLine() ?? "0");
            while (borrowerId <= 0)
            {
                Console.Write("\n\tEnter valid borrower id (number): ");
                borrowerId = int.Parse(Console.ReadLine() ?? "0");
            }
            int transactionId = _dataAccess.GetTransactionById(bookId, borrowerId);
            var book = _dataAccess.GetBookById(bookId);
            if (book != null && !book.IsAvailable && transactionId != -1)
            {
                var transaction = new Transaction
                {
                    TransactionId = transactionId,
                    BookId = bookId,
                    BorrowerId = borrowerId,
                    Date = DateTime.Now,
                    IsBorrowed = false
                };


                book.IsAvailable = true;
                _dataAccess.UpdateBook(book);
                _dataAccess.RecordTransaction(transaction);

                Console.WriteLine("\tBook returned successfully!");
            }
            else
            {
                Console.WriteLine("\tBook not found or was not borrowed.");
            }
            Console.ReadKey();
        }

        private void ViewBorrowedBooksByBorrower()
        {
            Console.Write("\n\tEnter Borrower ID: ");
            int borrowerId = int.Parse(Console.ReadLine() ?? "0");

            var transactions = _dataAccess.GetBorrowedBooksByBorrower(borrowerId);
            if (transactions.Count > 0)
            {
                foreach (var transaction in transactions)
                {
                    var book = _dataAccess.GetBookById(transaction.BookId);
                    if (book != null && transaction.IsBorrowed)
                    {
                        Console.WriteLine($"\tID: {book.BookId}, Title: {book.Title}, Author: {book.Author}, Date Borrowed: {transaction.Date}");
                    }
                }
            }
            else
            {
                Console.WriteLine("\tNo borrowed books found for this borrower.");
            }
        }
    }
}
