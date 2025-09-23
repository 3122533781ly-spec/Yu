using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

 namespace Redeem {	
	public class GiftCardInfoDataImporter
	{
	    public const string ExcelPath = "网赚/礼品卡信息配置.xlsx";
	
	    public static void Import()
	    {
	        string excelPath = string.Concat(Application.dataPath.Replace("Assets", DataImporter.ExcelSaveFolderPath), "/",
	            ExcelPath);
	
	        List<Dictionary<string, string>> rowsDic = NPOIExcelReader.ParseDic(excelPath, DataImporter.DataRowStart);
	
	        ParseToConfig(rowsDic);
	        EditorUtility.SetDirty(GiftCardInfoConfig.Instance);
	        Debug.Log("import " + excelPath + " completed ");
	    }
	
	    private static void ParseToConfig(List<Dictionary<string, string>> rowsDic)
	    {
	        GiftCardInfoConfig.Instance.All.Clear();
	        for (int i = 0; i < rowsDic.Count; i++)
	        {
	            GiftCardInfoData data = new GiftCardInfoData();
	            data.id = ExcelImportUtils.ParseTo<int>(rowsDic[i]["ID"]);
	            data.name = rowsDic[i]["Name"];
	            if (!string.IsNullOrEmpty(rowsDic[i]["Sprite"]))
	            {
	                data.sprite = ExcelImportUtils.ParserToSprite(rowsDic[i]["Sprite"]);
	            }
	
	            GiftCardInfoConfig.Instance.All.Add(data);
	        }
	    }
	}
}
