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
	public delegate void OnLevelGenerated(TileObject _endTile, TileObject _startTile, bool levelComplete);
	private Level _level;
	private int[,] _levelCoordinates;
	private List<TileObject> _levelTiles = new List<TileObject>();
	private TileObject _startTile;
	private TileObject _endTile;
	
	//Sorting order of sprite type
	public static readonly int GroundSortingOrder = 0;
	public static readonly int TrapSortingOrder = -1;
	public static readonly int OnTileSortingOrder = 1;
	public static readonly int CharacterSortingOrder = 2;
	
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
	public void GenerateLevel(Level levelInfo, OnLevelGenerated levelGenerated)
	{
		//Get level characteristics 
		_level = levelInfo;
		_levelCoordinates = new int[_level.Width,_level.Height];
		
		SetSizeAndOffset();//Setup tile infos
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
	 * Set size of tile to adapt level size and offset to adapt level generation to character jump and size
	 */
	private void SetSizeAndOffset()
	{
		MaxJumpDistance = _level.MaxJumpDistance;
		MinHeightOffset = LevelManager.BossSize;
		MinWidthOffset = LevelManager.PlayerSize;
		
		//Set tile floor size
		FloorInfo.MaxHeight = Mathf.CeilToInt(_level.Height / 3);
		FloorInfo.MaxWidth = _level.Width / (_level.Width / 10);
		
		//Set tile offset
		if (FloorInfo.HeightOffset < MinHeightOffset) FloorInfo.HeightOffset = MinHeightOffset;
		if (PlatformInfo.HeightOffset < MinHeightOffset) PlatformInfo.HeightOffset = MinHeightOffset;
		if (PlatformInfo.WidthOffset < MinWidthOffset) PlatformInfo.WidthOffset = MinWidthOffset;
		
		//Save tile information
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
		while (numberTileTest< MaxTilePerLoop)
		{
			
			//find an origin not already tried
			Coordinate originIndexes = FindOriginToTryIndexes(originsToTry);
			if(originIndexes.Equals(Coordinate.Null)) break; // If no origins are available break
			
			// Choose a tile to try with given origin.
			TileObject newTile = GetNewTile(originsToTry[originIndexes.x, originIndexes.y]);
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
		Coordinate tileOrigin;
		Coordinate tileSize;
		Area area;
		
		foreach (TileObject tile in _levelTiles)
		{
			tileOrigin = tile.Origin;
			tileSize = tile.Size;
			
			if(_levelCoordinates[tileOrigin.x,tileOrigin.y] > (int)CellType.Empty) continue;
			//When a tile is placed place its offsets around it
			area = tile.AreaOffsets;
			for (int x = area.Origin.x; x < area.Origin.x + area.Size.x; x++)
			{
				for (int y = area.Origin.y; y < area.Origin.y + area.Size.y; y++)
				{
					if(_levelCoordinates[x,y] > (int)CellType.Empty) continue;
					int cellType = (int)CellType.TileOffset;
					if (x >= tileOrigin.x && x <= (tileOrigin.x + tileSize.x) - 1)
					{
						if (y == (tileOrigin.y + tileSize.y) -1)
						{
							cellType = (boundTile)? (int) CellType.Bound:(int)CellType.Up;
						}

						if (y > tileOrigin.y && y < (tileOrigin.y + tileSize.y)-1)
						{
							cellType = (int)CellType.Corpse;
						}
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
		Area area = CreateAreaAroundTile(startTile.Origin, new Coordinate(2*MinWidthOffset + MaxJumpDistance.x, 2*MinHeightOffset+MaxJumpDistance.y), startTile.Size);
		Coordinate origin = area.Origin;
		Coordinate size = area.Size;

		Coordinate[,] origins = new Coordinate[area.Size.x,area.Size.y];
		int arrayX = 0;
		int arrayY = 0;

		//From position and size of startTile with area size , setup all origins to try
		for (int x = origin.x; x < origin.x + size.x; x++)
		{
			for (int y = origin.y; y < origin.y + size.y; y++)
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
		
		//A floor tile begin from the bottom of the map
		if (tileType == TileType.Floor)
		{
			size.y = origin.y + 1;
			origin.y = 0;
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
		//Create area around tile with it's offset
		Area area = CreateAreaAroundTile(origin, new Coordinate(tileInfo.WidthOffset, tileInfo.HeightOffset), size);

		int areaOriginX = area.Origin.x;
		int areaOriginY = area.Origin.y;
		int areaSizeX = area.Size.x;
		int areaSizeY = area.Size.y;
		
		//Get always an offset between tile bounds and Map bounds
		if (areaOriginX + areaSizeX > (_level.Width) || areaOriginY + areaSizeY >= (_level.Height) || areaOriginX <= 1)
		{
			return null;
		}
		
		//Check if an other tile and its offset have the same cell of the new tile
		for (int x = areaOriginX; x < areaOriginX + areaSizeX; x++)
		{
			for (int y = areaOriginY; y < areaOriginY + areaSizeY; y++)
			{
				if (_levelCoordinates[x, y] > (int)CellType.Empty)
				{
					return null;
				}
			}
		}
		return CreateTile(size, origin, area);
	}
	
	/**
	 * Create an area around a tile which contains all cell used or not used in level
	 */
	private Area CreateAreaAroundTile(Coordinate origin, Coordinate offset, Coordinate size)
	{
		Area area = new Area();
		int areaOriginX = (origin.x - offset.x < 0)? 0 : origin.x - offset.x;
		int areaOriginY = (origin.y - offset.y< 0 ) ? 0 : origin.y - offset.y;
		int areaSizeX = (areaOriginX+ (size.x + 2*offset.x) > _level.Width)?_level.Width - areaOriginX : (areaOriginX == 0)? size.x + offset.x : size.x + 2*offset.x;
		int areaSizeY = (areaOriginY+ (size.y + 2*offset.y) > _level.Height)?_level.Height - areaOriginY: (areaOriginY == 0)? size.y + offset.y : size.y + 2*offset.y;
		area.Size = new Coordinate(areaSizeX,areaSizeY);
		area.Origin = new Coordinate(areaOriginX,areaOriginY);
		return area;
	}

	/**
	 * Create Tile Object with given size and origin
	 * and offsets area to test and place tile correctly
	 */
	private TileObject CreateTile(Coordinate size, Coordinate origin, Area offsets)
	{
		TileObject tile = new TileObject();
		tile.Origin = origin;
		tile.Size =  size;
		tile.AreaOffsets = offsets;
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
		Coordinate origin = new Coordinate(x, 0);
		Area area = CreateAreaAroundTile(origin, new Coordinate(FloorInfo.WidthOffset, FloorInfo.HeightOffset), size);
		return CreateTile(size, origin,area);
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
		int widthOffset = _tileTypeInfos[target.TileType].WidthOffset;
		
		Area area = CreateAreaAroundTile(target.Origin, MaxJumpDistance, target.Size);
		
		for (int x = area.Origin.x; x < (area.Origin.x + area.Size.x); x++)
		{
			for (int y = area.Origin.y; y < (area.Origin.y+ area.Size.y); y++)
			{
				int cellType = _levelCoordinates[x, y];
				if (cellType > (int) CellType.Corpse)
				{

					if (y > (originY + sizeY)-1)
					{
						if (cellType == (int) CellType.Bound && x < (originX - widthOffset)) return true;
						if (x > (originX + widthOffset) || x < (originX + sizeX - 1) - widthOffset) return true;
						if (x < originX || x > (originX + sizeX - 1)) return true;
					}

					if (y < originY)
					{
						if (cellType == (int) CellType.Bound && x < (originX - widthOffset)) return true;
						if (x < (originX - widthOffset) || x > (originX + sizeX - 1) + widthOffset) return true;
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
			tilesNotUpgraded = tiles.FindAll(t => t.TileType == TileType.Platform);
			if(tilesNotUpgraded.Count<1) return;
			int r = Random.Range(0, tilesNotUpgraded.Count);
			TileManager tile = tilesNotUpgraded[r];
			//Get the specific trap to fit to tile type
			tile.Upgrade((TrapType)tile.TileType + "Trap");
			tile.TileType = TileType.Trap;
		}
	}
}

/**
 * Cell Type
 */
public enum CellType
{
	Empty = 0,
	TileOffset =1,
	Corpse = 2,
	Up = 3,
	Bound = 4
}

/**
 * Trap type define which trap are put on tile
 */
public enum TrapType
{
	Saw = TileType.Platform,
	Spike = TileType.Floor
}

/**
 * Tile Type define characteristic of tile
 */
public enum TileType
{
	Floor,
	Platform,
	Bonus,
	Trap
}

/**
 * Information in function of tile type
 * Width, Height and offsets around tile needed
 */
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

/**
 * Sprites to render cells of a tile
 */
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

/**
 * Save coordinates 
 */
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
}

/**
 * Define an area of coordinates
 */
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
	public Area AreaOffsets;
}
