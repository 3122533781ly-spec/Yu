using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlotRoleUI : MonoBehaviour
{
    [SerializeField] private Button clickBtn;

    [SerializeField] private Text content;

    [SerializeField] private RectTransform dialog;
    [SerializeField] private SmartLocalizedText nameTxt;
    [SerializeField] private Text speak;
    [SerializeField] private PlotRoleItem role1;
    [SerializeField] private PlotRoleItem role2;

    [SerializeField] private RectTransform transition;
    [SerializeField] private float Speed = 8;

    RolePlotData data;
    Image bg;
    Action _onEnd;
    //0.还没打字 1.正在打字 2.打字完成
    int typewrite, oldBg;
    Text typewriteTxt;
    IEnumerator enumerator;

    private void OnEnable()
    {
        clickBtn.onClick.AddListener(OnBtnClick);
        transition.SetActive(false);
    }

    private void OnDisable()
    {
        clickBtn.onClick.RemoveListener(OnBtnClick);
        oldBg = 0;
    }

    void OnBtnClick()
    {
        if (typewrite == 1) ShowAllTxt();
        else if (typewrite == 2) _onEnd?.Invoke();
    }

    public void SetData(Image bg, Action onEnd)
    {
        this.bg = bg;
        _onEnd = onEnd;
        oldBg = 0;
    }

    public void SetSpeak(RolePlotData data)
    {
        this.data = data;
        typewrite = 0;
        if (typewriteTxt) typewriteTxt.text = "";
        bg.rectTransform.anchoredPosition = Vector2.zero;
        bg.rectTransform.localScale = Vector3.one;
        StopTxt();
        StartTransition();
    }

    void FillRole()
    {
        if(data.Role1Act.Count == 0 && data.Role2Act.Count == 0)//无对话框
        {
            dialog.SetActive(false);
            if(data.Txt == 0)
            {
                content.transform.parent.SetActive(false);
                typewriteTxt = null;
            }
            else
            {
                content.transform.parent.SetActive(true);
                typewriteTxt = content;
            }
        }
        else
        {
            dialog.SetActive(true);
            content.transform.parent.SetActive(false);
            typewriteTxt = speak;
            nameTxt.text = LocalizationManager.Instance.GetTextByTag($"Txt{data.Speak}");

            if (data.Role1Act.Count != 0 && data.Role2Act.Count != 0)//两个人
            {
                role1.SetActive(true);
                role2.SetActive(true);
                role1.SetPosX(false);
                role2.SetPosX(false);
                role1.Play(data.Role1Act, GetPlayType(1, data.Special));
                role2.Play(data.Role2Act, GetPlayType(2, data.Special));
                role1.SetSpeak(data.Speak);
                role2.SetSpeak(data.Speak);
            }
            else
            {
                List<string> list;
                if(data.Role2Act.Count != 0) list = data.Role2Act;
                else list = data.Role1Act;

                if(role2.GetSpeak() == data.Speak)
                {
                    role1.SetActive(false);
                    role2.SetActive(true);
                    role2.SetPosX();
                    role2.Play(list, GetPlayType(2, data.Special));
                    role2.SetSpeak(data.Speak);
                }
                else
                {
                    role1.SetActive(true);
                    role2.SetActive(false);
                    role1.SetPosX();
                    role1.Play(list, GetPlayType(1, data.Special));
                    role1.SetSpeak(data.Speak);
                }
            }
        }
    }

    int GetPlayType(int role, int type)
    {
        if(role == 1)
        {
            if(type == 1 || type == 3) return 1;
        }
        else
        {
            if (type == 2 || type == 3) return 1;
        }
        return 0;
    }

    Sequence seq, seq1;
    void StartTransition()
    {
        if (oldBg == 0 || oldBg == data.Bg)
        {
            // oldBg = data.Bg;
            // var handle = YooAssets.LoadAssetSync<Sprite>($"PlotBg_{data.Bg}");
            // bg.sprite = handle.AssetObject as Sprite;
            // handle.Release();
            // bg.material.DOFloat(data.GaussianBlur, "_Size", 0.3f);
            // Special4Before();
            // Special();
            // FillRole();
            // StartTxt();
        }
        else
        {
            if(seq != null)
            {
                seq.Kill();
                seq = null;
            }
            transition.anchoredPosition = new Vector2(transition.rect.width + 300, 0);
            transition.SetActive(true);
            seq = DOTween.Sequence();
            seq.Append(transition.DOAnchorPosX(0, 0.5f));
            seq.AppendCallback(() =>
            {
                bg.sprite = Resources.Load<Sprite>($"PlotBg/{data.Bg}");
                bg.material.SetFloat("_Size", data.GaussianBlur);
                Special4Before();
                FillRole();
            });
            seq.Append(transition.DOAnchorPosX(-transition.rect.width - 300, 0.5f));
            seq.AppendCallback(() =>
            {
                oldBg = data.Bg;
                Special();
                StartTxt();
            });
        }
    }

    void Special4Before()
    {
        if (data.Special == 4)
        {
            bg.rectTransform.localScale = Vector3.one * 1.5f;
            var roat = Mathf.Min(1, Screen.height / 1920f);
            var posX = (bg.rectTransform.rect.width * 1.5f * roat - Screen.width) / 2;
            bg.rectTransform.anchoredPosition = new Vector2(-posX, 0);
        } 
    }
    void Special()
    {
        if(data.Special == 4)
        {
            if (seq1 != null)
            {
                seq1.Kill();
                seq1 = null;
            }
            var roat = Mathf.Min(1, Screen.height / 1920f);
            var posX = (bg.rectTransform.rect.width * 1.5f * roat - Screen.width) / 2;
            seq1 = DOTween.Sequence();
            seq1.Append(bg.rectTransform.DOAnchorPosX(posX, 1.5f));
            seq1.AppendInterval(0.5f);
            seq1.Append(bg.rectTransform.DOAnchorPosX(0, 0.8f));
            seq1.Append(bg.rectTransform.DOScale(Vector3.one, 0.8f));
            seq1.AppendCallback(() =>
            {
                bg.rectTransform.localScale = Vector3.one;
                SetEndTxt(true);
            });
        }
    }

    void SetEndTxt(bool isCheckNext = false)
    {
        typewrite = 2;
        if (isCheckNext && data.AutoNext == 1)
        {
            OnBtnClick();
        }
    }

    void StartTxt()
    {
        if(typewriteTxt == null) return;
        typewrite = 1;
        RunTxt(LocalizationManager.Instance.GetTextByTag($"Txt{data.Txt}"), typewriteTxt, () =>
        {
            SetEndTxt(true);
        });
    }

    void ShowAllTxt()
    {
        SetEndTxt();
        StopTxt();
        typewriteTxt.text = LocalizationManager.Instance.GetTextByTag($"Txt{data.Txt}");
    }

    void RunTxt(string textToType, Text textLabel, Action endFun)
    {
        enumerator = TypeText(textToType, textLabel, endFun);
        StartCoroutine(enumerator);
    }
    void StopTxt()
    {
        if (enumerator != null)
        {
            StopCoroutine(enumerator);
            enumerator = null;
        }
    }
    IEnumerator TypeText(string textToType, Text textLabel, Action endFun)
    {
        float t = 1;
        var temp = textToType.Split(' ');
        string[] charList;
        var str = new StringBuilder();
        if (temp.Length == 1)
        {
            charList = new string[textToType.Length];
            for (int i = 0; i < textToType.Length; i++)
            {
                charList[i] = textToType[i].ToString();
            }
        }
        else charList = temp;
        int charIndex = 0, oldIndex = 0;
        while (charIndex < charList.Length)
        {
            t += Time.deltaTime * Speed;
            oldIndex = Mathf.FloorToInt(t);
            if(oldIndex > charIndex)
            {
                charIndex = Mathf.Clamp(oldIndex, 0, charList.Length);
                str.Clear();
                for (int i = 0; i < charIndex; i++)
                {
                    str.Append(charList[i]);
                    if (temp.Length > 1 && i < (charIndex - 1)) str.Append(" ");
                }
                textLabel.text = str.ToString();
            }
            yield return null;
        }
        textLabel.text = textToType;
        endFun?.Invoke();
    }
}
