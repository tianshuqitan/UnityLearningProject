using System.IO;
using NPOI.HSSF.UserModel;

namespace Tools.Excel
{
    public class ExcelXLSExporter : ExcelNPOIExporter
    {
        public ExcelXLSExporter(Stream stream, ExcelConfig config) : base(stream, config)
        {
            m_Workbook = new HSSFWorkbook(m_Stream);
        }
    }
}