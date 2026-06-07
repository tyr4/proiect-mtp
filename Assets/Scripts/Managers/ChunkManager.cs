using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private Tilemap sharedTilemap;
    [SerializeField] private GameObject chunkPrefab;
    [SerializeField] private int chunkSize = 16;
    [SerializeField] private int viewDistance = 2;
    [SerializeField] private float tileWorldSize = 0.25f;
    
    [SerializeField] private Transform playerTransform;
    
    private Dictionary<Vector2Int, GameObject> _activeChunks = new();
    private Vector2Int _lastPlayerChunk;
    
    public static ChunkManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        var currentChunk = WorldToChunkCoord(playerTransform.position);
        if (currentChunk == _lastPlayerChunk) return;
        
        _lastPlayerChunk = currentChunk;
        UpdateChunks(currentChunk);
    }

    private void UpdateChunks(Vector2Int currentChunk)
    {
        // spawn missing chunks
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                var coord = new Vector2Int(currentChunk.x + x, currentChunk.y + y);

                if (!_activeChunks.ContainsKey(coord))
                    SpawnChunk(coord);
            }
        }
        
        // despawn far chunks
        var toRemove = new List<Vector2Int>();
        foreach (var coord in _activeChunks.Keys)
        {
            if (Mathf.Abs(coord.x - currentChunk.x) > viewDistance + 1 || Mathf.Abs(coord.y - currentChunk.y) > viewDistance + 1)
            {
                toRemove.Add(coord);
            }
        }

        foreach (var coord in toRemove)
        {
            DespawnChunk(coord);
        }
    }

    private void SpawnChunk(Vector2Int coord)
    {
        float chunkWorldSize = chunkSize * tileWorldSize;
        var worldPos = new Vector3(coord.x * chunkWorldSize, coord.y * chunkWorldSize, 0);
        var chunk = Instantiate(chunkPrefab, worldPos, Quaternion.identity);
        
        chunk.GetComponent<ChunkGenerator>().GenerateChunk(coord, chunkSize, sharedTilemap);
        _activeChunks[coord] = chunk;
    }

    private void DespawnChunk(Vector2Int coord)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                sharedTilemap.SetTile(new Vector3Int(coord.x * chunkSize + x, coord.y * chunkSize + y, 0), null);
            }
        }
        
        Destroy(_activeChunks[coord]);
        _activeChunks.Remove(coord);
    }

    private Vector2Int WorldToChunkCoord(Vector3 worldPos)
    {
        float chunkWorldSize = chunkSize * tileWorldSize;
        return new Vector2Int(
            Mathf.FloorToInt(worldPos.x / chunkWorldSize),
            Mathf.FloorToInt(worldPos.y / chunkWorldSize));
    }
}
