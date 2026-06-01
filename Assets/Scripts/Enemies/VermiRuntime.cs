using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class VermiRuntime : EnemyRuntime, IEnemyBehaviour
{
    private enum AbilityType
    {
        ProjectileAttack,
        SpawnAttack
    }
    
    private Vermi _vermi;
    
    private float _useAbilityTimer;
    private float _globalAbilityCooldown = 5f;

    private float _projectileAttackTimer;
    private float _spawnAttackTimer;

    private List<AbilityType> _available = new();
    private bool _isUsingAbility;
    
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsAttacking = Animator.StringToHash("isAttacking");

    public void Initialize(EnemyRuntime data, Enemy enemy)
    {
        _vermi = (Vermi)enemy;
    }
    
    public void Tick(float dt)
    {
        _useAbilityTimer += dt;
        _projectileAttackTimer += dt;
        _spawnAttackTimer += dt;
        
        if (_useAbilityTimer >= _globalAbilityCooldown)
        {
            ChooseAbilityToUse();
            _useAbilityTimer = 0;
        }
    }

    protected override void DespawnIfOutOfRange()
    {
        if (Distance < DespawnDistanceSquared) return;

        cachedTransform.position = WaveManager.Instance.GenerateRandomPosition();
    }

    private void ChooseAbilityToUse()
    {
        if (_isUsingAbility) return;
        
        _available.Clear();
        
        if (_projectileAttackTimer >= _vermi.ProjectileAttackCooldown)
            _available.Add(AbilityType.ProjectileAttack);
        
        if (_spawnAttackTimer >= _vermi.SpawnAttackCooldown)
            _available.Add(AbilityType.SpawnAttack);

        if (_available.Count == 0) return;

        var index = Random.Range(0, _available.Count);
        var chosen = _available[index];

        UseAbility(chosen);
    }

    private void UseAbility(AbilityType ability)
    {
        _isUsingAbility = true;

        switch (ability)
        {
            case AbilityType.ProjectileAttack:
                _projectileAttackTimer = 0;
                StartCoroutine(ProjectileAttackCoroutine());

                break;
            
            case AbilityType.SpawnAttack:
                _spawnAttackTimer = 0;
                StartCoroutine(SpawnAttackCoroutine());

                break;
        }
    }

    private IEnumerator SpawnAttackCoroutine()
    {
        Debug.Log("Using SpawnAttack");
        DisableMovement();
        
        animator.SetBool(IsAttacking, true);
        animator.SetBool(IsWalking, false);
        yield return null;
        
        // wait for the shoot animation to complete
        yield return new WaitUntil(() => 
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            // Debug.Log($"state: {info.fullPathHash}, normalizedTime: {info.normalizedTime}, isTransition: {_animator.IsInTransition(0)}");
            return animator is null ||
                   (info.normalizedTime >= 1f && !animator.IsInTransition(0));
        });

        animator.SetBool(IsAttacking, false);
        animator.SetBool(IsWalking, true);
        _isUsingAbility = false;
        
        EnableMovement();
    }

    private IEnumerator ProjectileAttackCoroutine()
    {
        Debug.Log("Using ProjectileAttack");
        DisableMovement();
        
        animator.SetBool(IsAttacking, true);
        animator.SetBool(IsWalking, false);
        yield return null;
        
        // wait for the animation to finish
        yield return new WaitUntil(() => 
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            // Debug.Log($"state: {info.fullPathHash}, normalizedTime: {info.normalizedTime}, isTransition: {_animator.IsInTransition(0)}");
            return animator is null ||
                   (info.normalizedTime >= 1f && !animator.IsInTransition(0));
        });

        // actual spawning logic
        var offset = 1.3f;
        
        for (int i = 0; i < _vermi.SpawnAmount; i++)
        {
            var runtime = WaveManager.Instance.SpawnEnemy(_vermi.SpawnEnemy);
            
            runtime.cachedTransform.position = (Vector2)cachedTransform.position + Random.insideUnitCircle * offset;
            
            yield return new WaitForSeconds(0.2f);
        }
        
        animator.SetBool(IsAttacking, false);
        animator.SetBool(IsWalking, true);
        _isUsingAbility = false;
        
        EnableMovement();
    }
}
