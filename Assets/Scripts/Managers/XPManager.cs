using System.Collections.Generic;
using UnityEngine;

public class XPManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private List<XPDrop> xpDropList;
    [SerializeField] private float cleanupInterval;
    [SerializeField] private int maxXPDrops;
    [field: SerializeField] public float DespawnRange { get; private set;}
    
    // TODO: hashset, cache getcomponent<runtime> in pool with a pool wrapper
    private ObjectPool<XPDrop> _objPool = new();
    private List<XPDropRuntime> _attractedDrops = new();
    private List<XPDropRuntime> _allDrops = new();

    private float _xpBuffer = 0;
    
    private float _cleanupTimer;
    
    public static XPManager Instance;
    
    private void Awake()
    {
        Instance = this;
    }

    private void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;
        _cleanupTimer += dt;

        if (_cleanupTimer > cleanupInterval)
        {
            Cleanup();
            _cleanupTimer = 0;
        }
        
        for (int i = 0; i < _attractedDrops.Count; i++)
        {
            var xpRuntime = _attractedDrops[i];
            
            xpRuntime.Tick(dt, playerTransform);
        }
    }

    public void SpawnXP(Vector3 position)
    {
        int choice = Random.Range(0, xpDropList.Count);
        var xpData = xpDropList[choice];

        // dont spawn any more if the limit has reached, store the value
        if (_allDrops.Count >= maxXPDrops)
        {
            _xpBuffer += xpData.Value;
            return;
        }
        
        var xpObj = _objPool.Get(xpData, xpData.Prefab);
        var xpRuntime = xpObj.GetComponent<XPDropRuntime>();
        var xpBoostValue = GetXPBoostValue();
        
        xpRuntime.Initialize(xpData, xpBoostValue);
        xpObj.transform.position = position;
        xpObj.SetActive(true);
        
        _allDrops.Add(xpRuntime);
    }

    public void ReturnToPool(XPDrop xpDrop, XPDropRuntime xpRuntime)
    {
        xpRuntime.gameObject.SetActive(false);
        
        _objPool.Return(xpDrop, xpRuntime.gameObject);
        _allDrops.Remove(xpRuntime);
    }

    public float GetXPForNextLevel(int currentLevel)
    {
        return Mathf.RoundToInt(50 * Mathf.Pow(1.25f, currentLevel - 1));
    }

    private float GetXPBoostValue()
    {
        if (_xpBuffer < 50f) return 0;
        
        var value = _xpBuffer * 0.05f; // 5%
        _xpBuffer *= 0.05f;

        return value;
    }

    public void RegisterAttracted(XPDropRuntime xpRuntime)
    {
        _attractedDrops.Add(xpRuntime);
    }

    public void UnregisterAttracted(XPDropRuntime xpRuntime)
    {
        _attractedDrops.Remove(xpRuntime);
    }

    private void Cleanup()
    {
        for (int i = 0; i < _allDrops.Count; i++)
        {
            var xp = _allDrops[i];
            
            if (xp.DespawnIfNecessary(playerTransform))
            {
                _xpBuffer += xp.Data.Value;
            }
        }
    }
}
