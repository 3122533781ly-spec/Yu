using Prime31.StateKit;
using UnityEngine;

public class InGamePlayingState : SKState<InGame>
{
    public override void begin()
    {
        HandleComponent();
    }

    private void HandleComponent()
    {
        _context.GetController<InGameTimeController>().Activate();
    }

    public override void end()
    {
    }
}