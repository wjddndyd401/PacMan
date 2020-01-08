using UnityEngine;
using System.Collections;

public enum Tile
{
	Empty, Obstacle, Cookie, PCookie, Prison, Entrance, End
}

public class JsonMap
{
	public int x;
	public int y;
	public int[] map;
	public Tile[,] FormatToTile()
	{
		Tile[,] returnValue = new Tile[y, x];
		for(int i = 0; i < y; i++)
		{
			for (int j = 0; j < x; j++)
			{
				returnValue[i, j] = (Tile)map[i * x + j];
			}
		}
		return returnValue;
	}
}

public class TileMap
{
	Tile[,] tiles;
	string json = "{\"x\":\"28\",\"y\":\"31\",\"map\":[" +
		"1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1," +
		"1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1," +
		"1,2,1,1,1,1,1,1,1,1,1,1,2,1,1,2,1,1,1,1,1,1,1,1,1,1,2,1," +
		"1,2,1,1,1,1,1,1,1,1,1,1,2,1,1,2,1,1,1,1,1,1,1,1,1,1,2,1," +
		"1,2,2,2,2,2,2,1,1,2,2,2,2,1,1,2,2,2,2,1,1,2,2,2,2,2,2,1," +
		"1,1,1,2,1,1,2,1,1,2,1,1,1,1,1,1,1,1,2,1,1,2,1,1,2,1,1,1," +
		"1,1,1,2,1,1,2,1,1,2,1,1,1,1,1,1,1,1,2,1,1,2,1,1,2,1,1,1," +
		"1,3,2,2,1,1,2,2,2,2,2,2,2,0,0,2,2,2,2,2,2,2,1,1,2,2,3,1," +
		"1,2,1,1,1,1,2,1,1,1,1,1,2,1,1,2,1,1,1,1,1,2,1,1,1,1,2,1," +
		"1,2,1,1,1,1,2,1,1,1,1,1,2,1,1,2,1,1,1,1,1,2,1,1,1,1,2,1," +
		"1,2,2,2,2,2,2,2,2,2,2,2,2,1,1,2,2,2,2,2,2,2,2,2,2,2,2,1," +
		"1,1,1,1,1,1,2,1,1,0,1,1,1,1,1,1,1,1,0,1,1,2,1,1,1,1,1,1," +
		"1,1,1,1,1,1,2,1,1,0,1,1,1,1,1,1,1,1,0,1,1,2,1,1,1,1,1,1," +
		"1,1,1,1,1,1,2,1,1,0,0,0,0,0,0,0,0,0,0,1,1,2,1,1,1,1,1,1," +
		"1,1,1,1,1,1,2,1,1,0,4,4,4,4,4,4,4,4,0,1,1,2,1,1,1,1,1,1," +
		"1,1,1,1,1,1,2,1,1,0,4,0,0,0,0,0,0,4,0,1,1,2,1,1,1,1,1,1," +
		"0,0,0,0,0,0,2,0,0,0,4,0,0,0,0,0,0,4,0,0,0,2,0,0,0,0,0,0," +
		"1,1,1,1,1,1,2,1,1,0,4,0,0,0,0,0,0,4,0,1,1,2,1,1,1,1,1,1," +
		"1,1,1,1,1,1,2,1,1,0,4,4,4,5,5,4,4,4,0,1,1,2,1,1,1,1,1,1," +
		"1,1,1,1,1,1,2,1,1,0,0,0,0,0,0,0,0,0,0,1,1,2,1,1,1,1,1,1," +
		"1,1,1,1,1,1,2,1,1,1,1,1,0,1,1,0,1,1,1,1,1,2,1,1,1,1,1,1," +
		"1,1,1,1,1,1,2,1,1,1,1,1,0,1,1,0,1,1,1,1,1,2,1,1,1,1,1,1," +
		"1,2,2,2,2,2,2,1,1,2,2,2,2,1,1,2,2,2,2,1,1,2,2,2,2,2,2,1," +
		"1,2,1,1,1,1,2,1,1,2,1,1,1,1,1,1,1,1,2,1,1,2,1,1,1,1,2,1," +
		"1,2,1,1,1,1,2,1,1,2,1,1,1,1,1,1,1,1,2,1,1,2,1,1,1,1,2,1," +
		"1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1," +
		"1,2,1,1,1,1,2,1,1,1,1,1,2,1,1,2,1,1,1,1,1,2,1,1,1,1,2,1," +
		"1,3,1,1,1,1,2,1,1,1,1,1,2,1,1,2,1,1,1,1,1,2,1,1,1,1,3,1," +
		"1,2,1,1,1,1,2,1,1,1,1,1,2,1,1,2,1,1,1,1,1,2,1,1,1,1,2,1," +
		"1,2,2,2,2,2,2,2,2,2,2,2,2,1,1,2,2,2,2,2,2,2,2,2,2,2,2,1," +
		"1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1]}";
	int maxX = 0;
	int maxY = 0;


	public TileMap(string mapFileName)
	{
		JsonMap jsonMap = JsonUtility.FromJson<JsonMap>(json);
		tiles = jsonMap.FormatToTile();
		maxX = tiles.GetLength(1);
		maxY = tiles.GetLength(0);
	}

	public Tile[,] GetTiles()
	{
		return tiles;
	}

	public bool IsObstacle(int x, int y)
	{
		return tiles[y, x] == Tile.Obstacle;
	}

	public bool IsPrision(int x, int y)
	{
		return tiles[y, x] == Tile.Prison;
	}

	public bool IsPrisonEntrance(int x, int y)
	{
		return tiles[y, x] == Tile.Entrance;
	}

	public Vector2 CenterPosition()
	{
		return new Vector2(tiles.GetLength(1) * 0.5f, tiles.GetLength(0) * 0.5f);
	}

	public bool[] IsAdjacentTile(Tile standard, int x, int y)
	{
		bool[] isBlocked = new bool[(int)Direction.End];
		for (int i = 0; i < (int)Direction.End; i++)
		{
			int newX = (int)Global.direction[i].x + x;
			int newY = (int)Global.direction[i].y + y;

			bool isOutOfIndex = newX < 0 || newX >= maxX || newY < 0 || newY >= maxY;
			if (isOutOfIndex || tiles[newY, newX] == standard)
				isBlocked[i] = true;
			else
				isBlocked[i] = false;
		}
		return isBlocked;
	}

	public AdjacentObstacle IndexOfObstacleSprite(int x, int y)
	{
		bool[] isBlocked = IsAdjacentTile(Tile.Obstacle, x, y);

		if (tiles[y, x] == Tile.Obstacle)
		{
			if (isBlocked[0] && isBlocked[2] && !isBlocked[1])
			{
				return AdjacentObstacle.UpDownLeft;
			}
			else if (isBlocked[0] && isBlocked[2] && !isBlocked[3])
			{
				return AdjacentObstacle.UpRightDown;
			}
			else if (!isBlocked[0] && isBlocked[1] && isBlocked[3])
			{
				return AdjacentObstacle.RightDownLeft;
			}
			else if (isBlocked[1] && !isBlocked[2] && isBlocked[3])
			{
				return AdjacentObstacle.UpRightLeft;
			}
			else if (isBlocked[0] && isBlocked[1] && !isBlocked[2] && !isBlocked[3])
			{
				return AdjacentObstacle.UpRight;
			}
			else if (isBlocked[0] && !isBlocked[1] && !isBlocked[2] && isBlocked[3])
			{
				return AdjacentObstacle.UpLeft;
			}
			else if (!isBlocked[0] && isBlocked[1] && isBlocked[2] && !isBlocked[3])
			{
				return AdjacentObstacle.RightDown;
			}
			else if (!isBlocked[0] && !isBlocked[1] && isBlocked[2] && isBlocked[3])
			{
				return AdjacentObstacle.DownLeft;
			}
			else if (isBlocked[0] && isBlocked[1] && isBlocked[2] && isBlocked[3])
			{
				if (x + 1 < maxX && y + 1 < maxY && !(tiles[y + 1, x + 1] == Tile.Obstacle))
					return AdjacentObstacle.ExceptRightup;
				else if (x + 1 < maxX && y > 0 && !(tiles[y - 1, x + 1] == Tile.Obstacle))
					return AdjacentObstacle.ExceptRightdown;
				else if (x > 0 && y + 1 < maxY && !(tiles[y + 1, x - 1] == Tile.Obstacle))
					return AdjacentObstacle.ExceptLeftup;
				else if (x > 0 && y > 0 && !(tiles[y - 1, x - 1] == Tile.Obstacle))
					return AdjacentObstacle.ExceptLeftdown;
			}
		}
		return AdjacentObstacle.None;
	}

	public AdjacentObstacle IndexOfPrisionSprite(int x, int y)
	{
		bool[] isBlocked = IsAdjacentTile(Tile.Prison, x, y);

		if(y > CenterPosition().y && !isBlocked[2])
		{
			return AdjacentObstacle.RightDownLeft;
		} else if(y > CenterPosition().y && isBlocked[1])
		{
			return AdjacentObstacle.RightDown;
		} else if(y > CenterPosition().y && isBlocked[3])
		{
			return AdjacentObstacle.DownLeft;
		}
		else if(y < CenterPosition().y && !isBlocked[0])
		{
			return AdjacentObstacle.UpRightLeft;
		}
		else if (y < CenterPosition().y && isBlocked[1])
		{
			return AdjacentObstacle.UpRight;
		}
		else if (y < CenterPosition().y && isBlocked[3])
		{
			return AdjacentObstacle.UpLeft;
		} else if(x < CenterPosition().x)
		{
			return AdjacentObstacle.UpRightDown;
		} else if(x > CenterPosition().x)
		{
			return AdjacentObstacle.UpDownLeft;
		}
		return AdjacentObstacle.None;
	}
}
