using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
	private UIManager _uiManager;
	private DataManager _dataManager;

	private int _currentLevelNumber = 0;
	private int _levelUnlock = 1;
	private Dictionary<int, LevelState> _levelStates = new Dictionary<int, LevelState>();
	private int _maxLevelNumber=3;
	
	private GameState _currentGameState;
	
	public float LoadingWaitTime = 2.0f;
	
	private void Start()
	{
		_uiManager = UIManager.Instance;
		_dataManager = DataManager.Instance;

		_maxLevelNumber = _dataManager.GetLevelCount();
		_currentGameState = GameState.MainMenu;
		if(_uiManager)_uiManager.InitUI();
		InitLevelState();
	}
	
	/**
	 * Init level state in function of level numbers
	 */
	private void InitLevelState()
	{
		for (int i = 1; i <= _maxLevelNumber; i++)
		{
			if (i < _levelUnlock)
			{
				_levelStates[i] = LevelState.Played;
			}
			else if (i > _levelUnlock)
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
	 * Switch between game state and do actions
	 */
	public void SwitchGameState(GameState newGameState)
	{
		//Do actions
		switch (newGameState)
		{
			case GameState.MainMenu:
				//Load menu
				SceneManager.LoadScene("Menu", LoadSceneMode.Single);
				break;
			case GameState.LevelSelection:
				//Load menu if the scene not currently loaded
				if (_currentGameState != GameState.MainMenu)
				{
					SceneManager.LoadScene("Menu", LoadSceneMode.Single);
				}
				break;
			case GameState.LoadLevel:
				//Load level in background
				StopAllCoroutines();
				StartCoroutine(LoadLevelAsync("Game"));
				break;
		}
		_currentGameState = newGameState;
		//Display corresponding UI panel
		_uiManager.SwitchPanel(_currentGameState.ToString());
	}
	
	/**
	 * Return the current game state [MainMenu, LevelSelection,LoadLevel,InGame,GameResult]
	 */
	public GameState GetCurrentGameState()
	{
		return _currentGameState;
	}
	
	/**
	 * Return the current level number played
	 */
	public int GetCurrentLevel()
	{
		return _currentLevelNumber;
	}
	
	/**
	 * Return a level state [close, played, new] for a given level number
	 */
	public LevelState GetLevelState(int levelNumber)
	{
		return _levelStates[levelNumber];
	}
	
	/**
	 * Load specific level
	 */
	public void LoadLevel(int levelNumber)
	{
		_currentLevelNumber = levelNumber;
		SwitchGameState(GameState.LoadLevel);
	}
	
	/**
	 * Wait until the level is fully loaded
	 */
	IEnumerator LoadLevelAsync(string sceneName)
	{
		yield return new WaitForSecondsRealtime(LoadingWaitTime);
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

		// Wait until the asynchronous scene fully loads
		while (!asyncLoad.isDone)
		{
			yield return null;
		}
		
		SwitchGameState(GameState.InGame);
	}

	/**
	 * Called when game ended
	 * Update the level state
	 * Game over : replay or return to level selection
	 * Win : unlock a new level
	 * 		Return to main menu if this level is the last level
	 */
	public void EndGame(bool gameOver)
	{
		_levelStates[_currentLevelNumber] = LevelState.Played;
		GameResults gameResult = (gameOver) ? GameResults.Lose : GameResults.Win;
		if (gameResult == GameResults.Win)
		{
			_levelUnlock++;
			if (_levelUnlock == _maxLevelNumber)
			{
				SwitchGameState(GameState.MainMenu);
				return;
			}
			_levelStates[_levelUnlock] = LevelState.New;
		}
		SwitchGameState(GameState.GameResult);
		_uiManager.DisplayGameResults(gameResult.ToString());
	}
}

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
