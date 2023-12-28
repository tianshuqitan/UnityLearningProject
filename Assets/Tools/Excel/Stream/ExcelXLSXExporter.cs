using System.IO;
using NPOI.XSSF.UserModel;

namespace Tools.Excel
{
    public class ExcelXLSXExporter : ExcelNPOIExporter
    {
        public ExcelXLSXExporter(Stream stream, ExcelConfig config) : base(stream, config)
        {
            m_Workbook = new XSSFWorkbook(m_Stream);
        }
    }
}
