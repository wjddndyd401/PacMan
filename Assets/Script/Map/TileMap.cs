using UnityEngine;
using System.Collections;

public class JsonMap
{
	public int x;
	public int y;
	public int[] map;
	public Vector2Int playerPosition;

	public Tile[,] FormatToTile(out Vector2Int _playerPosition)
	{
		Tile[,] returnValue = new Tile[y, x];
		for(int i = 0; i < y; i++)
		{
			for (int j = 0; j < x; j++)
			{
				returnValue[i, j] = (Tile)map[i * x + j];
			}
		}
		_playerPosition = playerPosition;
		return returnValue;
	}

	public void FormatToJson(Tile[,] tiles)
	{
		x = tiles.GetLength(1);
		y = tiles.GetLength(0);
		map = new int[x * y];
		for (int i = 0; i < y; i++)
		{
			for (int j = 0; j < x; j++)
			{
				map[i * x + j] = (int)tiles[i, j];
				if (tiles[i, j] == Tile.PlayerPosition)
				{
					playerPosition = new Vector2Int(j, i);
				}
			}
		}
	}
}

public class TileMap
{
	Tile[,] tiles;
	int maxX = 0;
	int maxY = 0;
	Vector2Int playerPosition;
	Vector2Int prisonMin;
	Vector2Int prisonMax;

	readonly Vector2Int prisonSize = new Vector2Int(8, 5);

	public TileMap(string map)
	{
		JsonMap jsonMap = JsonUtility.FromJson<JsonMap>(map);
		tiles = jsonMap.FormatToTile(out playerPosition);
		maxX = tiles.GetLength(1);
		maxY = tiles.GetLength(0);

		for (int i = 0; i < maxY; i++)
		{
			for (int j = 0; j < maxX; j++)
			{
				if(tiles[i, j] == Tile.PrisonWall)
				{
					prisonMin.x = j;
					prisonMin.y = i;
					prisonMax = prisonMin + prisonSize - new Vector2Int(1, 1);

					i = maxY;
					break;
				}
			}
		}
	}

	public TileMap(Vector2Int size)
	{
		tiles = new Tile[size.y, size.x];
		for (int i = 0; i < size.y; i++)
		{
			for(int j = 0; j < size.x; j++)
			{
				tiles[i, j] = Tile.Obstacle;
			}
		}

		prisonMin = new Vector2Int(tiles.GetLength(1) / 2 - prisonSize.x / 2, tiles.GetLength(0) / 2 - prisonSize.y / 2);
		prisonMax = prisonMin + prisonSize - new Vector2Int(1, 1);
		SetPrisonTile();

		maxX = tiles.GetLength(1);
		maxY = tiles.GetLength(0);
	}

	public Tile[,] GetTiles()
	{
		return tiles;
	}

	public bool CheckTileType(Vector2Int coord, Tile type)
	{
		return tiles[coord.y, coord.x] == type;
	}

	public Vector2Int CenterPosition()
	{
		return new Vector2Int(tiles.GetLength(1) / 2, tiles.GetLength(0) / 2);
	}

	public Vector2Int PlayerPosition()
	{
		return playerPosition;
	}

	public Vector2Int GhostPosition(GhostPattern name)
	{
		Vector2Int prisonCenter = (prisonMin + prisonMax);
		prisonCenter.x /= 2;
		prisonCenter.y /= 2;

		if (name == GhostPattern.Blinky)
		{
			return (prisonMin + prisonMax) + new Vector2Int(1, 0);
		}
		else if (name == GhostPattern.Pinky)
		{
			return (prisonMin + prisonMax);
		}
		else if (name == GhostPattern.Inky)
		{
			return (prisonMin + prisonMax) + new Vector2Int(2, 0);
		}
		else if (name == GhostPattern.Clyde)
		{
			return (prisonMin + prisonMax) - new Vector2Int(1, 0);
		}
		return CenterPosition();
	}

	public Vector2Int MapSize()
	{
		return new Vector2Int(tiles.GetLength(1), tiles.GetLength(0));
	}

	public bool[] IsAdjacentTile(Tile standard, Vector2Int coord)
	{
		bool[] isBlocked = new bool[(int)Direction.End];
		for (int i = 0; i < (int)Direction.End; i++)
		{
			int newX = (int)Global.directions[i].x + coord.x;
			int newY = (int)Global.directions[i].y + coord.y;

			bool isOutOfIndex = newX < 0 || newX >= maxX || newY < 0 || newY >= maxY;
			if (isOutOfIndex || tiles[newY, newX] == standard)
				isBlocked[i] = true;
			else
				isBlocked[i] = false;
		}
		return isBlocked;
	}

	public AdjacentObstacle IndexOfObstacleSprite(Vector2Int coord)
	{
		bool[] isBlocked = IsAdjacentTile(Tile.Obstacle, coord);

		if (tiles[coord.y, coord.x] == Tile.Obstacle)
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
				for (int i = 0; i < Global.directions.Length; i++)
				{
					Vector2Int target = coord + Global.diagonals[i];
					if (IsCoordInRange(target) && tiles[target.y, target.x] != Tile.Obstacle)
						return Global.diagonalIndices[i];
				}
			}
		}
		return AdjacentObstacle.None;
	}

	public AdjacentObstacle IndexOfPrisionSprite(Vector2Int coord)
	{
		bool[] isBlocked = IsAdjacentTile(Tile.PrisonWall, coord);

		if(coord.y == prisonMax.y && !isBlocked[2])
		{
			return AdjacentObstacle.RightDownLeft;
		}
		else if (coord.y == prisonMax.y && isBlocked[1])
		{
			return AdjacentObstacle.RightDown;
		}
		else if (coord.y == prisonMax.y && isBlocked[3])
		{
			return AdjacentObstacle.DownLeft;
		}
		else if(coord.y == prisonMin.y && !isBlocked[0])
		{
			return AdjacentObstacle.UpRightLeft;
		}
		else if (coord.y == prisonMin.y && isBlocked[1])
		{
			return AdjacentObstacle.UpRight;
		}
		else if (coord.y == prisonMin.y && isBlocked[3])
		{
			return AdjacentObstacle.UpLeft;
		} else if(coord.x == prisonMin.x)
		{
			return AdjacentObstacle.UpRightDown;
		} else if(coord.x == prisonMax.x)
		{
			return AdjacentObstacle.UpDownLeft;
		}
		return AdjacentObstacle.None;
	}

	public void SetPrisonTile()
	{
		for (int i = prisonMin.y; i <= prisonMax.y; i++)
		{
			for (int j = prisonMin.x; j <= prisonMax.x; j++)
			{
				if (i == prisonMax.y && (j == (prisonMax.x + prisonMin.x) / 2 || j == (prisonMax.x + prisonMin.x) / 2 + 1))
				{
					tiles[i, j] = Tile.Entrance;
				}
				else if (i == prisonMin.y || i == prisonMax.y || j == prisonMin.x || j == prisonMax.x)
				{
					tiles[i, j] = Tile.PrisonWall;
				}
				else
				{
					tiles[i, j] = Tile.Prison;
				}
			}
		}
	}

	public bool IsCoordInRange(Vector2Int coord)
	{
		return coord.x >= 0 && coord.y >= 0 && coord.x < tiles.GetLength(1) && coord.y < tiles.GetLength(0);
	}

	public void SetTile(Vector2Int coord, Tile type)
	{
		tiles[coord.y, coord.x] = type;
	}

	public string GetJson()
	{
		JsonMap json = new JsonMap();
		json.FormatToJson(tiles);
		return JsonUtility.ToJson(json);
	}

	public void MovePrison(Direction direction)
	{
		Vector2Int newPrisonMin = prisonMin + Global.directions[(int)direction];
		Vector2Int newPrisonMax = prisonMax + Global.directions[(int)direction];

		if (IsCoordInRange(newPrisonMin - Vector2Int.one) && IsCoordInRange(newPrisonMax + Vector2Int.one))
		{
			for (int i = prisonMin.y; i <= prisonMax.y; i++)
			{
				for (int j = prisonMin.x; j <= prisonMax.x; j++)
				{
					tiles[i, j] = Tile.Obstacle;
				}
			}
			prisonMin = newPrisonMin;
			prisonMax = newPrisonMax;
			SetPrisonTile();
		}
	}


}
