using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SlotMciDataImporter
{
    public const string ExcelPath = "网赚相关/老虎机抽奖配置表.xlsx";

    public static void Import()
    {
        string excelPath = string.Concat(Application.dataPath.Replace("Assets", DataImporter.ExcelSaveFolderPath), "/",
            ExcelPath);

        List<Dictionary<string, string>> rowsDic = NPOIExcelReader.ParseDic(excelPath, DataImporter.DataRowStart);

        ParseToConfig(rowsDic);
        EditorUtility.SetDirty(SlotMciDataConfig.Instance);
        Debug.Log("import " + excelPath + " completed ");
    }

    private static void ParseToConfig(List<Dictionary<string, string>> rowsDic)
    {
        SlotMciDataConfig.Instance.All.Clear();
        for (int i = 0; i < rowsDic.Count; i++)
        {
            var data = new SlotMciData();

            data.id = ExcelImportUtils.ParseTo<int>(rowsDic[i]["ID"]);
            data.poolId = ExcelImportUtils.ParseTo<int>(rowsDic[i]["PoolId"]);
            data.goodsId =  ExcelImportUtils.ParseTo<int>(rowsDic[i]["GoodsId"]);
            data.count =  ExcelImportUtils.ParseTo<int>(rowsDic[i]["Count"]);
            
            data.goodType = (GoodType) ExcelImportUtils.ParseTo<int>(rowsDic[i]["GoodsType"]);
            data.goodSubtype = ExcelImportUtils.ParseTo<int>(rowsDic[i]["subType"]);
            data.probability = ExcelImportUtils.ParseTo<int>(rowsDic[i]["Probability"]);

            data.rateAmount = ExcelImportUtils.ParseTo<float>(rowsDic[i]["RateAmount"]);

            SlotMciDataConfig.Instance.All.Add(data);
        }
    }
}