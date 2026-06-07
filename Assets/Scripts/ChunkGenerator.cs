using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkGenerator : MonoBehaviour
{
    [SerializeField] private TileBase grassTile;
    [SerializeField] private RuleTile pathTile;
    [SerializeField] private List<GameObject> propPrefabs;

    [Header("Generation values")] 
    [SerializeField] private float noiseScale = 0.08f;
    [SerializeField] private float pathThreshold = 0.5f;

    public void GenerateChunk(Vector2Int coord, int size, Tilemap tilemap)
    {
        GenerateTiles(coord, size, tilemap);
        ScatterProps(coord, size);
    }

    private void GenerateTiles(Vector2Int coord, int size, Tilemap tilemap)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                var worldX = (coord.x * size + x) * noiseScale;
                var worldY = (coord.y * size + y) * noiseScale;
                
                var noise = Mathf.PerlinNoise(worldX, worldY);
                var tile = noise > pathThreshold ? pathTile : grassTile;
                
                tilemap.SetTile(new Vector3Int(coord.x * size + x, coord.y * size + y, 0), tile);
            }
        }
    }
    
    private void ScatterProps(Vector2Int coord, int size)
    {
        if (propPrefabs.Count == 0) return;
        
        var rng = new System.Random(coord.x * 1000 + coord.y);
        var propCount = rng.Next(2, 6);
        
        for (int i = 0; i < propCount; i++)
        {
            var localX = (float)rng.NextDouble() * size;
            var localY = (float)rng.NextDouble() * size;
            var worldPos = transform.position + new Vector3(localX, localY, 0);
            
            var prop = propPrefabs[rng.Next(0, propPrefabs.Count)];
            Instantiate(prop, worldPos, Quaternion.identity, transform);
        }
    }
}
