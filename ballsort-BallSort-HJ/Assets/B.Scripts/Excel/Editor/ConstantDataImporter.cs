using System.Collections.Generic;
using _02.Scripts.Config;
using UnityEditor;
using UnityEngine;

namespace ProjectSpace.Lei31Utils.Scripts.Excel.Editor
{
    public class ConstantDataImporter
    {
        private const string ExcelPath = "常量表.xlsx";

        public static void Import()
        {
            string excelPath = string.Concat(Application.dataPath.Replace("Assets", DataImporter.ExcelSaveFolderPath), "/",
                ExcelPath);

            List<Dictionary<string, string>> rowsDic = NPOIExcelReader.ParseDic(excelPath, DataImporter.DataRowStart);

            ParseToConfig(rowsDic);
            EditorUtility.SetDirty(ConstantConfig.Instance);
            Debug.Log("import " + excelPath + " completed ");
        }

        private static void ParseToConfig(List<Dictionary<string, string>> rowsDic)
        {
            ConstantConfig.Instance.All.Clear();
            var index = 0;
            for (; index < rowsDic.Count; index++)
            {
                var t = rowsDic[index];
                var data = new ConstantData
                {
                    IntID = ExcelImportUtils.ParseTo<int>(t["Id"]),
                    Value = ExcelImportUtils.ParseTo<int>(t["Value"]),
                    ValueList = ExcelImportUtils.ParseInts(t["ValueList"]),
                    Desc = ExcelImportUtils.ParseTo<string>(t["Desc"])
                };
                ConstantConfig.Instance.All.Add(data);
            }
        }
    }
}
