using System.Collections.Generic;
using _02.Scripts.Config;
using UnityEditor;
using UnityEngine;

namespace ProjectSpace.Lei31Utils.Scripts.Excel.Editor
{
    public class InGameRewardDataImporter
    {
        private const string ExcelPath = "C 抽奖表.xlsx";

        public static void Import()
        {
            string excelPath = string.Concat(Application.dataPath.Replace("Assets", DataImporter.ExcelSaveFolderPath), "/",
                ExcelPath);

            List<Dictionary<string, string>> rowsDic = NPOIExcelReader.ParseDic(excelPath, DataImporter.DataRowStart);

            ParseToConfig(rowsDic);
            EditorUtility.SetDirty(InGameRewardConfig.Instance);
            Debug.Log("import " + excelPath + " completed ");
        }

        private static void ParseToConfig(List<Dictionary<string, string>> rowsDic)
        {
            InGameRewardConfig.Instance.All.Clear();
            var index = 0;
            for (; index < rowsDic.Count; index++)
            {
                var t = rowsDic[index];
                var data = new InGameRewardData()
                {
                    intID = index,
                    ItemId = ExcelImportUtils.ParseTo<int>(t["ItemId"]),
                    Category = (GoodType)ExcelImportUtils.ParseTo<int>(t["Category"]),
                    Number = ExcelImportUtils.ParseTo<int>(t["Number"]),
                    Probability = ExcelImportUtils.ParseTo<int>(t["Probability"]),
                };
                InGameRewardConfig.Instance.All.Add(data);
            }
        }
    }
}
