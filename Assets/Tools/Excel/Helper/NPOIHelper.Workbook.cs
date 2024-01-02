using System.Collections;
using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace Tools.Excel
{
    public static partial class NPOIHelper
    {
        public static bool IsEmpty(IWorkbook workbook)
        {
            if (workbook == null || workbook.NumberOfSheets < 1)
            {
                return true;
            }
            
            return IsEmpty(workbook.GetSheetAt(0));
        }
        
        public static int GetSheetNum(IWorkbook workbook)
        {
            return workbook?.NumberOfSheets ?? 0;
        }

        public static List<ISheet> GetSheets(IWorkbook workbook)
        {
            var sheetList = new List<ISheet>();
            if (IsEmpty(workbook))
            {
                return sheetList;
            }

            using var itr = workbook.GetEnumerator();
            while (itr.MoveNext())
            {
                var sheet = itr.Current;
                if (IsEmpty(sheet))
                {
                    continue;
                }
                
                sheetList.Add(sheet);
            }
            
            return sheetList;
        }
    }
}
