using UnityEngine;

public class EnemyRuntime : MonoBehaviour
{
    public EnemyData Data { get; private set; }
    
    private float _health;
    private float _damage;
    private float _movementSpeed;
    
    public void Initialize(EnemyData enemyData)
    {
        Data = enemyData;
        _health = Data.Health;
        _damage = Data.Damage;
        _movementSpeed = Data.MovementSpeed;
    }
    
    public void Tick(float deltaTime, Vector3 direction)
    {
        transform.position += direction.normalized * (_movementSpeed * deltaTime);
    }

    // TODO: hurt logic here
    public void UpdateAnimation()
    {
        
    }
}
