using System.IO;
using NPOI.HSSF.UserModel;

namespace Tools.Excel
{
    public class ExcelXLSReader : ExcelNPOIReader
    {
        public ExcelXLSReader(Stream stream, ExcelConfig configuration) : base(stream, configuration)
        {
            m_Workbook = new HSSFWorkbook(m_Stream);
        }
    }
}
