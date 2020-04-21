using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel : UIComponent
{
	public UIComponent[] ComponentsToInitialize;
	public override void Init()
	{
		foreach (UIComponent component in ComponentsToInitialize)
		{
			component.Init();
		}
	}

	public override void Disable()
	{
		foreach (UIComponent component in ComponentsToInitialize)
		{
			component.Disable();
		}
	}
}
