using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionToggle : MonoBehaviour
{
    [SerializeField] ControlMode mode = ControlMode.TwoHand;

	private void Start()
	{
		Toggle toggle = GetComponent<Toggle>();
		if (mode == GameManager.Instance.controlMode)
		{
			toggle.isOn = true;
		}

		toggle.onValueChanged.AddListener((value) =>
		{
			if(value)
			{
				GameManager.Instance.SetControlMode(mode);
			}
		});
	}
}
