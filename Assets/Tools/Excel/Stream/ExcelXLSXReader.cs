using System.IO;
using NPOI.XSSF.UserModel;

namespace Tools.Excel
{
    public class ExcelXLSXReader : ExcelNPOIReader
    {
        public ExcelXLSXReader(Stream stream, ExcelConfig configuration) : base(stream, configuration)
        {
            m_Workbook = new XSSFWorkbook(m_Stream);
        }
    }
}