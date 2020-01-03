using UnityEngine;
using System.Collections;

public static class Global
{
	public static Vector2[] direction = { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0) };
}

public enum Direction
{
	Up, Right, Down, Left, End
}