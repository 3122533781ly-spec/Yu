using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Redeem
{
    public class MoneyRedeemDataImporter
    {
        public const string ExcelPath = "网赚相关/现金提现配置.xlsx";

        public static void Import()
        {
            string excelPath = string.Concat(Application.dataPath.Replace("Assets", DataImporter.ExcelSaveFolderPath), "/",
                ExcelPath);

            List<Dictionary<string, string>> rowsDic = NPOIExcelReader.ParseDic(excelPath, DataImporter.DataRowStart);

            ParseToConfig(rowsDic);
            EditorUtility.SetDirty(MoneyRedeemConfig.Instance);
            Debug.Log("import " + excelPath + " completed ");
        }

        private static void ParseToConfig(List<Dictionary<string, string>> rowsDic)
        {
            MoneyRedeemConfig.Instance.All.Clear();
            for (int i = 0; i < rowsDic.Count; i++)
            {
                MoneyRedeemData data = new MoneyRedeemData();
                data.id = ExcelImportUtils.ParseTo<int>(rowsDic[i]["ID"]);
                data.money = ExcelImportUtils.ParseTo<int>(rowsDic[i]["Money"]);
                data.configType = rowsDic[i]["ConfigType"];

                List<Vector3> vector3s = ExcelImportUtils.ParseToVector3List(rowsDic[i]["Condition"]);
                data.conditionDatas = new List<RedeemConditionData>();
                foreach (var item in vector3s)
                {
                    data.conditionDatas.Add(new RedeemConditionData(item));
                }

                MoneyRedeemConfig.Instance.All.Add(data);
            }
        }
    }
}
