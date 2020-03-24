using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
	private GameManager gameManager;
	private GameObject currentPanel;
	private List<GameObject> panels = new List<GameObject>();
	
	public void Start()
	{
		gameManager = GameManager.Instance;
		foreach (Transform child in transform)
		{
			panels.Add(child.gameObject);
		}
	}

	public void InitUI(String currentGameState)
	{
		panels.ForEach(p => p.SetActive(false));
		currentPanel = panels.Find(o => o.name == currentGameState);
		currentPanel.SetActive(true);
	}
	
	public void SwitchPanel(String panelName)
	{
		GameObject newPanel = panels.Find(o => o.name == panelName);
		newPanel.SetActive(true);
		currentPanel.SetActive(false);
		currentPanel = newPanel;
	}
	
	public void Play()
	{
		gameManager.SwitchGameState(GameManager.GameState.LevelSelection);
	}

	public void ReplayLevel()
	{
		gameManager.SwitchGameState(GameManager.GameState.LoadLevel);
	}
	
	public void NextLevel()
	{
		gameManager.LoadLevel(gameManager.GetCurrentLevel()+1);
	}
	
}
