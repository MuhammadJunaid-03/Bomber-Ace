using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUI : MonoBehaviour
{
    [Header("Stat Bars")]
    public Slider engineBar;
    public Slider armorBar;
    public Slider wingBar;
    public Slider bombBar;
    public Slider gunBar;

    [Header("Level Texts")]
    public TextMeshProUGUI engineLevelText;
    public TextMeshProUGUI armorLevelText;
    public TextMeshProUGUI wingLevelText;
    public TextMeshProUGUI bombLevelText;
    public TextMeshProUGUI gunLevelText;

    [Header("Cost Texts")]
    public TextMeshProUGUI engineCostText;
    public TextMeshProUGUI armorCostText;
    public TextMeshProUGUI wingCostText;
    public TextMeshProUGUI bombCostText;
    public TextMeshProUGUI gunCostText;

    [Header("Buttons")]
    public Button upgradeEngineBtn;
    public Button upgradeArmorBtn;
    public Button upgradeWingBtn;
    public Button upgradeBombBtn;
    public Button upgradeGunBtn;
    public Button backButton;

    [Header("Stats Display")]
    public TextMeshProUGUI speedValueText;
    public TextMeshProUGUI healthValueText;
    public TextMeshProUGUI damageValueText;
    public TextMeshProUGUI planeNameText;

    void Start()
    {
        if (upgradeEngineBtn != null) upgradeEngineBtn.onClick.AddListener(OnUpgradeEngine);
        if (upgradeArmorBtn != null) upgradeArmorBtn.onClick.AddListener(OnUpgradeArmor);
        if (upgradeWingBtn != null) upgradeWingBtn.onClick.AddListener(OnUpgradeWings);
        if (upgradeBombBtn != null) upgradeBombBtn.onClick.AddListener(OnUpgradeBombs);
        if (upgradeGunBtn != null) upgradeGunBtn.onClick.AddListener(OnUpgradeGuns);

        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.OnUpgradeChanged += RefreshUI;

        RefreshUI();
    }

    void OnUpgradeEngine() { UpgradeManager.Instance?.UpgradeEngine(); }
    void OnUpgradeArmor() { UpgradeManager.Instance?.UpgradeArmor(); }
    void OnUpgradeWings() { UpgradeManager.Instance?.UpgradeWings(); }
    void OnUpgradeBombs() { UpgradeManager.Instance?.UpgradeBombs(); }
    void OnUpgradeGuns() { UpgradeManager.Instance?.UpgradeGuns(); }

    void RefreshUI()
    {
        var plane = UpgradeManager.Instance?.CurrentPlane;
        if (plane == null) return;

        UpdateBar(engineBar, engineLevelText, engineCostText, plane.engineLevel, plane.engineUpgradeCosts);
        UpdateBar(armorBar, armorLevelText, armorCostText, plane.armorLevel, plane.armorUpgradeCosts);
        UpdateBar(wingBar, wingLevelText, wingCostText, plane.wingLevel, plane.wingUpgradeCosts);
        UpdateBar(bombBar, bombLevelText, bombCostText, plane.bombLevel, plane.bombUpgradeCosts);
        UpdateBar(gunBar, gunLevelText, gunCostText, plane.gunLevel, plane.gunUpgradeCosts);

        if (speedValueText != null) speedValueText.text = $"{plane.GetSpeed():F0}";
        if (healthValueText != null) healthValueText.text = $"{plane.GetHealth():F0}";
        if (damageValueText != null) damageValueText.text = $"{plane.GetBombDamage():F0}";
        if (planeNameText != null) planeNameText.text = plane.planeName;
    }

    void UpdateBar(Slider bar, TextMeshProUGUI levelText, TextMeshProUGUI costText, int level, int[] costs)
    {
        if (bar != null) bar.value = level / 5f;
        if (levelText != null) levelText.text = $"Lv.{level}";
        if (costText != null)
        {
            if (level >= 5)
                costText.text = "MAX";
            else if (costs != null && level < costs.Length)
                costText.text = $"{costs[level]}";
        }
    }

    void OnDestroy()
    {
        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.OnUpgradeChanged -= RefreshUI;
    }
}
