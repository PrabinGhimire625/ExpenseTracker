namespace ExpenseTracker.Models
{
    public enum DebtType
    {
        Take, // Debt taken from someone
        Give  // Debt return or give to someone
    }

    public class DebtModel
    {
        public int Id { get; set; } 
        public int UserId { get; set; } 
        public decimal Amount { get; set; } 
        public decimal PaidAmount { get; set; } 
        public decimal RemainigAmount => Type == DebtType.Give ? Amount - PaidAmount : 0;  
        public string Source { get; set; } 
        public DateTime DueDate { get; set; } 
        public bool IsCleared => Type == DebtType.Give && RemainigAmount == 0;
        public DateTime? ClearedDate => IsCleared ? DateTime.Now : (DateTime?)null;
        public string Notes { get; set; } 
        public DateTime Date { get; set; }
        public DebtType Type { get; set; } 
    }
}
