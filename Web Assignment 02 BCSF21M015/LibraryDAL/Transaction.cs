
using System;

namespace LibraryDAL
{
    public class Transaction
    {
        public int TransactionId { set; get; }
        public int BookId { set; get; }
        public int BorrowerId { set; get; }
        public DateTime Date { set; get; }
        public bool IsBorrowed { set; get; }
    }
}