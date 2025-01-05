namespace ExpenseTracker.Models
{
    public class TransactionModel
    {
        public enum TransactionType
        {
            Credit,  // Inflow
            Debit,   // Outflow
        }

        public int Id { get; set; } 
        public int UserId { get; set; }
        public string Title { get; set; } 
        public decimal Amount { get; set; } 
        public TransactionType Type { get; set; } 
        public List<string> Tags { get; set; } = new List<string>(); 
        public List<string> Source { get; set; } = new List<string>(); 
        public string Notes { get; set; } 
        public DateTime Date { get; set; } 
    }
}
