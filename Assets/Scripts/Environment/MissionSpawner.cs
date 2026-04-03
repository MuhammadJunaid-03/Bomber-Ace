using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(50)]
public class MissionSpawner : MonoBehaviour
{
    [Header("Target Prefabs")]
    public GameObject tankPrefab;
    public GameObject truckPrefab;
    public GameObject aaGunPrefab;
    public GameObject radarPrefab;
    public GameObject buildingPrefab;
    public GameObject warshipPrefab;

    [Header("Air Enemy Prefabs")]
    public GameObject enemyPlanePrefab;
    public GameObject enemyHelicopterPrefab;

    [Header("Spawn Settings")]
    public float spawnAreaLength = 400f;
    public float spawnAreaWidth = 80f;
    public float groundLevel = 0f;

    [Header("Convoy Settings")]
    public bool spawnAsConvoys = true;
    public int convoySize = 3;
    public float convoySpacing = 15f;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
        Debug.Log("MissionSpawner.Start() called");
        SpawnMission();
    }

    public void SpawnMission()
    {
        ClearMission();

        MissionData mission = null;
        if (GameManager.Instance != null && GameManager.Instance.missions != null)
        {
            int idx = GameManager.Instance.currentMissionIndex;
            if (idx >= 0 && idx < GameManager.Instance.missions.Length)
                mission = GameManager.Instance.missions[idx];
        }

        if (mission != null)
            SpawnFromMissionData(mission);
        else
            SpawnDefaultMission();

        Debug.Log($"MissionSpawner: Spawned {spawnedObjects.Count} objects");
    }

    void SpawnFromMissionData(MissionData data)
    {
        SpawnGroundTargets(tankPrefab, data.tankCount, "Tank");
        SpawnGroundTargets(truckPrefab, data.truckCount, "Truck");
        SpawnGroundTargets(aaGunPrefab, data.aaGunCount, "AAGun");
        SpawnGroundTargets(radarPrefab, data.radarCount, "Radar");
        SpawnGroundTargets(buildingPrefab, data.buildingCount, "Building");
        SpawnGroundTargets(warshipPrefab, data.warshipCount, "Warship");
        SpawnAirEnemies(enemyPlanePrefab, data.enemyPlaneCount);
        SpawnAirEnemies(enemyHelicopterPrefab, data.enemyHelicopterCount);
    }

    void SpawnDefaultMission()
    {
        SpawnGroundTargets(tankPrefab, 5, "Tank");
        SpawnGroundTargets(truckPrefab, 3, "Truck");
        SpawnGroundTargets(aaGunPrefab, 2, "AAGun");
        SpawnGroundTargets(buildingPrefab, 3, "Building");
        SpawnAirEnemies(enemyPlanePrefab, 2);
    }

    void SpawnGroundTargets(GameObject prefab, int count, string typeName)
    {
        if (prefab == null || count <= 0) return;

        if (spawnAsConvoys && (typeName == "Tank" || typeName == "Truck"))
        {
            int convoys = Mathf.CeilToInt(count / (float)convoySize);
            int remaining = count;

            for (int c = 0; c < convoys; c++)
            {
                int thisConvoySize = Mathf.Min(convoySize, remaining);
                Vector3 convoyCenter = GetRandomGroundPosition();

                for (int i = 0; i < thisConvoySize; i++)
                {
                    Vector3 pos = convoyCenter + Vector3.forward * (i * convoySpacing);
                    pos.x += Random.Range(-3f, 3f);
                    SpawnTarget(prefab, pos, typeName);
                    remaining--;
                }
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 pos = GetRandomGroundPosition();
                SpawnTarget(prefab, pos, typeName);
            }
        }
    }

    void SpawnTarget(GameObject prefab, Vector3 position, string typeName)
    {
        // Match terrain height using same perlin noise as TerrainGenerator
        var terrainGen = FindAnyObjectByType<TerrainGenerator>();
        float noiseScale = terrainGen != null ? terrainGen.noiseScale : 0.02f;
        float maxHeight = terrainGen != null ? terrainGen.maxHeight : 8f;
        float noiseSeed = terrainGen != null ? terrainGen.noiseSeed : 0f;

        position.y = Mathf.PerlinNoise(
            (position.x + noiseSeed) * noiseScale,
            (position.z + noiseSeed) * noiseScale
        ) * maxHeight + 0.5f;

        GameObject obj = Instantiate(prefab, position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
        obj.name = $"{typeName}_{spawnedObjects.Count}";
        spawnedObjects.Add(obj);
        Debug.Log($"Spawned {typeName} at {position}");
    }

    void SpawnAirEnemies(GameObject prefab, int count)
    {
        if (prefab == null || count <= 0) return;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-spawnAreaWidth * 0.5f, spawnAreaWidth * 0.5f),
                Random.Range(40f, 80f),
                Random.Range(50f, spawnAreaLength)
            );

            GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
            obj.name = $"EnemyAir_{spawnedObjects.Count}";
            spawnedObjects.Add(obj);
        }
    }

    Vector3 GetRandomGroundPosition()
    {
        return new Vector3(
            Random.Range(-spawnAreaWidth * 0.5f, spawnAreaWidth * 0.5f),
            groundLevel,
            Random.Range(50f, spawnAreaLength)
        );
    }

    void ClearMission()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedObjects.Clear();
    }
}
