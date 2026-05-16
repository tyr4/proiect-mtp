using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _sr;
    
    static readonly int IsWalking = Animator.StringToHash("isWalking");
    static readonly int HasTakenDamage = Animator.StringToHash("hasTakenDamage");
    static readonly int HasDied = Animator.StringToHash("hasDied");
    
    // events
    public static event Action<float, float> OnHealthChanged;
    
    // runtime global stats
    private PlayerStats _rts;
    
    // runtime changing stats
    private float _currentHealth;
    
    private int _currentLevel;
    private float _currentXp;
    private float _nextLevelXp;
    
    private void Awake()
    {
        _rts = Instantiate(playerStats);
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
        
        _currentHealth = playerStats.MaxHealth;
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
        _rb.linearVelocity = input * _rts.MovementSpeed;

        if (!input.Equals(Vector2.zero))
        {
        }
        
        if (input.x != 0)
        {
            _sr.flipX = input.x < 0;
            // transform.rotation = Quaternion.Euler(0f, input.x < 0 ? 180f : 0f, 0f);
        }
    }

    private void UpdateAnimation(Vector2 input)
    {
        var isWalking = input != Vector2.zero;
        // Debug.Log(isWalking + " " + input + " " + Vector2.zero);
        _animator.SetBool(IsWalking, isWalking);
        
        // TODO: logic for damage taken/death
    }

    private void TakeDamage(float value)
    {
        _currentHealth -= value;
        
        if (_currentHealth <= 0)
        {
            Die();
        }
        
        OnHealthChanged?.Invoke(_currentHealth, _rts.MaxHealth);
    }

    private void Die()
    {
        Debug.Log("ai murit vro");
        _currentHealth = _rts.MaxHealth;
    }

    public void HandleEnemyCollision(EnemyRuntime enemy)
    {
        TakeDamage(enemy.Data.Damage);
        Debug.Log("am luat dmg ouch");
    }
    
    public void HandleXpPickup(XPDropRuntime xpRuntime)
    {
        Debug.Log("am primit xp am primit xp");
    }
}