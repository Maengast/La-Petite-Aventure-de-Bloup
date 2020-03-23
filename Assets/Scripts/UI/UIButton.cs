using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Sprites;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UIButton : MonoBehaviour
{

	public ButtonSpriteState SpriteState;

	protected UIManager uiManager;
	public UnityEvent ButtonEvent;
	
	private void Start()
	{
		uiManager = UIManager.Instance;
	}

	// Start is called before the first frame update
    private void OnMouseOver()
    {

    }

    private void OnMouseExit()
    {
	    
    }

    private void OnMouseUpAsButton()
    {
	    ButtonEvent.Invoke();
    }

}

[Serializable]
public class ButtonSpriteState
{
	public Sprite DefaultSprite;
	public Sprite HighlightSprite;
	public Sprite PressedSprite;
}
