using System;

namespace Tools.Excel
{
    public interface IExcelExporter : IDisposable
    {
        public void Export(ExportType exportType, string sheetName, IExportConfig config);
    }
}
