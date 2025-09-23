using Fangtang;
using System.Collections.Generic;
using System.Linq;
using _02.Scripts.InGame.UI;
using _02.Scripts.LevelEdit;
using UnityEngine;

public class InGameModel : ElementModel
{
    public LevelData LevelData;

    public List<InGamePipeUI> LevelPipeList;
    public int temp = 0;

    public InGameModel()
    {
        LevelPipeList = new List<InGamePipeUI>();
    }

    public bool IsUseAddPipeTool()
    {
        return LevelPipeList.Count > (int)LevelData.GetPipeCount();
    }

    public bool CanAddPipe()
    {
        if (LevelPipeList.Count == temp + 1 && LevelPipeList.Last().PipeLength() == LevelPipeList.First().PipeLength())
            return true;
        return false;
    }

    public void Dispose()
    {
    }
}