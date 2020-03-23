using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	private UIManager _uiManager;

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
		LoadGame,
		InGame,
		GameResults
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
				//open scene main menu and switch panel level selection
				break;
			case GameState.LoadGame:
				//open scene in game
				break;
			case GameState.InGame:
				break;
			case GameState.GameResults:
				//show results
				break;
		}
		
		CurrentGameState = newGameState;
		_uiManager.SwitchPanel(CurrentGameState.ToString());
	}

	public GameState GetCurrentGameState()
	{
		return CurrentGameState;
	}

	public void InstantiatePlayer()
	{
		
	}

	public void LoadLevel(int levelNumber)
	{
		//save level number
		SwitchGameState(GameState.LoadGame);
		//Tell ui manager to switch to level panel and display level number
		
	}

	public void EndLoadLevel()
	{
		SwitchGameState(GameState.InGame);
	}
	
	
	public void StartGame()
	{
		
	}

	public void EndGame(bool gameOver)
	{
		SwitchGameState(GameState.GameResults);
	}
	
}
