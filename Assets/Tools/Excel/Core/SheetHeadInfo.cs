using System.Collections.Generic;

namespace Tools.Excel
{
    public class SheetHeadField
    {
        public string FieldType;
        public string FieldName;
        public int Column;
    }
    
    public class SheetHeadInfo
    {
        private readonly Dictionary<string, int> m_FieldNameIndex;
        
        private Dictionary<int, SheetHeadField> Fields { get; }

        public readonly List<SheetHeadField> FieldsList;

        public SheetHeadInfo(int capacity = 1)
        {
            Fields = new Dictionary<int, SheetHeadField>(capacity);
            m_FieldNameIndex = new Dictionary<string, int>(capacity);
            FieldsList = new List<SheetHeadField>(capacity);
        }

        public void AddHeadField(int column, string fieldType, string fieldName)
        {
            var headField = new SheetHeadField
            {
                Column = column,
                FieldType = fieldType,
                FieldName = fieldName
            };
            Fields.Add(column, headField);
            m_FieldNameIndex.Add(fieldName, column);
            FieldsList.Add(headField);
        }
        
        public SheetHeadField GetHeadField(int column)
        {
            return !Fields.ContainsKey(column) ? null : Fields[column];
        }
        
        public int GetHeadFieldColumn(string fieldName)
        {
            return !m_FieldNameIndex.ContainsKey(fieldName) ? -1 : m_FieldNameIndex[fieldName];
        }
    }
}