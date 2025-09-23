using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class LogSetting : ScriptableSingleton<LogSetting>
{
    public string MailLogFileName
    {
        get
        {
            return _mailLogFileName;
        }
        set
        {
            _mailLogFileName = value;
        }
    }

    public static string ConfigName
    {
        get
        {
            return typeof(LogSetting).ToString();
        }
    }

    public string FullExportFileSavePath
    {
        get
        {
            //return Path.Combine(Application.persistentDataPath, ExportFileSavePath);
            return string.Format("{0}/{1}", Application.persistentDataPath, ExportFileSavePath);
        }
    }

    public string ExportFileSaveDirectory
    {
        get
        {
            return Path.GetDirectoryName(FullExportFileSavePath);
        }
    }

    public int MaxDisplayHeight
    {
        get
        {
            return FrontSize * PageLength - 350;
        }
    }

    [SerializeField]
    public List<string> All;
    //[SerializeField]
    //public int LogInfoQueueLength = 8000;
    [SerializeField]
    public int PageLength = 100;
    [SerializeField]
    public float ClickInterval = 0.5f;
    [SerializeField]
    public int OpenEntryClickCount = 5;
    [SerializeField]
    public float OpenEntryHoldPointerTime = 3f;
    [SerializeField]
    public string ExportFileSavePath = "log.txt";
    [SerializeField]
    public int FrontSize = 15;

    [SerializeField]
    public List<string> MailToAccountList;
    [SerializeField]
    public string MailSenderAccount;
    [SerializeField]
    public string MailSenderPassword;
    [SerializeField]
    public string MailSMTPClient;

    private static LogSetting _instance;
    private string _mailLogFileName;
}
