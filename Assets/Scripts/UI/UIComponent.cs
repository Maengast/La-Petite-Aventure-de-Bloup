using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIComponent : MonoBehaviour
{
	public abstract void Init();

	public abstract void Disable();
}
