using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DollarClearDataImporter
{
    public const string ExcelPath = "网赚相关/结算美元配置表.xlsx";

    public static void Import()
    {
        string excelPath = string.Concat(Application.dataPath.Replace("Assets", DataImporter.ExcelSaveFolderPath), "/",
            ExcelPath);

        List<Dictionary<string, string>> rowsDic = NPOIExcelReader.ParseDic(excelPath, DataImporter.DataRowStart);

        ParseToConfig(rowsDic);
        EditorUtility.SetDirty(DollarClearDataConfig.Instance);
        Debug.Log("import " + excelPath + " completed ");
    }

    private static void ParseToConfig(List<Dictionary<string, string>> rowsDic)
    {
        DollarClearDataConfig.Instance.All.Clear();
        for (int i = 0; i < rowsDic.Count; i++)
        {
            var data = new DollarClearData();

            data.id = ExcelImportUtils.ParseTo<int>(rowsDic[i]["ID"]);
            data.range = ExcelImportUtils.ParseVector2(rowsDic[i]["Range"]);
            data.number = ExcelImportUtils.ParseTo<float>(rowsDic[i]["Number"]);

            data.probability = ExcelImportUtils.ParseTo<int>(rowsDic[i]["Probability"]);

            data.gemRange = ExcelImportUtils.ParseVector2(rowsDic[i]["GemRange"]);
            data.gemNumber = ExcelImportUtils.ParseTo<int>(rowsDic[i]["GemNumber"]);

            DollarClearDataConfig.Instance.All.Add(data);
        }
    }

}
