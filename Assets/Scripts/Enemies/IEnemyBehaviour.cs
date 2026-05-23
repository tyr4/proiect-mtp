public interface IEnemyBehaviour
{
    void Initialize(EnemyRuntime data, Enemy enemy);
    void Tick(float dt);
}
