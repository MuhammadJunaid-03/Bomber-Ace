using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionSelectUI : MonoBehaviour
{
    [Header("Mission List")]
    public Transform missionListContent;
    public GameObject missionButtonPrefab;

    [Header("Mission Info")]
    public TextMeshProUGUI missionNameText;
    public TextMeshProUGUI missionDescText;
    public TextMeshProUGUI missionTargetsText;
    public TextMeshProUGUI rewardText;
    public Button startButton;
    public Button backButton;

    private int selectedMission = 0;
    private int highestUnlocked;

    void OnEnable()
    {
        highestUnlocked = PlayerPrefs.GetInt("HighestMission", 0);
        PopulateMissionList();
        SelectMission(0);

        if (startButton != null)
            startButton.onClick.AddListener(StartSelectedMission);
    }

    void PopulateMissionList()
    {
        if (missionListContent == null || missionButtonPrefab == null) return;

        // Clear existing
        foreach (Transform child in missionListContent)
            Destroy(child.gameObject);

        if (GameManager.Instance == null || GameManager.Instance.missions == null) return;

        for (int i = 0; i < GameManager.Instance.missions.Length; i++)
        {
            var mission = GameManager.Instance.missions[i];
            GameObject btn = Instantiate(missionButtonPrefab, missionListContent);

            TextMeshProUGUI text = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = $"Mission {mission.missionNumber}";

            Button button = btn.GetComponent<Button>();
            int index = i;
            if (button != null)
            {
                button.onClick.AddListener(() => SelectMission(index));
                button.interactable = i <= highestUnlocked;
            }

            // Lock icon for locked missions
            if (i > highestUnlocked)
            {
                var colors = button.colors;
                colors.normalColor = new Color(0.3f, 0.3f, 0.3f);
                button.colors = colors;
            }
        }
    }

    void SelectMission(int index)
    {
        selectedMission = index;

        if (GameManager.Instance == null || GameManager.Instance.missions == null) return;
        if (index >= GameManager.Instance.missions.Length) return;

        var mission = GameManager.Instance.missions[index];

        if (missionNameText != null) missionNameText.text = mission.missionName;
        if (missionDescText != null) missionDescText.text = mission.description;
        if (missionTargetsText != null)
        {
            string targets = "";
            if (mission.tankCount > 0) targets += $"Tanks: {mission.tankCount}\n";
            if (mission.truckCount > 0) targets += $"Trucks: {mission.truckCount}\n";
            if (mission.aaGunCount > 0) targets += $"AA Guns: {mission.aaGunCount}\n";
            if (mission.radarCount > 0) targets += $"Radars: {mission.radarCount}\n";
            if (mission.buildingCount > 0) targets += $"Buildings: {mission.buildingCount}\n";
            if (mission.warshipCount > 0) targets += $"Warships: {mission.warshipCount}\n";
            if (mission.enemyPlaneCount > 0) targets += $"Enemy Planes: {mission.enemyPlaneCount}\n";
            missionTargetsText.text = targets;
        }

        if (rewardText != null)
            rewardText.text = $"Coins: {mission.baseCoins}+  Gems: {mission.baseGems}";

        if (startButton != null)
            startButton.interactable = index <= highestUnlocked;
    }

    void StartSelectedMission()
    {
        PlayerPrefs.SetInt("LastMission", selectedMission);
        GameManager.Instance?.LoadMission(selectedMission);
    }
}
