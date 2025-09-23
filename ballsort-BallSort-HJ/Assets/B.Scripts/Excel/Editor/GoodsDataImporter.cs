using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GoodsDataImporter
{
    public const string ExcelPath = "物品表.xlsx";

    public static void Import()
    {
        string excelPath = string.Concat(Application.dataPath.Replace("Assets", DataImporter.ExcelSaveFolderPath), "/",
            ExcelPath);

        List<Dictionary<string, string>> rowsDic = NPOIExcelReader.ParseDic(excelPath, DataImporter.DataRowStart);

        ParseToConfig(rowsDic);
        EditorUtility.SetDirty(GoodsConfig.Instance);
        Debug.Log("import " + excelPath + " completed ");
    }

    private static void ParseToConfig(List<Dictionary<string, string>> rowsDic)
    {
        GoodsConfig.Instance.All.Clear();
        for (int i = 0; i < rowsDic.Count; i++)
        {
            GoodsData data = new GoodsData();
            data.IntID = ExcelImportUtils.ParseTo<int>(rowsDic[i]["ID"]);
            data.Name = ExcelImportUtils.ParseTo<string>(rowsDic[i]["Name"]);
            data.Type = (GoodType) ExcelImportUtils.ParseTo<int>(rowsDic[i]["Type"]);
            data.subType =  ExcelImportUtils.ParseTo<int>(rowsDic[i]["subType"]);
            GoodsConfig.Instance.All.Add(data);
        }
    }
}