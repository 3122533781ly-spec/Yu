using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Redeem
{
    public class RedeemCountryDataImporter : MonoBehaviour
    {
        public const string ExcelPath = "网赚相关/国家配置.xlsx";
        public const string ExcelPathB = "网赚相关/国家配置B.xlsx";
        public const string ExcelPathC = "网赚相关/国家配置C.xlsx";

        public static void Import()
        {
            ImportA();
            ImportB();
            ImportC();
        }

        public static void ImportA()
        {
            string excelPath = string.Concat(Application.dataPath.Replace("Assets", DataImporter.ExcelSaveFolderPath), "/",
                ExcelPath);

            List<Dictionary<string, string>> rowsDic = NPOIExcelReader.ParseDic(excelPath, DataImporter.DataRowStart);

            RedeemCountryConfig.Instance.All.Clear();
            RedeemCountryConfig.Instance.All.AddRange(ParseToConfig(rowsDic));

            EditorUtility.SetDirty(RedeemCountryConfig.Instance);
            Debug.Log("import " + excelPath + " completed ");
        }

        public static void ImportB()
        {
            string excelPath = string.Concat(Application.dataPath.Replace("Assets", DataImporter.ExcelSaveFolderPath), "/",
                ExcelPathB);

            List<Dictionary<string, string>> rowsDic = NPOIExcelReader.ParseDic(excelPath, DataImporter.DataRowStart);

            RedeemCountryBConfig.Instance.All.Clear();
            RedeemCountryBConfig.Instance.All.AddRange(ParseToConfig(rowsDic));

            EditorUtility.SetDirty(RedeemCountryBConfig.Instance);
            Debug.Log("import " + ExcelPathB + " completed ");
        }

        public static void ImportC()
        {
            string excelPath = string.Concat(Application.dataPath.Replace("Assets", DataImporter.ExcelSaveFolderPath), "/",
                ExcelPathC);

            List<Dictionary<string, string>> rowsDic = NPOIExcelReader.ParseDic(excelPath, DataImporter.DataRowStart);

            RedeemCountryCConfig.Instance.All.Clear();
            RedeemCountryCConfig.Instance.All.AddRange(ParseToConfig(rowsDic));

            EditorUtility.SetDirty(RedeemCountryConfig.Instance);
            Debug.Log("import " + ExcelPathC + " completed ");
        }

        private static List<RedeemCountryData> ParseToConfig(List<Dictionary<string, string>> rowsDic)
        {
            List<RedeemCountryData> datas = new List<RedeemCountryData>();
            for (int i = 0; i < rowsDic.Count; i++)
            {
                RedeemCountryData data = new RedeemCountryData();
                data.id = ExcelImportUtils.ParseTo<int>(rowsDic[i]["ID"]);
                data.countryCode = rowsDic[i]["CountryCode"];
                data.remark = rowsDic[i]["Remark"];
                data.moneyConfigs = ExcelImportUtils.ParseInts(rowsDic[i]["MoneyConfig"]);
                data.giftCardConfigs = ExcelImportUtils.ParseInts(rowsDic[i]["GiftCardConfig"]);

                datas.Add(data);
            }
            return datas;
        }
    }
}
