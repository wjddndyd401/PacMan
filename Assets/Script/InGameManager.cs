using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour
{
	public static InGameManager Instance { get; private set; } = null;

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
	[SerializeField] Sprite[] alphabetSprites = null;
	[SerializeField] Sprite[] numberSprites = null;
	[SerializeField] UiMenu scoreField = new UiMenu();
	[SerializeField] UiMenu highScoreField = new UiMenu();
	[SerializeField] UiMenu lifeField = new UiMenu();
	int score;
	int highScore;
	int currentLife;
	[SerializeField] Sprite lifeSprite = null;
	[SerializeField] Image menuObject = null;
	[SerializeField] int cookieScore = 0;
	[SerializeField] int pCookieScore = 0;
	Dictionary<GhostPattern, Vector2Int> ghostTargetInVulnerable;
	Vector2Int blinkyPosition;      // for inky
	[SerializeField] GameObject loadingScreen = null;
	[SerializeField] Text loadingText = null;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);

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

		ghostTargetInVulnerable = new Dictionary<GhostPattern, Vector2Int>();
		ghostTargetInVulnerable.Add(GhostPattern.Blinky, tileMap.MapSize());
		ghostTargetInVulnerable.Add(GhostPattern.Pinky, new Vector2Int(tileMap.MapSize().x, 0));
		ghostTargetInVulnerable.Add(GhostPattern.Inky, new Vector2Int(0, tileMap.MapSize().y));
		ghostTargetInVulnerable.Add(GhostPattern.Clyde, new Vector2Int(0, 0));

		pacman.gameObject.SetActive(false);
	}

	private void Start()
	{
		if (tileMapBackground != null)
			PrintMap();

		Camera.main.transform.position = new Vector3(tileMap.CenterPosition().x, tileMap.CenterPosition().y, Camera.main.transform.position.z);

		score = 0;
		SetUiMenuName(scoreField);
		SetScoreField(scoreField, score);

		highScore = PlayerPrefs.GetInt("HighScore", 0);
		SetUiMenuName(highScoreField);
		SetScoreField(highScoreField, highScore);

		currentLife = GameManager.Instance.MaxLife;

		Restart();
	}

	private void Update()
	{
		if (pacman.gameObject.activeSelf == false)
		{
			currentLife--;
			if (currentLife < 0)
			{
				SceneManager.LoadScene("Title");
			}
			else
			{
				Restart();
			}
		}
	}

	void Restart()
	{
		pacman.gameObject.SetActive(true);
		pacman.transform.position = new Vector3(13, 7, 0);
		pacman.Init();
		for (int i = 0; i < ghosts.Length; i++)
		{
			ghosts[i].transform.position = new Vector3(tileMap.CenterPosition().x, tileMap.CenterPosition().y, 0);
			ghosts[i].Init();
		}
		blinkyPosition = tileMap.CenterPosition();

		SetUiMenuName(lifeField);
		SetLifeField();

		StartCoroutine(ReadyToStart());
	}

	IEnumerator ReadyToStart()
	{
		loadingScreen.gameObject.SetActive(true);
		loadingText.text = "Stage " + GameManager.Instance.CurrentStage + "\n\n   x " + currentLife;
		Time.timeScale = 0;
		float loadingFinishTime = Time.time + GameManager.Instance.LoadingTime;
		float currentTime = Time.time;
		while (currentTime < loadingFinishTime)
		{
			yield return null;
			currentTime += Time.unscaledDeltaTime;
		}
		loadingScreen.gameObject.SetActive(false);
		Time.timeScale = 1;
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

				Vector2Int coord = new Vector2Int(j, i);
				if (tileMap.IsObstacle(coord))
				{
					AdjacentObstacle spriteIndex = tileMap.IndexOfObstacleSprite(j, i);
					sprite.sprite = obstacleSpriteDictionary[spriteIndex];
				}
				else if (tileMap.IsPrision(coord))
				{
					AdjacentObstacle spriteIndex = tileMap.IndexOfPrisionSprite(j, i);
					sprite.sprite = prisonSpriteDictionary[spriteIndex];
				}
				else if (tileMap.IsPrisonEntrance(coord))
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

	public bool IsObstacle(Vector2Int coord)
	{
		return tileMap.IsObstacle(coord) || tileMap.IsPrision(coord);
	}

	public bool IsPrisonEntrance(Vector2Int coord)
	{
		return tileMap.IsPrisonEntrance(coord);
	}

	public Vector2Int GetNextTileToVector(Vector2Int current, Vector2Int target, Direction direction, bool canReturn = false)
	{
		List<Vector2Int> path = pathFinder.FindPath(current, target, direction, canReturn);
		if (path.Count > 1)
			return path[1];
		return current;
	}

	public Vector2Int GetNextTileToPlayer(GhostPattern pattern, Vector2Int current, Direction direction, bool canReturn = false)
	{
		if (pacman != null)
		{
			Vector2Int target = new Vector2Int();
			if (pattern == GhostPattern.Blinky)
			{
				target.x = Mathf.RoundToInt(pacman.transform.position.x);
				target.y = Mathf.RoundToInt(pacman.transform.position.y);
				target = CoordInRange(target);
				blinkyPosition = current;
			}
			else if (pattern == GhostPattern.Pinky)
			{
				target.x = Mathf.RoundToInt(pacman.transform.position.x) + Global.direction[(int)pacman.GetDirection()].x * 4;
				target.y = Mathf.RoundToInt(pacman.transform.position.y) + Global.direction[(int)pacman.GetDirection()].y * 4;
			}
			else if (pattern == GhostPattern.Inky)
			{
				target.x = Mathf.RoundToInt(pacman.transform.position.x) + Global.direction[(int)pacman.GetDirection()].x * 2;
				target.y = Mathf.RoundToInt(pacman.transform.position.y) + Global.direction[(int)pacman.GetDirection()].y * 2;
				target += (target - blinkyPosition);
			}
			else if (pattern == GhostPattern.Clyde)
			{
				target.x = Mathf.RoundToInt(pacman.transform.position.x);
				target.y = Mathf.RoundToInt(pacman.transform.position.y);
				if (Mathf.Abs(target.x - current.x) + Mathf.Abs(target.y - current.y) <= 8)
				{
					target = new Vector2Int(1, 1);
				}
			}
			return GetNextTileToVector(current, target, direction, canReturn);
		}
		return current;
	}

	public Vector2Int GetNextTileToCenter(Vector2Int current, Direction direction, bool canReturn = false)
	{
		return GetNextTileToVector(current, tileMap.CenterPosition(), direction, canReturn);
	}

	public Vector2Int GetNextTileForWander(GhostPattern pattern, Vector2Int current, Direction direction, bool canReturn = false)
	{
		try
		{
			return GetNextTileToVector(current, ghostTargetInVulnerable[pattern], direction, canReturn);
		}
		catch (KeyNotFoundException) { }

		return current;
	}

	public void EatCookie(int cookieScore, bool isPCookie)
	{
		score += cookieScore;
		SetScoreField(scoreField, score);
		if (score > highScore)
		{
			highScore = score;
			PlayerPrefs.SetInt("HighScore", highScore);
			SetScoreField(highScoreField, highScore);
		}

		if (isPCookie)
		{
			for (int i = 0; i < ghosts.Length; i++)
			{
				ghosts[i].SetState(GhostState.Vulnerable);
			}
		}
	}

	void SetUiMenuName(UiMenu field)
	{
		for (int i = 0; i < field.name.Length; i++)
		{
			Image newSprite = Instantiate(menuObject, field.parent.transform.position, Quaternion.identity);
			newSprite.transform.SetParent(field.parent.transform);
			newSprite.rectTransform.MoveOnXAxis(-i * (newSprite.rectTransform.sizeDelta.x + 1));
			newSprite.rectTransform.MoveOnYAxis((newSprite.rectTransform.sizeDelta.y + 1));
			newSprite.transform.localScale = new Vector3(1, 1, 1);
			newSprite.sprite = alphabetSprites[Char.ToUpper(field.name[field.name.Length - i - 1]) - 'A'];
		}
	}

	void SetScoreField(UiMenu field, int score)
	{
		int scoreForPrint = score;
		int unit = 1;
		int digit = 0;

		if (field.contentList.Count == 0)
		{
			Image newSprite = NewMenuObject(field);
			newSprite.sprite = numberSprites[0];
			field.contentList.Add(newSprite);
		}

		while (scoreForPrint / unit > 0)
		{
			if (field.contentList.Count <= digit)
			{
				Image newSprite = NewMenuObject(field);
				field.contentList.Add(newSprite);
				newSprite.rectTransform.MoveOnXAxis(-digit * (newSprite.rectTransform.sizeDelta.x + 1));
			}
			field.contentList[digit].gameObject.SetActive(true);
			field.contentList[digit].sprite = numberSprites[(scoreForPrint / unit) % 10];
			digit++;
			unit *= 10;
		}

		for (int i = digit; i < field.contentList.Count; i++)
		{
			if (i > 0)
			{
				field.contentList[i].gameObject.SetActive(false);
			}
		}
	}

	void SetLifeField()
	{
		for (int i = 0; i < currentLife || i < lifeField.contentList.Count; i++)
		{
			if (lifeField.contentList.Count <= i)
			{
				Image newSprite = NewMenuObject(lifeField);
				newSprite.sprite = lifeSprite;
				lifeField.contentList.Add(newSprite);
				newSprite.rectTransform.MoveOnXAxis(-i * (newSprite.rectTransform.sizeDelta.x + 1));
			}

			if (i < currentLife)
				lifeField.contentList[i].gameObject.SetActive(true);
			else
				lifeField.contentList[i].gameObject.SetActive(false);
		}
	}

	Image NewMenuObject(UiMenu field)
	{
		Image newSprite = Instantiate(menuObject, field.parent.transform.position, Quaternion.identity);
		newSprite.transform.SetParent(field.parent.transform);
		newSprite.transform.localScale = new Vector3(1, 1, 1);
		return newSprite;
	}

	public bool IsCoordInRange(Vector2Int coord)
	{
		return pathFinder.IsCoordInRange(coord);
	}

	public Vector2Int CoordInRange(Vector2Int coord)
	{
		Vector2Int mapSize = tileMap.MapSize();
		Vector2Int result = coord;
		result.x = (result.x + mapSize.x) % mapSize.x;
		result.y = (result.y + mapSize.y) % mapSize.y;
		return result;
	}

	public bool OutOfTileMap(Vector3 position)
	{
		return position.x <= -1 || position.y <= -1 || position.x >= tileMap.MapSize().x || position.y >= tileMap.MapSize().y;
	}

	public Vector3 OppositePosition(Vector3 position)
	{
		Vector3 result = position;
		if (result.x <= -1 || result.x >= tileMap.MapSize().x)
		{
			result.x = tileMap.MapSize().x - position.x - 1;
		}
		if (result.y <= -1 || result.y >= tileMap.MapSize().x)
		{
			result.y = tileMap.MapSize().y - position.y - 1;
		}
		return result;
	}
}
