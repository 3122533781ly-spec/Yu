using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestData
{
    public string Id; 
    public TestData(string id, string n)
    {
        Id = id; 
        //.....������ĸ�������
    }
}

public class TestView : ICustomGridItem<TestData>
{
    Text Id; 

    public override void Init(GameObject go)
    {
        base.Init(go);
         
        Id = go.transform.Find("Id").GetComponent<Text>();
    }

    public override void UpdateView()
    {
        base.UpdateView();
        if (info == null)
        { 
            //���û������ ������͸����Ϊ ���ɼ��� ǧ������ go.SetActive(false); ��Ȼ�������������ԣ��ͼ��㲻����
            SetAlpha(go.gameObject, (int)(0.01 * 255), true);
            return;
        } 
        Id.text = info.Id; 
        go.SetActive(true);
        SetAlpha(go.gameObject, (int)(1 * 255), true);
        
    }



    /// <summary>
    /// ����ͼƬ͸����  isAll:true �����������
    /// </summary>       
    public void SetAlpha(GameObject target, int alpha, bool isAll = false)
    {
        Graphic[] imgs = target.GetComponentsInChildren<Graphic>();
        float a = (1f / 255) * alpha;
        for (int i = 0; i < imgs.Length; i++)
        {
            if (imgs[i] != null && (imgs[i] is Text || imgs[i] is Image))
            {
                imgs[i].color = new Color(imgs[i].color.r, imgs[i].color.g, imgs[i].color.b, a);
            }
            if (!isAll) break;
        }
    }
}
