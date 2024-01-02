using System.IO;
using NPOI.HSSF.UserModel;

namespace Tools.Excel
{
    public class ExcelXLSReader : ExcelNPOIReader
    {
        public ExcelXLSReader(Stream stream, ExcelConfig config = null) : base(stream, config)
        {
            m_Workbook = new HSSFWorkbook(m_Stream);
            OnWorkbookInit();
        }
    }
}
