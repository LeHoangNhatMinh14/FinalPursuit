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
        private int playerLives = 3;
        [SerializeField] private bool usePortal = true;
        [SerializeField] private GameObject portalObject;

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
            
            // Hide portal if not used
            if (portalObject != null && !usePortal)
            {
                portalObject.SetActive(false);
            }
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
            if (usePortal)
            {
                // Portal level completion
                if (portalObject != null)
                {
                    portalObject.SetActive(true);
                    Debug.Log("[EnemyCounter] All enemies defeated. Portal is now active.");
                }
            }
            else
            {
                // Direct level completion
                Debug.Log("[EnemyCounter] All enemies defeated. Proceeding to next level.");
                yield return new WaitForSeconds(1f); // Brief delay before transition
                LoadNextLevel();
            }
            
            yield break;
        }

        private void LoadNextLevel()
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogWarning("[EnemyCounter] No next scene name specified!");
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // This method is called every time a new scene is loaded
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RefreshSceneReferences();
        }

        private void RefreshSceneReferences()
        {
            // Find new portal reference in the scene
            portalObject = GameObject.FindGameObjectWithTag("Portal"); // Make sure your portal has "Portal" tag
            if (portalObject != null)
            {
                portalObject.SetActive(!usePortal); // Hide if not used
            }

            // Find the LevelSettings object in the scene to get next level info
            LevelSettings levelSettings = FindObjectOfType<LevelSettings>();
            if (levelSettings != null)
            {
                nextSceneName = levelSettings.nextSceneName;
                usePortal = levelSettings.usePortal;
                
                // Update portal active state based on new settings
                if (portalObject != null)
                {
                    portalObject.SetActive(usePortal);
                }
            }
            else
            {
                Debug.LogWarning("No LevelSettings found in the scene!");
            }

            // Refresh perk UI references if needed
            perkPanel = GameObject.Find("PerkPanel"); // Or use a tag
            cardContainer = perkPanel?.transform.Find("CardContainer");
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

            public void ShowPerksAfterPortal()
        {
            StartCoroutine(StartPerkSelection());
        }

        private IEnumerator StartPerkSelection()
        {
            isChoosingPerk = true;

            // Lock player controls
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

            // ✅ Now load the scene
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }

        private void SetupCard(GameObject card, Perk perk)
        {
            var image = card.GetComponent<Image>(); // assuming your card prefab root has the image
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
            Debug.Log($"Selected perk: {perk.perkName}");

            // Apply the perk effect
            perk.ApplyEffect();

            perkSelected = true;
            perkPanel.SetActive(false);
            
            // Restore player controls
            var player = FindObjectOfType<FirstPersonController>();
            if (player != null)
            {
                player.GetComponent<CharacterController>().enabled = true;
                player.enabled = true;
            }
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // For non-portal levels, we might want to skip the delay
            float transitionDelay = usePortal ? 1.5f : 0.5f;
            StartCoroutine(DelayedSceneTransition(transitionDelay));
        }

        private IEnumerator DelayedSceneTransition(float delay)
        {
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(nextSceneName);
        }

        private IEnumerator LoadNextSceneAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(nextSceneName);
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

        public void OnPlayerRespawn(int remainingLives)
        {
            playerLives = remainingLives;
            Debug.Log($"[EnemyCounter] Player respawned. Lives left: {playerLives}");
        }

        public void OnPlayerGameOver()
        {
            Debug.Log("[EnemyCounter] Player has lost the game!");
            // Show Game Over UI, reload scene, etc.
        }
    }
