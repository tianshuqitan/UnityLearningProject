using System;

namespace Tools.Excel
{
    public interface IExcelReader : IDisposable
    {
        T Read<T>(string sheetName, string identify) where T : class, new();
    }
}
