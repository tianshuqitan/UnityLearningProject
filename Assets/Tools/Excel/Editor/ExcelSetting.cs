using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Tools.Excel.Editor
{
    [Serializable]
    public class ExcelRelativeClass
    {
        public string excelName;
        public string className;

        public Type GetRelativeType()
        {
            return Type.GetType("Tools.Excel." + className);
        }
    }
    
    // [CreateAssetMenu(menuName = "Tools/Excel/CreateExcelSetting", fileName = "ExcelSettingAsset")]
    public class ExcelSetting : ScriptableObject
    {
        [SerializeField] private string filePath = "";
        [SerializeField] private string excelFolder = "";
        [SerializeField] private string generateFolder = "";
        [SerializeField] public TextAsset classTemplate;

        [SerializeField] public List<ExcelRelativeClass> relativeList = new List<ExcelRelativeClass>();
        
        public string FilePath => GetFullPath(filePath);
        public string ExcelFolder => GetFullPath(excelFolder);
        public string GenerateFolder => GetFullPath(generateFolder);
        
        private static string GetFullPath(string relativePath)
        {
            var path = Application.dataPath + "/../" + relativePath;
            return Path.GetFullPath(path);
        }
    }
}