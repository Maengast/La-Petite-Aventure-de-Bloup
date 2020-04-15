using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using DataBase;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.WSA;
using Random = UnityEngine.Random;


public class LevelGenerator : MonoBehaviour
{
	public delegate void OnLevelGenerated(TileObject _bossTile, TileObject _playerTile, bool levelComplete);
	private Level _level;
	private int[,] _levelCoordinates;
	private List<TileObject> _levelTiles = new List<TileObject>();
	private TileObject _startTile;
	private TileObject _endTile;
	
	[Header("Level Tile")]
	public TileSprites LevelTileSprites;
	public GameObject TilePrefab;
	public int MaxTilePerLoop = 3;
	
	[Header("Ground Info")]
	public TileTypeInfo FloorInfo;
	public TileTypeInfo PlatformInfo;
	Dictionary<TileType,TileTypeInfo> _tileTypeInfos = new Dictionary<TileType, TileTypeInfo>();
	
	[Header("Tile Offsets")]
	public Coordinate MaxJumpDistance = new Coordinate(8,6);
	public int MinHeightOffset;
	public int MinWidthOffset;
	
	/**
	 * All steps to generate level
	 */
	public void GenerateLevel(int levelNumber, OnLevelGenerated levelGenerated)
	{
		//Get level characteristics 
		_level = DataManager.Instance.GetLevelInfo(1);
		_levelCoordinates = new int[_level.Width,_level.Height];
		
		SetTileTypeInfo();//Setup tile infos
		//Create start and end tile
		_startTile = CreateBoundaryTile(GetTileSize(TileType.Floor),0);
		_endTile = CreateBoundaryTile(GetTileSize(TileType.Floor), _level.Width-1);
		//Save a trace of start and end tile
		_levelTiles.Add(_startTile);
		_levelTiles.Add(_endTile);
		UpdateLevelTilesCoordinates(true);
		
		// Create a feasible path between the start and end platform.
		bool complete = CompleteLevel();

		//Set the tile sprites for render tiles
		TileManager.LevelTileSprites = LevelTileSprites;
		//Instantiate and upgrades tiles
		UpgradeTiles(InstantiateTiles());
		
		//Tell the level is generated end give end and start tile
		levelGenerated(_endTile, _startTile,complete);
	}
	
	/**
	 * 
	 */
	private void SetTileTypeInfo()
	{
		if (FloorInfo.HeightOffset < MinHeightOffset) FloorInfo.HeightOffset = MinHeightOffset;
		if (PlatformInfo.HeightOffset < MinHeightOffset) PlatformInfo.HeightOffset = MinHeightOffset;
		_tileTypeInfos[TileType.Floor] = FloorInfo;
		_tileTypeInfos[TileType.Platform] = PlatformInfo;
	}
	
	/**
	 * Recursive method to take an incomplete level and make it feasible by adding more blocks.
	 */
	bool CompleteLevel()
	{
		bool reachEnd = false;
		// We will keep track of our start tile from which we do our test
		TileObject startTile = (_levelTiles.Count < 3) ? _startTile : _levelTiles[_levelTiles.Count - 1];
		Coordinate[,] originsToTry = CreateOriginsToTry(startTile); // get all level cell coordinates in area
		int numberTileTest = 0;
		int numberTileAdd = 0;
		
		// Keep trying different tiles until we find one that is valid.
		// When all possible origins are tried, return to the parent node.
		while (numberTileTest<originsToTry.Length)
		{
			//find an origin not already tried
			Coordinate originIndexes = FindOriginToTryIndexes(originsToTry);
			if(originIndexes.Equals(Coordinate.Null)) break; // If no origins are available break
			Coordinate origin = originsToTry[originIndexes.x, originIndexes.y];
			// Choose a tile to try with given origin.
			TileObject newTile = GetNewTile(origin);
			//origin is tried
			originsToTry[originIndexes.x, originIndexes.y] = Coordinate.Null;
			
			//Force next loop iteration for each no valid origins
			 if (newTile == null)
			 {
			 	continue;
			 }
			 _levelTiles.Add(newTile);
			 numberTileTest++;
			//Check to see if we can reach the new tile
			if (TileIsReachable(newTile))
			{
				UpdateLevelTilesCoordinates();
				numberTileAdd++;
				if(numberTileAdd > MaxTilePerLoop) break;
				//Keep going even if a tile reach the end to complete the level
				if(reachEnd) continue;
				//Check end tile
				if (TileIsReachable(_endTile,true)) return true;
				//Otherwise, recursively proceed down the tree. If the recursive call returns true, then the new tile is a good choice.
				reachEnd = CompleteLevel();
				//Refresh level tiles coordinates
			}
			else
			{
				_levelTiles.Remove(newTile);
			}
		}
		return reachEnd; 
	}
	
	/**
	 * Keep a trace of free coordinates in level.
	 * When tile take place on level, the coordinates are no longer available
	 */
	private void UpdateLevelTilesCoordinates(bool boundTile = false)
	{
		foreach (TileObject tile in _levelTiles)
		{
			if(_levelCoordinates[tile.Origin.x,tile.Origin.y] > (int)CellType.Empty) continue;
			TileTypeInfo tileInfo = _tileTypeInfos[tile.TileType];
			Area area = CreateAreaAroundTile(tile.Origin, new Coordinate(tileInfo.WidthOffset, tileInfo.HeightOffset), tile.Size);
			for (int x = area.Origin.x; x < area.Origin.x + area.Size.x; x++)
			{
				for (int y = area.Origin.y; y > area.Origin.y - area.Size.y; y--)
				{
					int cellType = (int)CellType.Corpse;
					if (y == tile.Origin.y)
					{
						cellType = (boundTile)? (int) CellType.Bound:(int)CellType.Up;
					}
					_levelCoordinates[x, y] = cellType;
				}
			}
		}
	}

	/**
	 * Create origins to try from a tile in a specified area
	 */
	private Coordinate[,] CreateOriginsToTry(TileObject startTile)
	{
		//Create area of test
		//areaHeight : height position of current platform with an offset
		//areaWidth : width calculate with max jump distance in width and an offset
		Area area = CreateAreaAroundTile(startTile.Origin, new Coordinate(MinWidthOffset + MaxJumpDistance.x, MinHeightOffset+MaxJumpDistance.y), startTile.Size);
		//Adjust area
		if (area.Origin.x > 0)
		{
			area.Size.x -= startTile.Origin.x - area.Origin.x;
			area.Origin.x = startTile.Origin.x;
		}

		if (area.Origin.y == _level.Height - 1)
		{
			area.Origin.y = (_level.Height -1) - MinHeightOffset;
			area.Size.y -= MinHeightOffset;
		}
		Coordinate[,] origins = new Coordinate[area.Size.x,area.Size.y];
		int arrayX = 0;
		int arrayY = 0;
		//From position and size of startTile with area size , setup all origins to try
		for (int x = area.Origin.x; x < area.Origin.x + area.Size.x; x++)
		{
			for (int y = area.Origin.y; y > area.Origin.y-area.Size.y; y--)
			{
				Coordinate coordinates = new Coordinate(x,y);
				//test if coordinates are already use by another tile
				origins[arrayX, arrayY] = _levelCoordinates[x,y] > (int)CellType.Empty ? Coordinate.Null : coordinates;
				arrayY++;
			}
			arrayX++;
			arrayY = 0;
		}
		return origins;
	}

	/**
	 * Get x,y index for a non-tried and non occupied origin
	 */
	private Coordinate FindOriginToTryIndexes(Coordinate[,] originsToTry)
	{
		List<Coordinate> validOriginsIndexes = new List<Coordinate>();
		for (var x = 0; x < originsToTry.GetLength(0); x++)
		{
			for (var y = 0; y < originsToTry.GetLength(1); y++)
			{
				//Valid the coordinates if are not already use by another tile or already tested
				if (!originsToTry[x, y].Equals(Coordinate.Null) && _levelCoordinates[x,y] == (int)CellType.Empty)
				{
					validOriginsIndexes.Add(new Coordinate(x,y));
				}
			}
		}
		return (validOriginsIndexes.Count > 0 )? validOriginsIndexes[Random.Range(0, validOriginsIndexes.Count)] : Coordinate.Null;
	}
	
	/**
	 * Determine an generated new Tile with given origin
	 * Return : Valid Tile to test
	 * 		: null if no tiles can be generated
	 */
	[CanBeNull]
	private TileObject GetNewTile(Coordinate origin)
	{
		//Define ground type tile
		//if origin under maxFloor height it's a floor
		TileType tileType = (origin.y <= FloorInfo.MaxHeight) ? TileType.Floor : TileType.Platform;
		
		//get size of tile
		Coordinate size = GetTileSize(tileType);
		//Floor tile expend until they reach the bottom of the map
		if (tileType == TileType.Floor)
		{
			size.y = origin.y + 1;
		}

		TileObject tile = VerifyTileValidity(size,origin,_tileTypeInfos[tileType]);
		
		if (tile != null) tile.TileType = tileType;

		return tile;
	}
	
	/**
	 * Verify tile validity
	 * Create an area with height and width offset need around the tile
	 * Check in this area with level coordinates if another there are another tiles
	 * Adjust the tile size to fit if it's possible
	 * Return : null if cannot be created
	 * 		: Tile created 
	 */
	[CanBeNull]
	private TileObject VerifyTileValidity(Coordinate size, Coordinate origin, TileTypeInfo tileInfo)
	{
		//Check level bounds
		if (size.y > 1 && origin.y == 0)
		{
			size.y = 1;
		}
		if (origin.x + size.x > (_level.Width) - tileInfo.WidthOffset || origin.y > (_level.Height-1) - tileInfo.HeightOffset || origin.x <= 1 + tileInfo.WidthOffset)
		{
			return null;
		}
		
		//Check if an other tile have the same cell of the new tile
		for (int x = origin.x; x < origin.x + size.x; x++)
		{
			for (int y = origin.y; y > origin.y-size.y; y--)
			{
				if (_levelCoordinates[x, y] > (int)CellType.Empty)
				{
					return null;
				}
			}
		}
		return CreateTile(size, origin);
	}
	
	/**
	 * Create an area around a tile which contains all cell used or not used in level
	 */
	private Area CreateAreaAroundTile(Coordinate origin, Coordinate offset, Coordinate size)
	{
		Area area = new Area();
		int areaOriginX = (origin.x - offset.x < 0)? 0 : origin.x - offset.x;
		int areaOriginY = (origin.y + offset.y> _level.Height-1 ) ? _level.Height-1 : origin.y + offset.y;
		int areaSizeX = (areaOriginX+ size.x + 2*offset.x > _level.Width)?_level.Width - areaOriginX : (areaOriginX == 0)?size.x + offset.x : size.x + 2*offset.x;
		int areaSizeY = (areaOriginY+1- (size.y + 2*offset.y) < 1)?areaOriginY+1: (areaOriginY == _level.Height-1)?size.y + offset.y : size.y + 2*offset.y;
		area.Size = new Coordinate(areaSizeX,areaSizeY);
		area.Origin = new Coordinate(areaOriginX,areaOriginY);
		return area;
	}

	/**
	 * Create Tile Object with given size and origin
	 */
	private TileObject CreateTile(Coordinate size, Coordinate origin)
	{
		TileObject tile = new TileObject();
		tile.Origin = origin;
		tile.Size =  size;
		return tile;
	}
	
	/**
	 * Calculate new origin for boundary tiles like start and end tile.
	 * Level created from left to right
	 * Each x origin > 0, origin recalculate with a subtract of tile size
	 */
	private TileObject CreateBoundaryTile(Coordinate size, int originX)
	{
		int x = (originX <= 0) ? 0 : (originX - size.x)+1;
		return CreateTile(size, new Coordinate(x, size.y -1));
	}

	/**
	 * Return a random tile size depending on TileType and its size limit
	 */
	private Coordinate GetTileSize(TileType tileType)
	{
		int xSize, ySize;
		TileTypeInfo tileInfos = _tileTypeInfos[tileType];
		//add 1 because max value is exclusif
		xSize = Random.Range(tileInfos.MinWidth, tileInfos.MaxWidth + 1);
		ySize = Random.Range(tileInfos.MinHeight, tileInfos.MaxHeight + 1);
		return new Coordinate(xSize,ySize);
	}
	
	/**
	 * Check if the a given tile can be reached by another already placed
	 * Check above and sides tile for an upper cell of an another tile with offsets
	 */
	private bool TileIsReachable(TileObject target, bool endTile = false)
	{
		int originX = target.Origin.x;
		int originY = target.Origin.y;
		int sizeX = target.Size.x;
		int sizeY = target.Size.y;
		TileTypeInfo tileInfo = _tileTypeInfos[target.TileType];
		Area area = CreateAreaAroundTile(target.Origin, MaxJumpDistance, target.Size);
		for (int x = area.Origin.x; x < area.Origin.x + area.Size.x; x++)
		{
			for (int y = area.Origin.y; y > area.Origin.y-area.Size.y; y--)
			{
				int cellType = _levelCoordinates[x, y];
				if (cellType > (int) CellType.Corpse)
				{
					if (y > originY && y < area.Origin.y)
					{
						if (cellType == (int) CellType.Bound && x < originX - tileInfo.WidthOffset) return true;
						if (x > originX + tileInfo.WidthOffset || x < (originX + sizeX - 1) - tileInfo.WidthOffset) return true;
						if (x < originX || x > (originX + sizeX - 1)) return true;
					}

					if (y < originY)
					{
						if (cellType == (int) CellType.Bound && x < originX - tileInfo.WidthOffset) return true;
						if (x < originX - tileInfo.WidthOffset || x > (originX + sizeX - 1) + tileInfo.WidthOffset) return true;
					}
				}
			}
		}
		return false;
	}
	
	/**
	 * Instantiate all created tiles
	 */
	private List<TileManager> InstantiateTiles()
	{
		List<TileManager> tileManagers = new List<TileManager>();
		foreach (TileObject tile in _levelTiles)
		{
			Debug.Log(_levelTiles.Count);
			GameObject tileObj = Instantiate(TilePrefab,transform);
			TileManager tileManager = tileObj.GetComponent<TileManager>();
			tileManager.InitTile(new Vector2(tile.Size.x,tile.Size.y),new Vector2(tile.Origin.x, tile.Origin.y));
			tileManager.TileType = tile.TileType;
			tileManagers.Add(tileManager);
		}

		return tileManagers;
	}
	
	/**
	 * Upgrade tiles to add Traps and Bonus
	 */
	private void UpgradeTiles(List<TileManager> tiles)
	{
		List<TileManager> tilesNotUpgraded = new List<TileManager>();
		//Place traps
		for (int i = 0; i < _level.TrapsCount; i++)
		{
			tilesNotUpgraded = tiles.FindAll(t => t.TileType < TileType.Trap);
			TileManager tile = tilesNotUpgraded[Random.Range(0,tilesNotUpgraded.Count)];
			//Saw on platform and spike on floor
			if (tile.TileType == TileType.Platform)
			{
				tile.Upgrade(TrapType.Saw + "Trap");
			}
			else
			{
				tile.Upgrade(TrapType.Spike + "Trap");
			}
			
			//This tile is now upgraded
			tile.TileType = TileType.Trap;
		}
	}
}

public enum CellType
{
	Empty = 0,
	Corpse = 1,
	Up = 2,
	Bound = 3
}

public enum TrapType
{
	Saw,
	Spike
}
public enum TileType
{
	Floor,
	Platform,
	Bonus,
	Trap
}

[Serializable]
public struct TileTypeInfo
{
	public int MaxWidth;
	public int MaxHeight;
	public int MinWidth;
	public int MinHeight;
	public int HeightOffset;
	public int WidthOffset;
}

[Serializable]
public struct TileSprites
{
	public Sprite LeftUp;
	public Sprite RightUp;
	public Sprite MiddleUp;
	public Sprite Left;
	public Sprite Right;
	public Sprite Middle;
	public Sprite LeftDown;
	public Sprite RightDown;
	public Sprite MiddleDown;
}

public struct Coordinate
{
	public int x;
	public int y;
	public Coordinate(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	
	public static Coordinate Null { get; } = new Coordinate(-1,-1);
	
	public bool Equals(Coordinate coordinate)
	{
		return x.Equals(coordinate.x) && y.Equals(coordinate.y);
	}
	
	public bool NotEquals(Coordinate coordinate)
	{
		return !x.Equals(coordinate.x) && !y.Equals(coordinate.y);
	}
}

public struct Area
{
	public Coordinate Origin;
	public Coordinate Size;
}

public class TileObject
{
	public Coordinate Origin;
	public Coordinate Size;
	public TileType TileType;
}
