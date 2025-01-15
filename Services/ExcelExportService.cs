using ExpenseTracker.Models;
using Microsoft.JSInterop;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ExcelExportService
{
    private readonly IJSRuntime _jsRuntime;

    public ExcelExportService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task DownloadTransactions(List<TransactionModel> transactions)
    {
        try
        {
            // Filter transactions for the current month 
            var currentMonthTransactions = transactions.Where(t => t.Date.Month == DateTime.Now.Month).ToList();

            using (var package = new ExcelPackage())
            {
                // Create a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Transactions");

                // Add column headers
                worksheet.Cells[1, 1].Value = "Title";
                worksheet.Cells[1, 2].Value = "Type";
                worksheet.Cells[1, 3].Value = "Amount";
                worksheet.Cells[1, 4].Value = "Date";

                // Add rows for each transaction
                for (int i = 0; i < currentMonthTransactions.Count; i++)
                {
                    var transaction = currentMonthTransactions[i];
                    worksheet.Cells[i + 2, 1].Value = transaction.Title;
                    worksheet.Cells[i + 2, 2].Value = transaction.Type;
                    worksheet.Cells[i + 2, 3].Value = transaction.Amount;
                    worksheet.Cells[i + 2, 4].Value = transaction.Date.ToShortDateString();
                }

                // Prepare the file for download
                var fileBytes = package.GetAsByteArray();

                // Trigger the download using JavaScript
                await _jsRuntime.InvokeVoidAsync("saveAsFile", "transactions.xlsx", fileBytes);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting transactions: {ex.Message}");
        }
    }
}
