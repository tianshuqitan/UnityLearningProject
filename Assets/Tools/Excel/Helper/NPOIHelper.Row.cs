using System.Collections;
using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace Tools.Excel
{
    public static partial class NPOIHelper
    {
        public static bool IsEmpty(IRow row)
        {
            return row == null || GetCellNum(row) == 0;
        }

        public static int GetCellNum(IRow row)
        {
            return row?.PhysicalNumberOfCells ?? 0;
        }
        
        public static List<ICell> GetCells(IRow row)
        {
            var cellList = new List<ICell>();
            if (IsEmpty(row))
            {
                return cellList;
            }

            using var itr = row.GetEnumerator();
            while (itr.MoveNext())
            {
                cellList.Add(itr.Current);
            }

            return cellList;
        }
    }
}