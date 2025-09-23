using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using UnityEngine;

public class BasicComponent : MonoBehaviour
{
    private void Start()
    {
        Game.Instance.InitCreateInstance();
        DontDestroyOnLoad(gameObject);

        Game.Instance.IsBasicComponentLoaded = true;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            PlayerPrefs.Save();
        }
    }
}