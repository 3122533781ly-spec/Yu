using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelDataImporter
{
    public const string ExcelPath = "关卡表.xlsx";

    public static void Import()
    {
        string excelPath = string.Concat(Application.dataPath.Replace("Assets", DataImporter.ExcelSaveFolderPath), "/",
            ExcelPath);

        List<Dictionary<string, string>> rowsDic = NPOIExcelReader.ParseDic(excelPath, DataImporter.DataRowStart);

        List<LineBeeLevelData> list = ParseToConfig(rowsDic);

        LevelDataConfig.Instance.All.Clear();
        LevelDataConfig.Instance.All.AddRange(list);
#if UNITY_EDITOR  //
        EditorUtility.SetDirty(LevelDataConfig.Instance);
#endif
        Debug.Log("import " + excelPath + " completed ");
    }

    private static List<LineBeeLevelData> ParseToConfig(List<Dictionary<string, string>> rowsDic)
    {
        List<LineBeeLevelData> result = new List<LineBeeLevelData>();

        for (int i = 0; i < rowsDic.Count; i++)
        {
            LineBeeLevelData data = new LineBeeLevelData();
            data.IntID = ExcelImportUtils.ParseTo<int>(rowsDic[i]["Id"]);
            data.BoardID = ExcelImportUtils.ParseTo<int>(rowsDic[i]["BoardID"]);
            result.Add(data);
        }

        return result;
    }
}