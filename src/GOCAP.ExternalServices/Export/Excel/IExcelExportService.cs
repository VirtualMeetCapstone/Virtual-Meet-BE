namespace GOCAP.ExternalServices;

public interface IExcelExportService
{
    byte[] ExportToExcel<T>(
           string sheetName,
           List<(string Header, Func<T, object?> Selector)> columns,
           List<T> data,
           Dictionary<string, object?>? overview = null);
}
