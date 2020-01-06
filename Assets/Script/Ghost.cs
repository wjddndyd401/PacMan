using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
	Direction direction;
	float directionChangeTime;
	Animator animator;
	[SerializeField] float speed = 1.0f;

	Vector2Int targetPosition;
	Vector2Int currentPosition;

	private void Awake()
	{
		direction = Direction.Right;
		animator = GetComponent<Animator>();
	}

	void Start()
    {
		directionChangeTime = Time.time;

		Vector3 position = transform.position;
		currentPosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
		position.x = currentPosition.x;
		position.y = currentPosition.y;
		transform.position = position;
	}

    void Update()
	{
		Vector2 position = transform.position;

		if (position.Approximately(currentPosition))
		{
			while (true)
			{
				Direction newDirection = (Direction)Random.Range(0, (int)Direction.End);

				Vector2Int newTarget = Vector2Int.zero;
				newTarget.x = currentPosition.x + (int)Global.direction[(int)newDirection].x;
				newTarget.y = currentPosition.y + (int)Global.direction[(int)newDirection].y;

				if (!GameManager.Instance.IsObstacle(newTarget.x, newTarget.y))
				{
					direction = newDirection;
					targetPosition = newTarget;
					break;
				}
			}
		}

		position = Vector2.MoveTowards(position, targetPosition, Time.deltaTime * speed);
		transform.position = position;
		if (position.Approximately(targetPosition))
		{
			currentPosition = targetPosition;
		}
    }
}
