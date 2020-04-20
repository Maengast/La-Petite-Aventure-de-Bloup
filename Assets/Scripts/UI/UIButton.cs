using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(BoxCollider2D))]

public class UIButton : MonoBehaviour
{

	public ButtonSpriteState SpriteState;
	public SpriteRenderer SpriteRenderer;

	protected UIManager uiManager;
	public UnityEvent ButtonEvent;
	
	private void Start()
	{
		uiManager = UIManager.Instance;
		if(!SpriteRenderer)SpriteRenderer = GetComponent<SpriteRenderer>();
	}

	// Start is called before the first frame update
    private void OnMouseOver()
    {
	    SpriteRenderer.sprite = SpriteState.HighlightSprite;
    }

    private void OnMouseExit()
    {
	    SpriteRenderer.sprite = SpriteState.DefaultSprite;
    }

    private void OnMouseDown()
    {
	    SpriteRenderer.sprite = SpriteState.DefaultSprite;
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
}
