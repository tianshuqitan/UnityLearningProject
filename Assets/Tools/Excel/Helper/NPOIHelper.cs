using System;
using System.Globalization;
using NPOI.SS.UserModel;

namespace Tools.Excel
{
    public static partial class NPOIHelper
    {
        private static string GetHeadFieldType(ICell cell)
        {
            var value = GetValue(cell);
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            
            var fieldType = value.Trim().ToLower();
            switch (fieldType)
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
                    throw new NotSupportedException($"{fieldType} is not supported!!!");
            }
        }
        
        public static SheetHeadInfo ParseHeadInfo(ISheet sheet, ExcelConfigHead config)
        {
            if (IsEmpty(sheet))
            {
                return null;
            }
            
            var typeRow = sheet.GetRow(config.TypeIndex);
            var nameRow = sheet.GetRow(config.NameIndex);
            if (IsEmpty(typeRow) || IsEmpty(nameRow))
            {
                return null;
            }
            
            var fieldsNum = Math.Max(typeRow.LastCellNum, nameRow.LastCellNum) + 1;
            var headInfo = new SheetHeadInfo(fieldsNum);

            for (var i = 0; i < fieldsNum; i++)
            {
                var fieldType = GetHeadFieldType(typeRow.GetCell(i));
                if (string.IsNullOrEmpty(fieldType) || fieldType.StartsWith(config.IgnoreMark))
                {
                    continue;
                }
                
                var fieldName = GetValue(nameRow.GetCell(i));
                if (string.IsNullOrEmpty(fieldType) || fieldName.StartsWith(config.IgnoreMark))
                {
                    continue;
                }
                
                headInfo.AddHeadField(i, fieldType, fieldName);
            }

            return headInfo;
        }

        public static IRow GetRowByIdentify(ISheet sheet, int identifyColumn, int identify, int beganRow,
            char ignoreMark)
        {
            if (IsEmpty(sheet))
            {
                return null;
            }

            IRow result = null;
            var firstRow = Math.Max(sheet.FirstRowNum, beganRow);
            for (var i = firstRow; i < sheet.LastRowNum + 1; i++)
            {
                var row = sheet.GetRow(i);
                if (IsEmpty(row))
                {
                    continue;
                }

                var cell = row.GetCell(identifyColumn);
                if (cell == null)
                {
                    continue;
                }

                var cellValue = GetValue(cell);
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