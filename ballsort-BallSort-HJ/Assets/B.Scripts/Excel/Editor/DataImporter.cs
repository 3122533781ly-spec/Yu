using ProjectSpace.Lei31Utils.Scripts.Excel.Editor;
using Redeem;
using UnityEditor;
using UnityEngine;

public static class DataImporter
{
    //Excel 存储文件夹
    public const string ExcelSaveFolderPath = "Excel";

    //数据起始行
    public const int DataRowStart = 2;

    [MenuItem("VentMatch/Excel/ImportAllExcelData")]
    public static void Import()
    {
        Debug.Log("Start importExcelData");


        GuideUIImporter.Import();
        // PriceListImporter.Import();
        // RolePlotImporter.Import();
        PipeAndBallDataImporter.Import();
        ConstantDataImporter.Import();
        InGameRewardDataImporter.Import();
        SlotMciDataImporter.Import();
        GoodsDataImporter.Import();
        // constant.Import();
        
        
        MoneyRedeemDataImporter.Import();
        DollarClearDataImporter.Import();
        RedeemCountryDataImporter.Import();
        GiftCardRedeemDataImporter.Import();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}