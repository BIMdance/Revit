namespace BIMdance.Revit.Utils.Common;

public class CsvUtils
{
    public static DataTable CreateDataTable(string content)
    {
        var dataTable = new DataTable();

        if (string.IsNullOrWhiteSpace(content))
            return dataTable;
        
        var lines = content.Replace(@"\r\n", "\r\n").Replace(@"\n", "\n").Replace(@"\r", "\r").Split('\n').Select(x => x.Trim('\n', '\r')).ToArray();
        var headers = lines[0].Split(';');

        foreach (var header in headers)
            dataTable.Columns.Add(!string.IsNullOrWhiteSpace(header) ? header.Split('#')[0] : "-");

        foreach (var line in lines.Skip(1))
        {
            var row = dataTable.NewRow();
            row.ItemArray = line.Split(';').OfType<object>().ToArray();
            dataTable.Rows.Add(row);
        }

        return dataTable;
    }
}