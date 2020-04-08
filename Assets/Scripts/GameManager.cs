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
	private Dictionary<int, LevelState> _levelStates = new Dictionary<int, LevelState>();
	private int _maxLevelNumber=3;
	
	
	/**
	 * Enum for game states
	 */
	public enum GameState
	{
		MainMenu,
		LevelSelection,
		LoadLevel,
		InGame,
		GameResult
	}
	 
	/**
	 * Enum for level states
	 */
	public enum LevelState
	{
		Close,
		New,
		Played
	}
	
	/**
	 * Enum for game results
	 */
	public enum GameResults
	{
		Win,
		Lose
	}

	private GameState _currentGameState;
	
	private void Start()
	{
		_uiManager = UIManager.Instance;
		_currentGameState = GameState.MainMenu;
		_uiManager.InitUI();
		InitLevelState();
	}

	private void InitLevelState()
	{
		for (int i = 1; i <= _maxLevelNumber; i++)
		{
			if (i < _nextLevelNumber)
			{
				_levelStates[i] = LevelState.Played;
			}
			else if (i > _nextLevelNumber)
			{
				_levelStates[i] = LevelState.Close;
			}
			else
			{
				_levelStates[i] = LevelState.New;
			}
		}
	}
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
		_currentGameState = newGameState;
		_uiManager.SwitchPanel(_currentGameState.ToString());
	}

	public GameState GetCurrentGameState()
	{
		return _currentGameState;
	}

	public int GetCurrentLevel()
	{
		return _currentLevelNumber;
	}

	public LevelState GetLevelState(int levelNumber)
	{
		return _levelStates[levelNumber];
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
		GameResults gameResult = (gameOver) ? GameResults.Lose : GameResults.Win;
		SwitchGameState(GameState.GameResult);
		_uiManager.DisplayGameResults(gameResult.ToString());
	}
	
}
