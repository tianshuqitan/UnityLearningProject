using System;
using System.IO;

namespace Tools.Excel
{
    public static class ExcelReaderFactory
    {
        public static IExcelReader GetProvider(Stream stream, ExcelType excelType, ExcelConfig config)
        {
            switch (excelType)
            {
                case ExcelType.XLS:
                    return new ExcelXLSReader(stream, config);
                case ExcelType.XLSX:
                    return new ExcelXLSXReader(stream, config);
                case ExcelType.UNKNOWN:
                default:
                    throw new NotSupportedException("this is not support");
            }
        }
    }

    public static class ExcelWriterFactory
    {
        public static IExcelWriter GetProvider(Stream stream, ExcelType excelType, ExcelConfig config)
        {
            switch (excelType)
            {
                case ExcelType.XLS:
                    return new ExcelXLSWriter(stream, config);
                case ExcelType.XLSX:
                    return new ExcelXLSXWriter(stream, config);
                case ExcelType.UNKNOWN:
                default:
                    throw new NotSupportedException($"Please Issue for me");
            }
        }
    }

    public static class ExcelExporterFactory
    {
        public static IExcelExporter GetProvider(Stream stream, ExcelConfig config, ExcelType excelType)
        {
            switch (excelType)
            {
                case ExcelType.XLS:
                    return new ExcelXLSExporter(stream, config);
                case ExcelType.XLSX:
                    return new ExcelXLSXExporter(stream, config);
                case ExcelType.UNKNOWN:
                default:
                    throw new NotSupportedException($"{excelType} is not supported!!!");
            }
        }
    }
}