using System;
using System.IO;
using NPOI.SS.UserModel;

namespace Tools.Excel
{
    public abstract class ExcelNPOIWriter : ExcelNPOIBase, IExcelWriter
    {
        private bool m_Disposed = false;

        #region Contructor

        protected ExcelNPOIWriter(Stream stream, ExcelConfig config) : base(stream, config)
        {
        }

        #endregion

        #region InterfaceImplement

        public bool Write<T>(string path, T value)
        {
            var className = value.GetType().Name;
            var sheet = m_Workbook.GetSheet(className);
            return sheet != null && Write<T>(path, sheet, value);
        }

        private bool Write<T>(string path, ISheet sheet, T instance)
        {
            var headInfo = GetSheetHeadInfo(sheet);
            var headField = headInfo.GetHeadField(m_Config.IdentifyColumn);

            var instanceType = instance.GetType();
            var identifyFieldInfo = instanceType.GetField(headField.FieldName);
            var identifyField = identifyFieldInfo.GetValue(instance).ToString();
            var row = GetRowByIdentify(sheet, identifyField);
            if (row == null)
            {
                return false;
            }

            var fields = instanceType.GetFields();
            foreach (var fieldInfo in fields)
            {
                var fieldColumn = headInfo.GetHeadFieldColumn(fieldInfo.Name);
                var fieldValue = fieldInfo.GetValue(instance).ToString();
                row.GetCell(fieldColumn).SetCellValue(fieldValue);
            }

            using var stream = new FileStream(path, FileMode.Open, FileAccess.Write);
            m_Workbook.Write(stream, false);
            
            return true;
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