using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.Excel.Editor
{
    public class ExcelToolsWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset m_TreeAsset;
        [SerializeField] public ExcelSetting m_Setting;

        private int m_TabIndex = 0;
        private List<VisualElement> m_TabContent = new List<VisualElement>();
        private Label m_ExcelContent;

        [SerializeField] private string m_FilePath;
        [SerializeField] private string m_FolderPath = "";
        [SerializeField] private string m_ExcelName = "NewExcel";
        [SerializeField] private string m_ExcelExtension = "xls";
        [SerializeField] private string m_ExcelClassName = "NewClass";

        #region ToolsFunction

        private T GetElement<T>(string name = null) where T : VisualElement
        {
            return rootVisualElement.Query<T>(name).First();
        }

        private static void SetVisible(VisualElement element, bool visible)
        {
            element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void RegisterButtonClick(string name, Action clickAction)
        {
            var button = GetElement<Button>(name);
            if (button != null)
            {
                button.clicked += clickAction;
            }
        }

        private void BindTextField(string name, EventCallback<ChangeEvent<string>> callback)
        {
            var element = GetElement<TextField>(name);
            element?.RegisterValueChangedCallback(callback);
        }

        #endregion ToolsFunction

        #region Inherit

        private void CreateGUI()
        {
            // m_TreeAsset.CloneTree(rootVisualElement);
            rootVisualElement.Add(m_TreeAsset.CloneTree());
            rootVisualElement.Bind(new SerializedObject(this));

            m_FilePath = m_Setting.FilePath;
            m_FolderPath = m_Setting.ExcelFolder;

            InitToolbar();
            InitReadPanel();
            InitCreatePanel();
        }

        #endregion Inherit

        #region Toolbar

        private void InitToolbar()
        {
            RegisterButtonClick("ToolbarButtonRead", () => { OnTabButtonClicked(0); });
            RegisterButtonClick("ToolbarButtonCreate", () => { OnTabButtonClicked(1); });
            RegisterButtonClick("ToolbarButtonExport", () => { OnTabButtonClicked(2); });

            m_TabContent.Add(GetElement<VisualElement>("ReadPanel"));
            m_TabContent.Add(GetElement<VisualElement>("CreatePanel"));

            OnTabButtonClicked(m_TabIndex, true);
        }

        private void OnTabButtonClicked(int index, bool force = false)
        {
            if (m_TabIndex == index && force == false)
            {
                return;
            }

            for (var i = 0; i < m_TabContent.Count; i++)
            {
                SetVisible(m_TabContent[i], i == index);
            }

            m_TabIndex = index;
        }

        #endregion Toolbar

        #region ReadPanel

        private void InitReadPanel()
        {
            m_ExcelContent = GetElement<Label>("LabelContent");

            RegisterButtonClick("ButtonReadExcel", () =>
            {
                if (!File.Exists(m_FilePath))
                {
                    EditorUtility.DisplayDialog("警告", "文件不存在!!!", "确定");
                    return;
                }
                
                // var result = ExcelHelper.Read<SkillTest>(m_FilePath, "SkillTest", 1000, ExcelConfig.DefaultConfig);
                // m_ExcelContent.text = result.ToString();
                // result.name = "12346";
                // ExcelHelper.Write(m_FilePath, result, ExcelConfig.DefaultConfig);
            });
            
            RegisterButtonClick("ButtonGenerate", () =>
            {
                if (!File.Exists(m_FilePath))
                {
                    EditorUtility.DisplayDialog("警告", "文件不存在!!!", "确定");
                    return;
                }
                
                var exportConfig = new ExportConfigCs
                {
                    ClassNameSpace = "Tools.Excel",
                    ClassTemplate = m_Setting.classTemplate.text,
                    SavePath = m_Setting.GenerateFolder,
                };

                ExcelHelper.Export(m_FilePath, null, ExportType.CS, ExcelConfig.DefaultConfig, exportConfig);
                AssetDatabase.Refresh();
            });
        }
        
        #endregion ReadPanel

        #region CreatePanel

        private void InitCreatePanel()
        {
            RegisterButtonClick("ButtonOpenFolder", () =>
            {
#if UNITY_EDITOR_WIN
                System.Diagnostics.Process.Start("explorer.exe", m_FolderPath);
#endif
            });

            RegisterButtonClick("ButtonCreateExcel", () => { });
        }

        private string GetNewExcelFullPath()
        {
            var fullPath = "";
            return fullPath;
        }

        #endregion
    }
}