using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UILevelNumber : UIComponent
{
	public TextMeshPro TextObject;

	public override void Init()
    {
	    if(!TextObject)TextObject = GetComponent<TextMeshPro>();
	    TextObject.text = "Level " + GameManager.Instance.GetCurrentLevel();
    }

	public override void Disable()
	{
		
	}
}
