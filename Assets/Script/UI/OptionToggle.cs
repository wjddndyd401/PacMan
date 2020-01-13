using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionToggle : MonoBehaviour
{
    [SerializeField] ControlMode mode = ControlMode.TwoHand;
    public ControlMode ControlMode { get { return mode; } }
}
