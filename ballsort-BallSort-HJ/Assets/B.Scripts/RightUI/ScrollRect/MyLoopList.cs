using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ICustomGridItem<T> where T : class
{

    protected T info;
    protected GameObject go;


    public virtual void Init(GameObject go)
    {
        this.go = go;
    }

    public virtual void UpdateView()
    {

    }
    public void SetInfo(T info)
    {
        this.info = info;
        UpdateView();
    }

}


public class MyLoopList<DataType, ItemType> where ItemType : ICustomGridItem<DataType>, new() where DataType : class
{
    public LevelLoopList levelLoopList;
    RectTransform scrollRect;

    RectTransform contentTR;
    public GameObject ItemGo;

    public int GridHeight;          //
    public int GridWidth;
    public int wholeItemCount;     //��������

    public int lineCount;           //ÿһ�еĸ�����
    public float top;//�߾�
    public float left;

    public Vector2 space;
    public int firstLineIndex;//��ǰ����еĵ�һ��Item��index
    int line;                       //����������
    float contentHeight;            //content����grid������ʼ����ĸ߶�

    float lastContentRTy;

    int itemCountInView;            //���������ɵ����Item����
    float scrollViewHeight;         //���ĳ�ʼ�߶�



    LinkedList<RectTransform> itemList = new LinkedList<RectTransform>();
    Dictionary<GameObject, ICustomGridItem<DataType>> gridList = new Dictionary<GameObject, ICustomGridItem<DataType>>();

    DataType[] datas;

    public void Init(DataType[] datas, float left, float top, Vector2 space, int gridHeight,
    int gridWidth, int lineCount, int wholeItemCount, GameObject itemGo, RectTransform scrollRect, LevelLoopList levelLoopList)
    {
        this.scrollRect = scrollRect;
        this.levelLoopList = levelLoopList;
        this.datas = datas;
        GridHeight = gridHeight;
        GridWidth = gridWidth;
        this.lineCount = lineCount;
        this.wholeItemCount = wholeItemCount;
        this.space = space;
        this.left = left;
        this.top = top;
        ItemGo = itemGo;

        contentTR = scrollRect.Find("Viewport/Content").GetComponent<RectTransform>();
        if (contentTR == null)
        {
            Debug.LogError("Ӧ���н�'Content'��������");
            return;
        }

        var rect = scrollRect.GetComponent<RectTransform>().rect;
        scrollViewHeight = rect.height;
        Vector2 size = Vector2.zero;
        size.x = rect.width;
        contentHeight = (space.y + GridHeight) * wholeItemCount / lineCount + top; //����content�ĸ߶�
        size.y = contentHeight;

        contentTR.sizeDelta = size; //content���ܴ�С������
        //��һ֡�� content y��λ��
        lastContentRTy = GetContentPositionY();


        //��Ԥ��Ŀ������
        (ItemGo.transform as RectTransform).sizeDelta = new Vector2(GridWidth, GridHeight);

        //�������������ʾ������  +2 ��ֹ����
        line = (int)(scrollViewHeight / (GridHeight + space.x)) + 2;

        itemCountInView = line * lineCount; // view��������ʾ��item����
        for (int i = 1; i < itemCountInView + 1; i++)
        {
            GameObject go = GameObject.Instantiate(ItemGo, contentTR);
            levelLoopList.levelSelectUIs.Add(go.GetComponent<LevelSelectUI>());
            var item = new ItemType();
            item.Init(go);
            gridList.Add(go, item);

            Vector3 pos = Vector3.zero;

            int count = (i % lineCount);
            if (count == 0) count = lineCount;

            pos.x = (i - 1) % lineCount * GridWidth + count * space.x;
            pos.x += left;
            int hang = (int)(Mathf.Ceil(i * 1.0f / lineCount));

            pos.y = -((hang - 1) * GridHeight + hang * space.y);
            pos.y -= top;
            pos.z = 0;
            var rectt = (go.transform as RectTransform);
            rectt.anchoredPosition = pos;
            itemList.AddLast(rectt);

        }
        //��һ��go������  ���һ��go����������Ӧ������������
        firstLineIndex = 0;

        scrollRect.GetComponent<ScrollRect>().onValueChanged.AddListener(OnValueChanged);

        ItemGo.SetActive(false);

        Refresh();
    }

    public void OnValueChanged(Vector2 pos)
    {
        float nowContentRTy = GetContentPositionY();
        //������
        bool isUp = false;
        if (pos.x >= 1 || pos.y >= 1) return;
        if (nowContentRTy - lastContentRTy > 0)
        {
            if (scrollViewHeight + GetItemPositionY(false) + nowContentRTy > GridHeight
            && contentHeight - nowContentRTy > scrollViewHeight)
            {
                isUp = true;
                MoveItemGo(isUp);
                //firstLineIndex += lineCount;
            }

        }
        else
        {
            if (-GetItemPositionY(true) - nowContentRTy > space.y
            && nowContentRTy > 0)
            {
                isUp = false;
                MoveItemGo(isUp);
                //firstLineIndex -= lineCount;
            }
        }

        lastContentRTy = nowContentRTy;
    }
    //true ��ʾ�Ǵ����һ��
    float GetItemPositionY(bool first)
    {
        float y;
        if (first)
            y = itemList.First.Value.anchoredPosition.y;
        else
            y = itemList.Last.Value.anchoredPosition.y;

        return y;
    }
    float GetContentPositionY()
    {
        return contentTR.anchoredPosition.y;
    }

    void SetInfo(ICustomGridItem<DataType> item, int index)
    {
        if (index >= 0 && index < datas.Length)
        {
            item.SetInfo(datas[index]);
        }
        else
        {
            item.SetInfo(null);
        }
    }

    public void SetButtonState()
    {

        int passLevel = Game.Instance.LevelModel.MaxUnlockLevel.Value;
        for (int i = 0; i < levelLoopList.levelSelectUIs.Count; i++)
        {
            LevelSelectUI temp = levelLoopList.levelSelectUIs[i];
            //ͨ�صĴ򿪹�ѡ��ͨ�غ��һ��ͼƬĬ��δͨ�أ���֮�������
            if ((int.Parse)(temp.nubtext.text) < passLevel)
                temp.PassLevel();
            else if ((int.Parse)(temp.nubtext.text) > passLevel)
                temp.NotCleared();
            else
                temp.Init(passLevel);
        }
    }

    void MoveItemGo(bool up)
    {
        if (up)
        {//tou
            for (int i = 0; i < lineCount; i++)
            {
                var item = itemList.First.Value;

                Vector3 pos = item.anchoredPosition;

                pos.y -= line * (GridHeight + space.y);

                item.anchoredPosition = pos;

                itemList.RemoveFirst();

                itemList.AddLast(item);

                //ע������
                int index = firstLineIndex + (line - 1) * lineCount + lineCount + i;


                SetInfo(gridList[item.gameObject], index);


            }
            firstLineIndex += lineCount;
        }
        else
        {
            for (int i = 0; i < lineCount; i++)
            {
                var item = itemList.Last.Value;

                Vector3 pos = item.anchoredPosition;

                pos.y += line * (GridHeight + space.y);

                item.anchoredPosition = pos;

                itemList.RemoveLast();
                itemList.AddFirst(item);
                int index = firstLineIndex - i - 1;

                SetInfo(gridList[item.gameObject], index);

            }
            firstLineIndex -= lineCount;
        }

        SetButtonState();
    }

    public void SetPos()
    { 
        int passLevel = Game.Instance.LevelModel.MaxUnlockLevel.Value;

        if (firstLineIndex < Mathf.Clamp(passLevel - 40, 0, Game.Instance.LevelModel.MaxUnlockLevel.Value))
        {
            while (firstLineIndex < Mathf.Clamp(passLevel - 40, 0, Game.Instance.LevelModel.MaxUnlockLevel.Value))
            {
                MoveItemGo(true);
            }
           
        }
        else
        {
            while (firstLineIndex > Mathf.Clamp(passLevel - 40, 0, Game.Instance.LevelModel.MaxUnlockLevel.Value))
            {
                MoveItemGo(false);
            }
        }


        scrollRect.GetComponent<ScrollRect>().verticalNormalizedPosition = levelLoopList.GetVerticalValue(passLevel + 1);

        SetButtonState();
    }

    public void Refresh()
    {
        var value = itemList.First;
        for (int i = firstLineIndex; i < firstLineIndex + itemCountInView; i++)
        {
            if (i >= datas.Length) return;
            if (value == null) return;
            gridList[value.Value.gameObject].SetInfo(datas[i]);
            gridList[value.Value.gameObject].UpdateView();
            value = value.Next;
        }

        SetButtonState();
    }
}