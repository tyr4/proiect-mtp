using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkGenerator : MonoBehaviour
{
    [SerializeField] private TileBase grassTile;
    [SerializeField] private RuleTile pathTile;
    [SerializeField] private List<TileBase> propPrefabs;

    [Header("Generation values")] [Range(0, 1f)] [SerializeField]
    private float noiseScale = 0.08f;

    [Range(0, 1f)] [SerializeField] private float pathThreshold = 0.5f;

    [Range(0, 10f)] [SerializeField] private float pathWidth = 1f;

    public void GenerateChunk(Vector2Int coord, int size, Tilemap groundTilemap, Tilemap objectsTilemap)
    {
        GenerateTiles(coord, size, groundTilemap);
        ScatterProps(coord, size, objectsTilemap);
    }

    private void GenerateTiles(Vector2Int coord, int size, Tilemap tilemap)
    {
        bool[,] pathMap = new bool[size, size];

        // original perlin noise
        for (int x = 0; x < size; x++)
        for (int y = 0; y < size; y++)
        {
            float worldX = (coord.x * size + x) * noiseScale;
            float worldY = (coord.y * size + y) * noiseScale;
            pathMap[x, y] = Mathf.PerlinNoise(worldX, worldY) > pathThreshold;
        }

        // connect disconnected blobs
        ConnectRegions(pathMap, size);
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                var tilePos = new Vector3Int(coord.x * size + x, coord.y * size + y, 0);
                tilemap.SetTile(tilePos, pathMap[x, y] ? pathTile : grassTile);
            }
        }
    }

    private void ConnectRegions(bool[,] map, int size)
    {
        List<List<Vector2Int>> regions = GetAllRegions(map, size);
        if (regions.Count <= 1) return;

        regions.Sort((a, b) => b.Count.CompareTo(a.Count));

        for (int i = 1; i < regions.Count; i++)
        {
            Vector2Int a = Vector2Int.zero, b = Vector2Int.zero;
            float best = float.MaxValue;

            foreach (var pa in regions[i])
            foreach (var pb in regions[0])
            {
                float d = Vector2Int.Distance(pa, pb);
                if (d < best)
                {
                    best = d;
                    a = pa;
                    b = pb;
                }
            }

            CarveTunnel(map, a, b);
        }
    }

    private void CarveTunnel(bool[,] map, Vector2Int from, Vector2Int to, int radius = 2)
    {
        Vector2 pos = from;
        Vector2 target = to;
        float bias = 0.55f;
        float drift = 1.4f;
        int maxSteps = (int)(Vector2Int.Distance(from, to) * 3);

        for (int step = 0; step < maxSteps; step++)
        {
            Vector2 toTarget = (target - pos).normalized;
            Vector2 perp = new Vector2(-toTarget.y, toTarget.x);
            Vector2 move = (toTarget * bias + perp * (Random.Range(-drift, drift) * (1f - bias))).normalized;

            pos += move;
            PaintCircle(map, Vector2Int.RoundToInt(pos), radius);

            if (Vector2.Distance(pos, target) < radius) break;
        }
    }

    private void PaintCircle(bool[,] map, Vector2Int center, int radius)
    {
        for (int rx = -radius; rx <= radius; rx++)
            for (int ry = -radius; ry <= radius; ry++)
            {
                if (rx * rx + ry * ry > radius * radius) continue;
                int nx = Mathf.Clamp(center.x + rx, 0, map.GetLength(0) - 1);
                int ny = Mathf.Clamp(center.y + ry, 0, map.GetLength(1) - 1);
                map[nx, ny] = true;
            }
    }

    private List<List<Vector2Int>> GetAllRegions(bool[,] map, int size)
    {
        bool[,] visited = new bool[size, size];
        var regions = new List<List<Vector2Int>>();

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                if (map[x, y] && !visited[x, y])
                    regions.Add(FloodFill(map, visited, x, y, size));

        return regions;
    }

    List<Vector2Int> FloodFill(bool[,] map, bool[,] visited, int startX, int startY, int size)
    {
        var region = new List<Vector2Int>();
        var queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));
        visited[startX, startY] = true;

        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        while (queue.Count > 0)
        {
            var cell = queue.Dequeue();
            region.Add(cell);

            for (int d = 0; d < 4; d++)
            {
                int nx = cell.x + dx[d];
                int ny = cell.y + dy[d];
                if (nx < 0 || ny < 0 || nx >= size || ny >= size) continue;
                if (visited[nx, ny] || !map[nx, ny]) continue;
                visited[nx, ny] = true;
                queue.Enqueue(new Vector2Int(nx, ny));
            }
        }

        return region;
    }

    private void ScatterProps(Vector2Int coord, int size, Tilemap tilemap)
    {
        if (propPrefabs.Count == 0) return;

        // var rng = new System.Random(coord.x * 1000 + coord.y);
        var rng = new System.Random();
        var propCount = rng.Next(2, 6);

        for (int i = 0; i < propCount; i++)
        {
            var localX = rng.Next(0, size);
            var localY = rng.Next(0, size);
            var baseCell = new Vector3Int(coord.x * size + localX, coord.y * size + localY, 0);

            var prop = propPrefabs[rng.Next(0, propPrefabs.Count)];
            
            tilemap.SetTile(baseCell, prop);
        }
    }
}