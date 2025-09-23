using System.Collections.Generic;
using _02.Scripts.Config;
using UnityEditor;
using UnityEngine;

namespace ProjectSpace.Lei31Utils.Scripts.Excel.Editor
{
    public class PipeAndBallDataImporter : MonoBehaviour
    {
        public const string ExcelPath = "PipeAndBall.xlsx";

        public static void Import()
        {
            string excelPath = string.Concat(Application.dataPath.Replace("Assets", DataImporter.ExcelSaveFolderPath),
                "/",
                ExcelPath);

            List<Dictionary<string, string>> rowsDic = NPOIExcelReader.ParseDic(excelPath, DataImporter.DataRowStart);

            ParseToConfig(rowsDic);
            EditorUtility.SetDirty(PipeAndBallConfig.Instance);
            Debug.Log("import " + excelPath + " completed ");
        }

        private static void ParseToConfig(List<Dictionary<string, string>> rowsDic)
        {
            PipeAndBallConfig.Instance.All.Clear();
            for (int i = 0; i < rowsDic.Count; i++)
            {
                PipeAndBallData data = new PipeAndBallData();

                data.intID = ExcelImportUtils.ParseTo<int>(rowsDic[i]["id"]);
                data.subType = ExcelImportUtils.ParseTo<int>(rowsDic[i]["subType"]);
                data.ball = ExcelImportUtils.ParseTo<int>(rowsDic[i]["ball"]);
                data.pipeW = ExcelImportUtils.ParseTo<int>(rowsDic[i]["pipeW"]);
                data.pipeH = ExcelImportUtils.ParseTo<int>(rowsDic[i]["pipeH"]);
                data.pipeHSpace = ExcelImportUtils.ParseTo<int>(rowsDic[i]["pipeHSpace"]);
                data.pipeWSpace = ExcelImportUtils.ParseTo<int>(rowsDic[i]["pipeWSpace"]);

                PipeAndBallConfig.Instance.All.Add(data);
            }
        }
    }
}