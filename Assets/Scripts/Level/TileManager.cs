using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
	public static TileSprites LevelTileSprites;
	public GameObject CellPrefab;
	private Vector2 _tileSize;
	private Vector2 _tileOrigin;
	public TileType TileType;
	private BoxCollider2D collider;

	public void InitTile(Vector2 size, Vector2 origin)
	{
		_tileSize = size;
		_tileOrigin = origin;
		//Place tile on right position
		transform.position = _tileOrigin;
		
		//Set tile collider to fit origin and size
		collider = GetComponent<BoxCollider2D>();
		collider.size = _tileSize;
		collider.offset =  new Vector2(_tileSize.x/2,-_tileSize.y/2);
		RenderTile(); //render cell tile
	}

	private void RenderTile()
	{
		for (int x = 0; x < _tileSize.x; x++)
		{
			for (int y = 0; y < _tileSize.y; y++)
			{
				Sprite sprite;
				if (y < 1)
				{
					sprite = (x > 0) ? (x < _tileSize.x - 1) ? LevelTileSprites.MiddleUp : LevelTileSprites.RightUp : LevelTileSprites.LeftUp;
				}
				else if(y > _tileSize.y-1 )
				{
					sprite = (x > 0) ? (x < _tileSize.x - 1) ? LevelTileSprites.MiddleDown : LevelTileSprites.RightDown : LevelTileSprites.LeftDown;
				}
				else
				{
					sprite = (x > 0) ? (x < _tileSize.x - 1) ? LevelTileSprites.Middle : LevelTileSprites.Right : LevelTileSprites.Left;
				}
				GameObject obj = Instantiate(CellPrefab, transform);
				obj.transform.position = new Vector2(_tileOrigin.x + x , _tileOrigin.y-y);
				obj.GetComponent<SpriteRenderer>().sprite = sprite;
			}
		}
	}


	public void Upgrade(String upgradeName)
	{
	    
	}
	
	//    public void SetTileType(TileType tileType)
	//    {
	//     _tileType = tileType;
	//     if(tileType != TileType.Default)
	//     {
	// 	    UpgradeTile<_tileType.ToString()>();
	//     }
	//    }
	//    private void UpgradeTile<T>() where T: String,new()
	//    {
	//     gameObject.AddComponent<T>();
	//    }
	//    
	//    private void UpgradeTile<T>(string name)
	// {
	// 	Type type = Type.GetType(name);
	// }
}
