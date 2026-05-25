using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashRuntime : MonoBehaviour, IProjectileBehaviour
{
    private Projectile _projectile;
    private ProjectileRuntimeData _projRuntimeData;

    public void Shoot(ProjectileRuntimeData data, List<GameObject> objects, Vector2 playerPos, Vector2 nearestEnemyPos, Vector2 nearestEnemyVelocity)
    {
        StartCoroutine(ShootSequence(data, objects));
    }

    private IEnumerator ShootSequence(ProjectileRuntimeData data, List<GameObject> objects)
    {
        var slash = (Slash)data.ownedPowerup.Base;
        int count = objects.Count;
        
        // custom tier 3 behaviour
        bool hasMaxTier = data.ownedPowerup.CurrentTier == 3;
        bool isAttackingBehind = false;
        
        for (int i = 0; i < count; i++)
        {
            var obj = objects[i];
            if (obj is null) continue;
            
            if (obj.TryGetComponent<SlashRuntimeProjectile>(out var proj))
            {
                proj.Launch(data, slash, isAttackingBehind);
                
                // flip every attack
                if (hasMaxTier)
                {
                    isAttackingBehind = !isAttackingBehind;
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
}
