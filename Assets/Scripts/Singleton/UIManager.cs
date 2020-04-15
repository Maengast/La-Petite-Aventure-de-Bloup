using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
	private GameManager _gameManager;
	private UIPanel _currentPanel;
	private List<UIPanel> _panels = new List<UIPanel>();
	
	public void Start()
	{
		_gameManager = GameManager.Instance;
		foreach (Transform child in transform)
		{
			if(Enum.IsDefined(typeof(GameState), child.name))
			{
				UIPanel uiPanel = child.GetComponent<UIPanel>();
				if (!uiPanel) child.gameObject.AddComponent<UIPanel>();
				_panels.Add(child.GetComponent<UIPanel>());
			}
		}
	}

	public void InitUI()
	{
		_panels.ForEach(p => p.gameObject.SetActive(false));
		SwitchPanel(_gameManager.GetCurrentGameState().ToString());
	}
	
	public void SwitchPanel(String panelName)
	{
		UIPanel newPanel = _panels.Find(p => p.gameObject.name == panelName);
		newPanel.Init();
		if(_currentPanel)_currentPanel.Disable();
		_currentPanel = newPanel;
	}

	public void DisplayGameResults(String panelName)
	{
		foreach (Transform child in _currentPanel.transform)
		{
			if(Enum.IsDefined(typeof(GameResults), child.name))
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
	}
	
	public void Play()
	{
		_gameManager.SwitchGameState(GameState.LevelSelection);
	}

	public void ReplayLevel()
	{
		_gameManager.SwitchGameState(GameState.LoadLevel);
	}
	
	public void NextLevel()
	{
		_gameManager.LoadLevel(_gameManager.GetCurrentLevel()+1);
	}

	public void Quit()
	{
		Application.Quit();
	}
}
