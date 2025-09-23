using System.Collections.Generic;
using System.Linq;

public class GuideUIConfig : ScriptableConfigGroup<GuideUIData, GuideUIConfig>
{

    public List<GuideData> GetGuideByType(int type)
    {
        var temp = All.Where(d => d.Type == type).ToList();
        var list = temp.Select(d => d.Parse()).ToList();
        return list;
    }
}
