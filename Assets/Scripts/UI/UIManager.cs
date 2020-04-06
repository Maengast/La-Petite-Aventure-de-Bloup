using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
	private GameManager _gameManager;
	private GameObject _currentPanel;
	private List<GameObject> _panels = new List<GameObject>();
	
	public void Start()
	{
		_gameManager = GameManager.Instance;
		foreach (Transform child in transform)
		{
			_panels.Add(child.gameObject);
		}
	}

	public void InitUI(String currentGameState)
	{
		_panels.ForEach(p => p.SetActive(false));
		_currentPanel = _panels.Find(o => o.name == currentGameState);
		_currentPanel.SetActive(true);
	}
	
	public void SwitchPanel(String panelName)
	{
		GameObject newPanel = _panels.Find(o => o.name == panelName);
		newPanel.SetActive(true);
		_currentPanel.SetActive(false);
		_currentPanel = newPanel;
	}

	public void DisplayGameResults(String panelName)
	{
		foreach (Transform child in _currentPanel.transform)
		{
			if (child.name == panelName)
			{
				child.gameObject.SetActive(true);
			}
			else
			{
				child.gameObject.SetActive(false);
			}
		}
	}
	
	public void Play()
	{
		_gameManager.SwitchGameState(GameManager.GameState.LevelSelection);
	}

	public void ReplayLevel()
	{
		Debug.Log("Replay");
		_gameManager.SwitchGameState(GameManager.GameState.LoadLevel);
	}
	
	public void NextLevel()
	{
		_gameManager.LoadLevel(_gameManager.GetCurrentLevel()+1);
	}
	
}
