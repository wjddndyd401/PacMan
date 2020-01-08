using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
	float directionChangeTime;
	Animator animator;
	SpriteRenderer spriteRenderer;
	[SerializeField] float speed = 1.0f;
	[SerializeField] float vulnerableDuration = 3.0f;
	[SerializeField] float warningTime = 2.0f;
	float aliveTime;

	Vector2Int targetPosition;
	Vector2Int currentPosition;

	GhostState state;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Start()
	{
		directionChangeTime = Time.time;

		Vector3 position = transform.position;
		currentPosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
		position.x = currentPosition.x;
		position.y = currentPosition.y;
		transform.position = position;
		targetPosition = currentPosition;
	}

	void Update()
	{
		Vector2 position = transform.position;
		if (position.Approximately(currentPosition))
		{
			FindNextPosition();
		}

		if (state == GhostState.Vulnerable)
		{
			Blink();
		}

		Move();
	}

	void Move()
	{
		if (currentPosition != targetPosition)
		{
			Vector2 position = transform.position;
			animator.SetFloat("MoveX", targetPosition.x - position.x);
			animator.SetFloat("MoveY", targetPosition.y - position.y);
			position = Vector2.MoveTowards(position, targetPosition, Time.deltaTime * speed);
			transform.position = position;
			if (position.Approximately(targetPosition))
			{
				currentPosition = targetPosition;
			}
		}
	}

	void FindNextPosition()
	{
		if (state == GhostState.Normal)
			targetPosition = GameManager.Instance.GetNextTileToPlayer(currentPosition.x, currentPosition.y);
		else if (state == GhostState.Vulnerable)
		{
			targetPosition = GetNextTileForWander();
		}
		else
		{
			targetPosition = GameManager.Instance.GetNextTileToCenter(currentPosition.x, currentPosition.y);
			if (GameManager.Instance.IsPrisonEntrance(currentPosition.x, currentPosition.y))
			{
				Alive();
			}
		}
	}

	Vector2Int GetNextTileForWander()
	{
		while (true)
		{
			Direction newDirection = (Direction)Random.Range(0, (int)Direction.End);

			Vector2Int newTarget = Vector2Int.zero;
			newTarget.x = currentPosition.x + (int)Global.direction[(int)newDirection].x;
			newTarget.y = currentPosition.y + (int)Global.direction[(int)newDirection].y;

			if (!GameManager.Instance.IsObstacle(newTarget.x, newTarget.y))
			{
				return newTarget;
			}
		}
	}

	void Alive()
	{
		state = GhostState.Normal;
		animator.SetTrigger("Alive");
		GetComponent<Collider>().enabled = true;
	}

	public void ToVulnerable()
	{
		state = GhostState.Vulnerable;
		animator.SetTrigger("Vulnerable");
		aliveTime = Time.time + vulnerableDuration;
	}

	public void Death()
	{
		state = GhostState.Death;
		animator.SetTrigger("Death");
		GetComponent<Collider>().enabled = false;
	}

	public bool CanKillPacman()
	{
		return state == GhostState.Normal;
	}

	void Blink()
	{
		Color color = spriteRenderer.color;
		if (aliveTime - Time.time < warningTime)
		{
			if (color.a >= 0.5f)
				color.a = 0.0f;
			else
				color.a = 1.0f;
		}
		if (aliveTime <= Time.time)
		{
			color.a = 1.0f;
			Alive();
		}
		spriteRenderer.color = color;
	}
}
