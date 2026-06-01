using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private CircleCollider2D xpMagnetCollider;
    [SerializeField] private Transform passiveEffectsContainer;
    [SerializeField] private Transform visualsTransform;
    [SerializeField] private ParticleSystem walkingParticles;
    
    public Transform PassiveEffectsContainer => passiveEffectsContainer;
    
    private Rigidbody2D _rb;
    private Animator _animator;
    private Transform _walkingParticlesTransform;
    
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int HasTakenDamage = Animator.StringToHash("hasTakenDamage");
    private static readonly int HasDied = Animator.StringToHash("hasDied");
    
    // events
    public static event Action<float, float> OnHealthChanged;
    public static event Action<float, float> OnXPChanged;
    public static event Action<int> OnLevelUp;
    
    private StartingPlayerData _startingPlayerData;
    private PlayerStats _rts;
    
    // runtime changing stats
    private float _currentHealth;
    
    private int _currentLevel = 1;
    private float _currentXp;
    private float _nextLevelXp;

    public static Player Instance;
    
    private void Awake()
    {
        Instance = this;
        
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _walkingParticlesTransform = walkingParticles.gameObject.transform;
    }

    private void Start()
    {
        InputManager.Instance.OnPlayerMoveEvent += OnMove;

        _nextLevelXp = GetNextLevelXP();
        
        GameManager.Instance.SetPlayerData(this);
        _startingPlayerData = GameManager.Instance.GetStartingData();
        
        _rts = _startingPlayerData.playerStats.Clone();
        _currentHealth = _rts.maxHealth;
        
        // set ui in place
        OnXPChanged?.Invoke(_currentXp, _nextLevelXp);
        OnLevelUp?.Invoke(_currentLevel);
        
        AudioEvents.RequestMusic(AudioManager.Sounds.gameplay);
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
        _rb.linearVelocity = input * _rts.movementSpeed;

        // particle system
        if (input.Equals(Vector2.zero))
        {
            walkingParticles.Stop();
        }
        else
        {
            if (walkingParticles.isStopped) walkingParticles.Play();
        }
        
        // player and particles scale flipping
        if (input.x != 0)
        { 
            DirectionCorrectScale(visualsTransform, input, false);
            DirectionCorrectScale(_walkingParticlesTransform, input, true);
        }
    }

    private void UpdateAnimation(Vector2 input)
    {
        var isWalking = input != Vector2.zero;
        // Debug.Log(isWalking + " " + input + " " + Vector2.zero);
        _animator.SetBool(IsWalking, isWalking);
        
        // TODO: logic for damage taken/death
    }

    public void SetAnimationController(RuntimeAnimatorController controller)
    {
        _animator.runtimeAnimatorController = controller;
    }

    private void DirectionCorrectScale(Transform parent, Vector2 input, bool flipY = false)
    {
        var scale = parent.localScale;
        
        var newScale = new Vector3(input.x < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x), 
                                   flipY && input.x < 0 ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y), 
                                   scale.z);
        
        parent.localScale = newScale;
    }

    public void TakeDamage(float value)
    {
        _currentHealth -= value;
        
        if (_currentHealth <= 0)
        {
            Die();
        }

        AudioEvents.RequestSFX(AudioManager.Sounds.playerHurt);
        OnHealthChanged?.Invoke(_currentHealth, _rts.maxHealth);
    }

    private void Die()
    {
        _currentHealth = _rts.maxHealth;
    }

    public void HandleEnemyCollision(EnemyRuntime enemy)
    {
        TakeDamage(enemy.Damage);
    }
    
    public void HandleXpPickup(XPDropRuntime xpRuntime)
    {
        _currentXp += xpRuntime.GetXPValue();
        
        AudioEvents.RequestSFX(AudioManager.Sounds.xpPickup);
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

    public void ModifyMaxHealth(float value, OneTimeBuff.ValueType valueType)
    {
        switch (valueType)
        {
            case OneTimeBuff.ValueType.Additive:
                _rts.maxHealth += value;
                _currentHealth += value;
                break;
            
            case OneTimeBuff.ValueType.Multiplicative:
                _rts.maxHealth *= value;
                _currentHealth *= value;
                break;
            
            case OneTimeBuff.ValueType.Percentage:
                var val = value / 100f;
                
                _rts.maxHealth += _rts.maxHealth * val;
                _currentHealth += _currentHealth * val;
                break;
        }
        
        Debug.Log($"acum am {_rts.maxHealth} sefu");
        OnHealthChanged?.Invoke(_currentHealth, _rts.maxHealth);
    }
    
    public void ModifyXPRadius(float value, OneTimeBuff.ValueType valueType)
    {
        switch (valueType)
        {
            case OneTimeBuff.ValueType.Additive:
                xpMagnetCollider.radius += value;
                break;
            
            case OneTimeBuff.ValueType.Multiplicative:
                xpMagnetCollider.radius *= value;

                break;
            
            case OneTimeBuff.ValueType.Percentage:
                var val = value / 100f;

                xpMagnetCollider.radius += xpMagnetCollider.radius * val;
                break;
        }
    }
    
    public void ModifyMovementSpeed(float value, OneTimeBuff.ValueType valueType)
    {
        switch (valueType)
        {
            case OneTimeBuff.ValueType.Additive:
                _rts.movementSpeed += value;
                break;
            
            case OneTimeBuff.ValueType.Multiplicative:
                _rts.movementSpeed *= value;
                break;
            
            case OneTimeBuff.ValueType.Percentage:
                var val = value / 100f;

                _rts.movementSpeed += _rts.movementSpeed * val;
                break;
        }
    }
}