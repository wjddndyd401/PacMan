using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
	[SerializeField] GameObject mainMenu = null;
	[SerializeField] GameObject option = null;
	[SerializeField] ToggleGroup optionGroup = null;
	[SerializeField] OptionToggle[] toggles = null;

	private void Start()
	{
		OpenMainMenu();
	}

	public void OpenMainMenu()
	{
		mainMenu.SetActive(true);
		option.SetActive(false);
	}

	public void OpenOption()
	{
		mainMenu.SetActive(false);
		option.SetActive(true);

		for (int i = 0; i < toggles.Length; i++)
		{
			if (toggles[i].ControlMode == GameManager.Instance.controlMode)
			{
				toggles[i].GetComponent<Toggle>().isOn = true;
			}
		}
	}

	public void StartGame()
	{
		SceneManager.LoadScene("GameScene");
	}

	public void SetControlMode()
	{
		Toggle activeToggle = null;
		foreach (var toggle in optionGroup.ActiveToggles())
		{
			if (toggle.isOn)
			{
				activeToggle = toggle;
			}
		}

		if(activeToggle != null)
		{
			GameManager.Instance.SetControlMode(activeToggle.GetComponent<OptionToggle>().ControlMode);
		}
	}
}
