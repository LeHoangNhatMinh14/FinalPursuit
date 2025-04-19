using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using StarterAssets;
using UnityEditor;
using TMPro;
using UnityEngine.EventSystems;

public class EnemyCounter : MonoBehaviour
{
    public static EnemyCounter Instance;

    [Header("Scene Settings")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private SceneAsset sceneAsset;

    [Header("Perk UI")]
    [SerializeField] private GameObject perkPanel;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardPrefab;

    [Header("Perks")]
    [SerializeField] private List<Perk> allPerks;

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

    public void AddEnemy(GameObject enemy)
    {
        enemyCount++;
        Debug.Log($"[Counter] Added {enemy.name} | Total Enemies: {enemyCount}");
    }
    public void RemoveEnemy(GameObject enemy)
    {
        enemyCount--;
        Debug.Log($"[Counter] Removed {enemy.name} | Remaining Enemies: {enemyCount}");

        if (enemyCount <= 0 && !isChoosingPerk)
        {
            StartCoroutine(HandleLevelEnd());
        }
    }

    private IEnumerator HandleLevelEnd()
    {
        isChoosingPerk = true;

        // Disable player control
        var player = FindObjectOfType<FirstPersonController>();
        if (player != null)
        {
            player.GetComponent<CharacterController>().enabled = false;
            player.enabled = false;
        }

        // Show mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        yield return StartCoroutine(ShowPerkChoices());

        // Wait until a perk is selected
        yield return new WaitUntil(() => perkSelected);

        // Load next scene
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private IEnumerator ShowPerkChoices()
    {
        perkSelected = false;
        perkPanel.SetActive(true);

        // Clear old cards if needed
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        var perks = allPerks.OrderBy(x => Random.value).Take(3).ToList();

        foreach (var perk in perks)
        {
            var card = Instantiate(cardPrefab, cardContainer);
            SetupCard(card, perk);

            var canvasGroup = card.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
                StartCoroutine(AnimateCard(card)); // Play the animation
        }

        yield return null;
    }

private void SetupCard(GameObject card, Perk perk)
{
    card.transform.Find("Title").GetComponent<TMP_Text>().text = perk.perkName;
    card.transform.Find("Description").GetComponent<TMP_Text>().text = perk.description;
    card.transform.Find("Icon").GetComponent<Image>().sprite = perk.icon;

    var button = card.GetComponent<Button>();
    button.onClick.AddListener(() => SelectPerk(perk));

    // Add hover events for glow
    Image bgImage = card.GetComponent<Image>(); // or card.transform.Find("Background") if using child

    if (bgImage != null)
    {
        EventTrigger trigger = card.GetComponent<EventTrigger>();
        if (trigger == null) trigger = card.AddComponent<EventTrigger>();

        // Pointer Enter
        EventTrigger.Entry enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener((_) => {
            bgImage.color = new Color(1f, 1f, 0.6f, 1f); // Glow tint
        });

        // Pointer Exit
        EventTrigger.Entry exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener((_) => {
            bgImage.color = Color.white; // Reset to base color
        });

        trigger.triggers.Add(enter);
        trigger.triggers.Add(exit);
    }
}

    private void SelectPerk(Perk perk)
    {
        Debug.Log($"Selected perk: {perk.perkName}");

        // If it's a machine gun perk, swap the weapon
        if (perk.perkName == "Machine Gun" && weaponHolder != null && perk is MachineGunPerk mg)
        {
            weaponHolder.EquipOnlyWeapon(mg.weaponPrefab);
        }
        else
        {
            // All other perks apply their effects (e.g., health boost)
            perk.ApplyEffect();
        }

        perkSelected = true;
    }

    private IEnumerator AnimateCard(GameObject card)
    {
        RectTransform rt = card.GetComponent<RectTransform>();
        CanvasGroup cg = card.GetComponent<CanvasGroup>();

        if (rt == null || cg == null)
            yield break;

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
            elapsed += Time.unscaledDeltaTime; // In case game is paused
            yield return null;
        }

        rt.localScale = endScale;
        cg.alpha = 1;
    }
}
