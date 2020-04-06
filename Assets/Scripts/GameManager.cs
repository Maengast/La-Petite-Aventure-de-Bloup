using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
	private UIManager _uiManager;
	
	private int _currentLevelNumber = 0;
	private int _nextLevelNumber = 1;
	private int _maxLevelNumber;
	
	private void Start()
	{
		_uiManager = UIManager.Instance;
		_currentGameState = GameState.MainMenu;
		_uiManager.InitUI(_currentGameState.ToString());
	}
	
	public enum GameState
	{
		MainMenu,
		LevelSelection,
		LoadLevel,
		InGame,
		GameResult
	}
	  
	public enum LevelState
	{
		Close,
		New,
		Played
	}
	
	public enum GameResults
	{
		None,
		Win,
		Lose
	}

	private GameState _currentGameState;
	private GameResults _gameResult = GameResults.None;
	
	/**
	 * Switch between game state
	 */
	public void SwitchGameState(GameState newGameState)
	{
		switch (newGameState)
		{
			case GameState.MainMenu:
				SceneManager.LoadScene("Menu", LoadSceneMode.Single);
				break;
			case GameState.LevelSelection:
				if (_currentGameState != GameState.MainMenu)
				{
					SceneManager.LoadScene("Menu", LoadSceneMode.Single);
				}
				break;
			case GameState.LoadLevel:
				SceneManager.LoadScene("Game", LoadSceneMode.Single);
				break;
		}
		
		//Reset game result if game isn't in GameResults state anymore
		if (_currentGameState == GameState.GameResult)
		{
			_gameResult = GameResults.None;
		}
		
		_currentGameState = newGameState;
		_uiManager.SwitchPanel(_currentGameState.ToString());
	}

	public GameState GetCurrentGameState()
	{
		return _currentGameState;
	}
	
	public GameResults GetGameResult()
	{
		return _gameResult;
	}

	public int GetCurrentLevel()
	{
		return _currentLevelNumber;
	}

	public void InstantiatePlayer()
	{
		
	}

	public LevelState GetLevelState(int levelNumber)
	{
		if (levelNumber <= _currentLevelNumber)
		{
			return LevelState.Played;
		}

		if (levelNumber == _nextLevelNumber)
		{
			return LevelState.New;
		}

		return LevelState.Close;
	}

	public void LoadLevel(int levelNumber)
	{
		_currentLevelNumber = levelNumber;
		SwitchGameState(GameState.LoadLevel);
	}

	public void EndLoadLevel()
	{
		SwitchGameState(GameState.InGame);
	}
	
	public void EndGame(bool gameOver)
	{
		_gameResult = (gameOver) ? GameResults.Lose : GameResults.Win;
		SwitchGameState(GameState.GameResult);
		_uiManager.DisplayGameResults(_gameResult.ToString());
	}
	
}
