using ExpenseTracker.Models;
using System.Text.Json;


    public class DebtService
    {
        private readonly TransactionService _transactionService;

        public DebtService(TransactionService transactionService)
        {
            _transactionService = transactionService;
        }


        private static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private static readonly string FolderPath = Path.Combine(DesktopPath, "JsonData");
        private static readonly string FilePath = Path.Combine(FolderPath, "AppData.json");

        // Load all data  from the JSON file
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

        // Save debt data
        public void SaveDebts(List<DebtModel> debts)
        {
            var appData = LoadAppData();
            appData.Debts = debts;
            SaveAppData(appData);
        }

        // Get all debts 
        public List<DebtModel> GetAllUserDebts(int userId)
        {
            var debts = LoadAppData().Debts;
            return debts.Where(d => d.UserId == userId).ToList();
        }

        // Get all debts
        public List<DebtModel> GetAllDebts()
        {
            return LoadAppData().Debts;
        }


        // total cleared debt 
        public decimal CalculateClearedDebt(int userId)
        {
            var debts = LoadAppData().Debts.Where(d => d.UserId == userId && d.Type == DebtType.Give).ToList();
            return debts.Where(d => d.IsCleared).Sum(d => d.PaidAmount);
        }

        //remaining debt
        public decimal CalculateRemainingDebt(int userId)
        {
            var debts = LoadAppData().Debts.Where(d => d.UserId == userId && d.Type == DebtType.Give).ToList();
            return debts.Sum(d => d.RemainigAmount);
        }

        // Calculate Total Debt 
        public decimal CalculateTotalDebt(int userId)
        {
            var debts = GetAllUserDebts(userId);
            return debts.Sum(d => d.RemainigAmount);
        }

        // Get the lowest debt 
        public DebtModel GetLowestDebt(int userId)
        {
            var debts = GetAllUserDebts(userId);
            return debts.OrderBy(d => d.RemainigAmount).FirstOrDefault();
        }

        // Get the highest debt 
        public DebtModel GetHighestDebt(int userId)
        {
            var debts = GetAllUserDebts(userId);
            return debts.OrderByDescending(d => d.RemainigAmount).FirstOrDefault();
        }

        // Get the lowest debt 
        public DebtModel GetLowestDebtOverall()
        {
            var debts = GetAllDebts();
            return debts.OrderBy(d => d.RemainigAmount).FirstOrDefault();
        }

        // Get the highest debt 
        public DebtModel GetHighestDebtOverall()
        {
            var debts = GetAllDebts();
            return debts.OrderByDescending(d => d.RemainigAmount).FirstOrDefault();
        }

        // pending debts
        public List<DebtModel> GetPendingDebts(int userId)
        {
            var debts = LoadAppData().Debts
                                     .Where(d => d.UserId == userId && d.RemainigAmount > 0)
                                     .OrderBy(d => d.ClearedDate)
                                     .ToList();

            return debts;
        }
    }

