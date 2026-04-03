using UnityEditor;
using UnityEngine;

public class PrefabMaterialSetup
{
    [MenuItem("BomberAce/Apply Materials To Prefabs")]
    static void ApplyMaterials()
    {
        var tankGray = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Tank_Gray.mat");
        var truckKhaki = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Truck_Khaki.mat");
        var bombDark = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Bomb_Dark.mat");
        var radarMetal = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Radar_Metal.mat");
        var buildingBrown = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Building_Brown.mat");
        var enemyRed = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Enemy_Red.mat");
        var explosionOrange = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Explosion_Orange.mat");

        // Tank
        ApplyMaterialToPrefabChildren("Assets/Prefabs/Enemies/Tank.prefab", new System.Collections.Generic.Dictionary<string, Material> {
            {"Body", tankGray}, {"Turret", tankGray}, {"Barrel", radarMetal}, {"Track_L", bombDark}, {"Track_R", bombDark}
        });

        // Truck
        ApplyMaterialToPrefabChildren("Assets/Prefabs/Enemies/Truck.prefab", new System.Collections.Generic.Dictionary<string, Material> {
            {"Cabin", truckKhaki}, {"Cargo", tankGray}, {"Wheel_FL", bombDark}, {"Wheel_FR", bombDark}, {"Wheel_BL", bombDark}, {"Wheel_BR", bombDark}
        });

        // AA Gun
        ApplyMaterialToPrefabChildren("Assets/Prefabs/Enemies/AAGun.prefab", new System.Collections.Generic.Dictionary<string, Material> {
            {"Base", tankGray}, {"Turret", radarMetal}, {"Barrel_L", bombDark}, {"Barrel_R", bombDark}, {"FirePoint", null}
        });

        // Building
        ApplyMaterialToPrefabChildren("Assets/Prefabs/Enemies/Building.prefab", new System.Collections.Generic.Dictionary<string, Material> {
            {"Structure", buildingBrown}, {"Roof", tankGray}
        });

        // Radar
        ApplyMaterialToPrefabChildren("Assets/Prefabs/Enemies/Radar.prefab", new System.Collections.Generic.Dictionary<string, Material> {
            {"Base", radarMetal}, {"Dish", radarMetal}
        });

        // Enemy Plane
        ApplyMaterialToPrefabChildren("Assets/Prefabs/Enemies/EnemyPlane.prefab", new System.Collections.Generic.Dictionary<string, Material> {
            {"Fuselage", enemyRed}, {"Wings", enemyRed}, {"TailWing", enemyRed}, {"TailFin", enemyRed}, {"FirePoint", null}
        });

        // Enemy Helicopter
        ApplyMaterialToPrefabChildren("Assets/Prefabs/Enemies/EnemyHelicopter.prefab", new System.Collections.Generic.Dictionary<string, Material> {
            {"Body", enemyRed}, {"Tail", enemyRed}, {"RotorBlade", bombDark}, {"Skid_L", bombDark}, {"Skid_R", bombDark}, {"GunPoint", null}
        });

        // Explosion
        ApplyMaterialToPrefabRoot("Assets/Prefabs/Effects/Explosion.prefab", explosionOrange);

        Debug.Log("BomberAce: All prefab materials applied successfully!");
    }

    static void ApplyMaterialToPrefabChildren(string prefabPath, System.Collections.Generic.Dictionary<string, Material> childMaterials)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null) { Debug.LogWarning($"Prefab not found: {prefabPath}"); return; }

        string assetPath = AssetDatabase.GetAssetPath(prefab);
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        foreach (var kvp in childMaterials)
        {
            if (kvp.Value == null) continue;
            Transform child = instance.transform.Find(kvp.Key);
            if (child != null)
            {
                var renderer = child.GetComponent<MeshRenderer>();
                if (renderer != null)
                    renderer.sharedMaterial = kvp.Value;
            }
        }

        PrefabUtility.SaveAsPrefabAsset(instance, assetPath);
        Object.DestroyImmediate(instance);
    }

    static void ApplyMaterialToPrefabRoot(string prefabPath, Material mat)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null) return;

        string assetPath = AssetDatabase.GetAssetPath(prefab);
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        var renderer = instance.GetComponent<MeshRenderer>();
        if (renderer != null) renderer.sharedMaterial = mat;

        PrefabUtility.SaveAsPrefabAsset(instance, assetPath);
        Object.DestroyImmediate(instance);
    }
}
