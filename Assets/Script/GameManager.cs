using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	TileMap tileMap;
	[SerializeField] GameObject tileMapBackground = null;
	[SerializeField] Sprite[] sprites = null;
	[SerializeField] SpriteRenderer tilePrefab = null;
	[SerializeField] Pacman pacman = null;
	[SerializeField] Ghost[] ghosts = null;
	[SerializeField] SpriteRenderer cookiePrefab = null;
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

				int spriteIndex = tileMap.IndexOfObstacleSprite(tiles, j, i);
				if(spriteIndex == 1 || spriteIndex == 2)
				{
					sprite.sprite = sprites[0];

					position.z = 0.1f;
					SpriteRenderer cookie = Instantiate(cookiePrefab, position, Quaternion.identity);
					cookie.sprite = sprites[spriteIndex];
				} else
				{
					sprite.sprite = sprites[spriteIndex];
				}
			}
		}
	}

	public bool IsObstacle(int x, int y)
	{
		return tileMap.IsObstacle(x, y);
	}
}
