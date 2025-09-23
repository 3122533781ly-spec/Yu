using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GuideUIImporter
{
    private const string ExcelPath = "引导表.xlsx";

    public static void Import()
    {
        string excelPath = string.Concat(Application.dataPath.Replace("Assets", DataImporter.ExcelSaveFolderPath), "/",
            ExcelPath);

        List<Dictionary<string, string>> rowsDic = NPOIExcelReader.ParseDic(excelPath, DataImporter.DataRowStart);

        ParseToConfig(rowsDic);
        EditorUtility.SetDirty(GuideUIConfig.Instance);
        Debug.Log("import " + excelPath + " completed ");
    }

    private static void ParseToConfig(List<Dictionary<string, string>> rowsDic)
    {
        GuideUIConfig.Instance.All.Clear();
        var index = 0;
        for (; index < rowsDic.Count; index++)
        {
            var t = rowsDic[index];
            var data = new GuideUIData
            {
                Id = ExcelImportUtils.ParseTo<int>(t["Id"]),
                Type = ExcelImportUtils.ParseTo<int>(t["Type"]),
                Index = ExcelImportUtils.ParseTo<int>(t["Index"]),
                Info = ExcelImportUtils.ParseTo<string>(t["Info"]),
                DialogY = ExcelImportUtils.ParseTo<int>(t["DialogY"]),
                HideArrow = ExcelImportUtils.ParseTo<bool>(t["HideArrow"]),
                Gesture = ExcelImportUtils.ParseTo<int>(t["Gesture"]),
                GesturePath = ExcelImportUtils.ParseTo<string>(t["GesturePath"]),
                Path = ExcelImportUtils.ParseTo<string>(t["Path"]),
                Shape = ExcelImportUtils.ParseTo<int>(t["Shape"]),
                Event = ExcelImportUtils.ParseTo<int>(t["Event"]),
                EventPath = ExcelImportUtils.ParseTo<string>(t["EventPath"]),
                Role = ExcelImportUtils.ParseTo<int>(t["Role"]),
                RolePos = ExcelImportUtils.ParseVector2Int(t["RolePos"]),
                ArrowOffset = ExcelImportUtils.ParseVector2Int(t["ArrowOffset"]),
            };
            GuideUIConfig.Instance.All.Add(data);
        }
    }
}
