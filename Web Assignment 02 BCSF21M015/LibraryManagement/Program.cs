
using LibraryDAL;
// Driver function
namespace LibraryManagement
{
    public class Program
    {
        public static void Main(string[] args)

        {
            DataAccess dataAccess = new DataAccess();
            Menu menu = new Menu(dataAccess);
            menu.DisplayMenu();
        }
    }
}
