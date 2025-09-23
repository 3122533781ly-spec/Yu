using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class LogEmailSender
{
    public static void CancelSend()
    {
        if (_smtpClient != null && !_mailSent)
        {
            _smtpClient.SendAsyncCancel();
        }
    }

    public static void SendEmail()
    {
        CancelSend();

        _smtpClient = new SmtpClient(LogSetting.Instance.MailSMTPClient);
        _smtpClient.Credentials = new System.Net.NetworkCredential(
            LogSetting.Instance.MailSenderAccount, LogSetting.Instance.MailSenderPassword)
            as ICredentialsByHost;
        _smtpClient.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
            { return true; };

        System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
        mail.From = new MailAddress(LogSetting.Instance.MailSenderAccount);

        for (int i = 0; i < LogSetting.Instance.MailToAccountList.Count; i++)
        {
            mail.To.Add(LogSetting.Instance.MailToAccountList[i]);
        }

        mail.Subject = "LXL Log Mail";
        mail.Body = "Attachment: " + LogSetting.Instance.MailLogFileName + " Please login Mail to downland \n" +
            "[log@ftang.cn](https://exmail.qq.com/cgi-bin/loginpage) \n" +
            "Passward: Ftang1234";

        if (File.Exists(LogSetting.Instance.FullExportFileSavePath))
        {
            Debug.Log("add attachment " + LogSetting.Instance.FullExportFileSavePath);
            Attachment att = new Attachment(LogSetting.Instance.FullExportFileSavePath);
            att.Name = LogSetting.Instance.MailLogFileName;
            mail.Attachments.Add(att);
        }

        try
        {
            _mailSent = false;
            string userState = "Log message";
            _smtpClient.SendCompleted += new SendCompletedEventHandler(SendComplet);
            //_smtpClient.Send(mail);
            _smtpClient.SendAsync(mail, userState);
            Debug.Log(LogSetting.Instance.MailSenderAccount + " Send To " +
                    LogSetting.Instance.MailToAccountList[0] + "... ");
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    private static void SendComplet(object sender, AsyncCompletedEventArgs e)
    {
        String token = (string)e.UserState;

        if (e.Cancelled)
        {
            Debug.LogError(string.Format("[{0}] Send canceled.", token));
        }
        if (e.Error != null)
        {
            Debug.LogError(string.Format("[{0}] {1}", token, e.Error.ToString()));
        }
        else
        {
            Debug.Log("Log Send To " + LogSetting.Instance.MailSenderAccount +
                     LogSetting.Instance.MailToAccountList[0] + "  compoleted userState:" + token);
        }
        _mailSent = true;
    }

    private static SmtpClient _smtpClient;
    private static bool _mailSent = false;
}
