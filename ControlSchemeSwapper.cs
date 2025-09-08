using System;
using Framework.Input;
using Helpers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputHelpers
{
    public class ControlSchemeSwapper : MonoBehaviour, SwapControlSchemeInput.ISwitchActions
    {
        private SwapControlSchemeInput controls;
        public static event Action<ControlScheme> OnChanged;

        public static ControlScheme currentControlScheme = ControlScheme.KeyboardAndMouse;

        private void Awake()
        {
            controls = new SwapControlSchemeInput();
            controls.Enable();
            controls.Switch.AddCallbacks(this);
            
            if(Defines.IsGamePadSupported())
                currentControlScheme = ControlScheme.Gamepad;
            else if (Defines.IsTouchScreenSupported())
                currentControlScheme = ControlScheme.Touch;
            else
                currentControlScheme = ControlScheme.KeyboardAndMouse;
        }

        private void OnDestroy()
        {
            controls.Disable();
            controls.Switch.RemoveCallbacks(this);
        }

        public void OnSwitchToGamepad(InputAction.CallbackContext context)
        {
            if (Defines.IsGamePadSupported() && currentControlScheme != ControlScheme.Gamepad)
            {
                Debug.Log("Gamepad in control!");
                currentControlScheme = ControlScheme.Gamepad;
                OnChanged?.Invoke(currentControlScheme);
            }
        }

        public void OnSwitchToKBM(InputAction.CallbackContext context)
        {
            if (currentControlScheme != ControlScheme.KeyboardAndMouse)
            {
                Debug.Log("Keyboard+Mouse in control!");
                currentControlScheme = ControlScheme.KeyboardAndMouse;
                OnChanged?.Invoke(currentControlScheme);
            }
        }

        public void OnSwitchToTouch(InputAction.CallbackContext context)
        {
            if (Defines.IsTouchScreenSupported() && currentControlScheme != ControlScheme.Touch)
            {
                Debug.Log("Touchscreen in control!");
                currentControlScheme = ControlScheme.Touch;
                OnChanged?.Invoke(currentControlScheme);
            }
        }
    }
}

namespace Framework.Input
{
    public enum ControlScheme
    {
        KeyboardAndMouse,
        Gamepad,
        Touch
    }
}