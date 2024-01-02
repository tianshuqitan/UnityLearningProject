using System;
using NPOI.SS.UserModel;

namespace Tools.Excel
{
    public static partial class NPOIHelper
    {
        public static string GetValue(ICell cell)
        {
            if (cell == null)
            {
                return null;
            }
            
            switch (cell.CellType)
            {
                case CellType.Numeric:
                case CellType.String:
                case CellType.Boolean:
                    break;
                case CellType.Unknown:
                case CellType.Formula:
                case CellType.Blank:
                case CellType.Error:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return cell.ToString();
        }
    }
}
