using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class ControllerUI : MonoBehaviour
{
    [SerializeField] private Image buttonImage;

    [SerializeField] private Sprite controllerImage;
    [SerializeField] private Sprite keyboardImage;

    [SerializeField, ReadOnly] private PlayerControls playerControlsRef;

    [SerializeField] private TextMeshProUGUI textDisplay;

    [SerializeField] private PlayerInput _controls;

    public static ControlDeviceType currentControlDevice;
    public enum ControlDeviceType
    {
        Keyboard,
        Gamepad,
    }

    void Start()
    {
        textDisplay = GetComponent<TextMeshProUGUI>();
        _controls = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (_controls.currentControlScheme != currentControlDevice.ToString())
        {
            OnControlsChanged(_controls.currentControlScheme.ToString());
        }
    }

    private void OnControlsChanged(string scheme)
    {
        if (scheme == "Gamepad")
        {
            if (currentControlDevice != ControlDeviceType.Gamepad)
            {
                currentControlDevice = ControlDeviceType.Gamepad;
                //buttonImage.sprite = controllerImage;
                textDisplay.text = "X";
            }
        }

        else
        {
            if (currentControlDevice != ControlDeviceType.Keyboard)
            {
                currentControlDevice = ControlDeviceType.Keyboard;
                //buttonImage.sprite = keyboardImage;
                textDisplay.text = "E";
            }
        }
    }
}
