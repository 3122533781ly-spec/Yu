using System.Collections.Generic;
using Fangtang;
using UnityEngine;

public class InGameLevelLoader : ElementBehavior<InGameLineBee>
{
    public void LoadLevel(LineBeeLevelData levelData)
    {
        // LDebug.Log($"Start load level ID:{levelData.ID}");

        List<DotPoint> allPoint = new List<DotPoint>();
        BoardLayoutData boardData = BoardLayoutData.GetBoardDataByID(levelData.ID);

        allPoint = LoadBoard(boardData);
        Context.MatchModel.InitStartData(allPoint);
        GameStage.Instance.FitTileParentToCenter();
        Context.GetController<InGameCameraSizeFit>().FitSize();
    }

    private List<DotPoint> LoadBoard(BoardLayoutData board)
    {
        List<DotPoint> result = new List<DotPoint>();

        for (int i = 0; i < board.PointDatas.Count; i++)
        {
            DotPointData pointData = board.PointDatas[i];
            if (!ExistPoint(pointData.Cordinates, result))
            {
                DotPoint point = CreateDotPoint(pointData);
                result.Add(point);
            }
        }

        return result;
    }

    private bool ExistPoint(Vector2Int cordinates, List<DotPoint> target)
    {
        for (int i = 0; i < target.Count; i++)
        {
            if (target[i].Data.Cordinates.x == cordinates.x && target[i].Data.Cordinates.y == cordinates.y)
            {
                return true;
            }
        }

        return false;
    }

    private DotPoint CreateDotPoint(DotPointData data)
    {
        DotPoint result = Instantiate(_dotPointPrefab);

        result.name = $"Dot_{data.Value}_{_idCounter}";
        _idCounter++;
        result.Transform.SetParent(GameStage.Instance.PointParent);
        result.Transform.localPosition = GameHelper.GetPointLocalPosition(data.Cordinates);
        result.Set(data);
        // result.Model.InMapLocalPosition = result.Transform.localPosition;
        return result;
    }

    private void Awake()
    {
        _idCounter = 0;
    }

    [SerializeField] private DotPoint _dotPointPrefab = null;
    private int _idCounter;
}