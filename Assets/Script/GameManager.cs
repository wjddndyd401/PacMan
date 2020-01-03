using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	TileMap tileMap;
	[SerializeField] GameObject tileMapBackground;
	[SerializeField] Sprite[] sprites;
	[SerializeField] SpriteRenderer tilePrefab;

	public static GameManager Instance { get; private set; } = null;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);

		DontDestroyOnLoad(this);

		tileMap = new TileMap("");
	}

	private void Start()
	{
		if(tileMapBackground != null)
			PrintMap();
	}

	void PrintMap()
	{
		Tile[,] tiles = tileMap.GetTiles();
		for (int i = 0; i < tiles.GetLength(0); i++)
		{
			for (int j = 0; j < tiles.GetLength(1); j++)
			{
				Vector3 position = new Vector3(j - tiles.GetLength(1) / 2.0f + 0.5f, -i + tiles.GetLength(0) / 2.0f - 0.5f, 0);
				SpriteRenderer sprite = Instantiate(tilePrefab);

				sprite.transform.SetParent(tileMapBackground.transform);
				sprite.transform.localPosition = position;
				sprite.transform.localScale = new Vector3(1, 1, 1);
				
				sprite.sprite = sprites[IndexOfObstacleSprite(tiles, j, i)];
			}
		}
	}

	int IndexOfObstacleSprite(Tile[,] tiles, int x, int y)
	{
		bool[] isBlocked = new bool[(int)Direction.End];
		for(int i = 0; i < (int)Direction.End; i++)
		{
			int newX = (int)Global.direction[i].x + x;
			int newY = (int)Global.direction[i].y + y;

			bool isOutOfIndex = newX < 0 || newX >= tiles.GetLength(1) || newY < 0 || newY >= tiles.GetLength(0);
			if (isOutOfIndex || tiles[newY, newX] == Tile.Obstacle)
				isBlocked[i] = true;
			else
				isBlocked[i] = false;
		}

		if (tiles[y, x] == Tile.Obstacle)
		{
			if (isBlocked[0] && isBlocked[2] && !isBlocked[1])
			{
				return 73 - 44;
			}
			else if (isBlocked[0] && isBlocked[2] && !isBlocked[3])
			{
				return 72 - 44;
			}
			else if (!isBlocked[0] && isBlocked[1] && isBlocked[3])
			{
				return 62 - 44;
			} else if(isBlocked[1] && !isBlocked[2] && isBlocked[3])
			{
				return 68 - 44;
			}
			else if (isBlocked[0] && isBlocked[1] && !isBlocked[2] && !isBlocked[3])
			{
				return 82 - 44;
			}
			else if (isBlocked[0] && !isBlocked[1] && !isBlocked[2] && isBlocked[3])
			{
				return 83 - 44;
			}
			else if (!isBlocked[0] && isBlocked[1] && isBlocked[2] && !isBlocked[3])
			{
				return 84 - 44;
			}
			else if (!isBlocked[0] && !isBlocked[1] && isBlocked[2] && isBlocked[3])
			{
				return 85 - 44;
            }
            else if (isBlocked[0] && isBlocked[1] && isBlocked[2] && isBlocked[3])
            {
                if (x + 1 < tiles.GetLength(1) && y + 1 < tiles.GetLength(0) && !(tiles[y + 1, x + 1] == Tile.Obstacle))
                    return 87 - 44;
                else if (x + 1 < tiles.GetLength(1) && y > 0 && !(tiles[y - 1, x + 1] == Tile.Obstacle))
                    return 89 - 44;
                else if (x > 0 && y + 1 < tiles.GetLength(0) && !(tiles[y + 1, x - 1] == Tile.Obstacle))
                    return 86 - 44;
                else if (x > 0 && y > 0 && !(tiles[y - 1, x - 1] == Tile.Obstacle))
                    return 88 - 44;
            }
        }
		else if (tiles[y, x] == Tile.Cookie)
		{
			return 1;
		}
		else if(tiles[y, x] == Tile.PCookie)
		{
			return 2;
		}
		return 0;
	}
}
