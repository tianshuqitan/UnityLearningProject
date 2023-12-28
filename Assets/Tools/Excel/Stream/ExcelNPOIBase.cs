using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;

namespace Tools.Excel
{
    public abstract class ExcelNPOIBase : IExcelNPOI
    {
        protected readonly Stream m_Stream;
        protected readonly ExcelConfig m_Config;
        protected IWorkbook m_Workbook;
        protected Dictionary<string, SheetHeadInfo> m_HeadInfoMap;

        protected ExcelNPOIBase(Stream stream, ExcelConfig config)
        {
            m_Stream = stream;
            m_Config = config ?? ExcelConfig.DefaultConfig;
            m_HeadInfoMap = new Dictionary<string, SheetHeadInfo>();
        }

        protected SheetHeadInfo GetSheetHeadInfo(ISheet sheet)
        {
            if (!m_HeadInfoMap.ContainsKey(sheet.SheetName))
            {
                var newHeadInfo = ExcelNPOIHelper.ParseHeadInfo(sheet, m_Config.ConfigHead);
                if (newHeadInfo == null)
                {
                    return null;
                }

                m_HeadInfoMap.Add(sheet.SheetName, newHeadInfo);
            }

            return m_HeadInfoMap[sheet.SheetName];
        }

        protected IRow GetRowByIdentify(ISheet sheet, int identify)
        {
            var column = m_Config.IdentifyColumn;
            var beganRow = m_Config.DataBeganRow;
            var ignoreMark = m_Config.ConfigHead.IgnoreMark;
            return ExcelNPOIHelper.GetRowByIdentify(sheet, column, identify, beganRow, ignoreMark);
        }
        
        protected void OnDispose()
        {
            m_Workbook?.Dispose();
            m_Workbook = null;
        }
    }
}