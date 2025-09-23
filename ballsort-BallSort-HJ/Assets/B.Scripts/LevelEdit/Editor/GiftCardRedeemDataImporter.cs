using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Redeem
{
    public class GiftCardRedeemDataImporter
    {
        public const string ExcelPath = "网赚相关/礼品卡兑换配置.xlsx";

        public static void Import()
        {
            string excelPath = string.Concat(Application.dataPath.Replace("Assets", DataImporter.ExcelSaveFolderPath), "/",
                ExcelPath);

            List<Dictionary<string, string>> rowsDic = NPOIExcelReader.ParseDic(excelPath, DataImporter.DataRowStart);

            ParseToConfig(rowsDic);
            EditorUtility.SetDirty(GiftCardRedeemConfig.Instance);
            Debug.Log("import " + excelPath + " completed ");
        }

        private static void ParseToConfig(List<Dictionary<string, string>> rowsDic)
        {
            GiftCardRedeemConfig.Instance.All.Clear();
            for (int i = 0; i < rowsDic.Count; i++)
            {
                GiftCardRedeemData data = new GiftCardRedeemData();
                data.id = ExcelImportUtils.ParseTo<int>(rowsDic[i]["ID"]);
                data.money = ExcelImportUtils.ParseTo<int>(rowsDic[i]["Money"]);
                data.diamond = ExcelImportUtils.ParseTo<int>(rowsDic[i]["Diamond"]);
                data.configType = rowsDic[i]["ConfigType"];
                data.cardID = ExcelImportUtils.ParseTo<int>(rowsDic[i]["CardID"]);

                List<Vector3> vector3s = ExcelImportUtils.ParseToVector3List(rowsDic[i]["Condition"]);
                data.conditionDatas = new List<RedeemConditionData>();
                foreach (var item in vector3s)
                {
                    data.conditionDatas.Add(new RedeemConditionData(item));
                }

                GiftCardRedeemConfig.Instance.All.Add(data);
            }
        }
    }
}
