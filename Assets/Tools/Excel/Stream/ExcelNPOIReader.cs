using System;
using System.IO;
using System.Reflection;
using NPOI.SS.UserModel;

namespace Tools.Excel
{
    public abstract class ExcelNPOIReader : ExcelNPOIBase, IExcelReader
    {
        private bool m_Disposed = false;

        protected ExcelNPOIReader(Stream stream, ExcelConfig config = null) : base(stream, config)
        {
        }
        
        #region InterfaceImplement

        public T Read<T>(string sheetName, string identify) where T : class, new()
        {
            var sheet = m_Workbook.GetSheet(sheetName);
            return sheet == null ? default : Read<T>(sheet, identify);
        }
        
        private T Read<T>(ISheet sheet, string identify) where T : class, new()
        {
            var row = GetRowByIdentify(sheet, identify);
            if (row == null)
            {
                return default;
            }

            var headInfo = GetSheetHeadInfo(sheet);
            if (headInfo == null)
            {
                return default;
            }

            var instance = new T();
            var instanceType = instance.GetType();
            var fieldInfos = instanceType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var fieldInfo in fieldInfos)
            {
                var cellIndex = headInfo.GetHeadFieldColumn(fieldInfo.Name);
                var cell = row.GetCell(cellIndex);
                var value = Convert.ChangeType(NPOIHelper.GetValue(cell), fieldInfo.FieldType);
                fieldInfo.SetValue(instance, value);
            }

            return instance;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (m_Disposed)
            {
                return;
            }

            if (disposing)
            {
                OnDispose();
            }

            m_Disposed = true;
        }

        #endregion Dispose
    }
}