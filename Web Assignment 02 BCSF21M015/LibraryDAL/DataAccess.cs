using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace LibraryDAL
{
    public class DataAccess
    {
        private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=LibraryData;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;";
        public bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }
        public void AddBook(Book book)
        {
            string connectionString = _connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string query = "INSERT INTO Books (Title, Author ,Genre, IsAvailable) VALUES (@Title, @Author,@Genre, @IsAvailable)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@BookId", book.BookId);
            cmd.Parameters.AddWithValue("@Title", book.Title);
            cmd.Parameters.AddWithValue("@Author", book.Author);
            cmd.Parameters.AddWithValue("@Genre", book.Genre);
            cmd.Parameters.AddWithValue("@IsAvailable", book.IsAvailable);
            cmd.ExecuteNonQuery();
            con.Close();
            Console.WriteLine("\n\tBook added successfully.");
        }

        public void RemoveBook(int bookId)
        {
            string connectionString = _connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string query = "DELETE FROM Books WHERE BookId = @BookId";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@BookId", bookId);
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            if (rowsAffected > 0)
            {
                //Console.WriteLine("\n\tBook removed successfully.");
            }
            else
            {
                //Console.WriteLine("\n\tBook not found.");
            }
        }

        public void UpdateBook(Book book)
        {
            string connectionString = _connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string query = "UPDATE Books SET Title = @Title, Genre = @Genre, Author = @Author, IsAvailable = @IsAvailable WHERE BookId = @BookId";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@BookId", book.BookId);
            cmd.Parameters.AddWithValue("@Title", book.Title);
            cmd.Parameters.AddWithValue("@Genre", book.Genre);
            cmd.Parameters.AddWithValue("@Author", book.Author);
            cmd.Parameters.AddWithValue("@IsAvailable", book.IsAvailable);
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            if (rowsAffected > 0)
            {
                //Console.WriteLine("\n\tBook updated successfully.");
            }
            else
            {
                // Console.WriteLine("\n\tBook not found.");
            }
        }

        public Book GetBookById(int bookId)
        {
            Book book = null;
            string connectionString = _connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string query = "SELECT * FROM Books WHERE BookId = @BookId";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@BookId", bookId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                book = new Book
                {
                    BookId = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Author = reader.GetString(2),
                    Genre = reader.GetString(3),
                    IsAvailable = reader.GetBoolean(4)
                };
            }
            reader.Close();
            con.Close();
            if (book == null)
            {
                Console.WriteLine("\n\tBook not found.");
            }
            return book;
        }

        public List<Book> SearchBooks(string query)
        {
            List<Book> searchedBooks = new List<Book>();
            string connectionString = _connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string sqlQuery = "SELECT * FROM Books WHERE Title LIKE @Query OR Author LIKE @Query OR Genre LIKE @Query";
            SqlCommand cmd = new SqlCommand(sqlQuery, con);
            cmd.Parameters.AddWithValue("@Query", $"%{query}%");
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Book book = new Book
                {
                    BookId = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Genre = reader.GetString(2),
                    Author = reader.GetString(3),
                    IsAvailable = reader.GetBoolean(4)
                };
                searchedBooks.Add(book);
            }
            reader.Close();
            con.Close();
            return searchedBooks;
        }

        public void ViewAllBooks()
        {
            List<Book> books = GetAllBooks();
            if (books.Count > 0)
            {
                Console.WriteLine("\n\tBooks in Library:");
                foreach (Book book in books)
                {
                    Console.WriteLine($"\tID: {book.BookId}, Title: {book.Title}, Author: {book.Author}, Genre: {book.Genre},  Available: {book.IsAvailable}");
                }
            }
            else
            {
                Console.WriteLine("\n\tNo books available.");
            }
        }

        protected List<Book> GetAllBooks()
        {
            List<Book> books = new List<Book>();
            string connectionString = _connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string query = "SELECT * FROM Books";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Book book = new Book
                {
                    BookId = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Author = reader.GetString(2),
                    Genre = reader.GetString(3),
                    IsAvailable = reader.GetBoolean(4)
                };
                books.Add(book);
            }
            reader.Close();
            con.Close();
            return books;
        }

        public void RegisterBorrower(Borrower borrower)
        {
            string connectionString = _connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string query = "INSERT INTO Borrowers (Name, Email) VALUES (@Name, @Email)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Name", borrower.Name);
            cmd.Parameters.AddWithValue("@Email", borrower.Email);
            cmd.ExecuteNonQuery();
            con.Close();
            Console.WriteLine("\n\tBorrower registered successfully.");
        }

        public void BorrowBook(int bookId, int borrowerId)
        {
            string connectionString = _connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string checkBookQuery = "SELECT IsAvailable FROM Books WHERE BookId = @BookId";
            SqlCommand checkBookCmd = new SqlCommand(checkBookQuery, con);
            checkBookCmd.Parameters.AddWithValue("@BookId", bookId);
            object isAvailableObj = checkBookCmd.ExecuteScalar();

            if (isAvailableObj == null)
            {
                //Console.WriteLine("\n\tBook not found.");
                con.Close();
                return;
            }

            bool isAvailable = Convert.ToBoolean(isAvailableObj);
            if (!isAvailable)
            {
                Console.WriteLine("\n\tBook is currently unavailable.");
                con.Close();
                return;
            }

            string borrowQuery = "INSERT INTO Transactions (BookId, BorrowerId, Date, IsBorrowed) VALUES (@BookId, @BorrowerId, @Date, @IsBorrowed)";
            string updateBookQuery = "UPDATE Books SET IsAvailable = 0 WHERE BookId = @BookId";
            SqlCommand borrowCmd = new SqlCommand(borrowQuery, con);
            SqlCommand updateBookCmd = new SqlCommand(updateBookQuery, con);

            borrowCmd.Parameters.AddWithValue("@BookId", bookId);
            borrowCmd.Parameters.AddWithValue("@BorrowerId", borrowerId);
            borrowCmd.Parameters.AddWithValue("@BorrowedDate", DateTime.Now);
            updateBookCmd.Parameters.AddWithValue("@BookId", bookId);

            SqlTransaction transaction = con.BeginTransaction();
            borrowCmd.Transaction = transaction;
            updateBookCmd.Transaction = transaction;

            try
            {
                borrowCmd.ExecuteNonQuery();
                updateBookCmd.ExecuteNonQuery();
                transaction.Commit();
                Console.WriteLine("\n\tBook borrowed successfully.");
            }
            catch
            {
                transaction.Rollback();
                Console.WriteLine("\n\tAn error occurred while borrowing the book.");
            }
            con.Close();
        }

        public void ReturnBook(int bookId, int borrowerId)
        {
            string connectionString = _connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string checkBorrowQuery = "SELECT COUNT(*) FROM Transactions WHERE BookId = @BookId AND BorrowerId = @BorrowerId AND Date IS NULL";
            SqlCommand checkBorrowCmd = new SqlCommand(checkBorrowQuery, con);
            checkBorrowCmd.Parameters.AddWithValue("@BookId", bookId);
            checkBorrowCmd.Parameters.AddWithValue("@BorrowerId", borrowerId);
            int count = Convert.ToInt32(checkBorrowCmd.ExecuteScalar());

            if (count == 0)
            {
                Console.WriteLine("\n\tNo record of this book being borrowed by this borrower.");
                con.Close();
                return;
            }

            string returnQuery = "UPDATE Transactions SET Date = @Date WHERE BookId = @BookId AND BorrowerId = @BorrowerId";
            string updateBookQuery = "UPDATE Books SET IsAvailable = 1 WHERE BookId = @BookId";
            SqlCommand returnCmd = new SqlCommand(returnQuery, con);
            SqlCommand updateBookCmd = new SqlCommand(updateBookQuery, con);

            returnCmd.Parameters.AddWithValue("@BookId", bookId);
            returnCmd.Parameters.AddWithValue("@BorrowerId", borrowerId);
            returnCmd.Parameters.AddWithValue("@Date", DateTime.Now);
            updateBookCmd.Parameters.AddWithValue("@BookId", bookId);

            SqlTransaction transaction = con.BeginTransaction();
            returnCmd.Transaction = transaction;
            updateBookCmd.Transaction = transaction;

            try
            {
                returnCmd.ExecuteNonQuery();
                updateBookCmd.ExecuteNonQuery();
                transaction.Commit();
                Console.WriteLine("\n\tBook returned successfully.");
            }
            catch
            {
                transaction.Rollback();
                Console.WriteLine("\n\tAn error occurred while returning the book.");
            }
            con.Close();
        }
        public void UpdateBorrower(Borrower borrower)
        {
            string connectionString = _connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string query = "UPDATE Borrowers SET Name = @Name, Email = @Email WHERE BorrowerId = @BorrowerId";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@BorrowerId", borrower.BorrowerId);
            cmd.Parameters.AddWithValue("@Name", borrower.Name);
            cmd.Parameters.AddWithValue("@Email", borrower.Email);
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            if (rowsAffected > 0)
            {
                Console.WriteLine("\n\tBorrower updated successfully.");
            }
            else
            {
                Console.WriteLine("\n\tBorrower not found.");
            }
        }

        public void DeleteBorrower(int borrowerId)
        {
            string connectionString = _connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string query = "DELETE FROM Borrowers WHERE BorrowerId = @BorrowerId";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@BorrowerId", borrowerId);
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            if (rowsAffected > 0)
            {
                Console.WriteLine("\n\tBorrower removed successfully.");
            }
        }

        public List<Borrower> GetAllBorrowers()
        {
            List<Borrower> borrowers = new List<Borrower>();
            string connectionString = _connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string query = "SELECT * FROM Borrowers";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Borrower borrower = new Borrower
                {
                    BorrowerId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2)
                };
                borrowers.Add(borrower);
                Console.WriteLine($"\tID: {borrower.BorrowerId}, Name: {borrower.Name}, Email: {borrower.Email}");
            }
            reader.Close();
            con.Close();
            if (borrowers.Count == 0)
            {
                Console.WriteLine("\n\tNo borrowers found.");
            }
            return borrowers;
        }

        public List<Transaction> GetAllTransactions()
        {
            List<Transaction> transactions = new List<Transaction>();
            string connectionString = _connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string query = "SELECT * FROM Transactions";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Transaction transaction = new Transaction
                {
                    TransactionId = reader.GetInt32(0),
                    BookId = reader.GetInt32(1),
                    BorrowerId = reader.GetInt32(2),
                    Date = reader.GetDateTime(3),
                    IsBorrowed = reader.GetBoolean(4)
                };
                transactions.Add(transaction);
            }
            reader.Close();
            con.Close();
            if (transactions.Count == 0)
            {
                Console.WriteLine("\n\tNo transactions found.");
            }
            return transactions;
        }

        public void ViewAllTransactions()
        {
            List<Transaction> transactions = GetAllTransactions();
            foreach (Transaction transaction in transactions)
            {
                Console.WriteLine($"\tTransaction ID: {transaction.TransactionId}, Book ID: {transaction.BookId}, Borrower ID: {transaction.BorrowerId}, Date: {transaction.Date}, Borrowed: {transaction.IsBorrowed}");
            }
        }

        public bool IsBorrowerRegistered(int borrowerId)
        {
            bool isRegistered = false;
            string connectionString = _connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string query = "SELECT COUNT(*) FROM Borrowers WHERE BorrowerId = @BorrowerId";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@BorrowerId", borrowerId);
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            if (count > 0)
            {
                isRegistered = true;
            }
            con.Close();
            return isRegistered;
        }

        private void RemoveTransaction(Transaction transaction)
        {
            string connectionString = _connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string query = "DELETE FROM Transactions WHERE TransactionId = @TransactionId AND BookId = @BookId AND BorrowerId = @BorrowerId";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@TransactionId", transaction.TransactionId);
            cmd.Parameters.AddWithValue("@BookId", transaction.BookId);
            cmd.Parameters.AddWithValue("@BorrowerId", transaction.BorrowerId);
            cmd.Parameters.AddWithValue("@IsBorrowed", transaction.IsBorrowed);
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            if (rowsAffected > 0)
            {

                Console.WriteLine("\n\tTransaction removed successfully.");
            }
            else
            {
                Console.WriteLine("\n\tTransaction not found.");
            }
        }

        public int GetTransactionById(int bookId, int borrowerId)
        {
            int transactionId = -1;
            string connectionString = _connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string query = "SELECT TransactionId FROM Transactions WHERE BookId = @BookId AND BorrowerId = @BorrowerId AND IsBorrowed = 1";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@BookId", bookId);
            cmd.Parameters.AddWithValue("@BorrowerId", borrowerId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                transactionId = reader.GetInt32(0);
            }
            reader.Close();
            con.Close();
            return transactionId;
        }
        public Borrower GetBorrowerById(int borrowId)
        {
            Borrower borrower = null;
            SqlConnection con = new SqlConnection(_connectionString);
            con.Open();
            string query = "Select * from Borrowers where BorrowerId = @BorrowerId";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@BorrowerId", borrowId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                borrower = new Borrower()
                {
                    BorrowerId = (int)reader["BorrowerId"]
                };
            }
            return borrower;
        }
        public void RecordTransaction(Transaction transaction)
        {
            if (!transaction.IsBorrowed)
            {
                RemoveTransaction(transaction);
            }
            else
            {
                string connectionString = _connectionString;
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                string query = "INSERT INTO Transactions (BookId, BorrowerId, Date, IsBorrowed) VALUES (@BookId, @BorrowerId, @Date, @IsBorrowed)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@TransactionId", transaction.TransactionId);
                cmd.Parameters.AddWithValue("@BookId", transaction.BookId);
                cmd.Parameters.AddWithValue("@BorrowerId", transaction.BorrowerId);
                cmd.Parameters.AddWithValue("@Date", transaction.Date);
                cmd.Parameters.AddWithValue("@IsBorrowed", transaction.IsBorrowed);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public List<Transaction> GetBorrowedBooksByBorrower(int borrowerId)
        {
            List<Transaction> transactions = new List<Transaction>();
            string connectionString = _connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string query = "SELECT * FROM Transactions WHERE BorrowerId = @BorrowerId AND IsBorrowed = 1";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@BorrowerId", borrowerId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Transaction transaction = new Transaction
                {
                    TransactionId = reader.GetInt32(0),
                    BookId = reader.GetInt32(1),
                    BorrowerId = reader.GetInt32(2),
                    Date = reader.GetDateTime(3),
                    IsBorrowed = reader.GetBoolean(4)
                };
                transactions.Add(transaction);
            }
            reader.Close();
            con.Close();
            return transactions;
        }
    }
}

