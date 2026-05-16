using System.Collections.Generic;
using UnityEngine;

public class XPManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private List<XPDrop> xpDropList;

    private ObjectPool<XPDrop, XPDropRuntime> _objPool = new();
    private List<XPDropRuntime> _attractedDrops = new();
    
    public static XPManager Instance;
    
    private void Awake()
    {
        Instance = this;
    }

    private void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;
        
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
        var xpRuntime = _objPool.Get(xpData, xpData.Prefab);
        var xpObj = xpRuntime.gameObject;

        xpRuntime.Initialize(xpData);
        xpObj.transform.position = position;
        xpObj.SetActive(true);
    }

    public void ReturnToPool(XPDrop xpDrop, XPDropRuntime xpRuntime)
    {
        xpRuntime.gameObject.SetActive(false);
        _objPool.Return(xpDrop, xpRuntime);
    }

    public void GetXpForNextLevel(int currentLevel)
    {
        
    }

    public void RegisterAttracted(XPDropRuntime xpRuntime)
    {
        _attractedDrops.Add(xpRuntime);
    }

    public void UnregisterAttracted(XPDropRuntime xpRuntime)
    {
        _attractedDrops.Remove(xpRuntime);
    }
}
