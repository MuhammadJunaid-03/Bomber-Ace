using UnityEngine;

[CreateAssetMenu(fileName = "NewMission", menuName = "BomberAce/Mission Data")]
public class MissionData : ScriptableObject
{
    public string missionName = "Mission";
    public string description = "Destroy all targets";
    public int missionNumber = 1;

    [Header("Environment")]
    public EnvironmentType environmentType = EnvironmentType.Desert;
    public float terrainLength = 500f;
    public float terrainWidth = 200f;

    [Header("Targets")]
    public int tankCount = 3;
    public int truckCount = 2;
    public int aaGunCount = 1;
    public int radarCount = 0;
    public int buildingCount = 2;
    public int warshipCount = 0;

    [Header("Air Enemies")]
    public int enemyPlaneCount = 0;
    public int enemyHelicopterCount = 0;
    public bool hasHomingMissiles = false;

    [Header("Rewards")]
    public int baseCoins = 100;
    public int baseGems = 2;

    [Header("Difficulty")]
    public float enemyAccuracy = 0.3f;
    public float enemyFireRate = 1.5f;
    public float playerSpeedMultiplier = 1f;
}

public enum EnvironmentType
{
    Desert,
    Forest,
    Snow,
    Ocean,
    City
}
