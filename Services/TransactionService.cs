using ExpenseTracker.Models;
using System.Text.Json;

public class TransactionService
{
    private static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    private static readonly string FolderPath = Path.Combine(DesktopPath, "JsonData");
    private static readonly string FilePath = Path.Combine(FolderPath, "AppData.json"); 

    // Load all data from the JSON file
    public AppData LoadAppData()
    {
        if (!File.Exists(FilePath))
            return new AppData();  

        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<AppData>(json) ?? new AppData(); 
    }

    // Save all data to the JSON file
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

    // Get all income transactions f
    public List<TransactionModel> GetAllIncomeTransactions(int userId)
    {
        var transactions = LoadAppData().Transactions;
        var incomeTransactions = transactions
            .Where(t => t.UserId == userId && t.Type == TransactionModel.TransactionType.Credit) // Use enum value for Credit
            .ToList();
        return incomeTransactions;
    }

    // Calculate total income 
    public decimal CalculateTotalIncome(int userId)
    {
        var transactions = LoadAppData().Transactions;
        var incomeTransactions = transactions
            .Where(t => t.UserId == userId && t.Type == TransactionModel.TransactionType.Credit)
            .ToList();

        decimal totalIncome = incomeTransactions.Sum(t => t.Amount);
        var debts = LoadAppData().Debts
            .Where(d => d.UserId == userId && d.Type == DebtType.Give)
            .ToList();

        decimal totalClearedDebt = debts
            .Where(d => d.IsCleared)
            .Sum(d => d.PaidAmount);

        totalIncome -= totalClearedDebt;

        return totalIncome;
    }


    // Calculate total expenses 
    public decimal CalculateTotalExpenses(int userId)
    {
        var transactions = LoadAppData().Transactions;
        var expenseTransactions = transactions
            .Where(t => t.UserId == userId && t.Type == TransactionModel.TransactionType.Debit)
            .ToList();

        return expenseTransactions.Sum(t => t.Amount);
    }

     // Save transactions 
    public void SaveUserTransactions(int userId, List<TransactionModel> transactions)
    {
        var appData = LoadAppData();
        var userTransactions = appData.Transactions.Where(t => t.UserId != userId).ToList(); 
        userTransactions.AddRange(transactions);
        appData.Transactions = userTransactions;
        SaveAppData(appData);
    }

    // Get transactions 
    public List<TransactionModel> GetUserTransactions(int userId)
    {
        var appData = LoadAppData();
        return appData.Transactions.Where(t => t.UserId == userId).ToList();
    }

    //check the balance for the cash outflow
    public bool CheckSufficientBalance(int userId, decimal transactionAmount)
    {
        decimal totalIncome = CalculateTotalIncome(userId); 
        decimal totalExpenses = CalculateTotalExpenses(userId); 

        decimal balance = totalIncome - totalExpenses;

        return balance >= transactionAmount; 
    }

    //filter transaction
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
    
    //sort transaction by date
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

    //search by title
    public List<TransactionModel> SearchByTitle(int userId, string title)
    {
        return LoadAppData().Transactions
            .Where(t => t.UserId == userId &&
                        !string.IsNullOrEmpty(t.Title) &&
                        t.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    //get transaction by start date and end date
    public List<TransactionModel> GetTransactionsByDateRange(int userId, DateTime startDate, DateTime endDate)
    {
        return LoadAppData().Transactions
            .Where(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate)
            .ToList();
    }

    // Get highest income
    public TransactionModel GetHighestIncome(int userId)
    {
        return LoadAppData().Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionModel.TransactionType.Credit)
            .OrderByDescending(t => t.Amount)
            .FirstOrDefault();
    }

    // Get lowest income 
    public TransactionModel GetLowestIncome(int userId)
    {
        return LoadAppData().Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionModel.TransactionType.Credit)
            .OrderBy(t => t.Amount)
            .FirstOrDefault();
    }

    // Get highest expense
    public TransactionModel GetHighestExpense(int userId)
    {
        return LoadAppData().Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionModel.TransactionType.Debit)
            .OrderByDescending(t => t.Amount)
            .FirstOrDefault();
    }

    // Get lowest expense 
    public TransactionModel GetLowestExpense(int userId)
    {
        return LoadAppData().Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionModel.TransactionType.Debit)
            .OrderBy(t => t.Amount)
            .FirstOrDefault();
    }

    //total no of transaction
    public int CalculateTotalNumberOfTransactions(int userId)
    {
        var transactions = LoadAppData().Transactions;

        int totalTransactions = transactions.Count(t => t.UserId == userId);

        return totalTransactions; 
    }

    // top 5 highest transactions 
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

    //top 5 lowest transaction
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
