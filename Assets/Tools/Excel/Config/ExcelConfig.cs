namespace Tools.Excel
{
    public class ExcelConfig : IExcelConfig
    {
        public static readonly ExcelConfig DefaultConfig = new();
        public readonly ExcelConfigHead ConfigHead;

        /// <summary>
        /// 表格唯一标识列
        /// </summary>
        public int IdentifyColumn { get; } = 0;

        /// <summary>
        /// 表格数据从哪一行开始
        /// </summary>
        public int DataBeganRow { get; } = 3;

        public ExcelConfig()
        {
            ConfigHead = ExcelConfigHead.Default;
        }
        
        public ExcelConfig(int identifyColumn, int dataBeganRow, ExcelConfigHead headConfig = null)
        {
            IdentifyColumn = identifyColumn;
            DataBeganRow = dataBeganRow;
            ConfigHead = headConfig ?? ExcelConfigHead.Default;
        }
    }
}