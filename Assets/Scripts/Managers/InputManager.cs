using UnityEngine;
using UnityEngine.InputSystem;
using System;
using TMPro;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public Vector2 MoveInput { get; private set; }
    
    public event Action<Vector2> OnClickEvent;
    public event Action<InputAction.CallbackContext> OnPlayerMoveEvent;
    // public event Action<InputAction.CallbackContext> OnMouseMoveEvent;
    
    private Camera _mainCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _mainCamera = Camera.main;
    }

    public void OnPlayerMove(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
        OnPlayerMoveEvent?.Invoke(ctx);
    }

    public void OnClick(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        Vector2 worldPos = GetMouseWorldPosition();
        
        OnClickEvent?.Invoke(worldPos);
    }

    public Vector2 GetMouseWorldPosition()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 screenPos = new Vector3(mousePos.x, mousePos.y, -_mainCamera.transform.position.z);
        Vector3 world = _mainCamera.ScreenToWorldPoint(screenPos);
        Vector2 worldPos = new Vector2(world.x, world.y);

        return worldPos;
    }

    public Vector3 GetMouseScreenPosition()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 screenPos = new Vector3(mousePos.x, mousePos.y, -_mainCamera.transform.position.z);

        return screenPos;
    }
}