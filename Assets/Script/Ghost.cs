using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
	int directionIndex;
	float directionChangeTime;
	Animator animator;

	private void Awake()
	{
		directionIndex = 0;
		animator = GetComponent<Animator>();
	}

	void Start()
    {
		directionChangeTime = Time.time;
	}

    void Update()
    {
		Vector2 position = transform.position;
		position += Global.direction[directionIndex] * Time.deltaTime;
		transform.position = position;
	
        if(Time.time >= directionChangeTime)
		{
			directionIndex = Random.Range(0, (int)Direction.End);
			animator.SetFloat("MoveX", Global.direction[directionIndex].x);
			animator.SetFloat("MoveY", Global.direction[directionIndex].y);
			directionChangeTime = Time.time + 1.0f;
		}
    }
}
