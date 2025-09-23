using System.Collections.Generic;
using System.Linq;
using _02.Scripts.Util;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _02.Scripts.LevelEdit.Editor
{
    public enum EditType
    {
        Default,
        Special
    }


    public class LevelToolEditor : EditorWindow
    {
        private static int toolHeight = 800;
        private static int toolWidth = 820;

        private int _currentLevel = 1;
        private LevelConfig _levelAsset;
        private SpecialLevelConfig _specialLevelAsset;
        private LevelData _levelData;
        private GridLayoutGroup _panel;
        private PipeLevelEdit _pipePrefab;
        private List<BallType> _ballTypeList;
        private List<PipeLevelEdit> _pipeList = new List<PipeLevelEdit>();
        private static EditType _type;


        #region UI

        private Vector2 _scrollView;
        private PipeNumber _pipeNumber;
        private PipeCapacity _pipeCapacity;
        private string _rewardCoin;
        private int _rewardCoinCount;
        private string _randomStep;
        private int _randomStepCount;
        private bool _isBlindBox;

        #endregion

        private List<LevelData> GetConfigGroup()
        {
            if (EditType.Default == _type)
            {
                return _levelAsset.All;
            }

            return _specialLevelAsset.All;
        }


        [MenuItem("工具/关卡编辑器/默认关卡")]
        public static void EditAsset1()
        {
            _type = EditType.Default;

            EditAsset();
        }

        [MenuItem("工具/关卡编辑器/特殊关卡")]
        public static void EditAsset2()
        {
            _type = EditType.Special;

            EditAsset();
        }


        private static void EditAsset()
        {
            EditorSceneManager.OpenScene("Assets/01.Scenes/DefaultLevelEdit.unity", OpenSceneMode.Single);

            EditorWindow editorWin = GetWindowWithRect(typeof(LevelToolEditor),
                new Rect(new Vector2(0, 0), new Vector2(toolWidth, toolHeight)));
            editorWin.titleContent = new GUIContent("关卡编辑器");
            editorWin.Show();
        }

        private void OnEnable()
        {
            LoadData();
            Init();
        }


        private void LoadData()
        {
            if (_type == EditType.Default)
            {
                var path = $"Assets/Resources/LevelConfig.asset";
                _levelAsset = AssetDatabase.LoadAssetAtPath<LevelConfig>(path);
                if (!_levelAsset)
                {
                    var asset = CreateInstance<LevelConfig>();
                    AssetDatabase.CreateAsset(asset, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    _levelAsset = AssetDatabase.LoadAssetAtPath<LevelConfig>(path);
                }
            }
            else
            {
                var path = $"Assets/Resources/SpecialLevelConfig.asset";
                _specialLevelAsset = AssetDatabase.LoadAssetAtPath<SpecialLevelConfig>(path);
                if (!_specialLevelAsset)
                {
                    var asset = CreateInstance<SpecialLevelConfig>();
                    AssetDatabase.CreateAsset(asset, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    _specialLevelAsset = AssetDatabase.LoadAssetAtPath<SpecialLevelConfig>(path);
                }
            }
        }

        private void Init()
        {
            _panel = GameObject.Find("Canvas/Bg/Panel").GetComponent<GridLayoutGroup>();
            _pipePrefab = GameObject.Find("Pipe").GetComponent<PipeLevelEdit>();

            //创建一个空数据
            if (GetConfigGroup().Count == 0)
            {
                NewLevel();
            }
            else
            {
                _levelData = GetConfigGroup()[_currentLevel - 1];
            }

            _ballTypeList = new List<BallType>();

            for (int i = 0; i < 11; i++)
            {
                _ballTypeList.Add(BallType.ID1);
            }

            Refresh();
        }


        /// <summary>
        /// 创建新的关卡
        /// </summary>
        private void NewLevel(PipeNumber number = PipeNumber.None, PipeCapacity capacity = PipeCapacity.None)
        {
            _levelData = new LevelData();
            GetConfigGroup().Add(_levelData);
            _levelData.InitNewLevelData(GetConfigGroup().Count, number, capacity);
            Refresh();
        }


        private void Refresh()
        {
            RemovePipe();
            ResetPerp();
            SpawnPipe();
            SetGridLayOut(_pipeCapacity);
        }

        private void ResetPerp()
        {
            _randomStep = "";
            _randomStepCount = 0;
            _currentLevel = _levelData.levelId;
            // _rewardCoin = $"{_levelData.rewardData.count}";
            // _rewardCoinCount = _levelData.rewardData.count;
            _pipeNumber = _levelData.pipeNumber;
            _pipeCapacity = _levelData.GetPipeCapacity();
            _isBlindBox = _levelData.blindBox;
        }


        #region EditUI

        private void OnGUI()
        {
            ShowLevelList();
            ShowTop();
            ShowBottom();
            ShowContent();
        }

        private void ShowTop()
        {
            GUILayout.BeginArea(new Rect(0, 0, 800, 22), new GUIStyle(GUI.skin.box));

            GUILayout.BeginHorizontal();
            GUILayout.Label($"-总关卡:{GetConfigGroup().Count}    当前关卡:{_levelData.levelId}");

            if (GUILayout.Button("新建关卡"))
            {
                NewLevel();
            }


            if (GUILayout.Button("保存关卡"))
            {
                SaveLevel();
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void RandomFunc(int randomCount)
        {
            if (_pipeList.Count > 3)
            {
                //Step 1 先相同颜色摆好n-2管（n=试管数）	
                foreach (var t in _pipeList)
                {
                    t.CleanBall();
                }


                var randomTypeList = new List<BallType>();


                //给管子补充球
                for (int i = 0; i < _pipeList.Count - 2; i++)
                {
                    //特殊关卡必定有金币和钱
                    if (_type == EditType.Special && i == 0)
                    {
                        randomTypeList.Add(BallType.Coin);
                    }
                    else if (_type == EditType.Special && i == 1)
                    {
                        randomTypeList.Add(BallType.Money);
                    }
                    else
                    {
                        var randomType = (BallType) Random.Range(1, 10);

                        while (randomTypeList.Contains(randomType))
                        {
                            randomType = (BallType) Random.Range(1, 10);
                        }

                        randomTypeList.Add(randomType);
                    }

                    _pipeList[i].FullBall(new BallData(randomTypeList[i]));
                }


                for (int i = 0; i < randomCount; i++)
                {
                    RandomStep();
                }
            }
        }

        private void ShowBottom()
        {
            GUILayout.BeginArea(new Rect(140, 430, 660, 300), new GUIStyle(GUI.skin.box));
            GUILayout.BeginHorizontal();

            GUILayout.Label("管子数量:");
            _pipeNumber = (PipeNumber) EditorGUILayout.EnumPopup(_pipeNumber);
            GUILayout.Label("管子容量:");
            _pipeCapacity = (PipeCapacity) EditorGUILayout.EnumPopup(_pipeCapacity);

            if (GUILayout.Button("生成管子"))
            {
                RemovePipe();
                _levelData.InitNewLevelData(_currentLevel, _pipeNumber, _pipeCapacity);
                SpawnPipe();
                SetGridLayOut(_pipeCapacity);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();

            GUILayout.Label("奖励金币数量:", GUILayout.Width(80));
            _rewardCoin = GUILayout.TextField(_rewardCoin, GUILayout.Width(80));
            int.TryParse(_rewardCoin, out _rewardCoinCount);
            // _levelData.rewardData = new RewardData(GoodType.Coin, _rewardCoinCount);

            GUILayout.Space(10);
            GUILayout.Label("是否盲盒:", GUILayout.Width(60));
            _isBlindBox = GUILayout.Toggle(_isBlindBox, "");
            _levelData.blindBox = _isBlindBox;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            GUILayout.Label("随机步数:", GUILayout.Width(80));
            _randomStep = GUILayout.TextField(_randomStep, GUILayout.Width(80));
            int.TryParse(_randomStep, out _randomStepCount);

            GUILayout.Space(10);

            if (GUILayout.Button("确认随机"))
            {
                RandomFunc(_randomStepCount);
            }

            GUILayout.Space(10);
            if (GUILayout.Button("自动生成"))
            {
                AutoMove();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Label("循环几次:", GUILayout.Width(80));
            _spawnLevelCountStr = GUILayout.TextField(_spawnLevelCountStr, GUILayout.Width(80));
            int.TryParse(_spawnLevelCountStr, out _spawnLevelCount);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("关卡", GUILayout.Width(80));
            GUILayout.Label("管子数量", GUILayout.Width(80));
            GUILayout.Label("管子容量", GUILayout.Width(80));
            GUILayout.Label("随机步数", GUILayout.Width(80));
            GUILayout.Label("是否盲盒", GUILayout.Width(80));
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            GUILayout.Label("关卡1", GUILayout.Width(80));
            _pipeNumber1 = (PipeNumber) EditorGUILayout.EnumPopup(_pipeNumber1, GUILayout.Width(80));
            _pipeCapacity1 = (PipeCapacity) EditorGUILayout.EnumPopup(_pipeCapacity1, GUILayout.Width(80));
            _randomStep1 = GUILayout.TextField(_randomStep1, GUILayout.Width(80));
            int.TryParse(_randomStep1, out _randomStepCount1);
            GUILayout.Space(15);
            _isBlindBox1 = GUILayout.Toggle(_isBlindBox1, "", GUILayout.Width(80));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("关卡2", GUILayout.Width(80));
            _pipeNumber2 = (PipeNumber) EditorGUILayout.EnumPopup(_pipeNumber2, GUILayout.Width(80));
            _pipeCapacity2 = (PipeCapacity) EditorGUILayout.EnumPopup(_pipeCapacity2, GUILayout.Width(80));
            _randomStep2 = GUILayout.TextField(_randomStep2, GUILayout.Width(80));
            int.TryParse(_randomStep2, out _randomStepCount2);
            GUILayout.Space(15);
            _isBlindBox2 = GUILayout.Toggle(_isBlindBox2, "", GUILayout.Width(80));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("关卡3", GUILayout.Width(80));
            _pipeNumber3 = (PipeNumber) EditorGUILayout.EnumPopup(_pipeNumber3, GUILayout.Width(80));
            _pipeCapacity3 = (PipeCapacity) EditorGUILayout.EnumPopup(_pipeCapacity3, GUILayout.Width(80));
            _randomStep3 = GUILayout.TextField(_randomStep3, GUILayout.Width(80));
            int.TryParse(_randomStep3, out _randomStepCount3);
            GUILayout.Space(15);
            _isBlindBox3 = GUILayout.Toggle(_isBlindBox3, "", GUILayout.Width(80));
            GUILayout.EndHorizontal();


            if (GUILayout.Button("确认生成"))
            {
                for (int i = 0; i < _spawnLevelCount; i++)
                {
                    NewLevel(_pipeNumber1, _pipeCapacity1);
                    _isBlindBox = _isBlindBox1;
                    _levelData.blindBox = _isBlindBox1;
                    RandomFunc(_randomStepCount1);
                    AutoMove();

                    NewLevel(_pipeNumber2, _pipeCapacity2);
                    _isBlindBox = _isBlindBox2;
                    _levelData.blindBox = _isBlindBox2;
                    RandomFunc(_randomStepCount2);
                    AutoMove();

                    NewLevel(_pipeNumber3, _pipeCapacity3);
                    _isBlindBox = _isBlindBox3;
                    _levelData.blindBox = _isBlindBox3;
                    RandomFunc(_randomStepCount3);
                    AutoMove();
                }
            }

            GUILayout.EndArea();
        }

        private int _spawnLevelCount;
        private string _spawnLevelCountStr;

        private int _randomStepCount1;
        private bool _isBlindBox1;
        private string _randomStep1;
        private PipeNumber _pipeNumber1;
        private PipeCapacity _pipeCapacity1;

        private int _randomStepCount2;
        private bool _isBlindBox2;
        private string _randomStep2;
        private PipeNumber _pipeNumber2;
        private PipeCapacity _pipeCapacity2;


        private int _randomStepCount3;
        private bool _isBlindBox3;
        private string _randomStep3;
        private PipeNumber _pipeNumber3;
        private PipeCapacity _pipeCapacity3;


        private void ShowContent()
        {
            GUILayout.BeginArea(new Rect(140, 30, 660, 400), new GUIStyle(GUI.skin.box));

            for (int i = 0; i < _levelData.GetPipeCount(); i++)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label($"试管{i + 1}", GUILayout.Width(40));

                GUILayout.Label("要添加的球:", GUILayout.Width(80));

                var currentPipe = _pipeList[i];

                _ballTypeList[i] =
                    (BallType) EditorGUILayout.EnumPopup(_ballTypeList[i],
                        GUILayout.Width(80));


                if (GUILayout.Button("添加", GUILayout.Width(60)))
                {
                    currentPipe.AddBall(new BallData(_ballTypeList[i]));
                }

                if (GUILayout.Button("删除", GUILayout.Width(60)))
                {
                    currentPipe.DeleteBall();
                }

                GUILayout.EndHorizontal();
                GUILayout.Space(15);
            }

            GUILayout.EndArea();
        }

        private void ShowLevelList()
        {
            GUILayout.BeginArea(new Rect(0, 30, 134, 480), new GUIStyle(GUI.skin.box));
            _scrollView = GUILayout.BeginScrollView(_scrollView, GUILayout.Width(130));

            foreach (var levelData in GetConfigGroup())
            {
                if (GUILayout.Button("关卡" + levelData.levelId, GUILayout.Width(100), GUILayout.Height(30)))
                {
                    _currentLevel = levelData.levelId;
                    _levelData = levelData;
                    Refresh();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        #endregion


        private void SetGridLayOut(PipeCapacity pipeCapacity)
        {
            _panel.cellSize = UtilClass.GetPipeSize(pipeCapacity);
        }

        private void SpawnPipe()
        {
            for (int i = 0; i < _levelData.GetPipeCount(); i++)
            {
                var spawnPipe = Instantiate(_pipePrefab, _panel.gameObject.transform);
                spawnPipe.Init(_levelData.pipeDataList[i]);
                spawnPipe.name = $"Pipe_{i + 1}";
                _pipeList.Add(spawnPipe);
            }
        }

        private void RemovePipe()
        {
            foreach (var pipe in _pipeList.Where(pipe => pipe))
            {
                DestroyImmediate(pipe.gameObject);
            }

            _pipeList.Clear();
        }

        private bool CheckIsCanSave()
        {
            var allBallType = new Dictionary<BallType, int>();
            foreach (var pipeData in _levelData.pipeDataList)
            {
                for (int i = 0; i < pipeData.ballDataStack.Count; i++)
                {
                    var data = pipeData.ballDataStack.GetDataByIndex(i);
                    if (allBallType.ContainsKey(data.type))
                    {
                        allBallType[data.type]++;
                    }
                    else
                    {
                        allBallType.Add(data.type, 1);
                    }
                }
            }


            foreach (var dic in allBallType)
            {
                if (dic.Value != (int) _pipeCapacity)
                {
                    EditorUtility.DisplayDialog("关卡检测不通过",
                        $"球数量不匹配  管子容量为{(int) _pipeCapacity}   {dic.Key}共有{dic.Value}颗",
                        "确定");
                    return false;
                }
            }

            return true;
        }

        private void SaveLevel()
        {
            if (!CheckIsCanSave())
            {
                return;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            //不加这段话，项目关闭打开后不保存ScriptableObject数据
            if (_type == EditType.Default)
            {
                EditorUtility.SetDirty(_levelAsset);
            }
            else
            {
                EditorUtility.SetDirty(_specialLevelAsset);
            }
        }


        // ① 先相同颜色摆好n-2管（n=试管数）			
        // ② 从所有管内随机取出一颗球			
        // ③ 把取出的球放在任意一管最上面，需要与下面的球颜色不同			
        // 需要检查：是否有位置、是否和下面相同			
        // ④ 重复②③数次，次数策划输入			
        // ⑤ 点击完成，将下排靠右两管按照③的规则放到其他管内，保证最后两管空出	
        private void RandomStep()
        {
            //Step 2 从所有管内随机取出一颗球	
            var tempBallList = new Stack<BallData>();
            foreach (var t in _pipeList)
            {
                if (t.PeekBall())
                {
                    var obj = t._ballLevelEdits.Peek();
                    tempBallList.Push(obj.ballData);
                    t.DeleteBall();
                }
            }


            //Step 3 ③ 把取出的球放在任意一管最上面，需要与下面的球颜色不同	
            while (tempBallList.Count > 0)
            {
                var randomPipe = _pipeList[Random.Range(0, _pipeList.Count)];
                var canAdd = CanAddBallToAllPipe(tempBallList.Peek());

                if (randomPipe.CanPushBallLimit(tempBallList.Peek()) && canAdd)
                {
                    var addBall = tempBallList.Pop();
                    randomPipe.AddBall(addBall);
                }
                else if (canAdd == false)
                {
                    var addBall = tempBallList.Pop();
                    randomPipe.AddBall(addBall);
                }
            }
        }

        private void AutoMove()
        {
            var tempBallList = new Stack<BallData>();
            for (int i = 1; i <= 2; i++)
            {
                var pipe = _pipeList[_pipeList.Count - i];
                while (pipe._ballLevelEdits.Count > 0)
                {
                    if (pipe.PeekBall())
                    {
                        var obj = pipe._ballLevelEdits.Peek();
                        tempBallList.Push(obj.ballData);
                        pipe.DeleteBall();
                    }
                }
            }

            //Step 3 ③ 把取出的球放在任意一管最上面，需要与下面的球颜色不同	
            while (tempBallList.Count > 0)
            {
                var randomPipe = _pipeList[Random.Range(0, _pipeList.Count - 2)];
                if (randomPipe._pipeData.CanPushBall())
                {
                    var addBall = tempBallList.Pop();
                    randomPipe.AddBall(addBall);
                }
            }
        }

        private bool CanAddBallToAllPipe(BallData data)
        {
            var res = false;
            foreach (var pipeLevelEdit in _pipeList)
            {
                if (pipeLevelEdit.CanPushBallLimit(data))
                {
                    res = true;
                    break;
                }
            }

            return res;
        }
    }
}