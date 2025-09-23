using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace Redeem {	
	[Serializable]
	
	public class GiftCardInfoData : IConfig
	{
	    public int id;
	    public string name;
	    public Sprite sprite;
	    public int ID => id;
	}
}
