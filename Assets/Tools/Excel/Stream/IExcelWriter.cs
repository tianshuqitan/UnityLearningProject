using System;

namespace Tools.Excel
{
    public interface IExcelWriter : IDisposable
    {
        bool Write<T>(string path, T value);
    }
}
