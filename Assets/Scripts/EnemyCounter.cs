using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class EnemyCounter : MonoBehaviour
{
    public static EnemyCounter Instance;
    
    [Header("Scene Transition")]
    #if UNITY_EDITOR
    [SerializeField] private SceneAsset nextSceneAsset; // Editor-only reference
    #endif
    [SerializeField] private string nextSceneName; // Runtime name

    [Header("Perk System")]
    [SerializeField] private GameObject perkSelectionPanel;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private float cardRevealDelay = 0.3f;
    [SerializeField] private float cardRevealDuration = 0.5f;
    [SerializeField] private List<Perk> allPossiblePerks;

    private int currentEnemies = 0;
    private bool isShowingPerks = false;

        void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            #if UNITY_EDITOR
            if (nextSceneAsset != null)
            {
                nextSceneName = nextSceneAsset.name;
            }
            #endif
            
            // Ensure panel is hidden at start
            if(perkSelectionPanel != null)
                perkSelectionPanel.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddEnemy(GameObject enemy)
    {
        currentEnemies++;
        Debug.Log($"Enemy added: {enemy.name}. Total: {currentEnemies}");
    }

    public void RemoveEnemy(GameObject enemy)
    {
        currentEnemies--;
        Debug.Log($"Enemy removed: {enemy.name}. Remaining: {currentEnemies}");

        if (currentEnemies <= 0 && !isShowingPerks)
        {
            StartCoroutine(LevelCompleteSequence());
        }
    }

    IEnumerator LevelCompleteSequence()
    {
        isShowingPerks = true;
        
        // Disable player input
        var player = FindObjectOfType<FirstPersonController>();
        if (player != null)
        {
            player.GetComponent<CharacterController>().enabled = false;
            player.enabled = false;
        }

        // Show perk selection
        yield return StartCoroutine(ShowPerkSelection());

        // Load next scene after perk selection
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("No next scene assigned!");
        }
    }

    IEnumerator ShowPerkSelection()
    {
        // Activate the panel
        perkSelectionPanel.SetActive(true);

        // Get all card buttons (assuming you have exactly 3)
        Button[] cardButtons = cardContainer.GetComponentsInChildren<Button>(true);
        
        // Get 3 random perks
        List<Perk> selectedPerks = GetRandomPerks(3);

        // Setup each card
        for (int i = 0; i < cardButtons.Length && i < selectedPerks.Count; i++)
        {
            GameObject cardObj = cardButtons[i].gameObject;
            cardObj.SetActive(false); // Start hidden
            SetupCardUI(cardObj, selectedPerks[i]);
        }

        // Animate card reveal one by one
        for (int i = 0; i < cardButtons.Length; i++)
        {
            cardButtons[i].gameObject.SetActive(true);
            yield return StartCoroutine(AnimateCardReveal(cardButtons[i].gameObject));
            yield return new WaitForSeconds(cardRevealDelay);
        }
    }

    IEnumerator AnimateCardReveal(GameObject card)
    {
        // Initial state (scaled down)
        card.transform.localScale = Vector3.zero;
        
        // Animate scale up
        float timer = 0f;
        while (timer < cardRevealDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / cardRevealDuration);
            card.transform.localScale = Vector3.one * progress;
            yield return null;
        }

        // Final state
        card.transform.localScale = Vector3.one;
    }

    void SetupCardUI(GameObject cardObj, Perk perk)
    {
        // Get UI components - adjust these based on your actual UI structure
        Image icon = cardObj.transform.Find("Icon").GetComponent<Image>();
        Text title = cardObj.transform.Find("Title").GetComponent<Text>();
        Text description = cardObj.transform.Find("Description").GetComponent<Text>();
        Button button = cardObj.GetComponent<Button>();

        // Set perk info
        icon.sprite = perk.icon;
        title.text = perk.perkName;
        description.text = perk.description;

        // Setup button click
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnPerkSelected(perk));
    }

    List<Perk> GetRandomPerks(int count)
    {
        // Make sure we don't try to get more perks than available
        count = Mathf.Min(count, allPossiblePerks.Count);
        
        // Get random perks without duplicates
        return allPossiblePerks.OrderBy(x => Random.value).Take(count).ToList();
    }

    void OnPerkSelected(Perk selectedPerk)
    {
        Debug.Log($"Perk selected: {selectedPerk.perkName}");
        
        // Apply the perk effect
        selectedPerk.ApplyEffect();

        // Mark selection as made
        PlayerPrefs.SetInt("PerkSelected", 1);
    }

    bool PerkSelectionMade()
    {
        return PlayerPrefs.GetInt("PerkSelected", 0) == 1;
    }
}

[System.Serializable]
public class Perk
{
    public string perkName;
    public Sprite icon;
    [TextArea] public string description;

    public virtual void ApplyEffect()
    {
        // Base implementation - override this for specific perks
        Debug.Log($"Applying perk: {perkName}");
        
        // Example: You might want to store selected perks for the player
        // PlayerStats.Instance.AddPerk(this);
    }
}

// Example perk implementations
[System.Serializable]
public class HealthIncreasePerk : Perk
{
    public float healthIncreaseAmount = 25f;

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        // Example: FindObjectOfType<PlayerHealth>().IncreaseMaxHealth(healthIncreaseAmount);
    }
}

[System.Serializable]
public class DamageBoostPerk : Perk
{
    public float damageMultiplier = 1.2f;

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        // Example: PlayerStats.Instance.damageMultiplier *= damageMultiplier;
    }
}

[System.Serializable]
public class SpeedBoostPerk : Perk
{
    public float speedIncrease = 1.5f;

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        // Example: FindObjectOfType<FirstPersonController>().MoveSpeed += speedIncrease;
    }
}