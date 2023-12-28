using System;
using System.Globalization;
using NPOI.SS.UserModel;

namespace Tools.Excel
{
    public static class ExcelNPOIHelper
    {
        public static bool IsSheetEmpty(ISheet sheet)
        {
            return sheet == null || (0 == sheet.FirstRowNum && 0 == sheet.LastRowNum);
        }
        
        public static bool IsWorkbookEmpty(IWorkbook workbook)
        {
            if (workbook == null || workbook.NumberOfSheets < 1)
            {
                return true;
            }
            
            return IsSheetEmpty(workbook.GetSheetAt(0));
        }

        public static bool IsRowEmpty(ISheet sheet, int rowNum)
        {
            if (IsSheetEmpty(sheet))
            {
                return true;
            }

            if (rowNum < sheet.FirstRowNum || rowNum > sheet.LastRowNum)
            {
                return true;
            }

            return IsRowEmpty(sheet.GetRow(rowNum));
        }

        public static bool IsRowEmpty(IRow row)
        {
            if (row == null)
            {
                return true;
            }
            
            return row.FirstCellNum == 0 && row.LastCellNum == 0;
        }

        public static object GetCellValue(ICell cell)
        {
            switch (cell.CellType)
            {
                case CellType.String:
                    return cell.StringCellValue.Trim();
                case CellType.Numeric:
                    return cell.NumericCellValue;
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Unknown:
                case CellType.Formula:
                case CellType.Blank:
                case CellType.Error:
                default:
                    return null;
            }
        }
        
        private static string GetHeadFieldType(ICell cell)
        {
            if (cell == null)
            {
                return null;
            }

            if (cell.CellType != CellType.String)
            {
                return null;
            }
            
            var value = (string)GetCellValue(cell);
            value = value.ToLower();
            switch (value)
            {
                case "int":
                    return "int";
                case "bool":
                    return "bool";
                case "string":
                    return "string";
                case "float":
                    return "float";
                case "double":
                    return "double";
                default:
                    return null;
            }
        }

        public static string GetCellStringValue(ICell cell)
        {
            if (cell == null)
            {
                return string.Empty;
            }
            
            switch (cell.CellType)
            {
                case CellType.String:
                    return cell.StringCellValue.Trim();
                case CellType.Numeric:
                    return cell.NumericCellValue.ToString(CultureInfo.InvariantCulture);
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Unknown:
                case CellType.Formula:
                case CellType.Blank:
                case CellType.Error:
                default:
                    return null;
            }
        }
        
        public static SheetHeadInfo ParseHeadInfo(ISheet sheet, ExcelConfigHead config)
        {
            if (IsSheetEmpty(sheet))
            {
                return null;
            }
            
            if (IsRowEmpty(sheet, config.TypeIndex) || IsRowEmpty(sheet, config.NameIndex))
            {
                return null;
            }

            var typeRow = sheet.GetRow(config.TypeIndex);
            var nameRow = sheet.GetRow(config.NameIndex);
            var fieldsNum = Math.Max(typeRow.LastCellNum, nameRow.LastCellNum) + 1;

            var headInfo = new SheetHeadInfo(fieldsNum);

            for (var i = 0; i < fieldsNum; i++)
            {
                var fieldType = string.Empty;
                var fieldName = string.Empty;

                if (i >= typeRow.FirstCellNum || i <= typeRow.LastCellNum)
                {
                    fieldType = GetHeadFieldType(typeRow.GetCell(i));
                    if (fieldType != null && fieldType.StartsWith(config.IgnoreMark))
                    {
                        fieldType = null;
                    }
                }

                if (i >= nameRow.FirstCellNum || i <= nameRow.LastCellNum)
                {
                    fieldName = GetCellStringValue(nameRow.GetCell(i));
                    if (fieldType != null && fieldName.StartsWith(config.IgnoreMark))
                    {
                        fieldName = null;
                    }
                }
                
                if (string.IsNullOrEmpty(fieldType) || string.IsNullOrEmpty(fieldName))
                {
                    continue;
                }

                headInfo.AddHeadField(i, fieldType, fieldName);
            }
            
            return headInfo;
        }
        
        public static IRow GetRowByIdentify(ISheet sheet, int identifyColumn, int identify, int beganRow, char ignoreMark)
        {
            if (IsSheetEmpty(sheet))
            {
                return null;
            }
            
            IRow result = null;
            var firstRow = Math.Max(sheet.FirstRowNum, beganRow);
            for (var i = firstRow; i < sheet.LastRowNum + 1; i++)
            {
                var row = sheet.GetRow(i);
                if (IsRowEmpty(row))
                {
                    continue;
                }
                
                var cell = row.GetCell(identifyColumn);
                if (cell == null)
                {
                    continue;
                }
                
                var cellValue = GetCellStringValue(cell);
                if (cellValue.StartsWith(ignoreMark))
                {
                    continue;
                }
                
                if (int.TryParse(cellValue, out var valueInt) && identify == valueInt)
                {
                    result = row;
                    break;
                }
            }
            
            return result;
        }
    }
}