using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using StarterAssets;

public class EnemyCounter : MonoBehaviour
{
    public static EnemyCounter Instance;

    [Header("Scene Settings")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private bool usePortal = true;
    private GameObject portalObject;

    [Header("Perk UI")]
    [SerializeField] private GameObject perkPanel;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardPrefab;

    [Header("Perks")]
    [SerializeField] private List<Perk> allPerks;

    private int playerLives = 3;
    private int enemyCount = 0;
    private bool isChoosingPerk = false;
    private bool perkSelected = false;
    private WeaponHolder weaponHolder;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        weaponHolder = FindObjectOfType<WeaponHolder>();
        perkPanel.SetActive(false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshSceneReferences();
    }

    private void RefreshSceneReferences()
    {
        // 🔄 Get level settings
        LevelSettings levelSettings = FindObjectOfType<LevelSettings>();
        if (levelSettings != null)
        {
            nextSceneName = levelSettings.nextSceneName;
            usePortal = levelSettings.usePortal;
        }

        // 🔄 Get portal
        portalObject = GameObject.FindGameObjectWithTag("Portal");
        if (portalObject != null)
            portalObject.SetActive(false);

        // ✅ Only find UI if not already assigned (e.g. from Inspector)
        if (perkPanel == null)
            perkPanel = GameObject.Find("PerkPanel");
        
        if (perkPanel != null)
        {
            if (cardContainer == null)
                cardContainer = perkPanel.transform.Find("CardContainer");
            
            perkPanel.SetActive(false); // Hide initially
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        weaponHolder = FindObjectOfType<WeaponHolder>();
    }

    public void AddEnemy(GameObject enemy)
    {
        enemyCount++;
        Debug.Log($"[EnemyCounter] Added {enemy.name} | Total Enemies: {enemyCount}");
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemyCount--;
        Debug.Log($"[EnemyCounter] Removed {enemy.name} | Remaining Enemies: {enemyCount}");

        if (enemyCount <= 0 && !isChoosingPerk)
        {
            StartCoroutine(HandleLevelEnd());
        }
    }

    private IEnumerator HandleLevelEnd()
    {
        yield return new WaitForSeconds(0.5f);

        if (usePortal && portalObject != null)
        {
            portalObject.SetActive(true);
            Debug.Log("[EnemyCounter] All enemies defeated. Portal activated!");
        }
        else
        {
            Debug.Log("[EnemyCounter] No portal. Loading next scene directly...");
            yield return new WaitForSeconds(1f);
            LoadNextLevel();
        }
    }

    public void ShowPerksAfterPortal()
    {
        StartCoroutine(StartPerkSelection());
    }

    private IEnumerator StartPerkSelection()
    {
        isChoosingPerk = true;

        // Lock player
        var player = FindObjectOfType<FirstPersonController>();
        if (player != null)
        {
            player.GetComponent<CharacterController>().enabled = false;
            player.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yield return StartCoroutine(ShowPerkChoices());
        yield return new WaitUntil(() => perkSelected);

        LoadNextLevel();
    }

    private IEnumerator ShowPerkChoices()
    {
        perkSelected = false;
        perkPanel.SetActive(true);

        // Clear old cards
        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        // Choose random perks
        var perks = allPerks.OrderBy(x => Random.value).Take(3).ToList();

        foreach (var perk in perks)
        {
            var card = Instantiate(cardPrefab, cardContainer);
            SetupCard(card, perk);

            var canvasGroup = card.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
                StartCoroutine(AnimateCard(card));
        }

        yield return null;
    }

    private void SetupCard(GameObject card, Perk perk)
    {
        var image = card.GetComponent<Image>();
        if (image != null && perk.cardImage != null)
        {
            image.sprite = perk.cardImage;
            image.preserveAspect = true;
        }

        var button = card.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => SelectPerk(perk));
    }

    private void SelectPerk(Perk perk)
    {
        Debug.Log($"[Perk] Selected: {perk.perkName}");
        perk.ApplyEffect();

        perkSelected = true;
        perkPanel.SetActive(false);

        // Unlock player controls
        var player = FindObjectOfType<FirstPersonController>();
        if (player != null)
        {
            player.GetComponent<CharacterController>().enabled = true;
            player.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("[EnemyCounter] No next scene name assigned!");
        }
    }

    private IEnumerator AnimateCard(GameObject card)
    {
        RectTransform rt = card.GetComponent<RectTransform>();
        CanvasGroup cg = card.GetComponent<CanvasGroup>();

        if (rt == null || cg == null) yield break;

        float duration = 0.3f;
        float elapsed = 0f;

        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;

        cg.alpha = 0;
        rt.localScale = startScale;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            rt.localScale = Vector3.Lerp(startScale, endScale, t);
            cg.alpha = t;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        rt.localScale = endScale;
        cg.alpha = 1;
    }

    public void OnPlayerRespawn(int remainingLives)
    {
        playerLives = remainingLives;
        Debug.Log($"[EnemyCounter] Player respawned. Lives left: {playerLives}");
    }

    public void OnPlayerGameOver()
    {
        Debug.Log("[EnemyCounter] Player has lost all lives. Game Over!");
    }
}
