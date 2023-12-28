using System;
using System.IO;
using System.Text;
using NPOI.SS.UserModel;

namespace Tools.Excel
{
    public class ExcelNPOIExporter : ExcelNPOIBase, IExcelExporter
    {
        private bool m_Disposed = false;
        
        protected ExcelNPOIExporter(Stream stream, ExcelConfig config) : base(stream, config)
        {
        }

        #region Export
        
        public void Export(ExportType exportType, string sheetName, IExportConfig config)
        {
            if (ExcelNPOIHelper.IsWorkbookEmpty(m_Workbook))
            {
                return;
            }

            if (exportType == ExportType.CS)
            {
                ExportCs(sheetName, (ExportConfigCs)config);
            }
            else
            {
                throw new NotSupportedException($"{exportType} is not supporter!!!");
            }
        }
        
        private void ExportCs(string sheetName, ExportConfigCs config)
        {
            if (string.IsNullOrEmpty(sheetName))
            {
                for (var i = 0; i < m_Workbook.NumberOfSheets; i++)
                {
                    ExportSheetCs(m_Workbook.GetSheetAt(i), config);
                }
            }
            else
            {
                var sheet = m_Workbook.GetSheet(sheetName);
                ExportSheetCs(sheet, config);
            }
        }
        
        private void ExportSheetCs(ISheet sheet, ExportConfigCs config)
        {
            if (sheet == null)
            {
                return;
            }
            
            var headInfo = GetSheetHeadInfo(sheet);
            if (headInfo == null)
            {
                return;
            }
            
            var template = Scriban.Template.Parse(config.ClassTemplate);
            var templateRet = template.Render(new
            {
                namespace_name = config.ClassNameSpace,
                class_name = sheet.SheetName,
                export_fields = headInfo.FieldsList,
            });
            
            var fullPath = Path.Combine(config.SavePath, sheet.SheetName + ".cs");
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            using var writer = new StreamWriter(File.Create(fullPath), Encoding.UTF8);
            writer.Write(templateRet);
            writer.Flush();
            writer.Close();
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