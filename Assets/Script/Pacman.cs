using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
	Direction directionIndex;

	private void Awake()
	{
		directionIndex = Direction.Up;
		ChangeDirection();
	}

	void Start()
    {
        
    }

    void Update()
	{
		GetInput();
		ChangeDirection();
		Move();
	}

	public void Move()
	{
		Vector2 position = transform.position;
		position += Global.direction[(int)directionIndex] * Time.deltaTime;
		transform.position = position;
	}

	public void GetInput()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			directionIndex = Direction.Up;
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			directionIndex = Direction.Right;
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			directionIndex = Direction.Down;
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			directionIndex = Direction.Left;
		}
	}

	public void ChangeDirection()
	{
		Vector3 rotation = transform.rotation.eulerAngles;
		if (directionIndex == Direction.Up)
		{
			rotation.z = 90;
		}
		else if (directionIndex == Direction.Right)
		{
			rotation.y = 0;
			rotation.z = 0;
		}
		else if (directionIndex == Direction.Down)
		{
			rotation.z = -90;
		}
		else if (directionIndex == Direction.Left)
		{
			rotation.y = 180;
			rotation.z = 0;
		}
		transform.rotation = Quaternion.Euler(rotation);
	}
}
