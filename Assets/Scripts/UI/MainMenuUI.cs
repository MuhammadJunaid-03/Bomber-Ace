using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject missionSelectPanel;
    public GameObject upgradePanel;
    public GameObject planeSelectPanel;
    public GameObject settingsPanel;

    [Header("Main Buttons")]
    public Button playButton;
    public Button missionsButton;
    public Button upgradeButton;
    public Button planesButton;
    public Button settingsButton;

    [Header("Currency Display")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI gemText;

    [Header("Plane Display")]
    public TextMeshProUGUI planeNameText;
    public Image planeImage;

    void Start()
    {
        ShowPanel(mainPanel);

        if (playButton != null) playButton.onClick.AddListener(OnPlay);
        if (missionsButton != null) missionsButton.onClick.AddListener(() => ShowPanel(missionSelectPanel));
        if (upgradeButton != null) upgradeButton.onClick.AddListener(() => ShowPanel(upgradePanel));
        if (planesButton != null) planesButton.onClick.AddListener(() => ShowPanel(planeSelectPanel));
        if (settingsButton != null) settingsButton.onClick.AddListener(() => ShowPanel(settingsPanel));

        UpdateDisplay();
    }

    void Update()
    {
        UpdateCurrency();
    }

    void UpdateCurrency()
    {
        if (CurrencyManager.Instance == null) return;
        if (coinText != null) coinText.text = CurrencyManager.Instance.Coins.ToString();
        if (gemText != null) gemText.text = CurrencyManager.Instance.Gems.ToString();
    }

    void UpdateDisplay()
    {
        var plane = UpgradeManager.Instance?.CurrentPlane;
        if (plane != null)
        {
            if (planeNameText != null) planeNameText.text = plane.planeName;
            if (planeImage != null && plane.planeIcon != null) planeImage.sprite = plane.planeIcon;
        }
    }

    void OnPlay()
    {
        int lastMission = PlayerPrefs.GetInt("LastMission", 0);
        GameManager.Instance?.LoadMission(lastMission);
    }

    public void ShowPanel(GameObject panel)
    {
        mainPanel?.SetActive(false);
        missionSelectPanel?.SetActive(false);
        upgradePanel?.SetActive(false);
        planeSelectPanel?.SetActive(false);
        settingsPanel?.SetActive(false);

        panel?.SetActive(true);
    }

    public void BackToMain()
    {
        ShowPanel(mainPanel);
    }
}
