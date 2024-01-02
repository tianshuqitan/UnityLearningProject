using System.IO;
using NPOI.HSSF.UserModel;

namespace Tools.Excel
{
    public class ExcelXLSWriter : ExcelNPOIWriter
    {
        public ExcelXLSWriter(Stream stream, ExcelConfig configuration) : base(stream, configuration)
        {
            m_Workbook = new HSSFWorkbook(m_Stream);
            OnWorkbookInit();
        }
    }
}