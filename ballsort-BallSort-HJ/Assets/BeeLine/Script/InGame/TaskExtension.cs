using System.Threading.Tasks;

public static class TaskExtension
{
    public static async Task DelaySecond(float second)
    {
        await Task.Delay((int)(second * 1000));
    }
}