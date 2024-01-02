using System.Collections;
using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace Tools.Excel
{
    public static partial class NPOIHelper
    {
        public static bool IsEmpty(ISheet sheet)
        {
            return GetRowNum(sheet) == 0;
        }
        
        public static int GetRowNum(ISheet sheet)
        {
            // return sheet == null ? 0 : sheet.LastRowNum - sheet.FirstRowNum;
            return sheet?.PhysicalNumberOfRows ?? 0;
        }
        
        public static List<IRow> GetRows(ISheet sheet)
        {
            var rowList = new List<IRow>();
            if (IsEmpty(sheet))
            {
                return rowList;
            }
            
            var itr = sheet.GetEnumerator();
            while (itr.MoveNext())
            {
                var row = (IRow)(itr.Current);
                if (IsEmpty(row))
                {
                    continue;
                }
                
                rowList.Add(row);
            }
            
            return rowList;
        }
    }
}