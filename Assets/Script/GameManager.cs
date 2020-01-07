using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; } = null;

	TileMap tileMap;
	[SerializeField] GameObject tileMapBackground = null;
	[SerializeField] SpriteRenderer tilePrefab = null;
	[SerializeField] Pacman pacman = null;
	[SerializeField] Ghost[] ghosts = null;
	[SerializeField] SpriteRenderer cookiePrefab = null;
	[SerializeField] ObstacleSprite[] obstacleSprites = null;
	Dictionary<AdjacentObstacle, Sprite> obstacleSpriteDicrionary = null;
	[SerializeField] Sprite cookie = null;
	[SerializeField] Sprite pCookie = null;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);

		DontDestroyOnLoad(this);

		tileMap = new TileMap("");

		obstacleSpriteDicrionary = new Dictionary<AdjacentObstacle, Sprite>();
		for (int i = 0; i < obstacleSprites.Length; i++)
		{
			obstacleSpriteDicrionary.Add(obstacleSprites[i].adjacentObstacleDirection, obstacleSprites[i].sprite);
		}
	}

	private void Start()
	{
		if(tileMapBackground != null)
			PrintMap();

		pacman.transform.position = new Vector3(20, 20, 0);
		for(int i =0; i < ghosts.Length; i++)
		{
			ghosts[i].transform.position = new Vector3(8, 8, 0);
		}

		Camera.main.transform.position = new Vector3(tileMap.CenterPosition().x, tileMap.CenterPosition().y, Camera.main.transform.position.z);
	}

	void PrintMap()
	{
		Tile[,] tiles = tileMap.GetTiles();
		for (int i = 0; i < tiles.GetLength(0); i++)
		{
			for (int j = 0; j < tiles.GetLength(1); j++)
			{
				Vector3 position = new Vector3(j, i, 0);
				SpriteRenderer sprite = Instantiate(tilePrefab);

				sprite.transform.SetParent(tileMapBackground.transform);
				sprite.transform.localPosition = position;
				sprite.transform.localScale = new Vector3(1, 1, 1);

				if (IsObstacle(j, i))
				{
					AdjacentObstacle spriteIndex = tileMap.IndexOfObstacleSprite(tiles, j, i);
					sprite.sprite = obstacleSpriteDicrionary[spriteIndex];
				}
				else
				{
					sprite.sprite = obstacleSpriteDicrionary[AdjacentObstacle.None];

					if (tiles[i, j] == Tile.Cookie)
					{
						SpriteRenderer cookieTile = Instantiate(cookiePrefab, position, Quaternion.identity);
						cookieTile.sprite = cookie;
					} else if(tiles[i, j] == Tile.PCookie)
					{
						SpriteRenderer cookieTile = Instantiate(cookiePrefab, position, Quaternion.identity);
						cookieTile.sprite = pCookie;
					}
				}
			}
		}

		PathFinder pathFinder = new PathFinder(tiles);
		pathFinder.FindPath(21, 20, 12, 10);
	}

	public bool IsObstacle(int x, int y)
	{
		return tileMap.IsObstacle(x, y);
	}
}
