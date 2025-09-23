using UnityEditor;
using UnityEngine;

public static class DataImporter
{
    //Excel 存储文件夹
    public const string ExcelSaveFolderPath = "Excel";

    //数据起始行
    public const int DataRowStart = 2;

#if UNITY_EDITOR  //

    [MenuItem("Breaker/Excel/ImportAllExcelData")]
    public static void Import()
    {
        Debug.Log("Start importExcelData");
        LevelDataImporter.Import();
    }

#endif
}