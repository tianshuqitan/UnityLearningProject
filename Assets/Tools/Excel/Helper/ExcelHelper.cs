using System.IO;

namespace Tools.Excel
{
    public static class ExcelHelper
    {
        public static void Export(string path, string sheetName, ExportType exportType, ExcelConfig excelConfig,
            IExportConfig exportConfig)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var excelType = ExcelTypeHelper.GetExcelType(path);
            Export(stream, sheetName, exportType, excelType, excelConfig, exportConfig);
        }

        private static void Export(Stream stream, string sheetName, ExportType exportType, ExcelType excelType,
            ExcelConfig excelConfig, IExportConfig exportConfig)
        {
            using var exporter = ExcelExporterFactory.GetProvider(stream, excelConfig, excelType);
            exporter.Export(exportType, sheetName, exportConfig);
        }

        public static T1 Read<T1>(string path, string sheetName, string identify, ExcelConfig excelConfig)
            where T1 : class, new()
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var excelType = ExcelTypeHelper.GetExcelType(path);
            return Read<T1>(stream, sheetName, identify, excelType, excelConfig);
        }

        private static T1 Read<T1>(Stream stream, string sheetName, string identify, ExcelType excelType,
            ExcelConfig excelConfig) where T1 : class, new()
        {
            using var reader = ExcelReaderFactory.GetProvider(stream, excelType, excelConfig);
            return reader.Read<T1>(sheetName, identify);
        }

        public static bool Write<T1>(string path, T1 instance, ExcelConfig excelConfig) where T1 : class, new()
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            var excelType = ExcelTypeHelper.GetExcelType(path);
            using var writer = ExcelWriterFactory.GetProvider(stream, excelType, excelConfig);
            return writer.Write<T1>(path, instance);
        }
    }
}