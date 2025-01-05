using ExpenseTracker.Models;
using System.Text.Json;

public class TransactionService
{
    private static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    private static readonly string FolderPath = Path.Combine(DesktopPath, "JsonData");
    private static readonly string FilePath = Path.Combine(FolderPath, "AppData.json"); 

    // Load all data (users, debts, transactions) from the JSON file
    public AppData LoadAppData()
    {
        if (!File.Exists(FilePath))
            return new AppData();  

        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<AppData>(json) ?? new AppData(); 
    }

    // Save all data (users, debts, transactions) to the JSON file
    public void SaveAppData(AppData appData)
    {
        if (!Directory.Exists(FolderPath))
        {
            Directory.CreateDirectory(FolderPath);
        }

        var json = JsonSerializer.Serialize(appData, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json); 
    }

    // Save transaction data
    public void SaveTransactions(List<TransactionModel> transactions)
    {
        var appData = LoadAppData(); 
        appData.Transactions = transactions;  
        SaveAppData(appData);  
    }

    // Get all income transactions for a user
    public List<TransactionModel> GetAllIncomeTransactions(int userId)
    {
        var transactions = LoadAppData().Transactions;
        var incomeTransactions = transactions
            .Where(t => t.UserId == userId && t.Type == TransactionModel.TransactionType.Credit) // Use enum value for Credit
            .ToList();
        return incomeTransactions;
    }

    // Calculate total income for a user
    public decimal CalculateTotalIncome(int userId)
    {
        var transactions = LoadAppData().Transactions;
        var incomeTransactions = transactions
            .Where(t => t.UserId == userId && t.Type == TransactionModel.TransactionType.Credit)
            .ToList();

        return incomeTransactions.Sum(t => t.Amount);
    }

    // Calculate total expenses for a user
    public decimal CalculateTotalExpenses(int userId)
    {
        var transactions = LoadAppData().Transactions;
        var expenseTransactions = transactions
            .Where(t => t.UserId == userId && t.Type == TransactionModel.TransactionType.Debit)
            .ToList();

        return expenseTransactions.Sum(t => t.Amount);
    }

     // Save transactions for a specific user
    public void SaveUserTransactions(int userId, List<TransactionModel> transactions)
    {
        var appData = LoadAppData();
        var userTransactions = appData.Transactions.Where(t => t.UserId != userId).ToList(); 
        userTransactions.AddRange(transactions);
        appData.Transactions = userTransactions;
        SaveAppData(appData);
    }

    // Get transactions for a specific user
    public List<TransactionModel> GetUserTransactions(int userId)
    {
        var appData = LoadAppData();
        return appData.Transactions.Where(t => t.UserId == userId).ToList();
    }


    public bool CheckSufficientBalance(int userId, decimal transactionAmount)
    {
        decimal totalIncome = CalculateTotalIncome(userId); 
        decimal totalExpenses = CalculateTotalExpenses(userId); 

        decimal balance = totalIncome - totalExpenses;

        return balance >= transactionAmount; 
    }

    public List<TransactionModel> FilterTransactions(int userId, string? type = null, List<string>? tags = null, DateTime? date = null)
    {
        var transactions = LoadAppData().Transactions
            .Where(t => t.UserId == userId);

        // Filter by type (Credit/Debit)
        if (!string.IsNullOrEmpty(type))
        {
            transactions = transactions.Where(t => t.Type.ToString().Equals(type, StringComparison.OrdinalIgnoreCase));
        }

        // Filter by tags
        if (tags != null && tags.Any())
        {
            transactions = transactions.Where(t => t.Tags != null && t.Tags.Intersect(tags).Any());
        }

        // Filter by specific date
        if (date.HasValue)
        {
            transactions = transactions.Where(t => t.Date.Date == date.Value.Date);  
        }

        return transactions.ToList();
    }

    public List<TransactionModel> SortTransactionsByDate(List<TransactionModel> transactions, bool ascending = true)
    {
        if (ascending)
        {
            return transactions.OrderBy(t => t.Date).ToList();
        }
        else
        {
            return transactions.OrderByDescending(t => t.Date).ToList();
        }
    }

    public List<TransactionModel> SearchByTitle(int userId, string title)
    {
        return LoadAppData().Transactions
            .Where(t => t.UserId == userId &&
                        !string.IsNullOrEmpty(t.Title) &&
                        t.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<TransactionModel> GetTransactionsByDateRange(int userId, DateTime startDate, DateTime endDate)
    {
        return LoadAppData().Transactions
            .Where(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate)
            .ToList();
    }

    // Get highest income for a user
    public TransactionModel GetHighestIncome(int userId)
    {
        return LoadAppData().Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionModel.TransactionType.Credit)
            .OrderByDescending(t => t.Amount)
            .FirstOrDefault();
    }

    // Get lowest income for a user
    public TransactionModel GetLowestIncome(int userId)
    {
        return LoadAppData().Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionModel.TransactionType.Credit)
            .OrderBy(t => t.Amount)
            .FirstOrDefault();
    }

    // Get highest expense for a user
    public TransactionModel GetHighestExpense(int userId)
    {
        return LoadAppData().Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionModel.TransactionType.Debit)
            .OrderByDescending(t => t.Amount)
            .FirstOrDefault();
    }

    // Get lowest expense for a user
    public TransactionModel GetLowestExpense(int userId)
    {
        return LoadAppData().Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionModel.TransactionType.Debit)
            .OrderBy(t => t.Amount)
            .FirstOrDefault();
    }

    public int CalculateTotalNumberOfTransactions(int userId)
    {
        var transactions = LoadAppData().Transactions;

        int totalTransactions = transactions.Count(t => t.UserId == userId);

        return totalTransactions; 
    }

    // Get top 5 highest transactions for a user
    public async Task<List<TransactionModel>> GetTop5HighestTransactions(int userId)
    {
        if (userId <= 0)
            throw new ArgumentException("Invalid user ID.");

        var transactions = LoadAppData()
            .Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Amount)
            .Take(5)
            .ToList();

        return await Task.FromResult(transactions);
    }

    public async Task<List<TransactionModel>> GetTop5LowestTransactions(int userId)
    {
        if (userId <= 0)
            throw new ArgumentException("Invalid user ID.");

        var transactions = LoadAppData()
            .Transactions
            .Where(t => t.UserId == userId)
            .OrderBy(t => t.Amount)
            .Take(5)
            .ToList();

        return await Task.FromResult(transactions);
    }

}
