using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    // Auto-built UI references
    private Canvas canvas;
    private Slider healthBar;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI bombText;
    private TextMeshProUGUI ammoText;
    private TextMeshProUGUI targetText;
    private TextMeshProUGUI speedText;
    private TextMeshProUGUI altText;
    private Image damageOverlay;
    private GameObject missileWarningObj;

    // Panels
    private GameObject gameOverPanel;
    private GameObject missionCompletePanel;
    private GameObject pausePanel;
    private TextMeshProUGUI goScoreText;
    private TextMeshProUGUI mcScoreText;
    private TextMeshProUGUI mcDestructionText;
    private TextMeshProUGUI mcCoinsText;

    // Player refs
    private PlaneController playerController;
    private PlaneHealth playerHealth;
    private BombDropper playerBomber;
    private PlaneGun playerGun;

    private float damageFlash;

    void Start()
    {
        BuildUI();
        FindPlayer();
        HideAllPanels();

        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += OnStateChanged;
    }

    void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            playerController = p.GetComponent<PlaneController>();
            playerHealth = p.GetComponent<PlaneHealth>();
            playerBomber = p.GetComponent<BombDropper>();
            playerGun = p.GetComponent<PlaneGun>();

            if (playerHealth != null)
                playerHealth.OnHealthChanged += (cur, max) => {
                    if (healthBar != null) healthBar.value = cur / max;
                    damageFlash = 0.3f;
                };
        }
    }

    void Update()
    {
        UpdateHUD();
        UpdateDamageOverlay();

        // Pause input
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameManager.Instance != null)
            {
                if (GameManager.Instance.CurrentState == GameManager.GameState.Playing)
                    GameManager.Instance.PauseGame();
                else if (GameManager.Instance.CurrentState == GameManager.GameState.Paused)
                    GameManager.Instance.ResumeGame();
            }
        }
    }

    void UpdateHUD()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (scoreText != null) scoreText.text = $"SCORE: {gm.Score}";
        if (targetText != null) targetText.text = $"TARGETS: {gm.DestroyedTargets}/{gm.TotalTargets}";

        if (playerBomber != null && bombText != null)
            bombText.text = $"BOMBS: {playerBomber.CurrentBombs}";

        if (playerGun != null && ammoText != null)
            ammoText.text = playerGun.IsReloading ? "RELOAD..." : $"AMMO: {playerGun.CurrentAmmo}";

        if (playerController != null)
        {
            if (speedText != null) speedText.text = $"{Mathf.RoundToInt(playerController.CurrentSpeed * 3.6f)} km/h";
            if (altText != null) altText.text = $"ALT: {Mathf.RoundToInt(playerController.transform.position.y)}m";
        }
    }

    void UpdateDamageOverlay()
    {
        if (damageOverlay == null) return;
        if (damageFlash > 0f)
        {
            damageFlash -= Time.deltaTime;
            Color c = damageOverlay.color;
            c.a = Mathf.Clamp01(damageFlash / 0.3f) * 0.4f;
            damageOverlay.color = c;
        }
        else
        {
            float hp = playerHealth != null ? playerHealth.HealthPercentage : 1f;
            Color c = damageOverlay.color;
            c.a = hp < 0.3f ? (1f - hp / 0.3f) * 0.2f : 0f;
            damageOverlay.color = c;
        }
    }

    void OnStateChanged(GameManager.GameState state)
    {
        HideAllPanels();
        switch (state)
        {
            case GameManager.GameState.GameOver:
                gameOverPanel?.SetActive(true);
                if (goScoreText != null) goScoreText.text = $"Score: {GameManager.Instance.Score}";
                break;
            case GameManager.GameState.MissionComplete:
                missionCompletePanel?.SetActive(true);
                if (mcScoreText != null) mcScoreText.text = $"Score: {GameManager.Instance.Score}";
                if (mcDestructionText != null) mcDestructionText.text = $"Destruction: {GameManager.Instance.DestructionPercentage:F0}%";
                int coins = GameManager.Instance.Score;
                if (mcCoinsText != null) mcCoinsText.text = $"+{coins} Coins";
                break;
            case GameManager.GameState.Paused:
                pausePanel?.SetActive(true);
                break;
        }
    }

    void HideAllPanels()
    {
        gameOverPanel?.SetActive(false);
        missionCompletePanel?.SetActive(false);
        pausePanel?.SetActive(false);
    }

    // ========== BUILD UI PROGRAMMATICALLY ==========
    void BuildUI()
    {
        // Canvas
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        gameObject.AddComponent<GraphicRaycaster>();

        // Damage overlay
        damageOverlay = CreateImage(transform, "DamageOverlay", Color.clear);
        damageOverlay.rectTransform.anchorMin = Vector2.zero;
        damageOverlay.rectTransform.anchorMax = Vector2.one;
        damageOverlay.rectTransform.sizeDelta = Vector2.zero;
        damageOverlay.color = new Color(1, 0, 0, 0);
        damageOverlay.raycastTarget = false;

        // Top bar
        scoreText = CreateText(transform, "ScoreText", "SCORE: 0", 28, TextAlignmentOptions.Top,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -15), new Vector2(400, 40));

        // Health bar background
        var healthBG = CreateImage(transform, "HealthBG", new Color(0.2f, 0.2f, 0.2f, 0.8f));
        SetAnchored(healthBG.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(130, -20), new Vector2(200, 20));

        // Health bar fill
        var healthFillObj = new GameObject("HealthFill", typeof(RectTransform), typeof(Image));
        healthFillObj.transform.SetParent(healthBG.transform, false);
        healthBar = healthBG.gameObject.AddComponent<Slider>();
        healthBar.fillRect = healthFillObj.GetComponent<RectTransform>();
        var fillImg = healthFillObj.GetComponent<Image>();
        fillImg.color = new Color(0.2f, 0.85f, 0.2f);
        healthBar.fillRect.anchorMin = Vector2.zero;
        healthBar.fillRect.anchorMax = Vector2.one;
        healthBar.fillRect.sizeDelta = Vector2.zero;
        healthBar.interactable = false;
        healthBar.value = 1f;
        // Remove slider handle
        healthBar.handleRect = null;
        var nav = healthBar.navigation;
        nav.mode = Navigation.Mode.None;
        healthBar.navigation = nav;

        CreateText(transform, "HPLabel", "HP", 16, TextAlignmentOptions.Left,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -15), new Vector2(50, 30));

        // Bottom left - weapons
        bombText = CreateText(transform, "BombText", "BOMBS: 20", 20, TextAlignmentOptions.BottomLeft,
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(20, 45), new Vector2(250, 30));

        ammoText = CreateText(transform, "AmmoText", "AMMO: 200", 20, TextAlignmentOptions.BottomLeft,
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(20, 15), new Vector2(250, 30));

        // Bottom right - targets
        targetText = CreateText(transform, "TargetText", "TARGETS: 0/0", 20, TextAlignmentOptions.BottomRight,
            new Vector2(1, 0), new Vector2(1, 0), new Vector2(-20, 15), new Vector2(300, 30));

        // Left side - flight info
        speedText = CreateText(transform, "SpeedText", "144 km/h", 16, TextAlignmentOptions.Left,
            new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(15, 20), new Vector2(150, 25));

        altText = CreateText(transform, "AltText", "ALT: 50m", 16, TextAlignmentOptions.Left,
            new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(15, -5), new Vector2(150, 25));

        // Missile warning
        missileWarningObj = new GameObject("MissileWarning", typeof(RectTransform));
        missileWarningObj.transform.SetParent(transform, false);
        var warnBg = missileWarningObj.AddComponent<Image>();
        warnBg.color = new Color(0.8f, 0, 0, 0.7f);
        SetAnchored(warnBg.rectTransform, new Vector2(0.5f, 0.65f), new Vector2(0.5f, 0.65f), Vector2.zero, new Vector2(350, 45));
        CreateText(missileWarningObj.transform, "WarnText", "WARNING: MISSILE!", 22, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(340, 40));
        missileWarningObj.SetActive(false);

        // Crosshair
        var crosshair = CreateImage(transform, "Crosshair", new Color(1, 1, 1, 0.5f));
        SetAnchored(crosshair.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(24, 24));
        crosshair.raycastTarget = false;

        // ====== PANELS ======
        gameOverPanel = BuildPanel("MISSION FAILED", new Color(0.9f, 0.2f, 0.2f), out goScoreText,
            ("RETRY", () => GameManager.Instance?.RestartMission()),
            ("MAIN MENU", () => GameManager.Instance?.GoToMainMenu()));

        missionCompletePanel = BuildPanel("MISSION COMPLETE!", new Color(0.2f, 0.9f, 0.2f), out mcScoreText,
            ("NEXT MISSION", () => NextMission()),
            ("MAIN MENU", () => GameManager.Instance?.GoToMainMenu()));
        mcDestructionText = CreateText(missionCompletePanel.transform.GetChild(0), "Destruction", "Destruction: 100%", 20,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -30), new Vector2(300, 30));
        mcCoinsText = CreateText(missionCompletePanel.transform.GetChild(0), "Coins", "+500 Coins", 22,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -60), new Vector2(300, 30));
        mcCoinsText.color = new Color(1f, 0.84f, 0f);

        pausePanel = BuildPanel("PAUSED", Color.white, out _,
            ("RESUME", () => GameManager.Instance?.ResumeGame()),
            ("RETRY", () => GameManager.Instance?.RestartMission()),
            ("MAIN MENU", () => GameManager.Instance?.GoToMainMenu()));
    }

    GameObject BuildPanel(string title, Color titleColor, out TextMeshProUGUI scoreLabel,
        params (string text, System.Action action)[] buttons)
    {
        var panel = new GameObject(title + "_Panel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(transform, false);
        var panelImg = panel.GetComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.7f);
        var prt = panel.GetComponent<RectTransform>();
        prt.anchorMin = Vector2.zero;
        prt.anchorMax = Vector2.one;
        prt.sizeDelta = Vector2.zero;

        // Box
        var box = new GameObject("Box", typeof(RectTransform), typeof(Image));
        box.transform.SetParent(panel.transform, false);
        box.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.18f, 0.95f);
        SetAnchored(box.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(400, 350));

        var titleTxt = CreateText(box.transform, "Title", title, 32, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 120), new Vector2(380, 50));
        titleTxt.color = titleColor;

        scoreLabel = CreateText(box.transform, "Score", "Score: 0", 22, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 70), new Vector2(300, 30));

        float yOff = -20f;
        foreach (var btn in buttons)
        {
            CreateButton(box.transform, btn.text, btn.action,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, yOff), new Vector2(250, 50));
            yOff -= 60f;
        }

        panel.SetActive(false);
        return panel;
    }

    void NextMission()
    {
        if (GameManager.Instance != null)
        {
            int next = GameManager.Instance.currentMissionIndex + 1;
            if (GameManager.Instance.missions != null && next < GameManager.Instance.missions.Length)
                GameManager.Instance.LoadMission(next);
            else
                GameManager.Instance.RestartMission();
        }
    }

    // ====== UI HELPERS ======
    TextMeshProUGUI CreateText(Transform parent, string name, string text, int size, TextAlignmentOptions align,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 sizeDelta)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.alignment = align;
        tmp.color = Color.white;
        tmp.raycastTarget = false;
        tmp.enableAutoSizing = false;

        SetAnchored(go.GetComponent<RectTransform>(), anchorMin, anchorMax, pos, sizeDelta);
        return tmp;
    }

    Image CreateImage(Transform parent, string name, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.color = color;
        return img;
    }

    void CreateButton(Transform parent, string text, System.Action onClick, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(text + "_Btn", typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = new Color(0.2f, 0.45f, 0.8f);
        SetAnchored(go.GetComponent<RectTransform>(), anchorMin, anchorMax, pos, size);

        var btn = go.GetComponent<Button>();
        btn.onClick.AddListener(() => onClick?.Invoke());

        var colors = btn.colors;
        colors.highlightedColor = new Color(0.3f, 0.6f, 1f);
        colors.pressedColor = new Color(0.15f, 0.3f, 0.6f);
        btn.colors = colors;

        var label = CreateText(go.transform, "Label", text, 20, TextAlignmentOptions.Center,
            new Vector2(0, 0), new Vector2(1, 1), Vector2.zero, Vector2.zero);
        label.rectTransform.anchorMin = Vector2.zero;
        label.rectTransform.anchorMax = Vector2.one;
        label.rectTransform.sizeDelta = Vector2.zero;
    }

    void SetAnchored(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= OnStateChanged;
    }
}
