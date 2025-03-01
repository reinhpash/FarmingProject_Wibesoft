using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Aeterponis.InputHandler
{
    public class TouchManager : MonoBehaviour
    {
        //PRIVATE
        private PlayerTouchActions inputActions;
        private Vector2 touchPosition;
        private bool isDragging = false;

        //EVENTS
        public Action OnScreenTouched;
        public Action OnScreenToucheReleased;
        public Action OnHoldHappend;

        //PROPERTIES
        public Vector2 TouchPosition { get => touchPosition; private set => touchPosition = value; }
        public bool IsDragging { get => isDragging; private set => isDragging = value; }

        private void Awake()
        {
            inputActions = new PlayerTouchActions();
            inputActions.Touch.Enable();

            inputActions.Touch.TouchPress.performed += OnScreenTouch;
            inputActions.Touch.TouchPress.canceled += OnScreenTouchReleased;
            inputActions.Touch.HoldPress.performed += OnHoldPerformed;
        }



        private void OnDisable()
        {
            inputActions.Touch.TouchPress.performed -= OnScreenTouch;
            inputActions.Touch.TouchPress.canceled -= OnScreenTouchReleased;
            inputActions.Touch.HoldPress.performed -= OnHoldPerformed;
            inputActions.Touch.Disable();
        }

        private void OnScreenTouch(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                touchPosition = inputActions.Touch.TouchPosition.ReadValue<Vector2>();
                OnScreenTouched?.Invoke();
            }
        }

        private void OnScreenTouchReleased(InputAction.CallbackContext context)
        {
            OnScreenToucheReleased?.Invoke();
        }

        private void OnHoldPerformed(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
                OnHoldHappend?.Invoke();
        }


        private void Update()
        {
            if (inputActions.Touch.TouchPress.WasPressedThisFrame())
            {
                isDragging = true;
            }

            if (inputActions.Touch.TouchPress.WasReleasedThisFrame())
            {
                isDragging = false;
            }

            if (isDragging)
            {
                touchPosition = inputActions.Touch.TouchPosition.ReadValue<Vector2>();
            }
        }
    }
}