namespace ExpenseTracker.Models
{

    public class AppData
    {
        public List<User> Users { get; set; } = new();
        public List<DebtModel> Debts { get; set; } = new();
        public List<TransactionModel> Transactions { get; set; } = new();


        // Add CurrentUser to AppData class
        //public User? CurrentUser { get; set; }  // The currently logged-in user
    }
}
