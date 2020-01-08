﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
	Direction direction;
	Direction newDirection;
	Vector2Int targetPosition;
	Vector2Int currentPosition;
	[SerializeField] float speed = 1.0f;
	Animator animator;
	bool isDeath;

	private void Awake()
	{
		direction = Direction.Right;
		newDirection = direction;
		RotateToDirection();
		isDeath = false;
		animator = GetComponent<Animator>();
	}

	void Start()
	{
		Vector3 position = transform.position;
		currentPosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
		position.x = currentPosition.x;
		position.y = currentPosition.y;
		transform.position = position;
	}

    void Update()
	{
		if (!isDeath)
		{
			SetDirectionByInput();
			RotateToDirection();
			Move();
		}
		else
		{
			AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
			if (info.IsName("Death") && info.normalizedTime >= 1.0f)
			{
				Destroy(gameObject);
			}
		}
	}

	public void Move()
	{
		Vector2Int newTarget = GetTargetPosition(currentPosition, newDirection);

		Vector2 position = transform.position;
		if (Global.Opposition(direction) == newDirection ||
			(position.Approximately(currentPosition) && !GameManager.Instance.IsObstacle(newTarget.x, newTarget.y) && !GameManager.Instance.IsPrisonEntrance(newTarget.x, newTarget.y)))
		{
			direction = newDirection;
			targetPosition = newTarget;
		}
		else
		{
			targetPosition = GetTargetPosition(currentPosition, direction);
		}

		if (!GameManager.Instance.IsObstacle(targetPosition.x, targetPosition.y))
		{
			position = Vector2.MoveTowards(position, targetPosition, Time.deltaTime * speed);
			transform.position = position;

			if (position.Approximately(targetPosition))
			{
				currentPosition = targetPosition;
			}
		}
	}

	public void SetDirectionByInput()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			newDirection = Direction.Up;
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			newDirection = Direction.Right;
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			newDirection = Direction.Down;
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			newDirection = Direction.Left;
		}
	}

	public void RotateToDirection()
	{
		Vector3 rotation = transform.rotation.eulerAngles;
		if (direction == Direction.Up)
		{
			rotation.z = 90;
		}
		else if (direction == Direction.Right)
		{
			rotation.y = 0;
			rotation.z = 0;
		}
		else if (direction == Direction.Down)
		{
			rotation.z = -90;
		}
		else if (direction == Direction.Left)
		{
			rotation.y = 180;
			rotation.z = 0;
		}
		transform.rotation = Quaternion.Euler(rotation);
	}

	public Vector2Int GetTargetPosition(Vector2Int current, Direction direction)
	{
		Vector2Int result = Vector2Int.zero;
		result.x = current.x + (int)Global.direction[(int)direction].x;
		result.y = current.y + (int)Global.direction[(int)direction].y;
		return result;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Cookie"))
		{
			Destroy(other.gameObject);
			other.GetComponent<Cookie>().Collide();
		} else if(other.CompareTag("Ghost"))
		{
			Ghost ghost = other.GetComponent<Ghost>();
			if (ghost.CanKillPacman())
			{
				isDeath = true;
				animator.SetTrigger("Death");
			} else
			{
				ghost.Death();
			}
		}
	}
}
