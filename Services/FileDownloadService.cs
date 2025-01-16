// File: FileDownloadService.cs
using System.Text;
using ExpenseTracker.Models;

public class FileDownloadService
{
    public byte[] GenerateTransactionFile(List<TransactionModel> transactions)
    {
        var content = new StringBuilder();
        string separator = new string('-', 87); 
        string header = string.Format("{0,-40} | {1,-10} | {2,-12} | {3,-12}",
                                      "Title", "Type", "Amount", "Date");

        // Add header and separator
        content.AppendLine(separator);
        content.AppendLine(header);
        content.AppendLine(separator);

        // Add transaction details
        foreach (var transaction in transactions)
        {
            content.AppendLine(string.Format("{0,-40} | {1,-10} | {2,-12:C} | {3,-12}",
                                             transaction.Title,
                                             transaction.Type,
                                             transaction.Amount,
                                             transaction.Date.ToShortDateString()));
        }

        content.AppendLine(separator);

        return Encoding.UTF8.GetBytes(content.ToString());
    }
}
