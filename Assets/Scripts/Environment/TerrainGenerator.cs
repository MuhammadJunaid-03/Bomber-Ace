using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(-80)]
public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public float chunkSize = 100f;
    public int chunksAhead = 5;
    public int chunksBehind = 2;
    public float terrainWidth = 200f;

    [Header("Visual")]
    public Material terrainMaterial;
    public Material waterMaterial;
    public Color desertColor = new Color(0.76f, 0.70f, 0.50f);
    public Color forestColor = new Color(0.2f, 0.5f, 0.15f);
    public Color snowColor = new Color(0.9f, 0.92f, 0.95f);

    [Header("Height")]
    public float maxHeight = 8f;
    public float noiseScale = 0.02f;
    public int heightResolution = 20;

    [Header("Decorations")]
    public GameObject[] treePrefabs;
    public GameObject[] rockPrefabs;
    public GameObject[] buildingPrefabs;
    public int decorationsPerChunk = 10;

    private Transform player;
    private Dictionary<int, GameObject> activeChunks = new Dictionary<int, GameObject>();
    [HideInInspector] public float noiseSeed;

    void Start()
    {
        noiseSeed = Random.Range(0f, 10000f);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        UpdateChunks();
    }

    void Update()
    {
        if (player == null) return;
        UpdateChunks();
    }

    void UpdateChunks()
    {
        if (player == null) return;

        int playerChunk = Mathf.FloorToInt(player.position.z / chunkSize);

        // Create needed chunks
        for (int i = playerChunk - chunksBehind; i <= playerChunk + chunksAhead; i++)
        {
            if (!activeChunks.ContainsKey(i))
            {
                activeChunks[i] = CreateChunk(i);
            }
        }

        // Remove far chunks
        List<int> toRemove = new List<int>();
        foreach (var kvp in activeChunks)
        {
            if (kvp.Key < playerChunk - chunksBehind - 1 || kvp.Key > playerChunk + chunksAhead + 1)
            {
                toRemove.Add(kvp.Key);
            }
        }
        foreach (int key in toRemove)
        {
            Destroy(activeChunks[key]);
            activeChunks.Remove(key);
        }
    }

    GameObject CreateChunk(int chunkIndex)
    {
        GameObject chunk = new GameObject($"Chunk_{chunkIndex}");
        chunk.transform.parent = transform;
        chunk.transform.position = new Vector3(0f, 0f, chunkIndex * chunkSize);

        // Create terrain mesh
        GameObject terrain = CreateTerrainMesh(chunkIndex);
        terrain.transform.parent = chunk.transform;
        terrain.transform.localPosition = Vector3.zero;

        // Add decorations
        SpawnDecorations(chunk.transform, chunkIndex);

        return chunk;
    }

    GameObject CreateTerrainMesh(int chunkIndex)
    {
        GameObject terrainObj = new GameObject("Terrain");

        MeshFilter mf = terrainObj.AddComponent<MeshFilter>();
        MeshRenderer mr = terrainObj.AddComponent<MeshRenderer>();
        MeshCollider mc = terrainObj.AddComponent<MeshCollider>();

        int xVerts = heightResolution + 1;
        int zVerts = heightResolution + 1;

        Vector3[] vertices = new Vector3[xVerts * zVerts];
        int[] triangles = new int[heightResolution * heightResolution * 6];
        Vector2[] uv = new Vector2[xVerts * zVerts];

        for (int z = 0; z < zVerts; z++)
        {
            for (int x = 0; x < xVerts; x++)
            {
                float xPos = (x / (float)heightResolution - 0.5f) * terrainWidth;
                float zPos = (z / (float)heightResolution) * chunkSize + chunkIndex * chunkSize;

                float height = Mathf.PerlinNoise(
                    (xPos + noiseSeed) * noiseScale,
                    (zPos + noiseSeed) * noiseScale
                ) * maxHeight;

                int idx = z * xVerts + x;
                vertices[idx] = new Vector3(xPos, height, z / (float)heightResolution * chunkSize);
                uv[idx] = new Vector2(x / (float)heightResolution, z / (float)heightResolution);
            }
        }

        int tri = 0;
        for (int z = 0; z < heightResolution; z++)
        {
            for (int x = 0; x < heightResolution; x++)
            {
                int v = z * xVerts + x;
                triangles[tri + 0] = v;
                triangles[tri + 1] = v + xVerts;
                triangles[tri + 2] = v + 1;
                triangles[tri + 3] = v + 1;
                triangles[tri + 4] = v + xVerts;
                triangles[tri + 5] = v + xVerts + 1;
                tri += 6;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        mf.mesh = mesh;
        mc.sharedMesh = mesh;

        if (terrainMaterial != null)
            mr.material = terrainMaterial;

        terrainObj.layer = LayerMask.NameToLayer("Default");

        return terrainObj;
    }

    void SpawnDecorations(Transform parent, int chunkIndex)
    {
        Random.State prevState = Random.state;
        Random.InitState(chunkIndex * 12345 + (int)(noiseSeed * 100));

        for (int i = 0; i < decorationsPerChunk; i++)
        {
            float x = Random.Range(-terrainWidth * 0.4f, terrainWidth * 0.4f);
            float z = Random.Range(0f, chunkSize) + chunkIndex * chunkSize;

            float height = Mathf.PerlinNoise(
                (x + noiseSeed) * noiseScale,
                (z + noiseSeed) * noiseScale
            ) * maxHeight;

            Vector3 pos = new Vector3(x, height, z - chunkIndex * chunkSize);

            GameObject[] prefabs = Random.value > 0.7f ? rockPrefabs : treePrefabs;
            if (prefabs != null && prefabs.Length > 0)
            {
                GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
                if (prefab != null)
                {
                    GameObject deco = Instantiate(prefab, parent);
                    deco.transform.localPosition = pos;
                    deco.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                    float scale = Random.Range(0.8f, 1.5f);
                    deco.transform.localScale = Vector3.one * scale;
                }
            }
        }

        Random.state = prevState;
    }
}
