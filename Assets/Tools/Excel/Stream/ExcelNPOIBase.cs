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

        private Dictionary<ISheet, SheetHeadInfo> m_HeadInfoMap;
        private Dictionary<ISheet, Dictionary<string, IRow>> m_SheetRowMap;

        public IWorkbook Workbook => m_Workbook;

        protected ExcelNPOIBase(Stream stream, ExcelConfig config = null)
        {
            m_Stream = stream;
            m_Config = config ?? ExcelConfig.DefaultConfig;
        }

        protected void OnWorkbookInit()
        {
            var sheetNum = NPOIHelper.GetSheetNum(m_Workbook);
            m_HeadInfoMap = new Dictionary<ISheet, SheetHeadInfo>(sheetNum);
            m_SheetRowMap = new Dictionary<ISheet, Dictionary<string, IRow>>(sheetNum);
        }

        protected SheetHeadInfo GetSheetHeadInfo(ISheet sheet)
        {
            if (!m_HeadInfoMap.ContainsKey(sheet))
            {
                var newHeadInfo = NPOIHelper.ParseHeadInfo(sheet, m_Config.ConfigHead);
                if (newHeadInfo == null)
                {
                    return null;
                }

                m_HeadInfoMap.Add(sheet, newHeadInfo);
            }

            return m_HeadInfoMap[sheet];
        }

        private Dictionary<string, IRow> GetSheetRowMap(ISheet sheet, int identifyColumn, char ignoreMark)
        {
            if (NPOIHelper.IsEmpty(sheet))
            {
                return null;
            }

            if (!m_SheetRowMap.ContainsKey(sheet))
            {
                var rowNum = NPOIHelper.GetRowNum(sheet);
                var rowMap = new Dictionary<string, IRow>(rowNum);
                var rows = NPOIHelper.GetRows(sheet);
                foreach (var row in rows)
                {
                    var value = NPOIHelper.GetValue(row.GetCell(identifyColumn));
                    if (string.IsNullOrEmpty(value) || value.StartsWith(ignoreMark))
                    {
                        continue;
                    }

                    rowMap.Add(value, row);
                }

                m_SheetRowMap.Add(sheet, rowMap);
            }

            return m_SheetRowMap[sheet];
        }

        protected IRow GetRowByIdentify(ISheet sheet, string identify)
        {
            var rowMap = GetSheetRowMap(sheet, m_Config.IdentifyColumn, m_Config.ConfigHead.IgnoreMark);
            return rowMap?[identify];
        }

        public List<ISheet> GetSheets()
        {
            return NPOIHelper.GetSheets(m_Workbook);
        }
        
        public List<IRow> GetRows(ISheet sheet, bool ignoreHead = true)
        {
            var rowList = NPOIHelper.GetRows(sheet);
            if (ignoreHead)
            {
                rowList.RemoveAll((row => row.RowNum < m_Config.DataBeganRow));
            }
            
            return rowList;
        }

        public List<ICell> GetCells(IRow row)
        {
            return NPOIHelper.GetCells(row);
        }
        
        public string GetRowIdentify(IRow row)
        {
            var cell = row.GetCell(m_Config.IdentifyColumn);
            var identify = NPOIHelper.GetValue(cell);
            return identify;
        }
        
        protected void OnDispose()
        {
            m_Workbook?.Dispose();
            m_Workbook = null;
        }
    }
}