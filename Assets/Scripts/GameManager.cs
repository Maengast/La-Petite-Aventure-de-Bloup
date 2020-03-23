using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	private UIManager _uiManager;

	private int currentLevelNumber = 1;
	private int maxLevelNumber;
	
	private void Start()
	{
		_uiManager = UIManager.Instance;
		CurrentGameState = GameState.MainMenu;
		_uiManager.InitUI(CurrentGameState.ToString());
	}

	public enum GameState
	{
		MainMenu,
		LevelSelection,
		LoadLevel,
		InGame,
		GameResults
	}
	  
	public enum LevelState
	{
		Close,
		New,
		Played
	}

	private GameState CurrentGameState;

	public void SwitchGameState(GameState newGameState)
	{
		switch (newGameState)
		{
			case GameState.MainMenu :
				//open scene main menu
				break;
			case GameState.LevelSelection:
				//open scene main menu if old game state not main menu
				break;
			case GameState.LoadLevel:
				//open scene in game
				//tell ui display level number
				break;
		}
		
		CurrentGameState = newGameState;
		_uiManager.SwitchPanel(CurrentGameState.ToString());
	}

	public GameState GetCurrentGameState()
	{
		return CurrentGameState;
	}

	public int GetCurrentLevel()
	{
		return currentLevelNumber;
	}

	public void InstantiatePlayer()
	{
		
	}

	public LevelState GetLevelState(int levelNumber)
	{
		if (levelNumber < currentLevelNumber)
		{
			return LevelState.Played;
		}

		if (levelNumber == currentLevelNumber)
		{
			return LevelState.New;
		}

		return LevelState.Close;
	}

	public void LoadLevel(int levelNumber)
	{
		currentLevelNumber = levelNumber;
		SwitchGameState(GameState.LoadLevel);
	}

	public void EndLoadLevel()
	{
		SwitchGameState(GameState.InGame);
	}
	
	public void EndGame(bool gameOver)
	{
		SwitchGameState(GameState.GameResults);
		//ui display game result
	}
	
}
