using System;

namespace Tools.Excel
{
    public interface IExcelReader : IDisposable
    {
        T Read<T>(string sheetName, int identify) where T : class, new();
    }
}
