using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public static class Global
{
	public static Vector2Int[] directions = { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };
	public static Vector2Int[] diagonals = { new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 1) };
	public static AdjacentObstacle[] diagonalIndices = { AdjacentObstacle.ExceptRightup, AdjacentObstacle.ExceptRightdown, AdjacentObstacle.ExceptLeftdown, AdjacentObstacle.ExceptLeftup };

	public static bool Approximately(this Vector2 a, Vector2 b)
    {        
        return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y);
    }

	public static Direction Opposition(Direction value)
	{
		return (Direction) (((int) value + 2) % (int) Direction.End);
	}

	public static int GetTileDistance(Vector2Int current, Vector2Int target, Vector2Int size)
	{
		int diffX = Mathf.Abs(target.x - current.x);
		int diffY = Mathf.Abs(target.y - current.y);
		return Mathf.Min(size.x - diffX, diffX) + Mathf.Min(size.y - diffY, diffY);
	}

	public static void MoveOnXAxis(this RectTransform a, float value)
	{
		Vector2 position = a.anchoredPosition;
		position.x += value;
		a.anchoredPosition = position;
	}

	public static void MoveOnYAxis(this RectTransform a, float value)
	{
		Vector2 position = a.anchoredPosition;
		position.y += value;
		a.anchoredPosition = position;
	}

	public static Direction GetDirection(this Vector2 vector)
	{
		if(Mathf.Abs(vector.y) >= Mathf.Abs(vector.x))
		{
			if (vector.y >= 0)
			{
				return Direction.Up;
			}
			else
			{
				return Direction.Down;
			}
		}

		else
		{
			if (vector.x >= 0)
			{
				return Direction.Right;
			}
			else
			{
				return Direction.Left;
			}
		}
	}
}

public enum Direction
{
	Up, Right, Down, Left, End
}

public enum AdjacentObstacle
{
	None, UpDownLeft, UpRightDown, UpRightLeft, RightDownLeft, UpRight, RightDown, UpLeft, DownLeft, ExceptRightup, ExceptRightdown, ExceptLeftdown, ExceptLeftup
}

[System.Serializable]
public struct ObstacleSprite
{
	public AdjacentObstacle adjacentObstacleDirection;
	public Sprite sprite;
}

public enum GhostState
{
	Normal, Vulnerable, Death
}

public enum GhostPattern
{
	Blinky, Pinky, Inky, Clyde
}

[System.Serializable]
public struct GhostTargetInVulnerable
{
	public GhostPattern pattern;
	public Vector2Int target;
}

[System.Serializable]
public struct UiMenu
{
	public GameObject parent;
	public string name;
	[HideInInspector] public List<Image> contentList;
}

public enum Tile
{
	Empty, Obstacle, Cookie, PCookie, PrisonWall, Entrance, Prison, PlayerPosition, End
}

public enum FileSystemMode
{
	Save, Load
}

[System.Serializable]
public struct MoveButton
{
	public Direction direction;
	public Button button;
}

public enum ControlMode
{
	LeftHand, RightHand, TwoHand
}