namespace ExpenseTracker.Models
{
    public class CashInModel
    {
        public string Amount { get; set; }
        public DateTime Date { get; set; }
        public string Source { get; set; }
        public string Tags { get; set; }
        public string Notes { get; set; }
    }
}
