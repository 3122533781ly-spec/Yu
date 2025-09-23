using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DollarClearData : IConfig
{
    public int id;

    //��ҵ�ǰ��������
    public Vector2 range;

    //�������
    public float number;

    //���ָ��ʣ�����ж�������Ԫ�������ʯ��
    public int probability;

    //��ҵ�ǰ��ʯ����([1,10]����1~10��֮��)
    public Vector2 gemRange;

    //��ʯ�������
    public int gemNumber;

    public int ID => id;
}