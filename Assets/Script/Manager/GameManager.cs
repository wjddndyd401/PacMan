using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] int maxLife = 3;
	[SerializeField] int currentStage = 1;
	public ControlMode controlMode = ControlMode.TwoHand;

	public int MaxLife { get { return maxLife; } private set { maxLife = value; } }
	public int CurrentStage { get { return currentStage; } private set { currentStage = value; } }

	public static GameManager Instance { get; private set; } = null;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);

		DontDestroyOnLoad(this);

		controlMode = (ControlMode) PlayerPrefs.GetInt("ControlMode", (int)ControlMode.TwoHand);
	}

	public void GoToNextStage()
	{
		currentStage++;
	}

	public void Restart()
	{
		currentStage = 1;
	}

	public void SetControlMode(ControlMode newMode)
	{
		controlMode = newMode;
		PlayerPrefs.SetInt("ControlMode", (int)controlMode);
	}
}
