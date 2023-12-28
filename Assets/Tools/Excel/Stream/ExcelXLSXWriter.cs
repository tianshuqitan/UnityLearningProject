using System.IO;
using NPOI.HSSF.UserModel;

namespace Tools.Excel
{
    public class ExcelXLSXWriter : ExcelNPOIWriter
    {
        public ExcelXLSXWriter(Stream stream, ExcelConfig config) : base(stream,
            config)
        {
            m_Workbook = new HSSFWorkbook(m_Stream);
        }
    }
}