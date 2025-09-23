using System;
using Fangtang;

public class InGameEventModel : ElementModel
{
    public Action<DotPoint> OnTouchPoint = delegate { };
}
