using ClosedXML.Excel;

namespace GOCAP.ExternalServices;

public class ExcelExportService : IExcelExportService
{
    public byte[] ExportToExcel<T>(
         string sheetName,
         List<(string Header, Func<T, object?> Selector)> columns,
         List<T> data,
         Dictionary<string, object?>? overview = null)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(sheetName);

        int startRow = 1;

        // Ghi phần thống kê nếu có
        if (overview != null && overview.Count > 0)
        {
            int row = 1;
            foreach (var entry in overview)
            {
                sheet.Cell(row, 1).Value = entry.Key;
                SetCellValueSmart(sheet.Cell(row, 2), entry.Value);
                row++;
            }


            sheet.Range(1, 1, overview.Count, 1).Style.Font.Bold = true;
            startRow = overview.Count + 2;
        }

        // Header
        for (int i = 0; i < columns.Count; i++)
        {
            sheet.Cell(startRow, i + 1).Value = columns[i].Header;
        }

        sheet.Range(startRow, 1, startRow, columns.Count).Style.Font.Bold = true;

        // Data
        for (int i = 0; i < data.Count; i++)
        {
            var row = startRow + 1 + i;
            for (int j = 0; j < columns.Count; j++)
            {
                var value = columns[j].Selector(data[i]);
                SetCellValueSmart(sheet.Cell(row, j + 1), value);
            }
        }

        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static void SetCellValueSmart(IXLCell cell, object? value)
    {
        if (value == null)
        {
            cell.Value = string.Empty;
            return;
        }

        switch (value)
        {
            case DateTime dateTime:
                cell.Value = dateTime;
                cell.Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
                break;

            case int i:
                cell.Value = i;
                break;

            case double d:
                cell.Value = d;
                break;

            case float f:
                cell.Value = f;
                break;

            case bool b:
                cell.Value = b ? "Yes" : "No";
                break;

            case Guid g:
                cell.Value = g.ToString();
                break;

            default:
                cell.Value = value.ToString();
                break;
        }
    }

}
