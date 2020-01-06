using UnityEngine;
using System.Collections;

public static class Global
{
	public static Vector2[] direction = { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0) };
    public static bool Approximately(this Vector2 a, Vector2 b)
    {        
        return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y);
    }
}

public enum Direction
{
	Up, Right, Down, Left, End
}