using _02.Scripts.InGame.UI;
using Fangtang;
using UnityEngine;

public class InGameTimeController : ElementBehavior<InGame>
{
    private float waitTime;
    public int secound = 0;

    protected override void OnInit()
    {
        
    }

    void Update()
    {
        Timing();
    }
    
    private void Timing()
    {
        if (secound <= 0) return;
        waitTime += Time.deltaTime;
        if (waitTime >= 1)
        {
            secound--;
            waitTime = 0;
            Context.GetView<InGamePlayingUI>().SetTime(secound);
            if (secound == 0)
            {
                Context.Failed();
            }
        }
    }

    public void AddSecound()
    {
        secound += GlobalDef.INGAME_ADD_TIME;
        Context.GetView<InGamePlayingUI>().SetTime(secound);
    }
}
