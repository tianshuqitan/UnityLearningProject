namespace Tools.Excel
{
    public class ExcelConfigHead : IExcelConfigHead
    {
        public static readonly ExcelConfigHead Default = new ExcelConfigHead();

        public int NameIndex { get; } = 0;
        public int TypeIndex { get; } = 1;
        public char IgnoreMark { get; } = '#';

        public ExcelConfigHead()
        {
        }

        public ExcelConfigHead(int nameRow, int typeRow, int attributeRow, char ignore)
        {
            NameIndex = nameRow;
            TypeIndex = typeRow;
            IgnoreMark = ignore;
        }
    }
}