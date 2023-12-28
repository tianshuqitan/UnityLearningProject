using System;
using System.IO;

namespace Tools.Excel
{
    public static class ExcelTypeHelper
    {
        public static ExcelType GetExcelType(string path)
        {
            var extension = Path.GetExtension(path).ToLowerInvariant();
            switch (extension)
            {
                case ".xlsx":
                    return ExcelType.XLSX;
                case ".xls":
                    return ExcelType.XLS;
                default:
                    throw new NotSupportedException($"extension {extension} is not support!!!");
            }
        }
    }
}