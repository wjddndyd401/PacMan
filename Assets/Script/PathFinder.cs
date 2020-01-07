using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Node
{
	public int x;
	public int y;
	public int f;
	public int g;
	public int h;
	public bool isObstacle;
	public bool isOpened;
	public int parentX;
	public int parentY;
}

public class PathFinder
{
	Node[,] nodes;
	List<Node> queue;

	public PathFinder(Tile[,] tiles)
	{
		queue = new List<Node>();
		nodes = new Node[tiles.GetLength(0), tiles.GetLength(1)];
		for (int i = 0; i < tiles.GetLength(0); i++)
		{
			for (int j = 0; j < tiles.GetLength(1); j++)
			{
				nodes[i, j].g = -1;
				nodes[i, j].isOpened = true;
				nodes[i, j].isObstacle = (tiles[i, j] == Tile.Obstacle);
			}
		}
	}

	public void Reset()
	{
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				nodes[i, j].g = -1;
				nodes[i, j].isOpened = true;
			}
		}
	}

	public void FindPath(int startX, int startY, int endX, int endY)
	{
		int x = startX;
		int y = startY;
		Enqueue(startX, startY, -1, startX, startY, endX, endY);

		while (queue.Count != 0 || (x == endX && y == endY))
		{
			queue.Sort(delegate(Node a, Node b)
			{
				if (a.f > b.f) return 1;
				else if (a.f < b.f) return -1;
				return 0;
			});
			Node node = queue[0];
			queue.RemoveAt(0);
			x = node.x; y = node.y;
			nodes[y, x].isOpened = false;

			Enqueue(x, y + 1, node.g, x, y, endX, endY);
			Enqueue(x + 1, y, node.g, x, y, endX, endY);
			Enqueue(x, y - 1, node.g, x, y, endX, endY);
			Enqueue(x - 1, y, node.g, x, y, endX, endY);
		}

		string result = "";
		while (true)
		{						
			if (nodes[y, x].parentX == x && nodes[y, x].parentY == y)
				break;
			result = "(" + x + ", " + y + ")" + result;
			x = nodes[y, x].parentX;
			y = nodes[y, x].parentY;
		}
		Debug.Log(result);
	}

	void Enqueue(int x, int y, int g, int parentX, int parentY, int endX, int endY)
	{
		if (x >= 0 && y >= 0 && x < nodes.GetLength(1) && y < nodes.GetLength(1) &&
			!nodes[y, x].isObstacle && nodes[y, x].isOpened)
		{
			nodes[y, x].x = x;
			nodes[y, x].y = y;
			if (nodes[y, x].g < 0 || nodes[y, x].g > g)
			{
				nodes[y, x].g = g + 1;
				nodes[y, x].parentX = parentX;
				nodes[y, x].parentY = parentY;
			}
			nodes[y, x].h = Mathf.Abs(endX - x) + Mathf.Abs(endY - y);
			nodes[y, x].f = nodes[y, x].g + nodes[y, x].h;
			queue.Add(nodes[y, x]);
		}
	}
}
