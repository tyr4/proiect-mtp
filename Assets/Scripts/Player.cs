using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    
    private Rigidbody2D _rb;
    private Animator _animator;
    
    static readonly int IsWalking = Animator.StringToHash("isWalking");
    static readonly int HasTakenDamage = Animator.StringToHash("hasTakenDamage");
    static readonly int HasDied = Animator.StringToHash("hasDied");
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        InputManager.Instance.OnPlayerMoveEvent += OnMove;
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnPlayerMoveEvent -= OnMove;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        UpdateAnimation(InputManager.Instance.MoveInput);
    }
    
    private void FixedUpdate()
    {
        Vector2 input = InputManager.Instance.MoveInput;
        _rb.linearVelocity = input * movementSpeed;

        if (!input.Equals(Vector2.zero))
        {
        }
        
        if (input.x != 0)
        {
            transform.rotation = Quaternion.Euler(0f, input.x < 0 ? 180f : 0f, 0f);
        }
    }

    private void UpdateAnimation(Vector2 input)
    {
        var isWalking = input != Vector2.zero;
        // Debug.Log(isWalking + " " + input + " " + Vector2.zero);
        _animator.SetBool(IsWalking, isWalking);
        
        // TODO: logic for damage taken/death
    }
}