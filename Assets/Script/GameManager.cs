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
	Dictionary<AdjacentObstacle, Sprite> obstacleSpriteDictionary = null;
	[SerializeField] ObstacleSprite[] prisonSprites = null;
	Dictionary<AdjacentObstacle, Sprite> prisonSpriteDictionary = null;
	[SerializeField] Sprite prisonEntranceSprite = null;
	[SerializeField] Sprite cookie = null;
	[SerializeField] Sprite pCookie = null;
	PathFinder pathFinder;
	[SerializeField] Sprite[] numberSprites = null;
	[SerializeField] GameObject scoreField = null;
	int score;
	List<SpriteRenderer> numberFields;
	[SerializeField] GameObject highScoreField = null;
	int highScore;
	List<SpriteRenderer> highNumberFields;
	[SerializeField] SpriteRenderer numberObject = null;
	[SerializeField] int cookieScore = 0;
	[SerializeField] int pCookieScore = 0;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);

		DontDestroyOnLoad(this);

		tileMap = new TileMap("");
		pathFinder = new PathFinder(tileMap.GetTiles());

		obstacleSpriteDictionary = new Dictionary<AdjacentObstacle, Sprite>();
		for (int i = 0; i < obstacleSprites.Length; i++)
		{
			obstacleSpriteDictionary.Add(obstacleSprites[i].adjacentObstacleDirection, obstacleSprites[i].sprite);
		}

		prisonSpriteDictionary = new Dictionary<AdjacentObstacle, Sprite>();
		for (int i = 0; i < prisonSprites.Length; i++)
		{
			prisonSpriteDictionary.Add(prisonSprites[i].adjacentObstacleDirection, prisonSprites[i].sprite);
		}

		score = 100;
		numberFields = new List<SpriteRenderer>();
		SetScoreField(scoreField, numberFields, score);

		highScore = PlayerPrefs.GetInt("HighScore", 0);
		highNumberFields = new List<SpriteRenderer>();
		SetScoreField(highScoreField, highNumberFields, highScore);
	}

	private void Start()
	{
		if(tileMapBackground != null)
			PrintMap();

		pacman.transform.position = new Vector3(6, 20, 0);
		for (int i =0; i < ghosts.Length; i++)
		{
			ghosts[i].transform.position = new Vector3(tileMap.CenterPosition().x, tileMap.CenterPosition().y, 0);
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

				if (tileMap.IsObstacle(j, i))
				{
					AdjacentObstacle spriteIndex = tileMap.IndexOfObstacleSprite(j, i);
					sprite.sprite = obstacleSpriteDictionary[spriteIndex];
				}
				else if (tileMap.IsPrision(j, i))
				{
					AdjacentObstacle spriteIndex = tileMap.IndexOfPrisionSprite(j, i);
					sprite.sprite = prisonSpriteDictionary[spriteIndex];
				}
				else if(tileMap.IsPrisonEntrance(j, i))
				{
					sprite.sprite = prisonEntranceSprite;
				}
				else
				{
					sprite.sprite = obstacleSpriteDictionary[AdjacentObstacle.None];

					if (tiles[i, j] == Tile.Cookie)
					{
						SpriteRenderer cookieTile = Instantiate(cookiePrefab, position, Quaternion.identity);
						cookieTile.sprite = cookie;
						cookieTile.GetComponent<Cookie>().Init(cookieScore, false);
					}
					else if (tiles[i, j] == Tile.PCookie)
					{
						SpriteRenderer cookieTile = Instantiate(cookiePrefab, position, Quaternion.identity);
						cookieTile.sprite = pCookie;
						cookieTile.GetComponent<Cookie>().Init(pCookieScore, true);
					}
				}
			}
		}
	}

	public bool IsObstacle(int x, int y)
	{
		return tileMap.IsObstacle(x, y) || tileMap.IsPrision(x, y);
	}

	public bool IsPrisonEntrance(int x, int y)
	{
		return tileMap.IsPrisonEntrance(x, y);
	}

	Vector2Int GetNextTileToVector(Vector2Int target, int x, int y)
	{
		List<Vector2Int> path = pathFinder.FindPath(x, y, target.x, target.y);
		if (path.Count > 1)
			return path[0];
		return new Vector2Int(x, y);
	}

	public Vector2Int GetNextTileToPlayer(int x, int y)
	{
		if (pacman != null)
		{
			return GetNextTileToVector(new Vector2Int((int)pacman.transform.position.x, (int)pacman.transform.position.y), x, y);
		}
		return new Vector2Int(x, y);
	}

	public Vector2Int GetNextTileToCenter(int x, int y)
	{
		return GetNextTileToVector(new Vector2Int((int)tileMap.CenterPosition().x, (int)tileMap.CenterPosition().y), x, y);
	}

	public void EatCookie(int cookieScore, bool isPCookie)
	{
		score += cookieScore;
		SetScoreField(scoreField, numberFields, score);
		if(score > highScore)
		{
			highScore = score;
			PlayerPrefs.SetInt("HighScore", highScore);
			SetScoreField(highScoreField, highNumberFields, highScore);
		}

		if(isPCookie)
		{
			for(int i =0; i < ghosts.Length; i++)
			{
				ghosts[i].ToVulnerable();
			}
		}
	}

	void SetScoreField(GameObject field, List<SpriteRenderer> numberFields, int score)
	{
		int scoreForPrint = score;
		int unit = 1;
		int digit = 0;
		while(scoreForPrint / unit > 0)
		{
			if(numberFields.Count <= digit)
			{
				Vector3 position = field.transform.position;
				position.x -= digit;
				SpriteRenderer newSprite = Instantiate(numberObject, position, Quaternion.identity);
				newSprite.transform.SetParent(field.transform);
				numberFields.Add(newSprite);
			}
			numberFields[digit].sprite = numberSprites[(scoreForPrint / unit) % 10];
			digit++;
			unit *= 10;
		}
	}
}
