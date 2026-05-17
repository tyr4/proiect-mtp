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
    public static event Action<float, float> OnXPChanged;
    public static event Action<int> OnLevelUp;
    
    
    // runtime global stats
    private PlayerStats _rts;
    
    // runtime changing stats
    private float _currentHealth;
    
    private int _currentLevel = 1;
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

        _nextLevelXp = GetNextLevelXP();
        
        // set ui in place
        OnXPChanged?.Invoke(_currentXp, _nextLevelXp);
        OnLevelUp?.Invoke(_currentLevel);
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
        _currentHealth = _rts.MaxHealth;
    }

    public void HandleEnemyCollision(EnemyRuntime enemy)
    {
        // TODO: take enemy._damage as public param here
        TakeDamage(enemy.Damage);
    }
    
    public void HandleXpPickup(XPDropRuntime xpRuntime)
    {
        _currentXp += xpRuntime.Data.Value;
        OnXPChanged?.Invoke(_currentXp, _nextLevelXp);
        
        if (_currentXp >= _nextLevelXp)
        {
            LevelUp();
        }
    }

    
    private void LevelUp()
    {
        _currentLevel++;
        _currentXp = 0;
        
        _nextLevelXp = GetNextLevelXP();
        
        Debug.Log(_nextLevelXp);
        OnXPChanged?.Invoke(_currentXp, _nextLevelXp);
        OnLevelUp?.Invoke(_currentLevel);
    }

    private float GetNextLevelXP()
    {
        return XPManager.Instance.GetXPForNextLevel(_currentLevel);
    }
}