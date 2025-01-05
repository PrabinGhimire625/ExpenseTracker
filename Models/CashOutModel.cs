namespace ExpenseTracker.Models
{
    public class CashOutModel
    {
        public string Amount { get; set; }
        public DateTime Date { get; set; }
        public string Source { get; set; }
        public string Category { get; set; }
        public string Notes { get; set; }
    }
}
