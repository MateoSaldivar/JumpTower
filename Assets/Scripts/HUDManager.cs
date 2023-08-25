using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {
    public Image healthImagePrefab;
    public Transform healthBarParent;
    public float spacing = 10f;

    private List<Image> healthImages = new List<Image>();

    private void Start() {
        PlayerSystem.instance.OnHealthUpdated += UpdateHealthUI;
        InitializeHealthUI(PlayerSystem.instance.Health);
    }

    private void OnDestroy() {
        PlayerSystem.instance.OnHealthUpdated -= UpdateHealthUI;
    }

    private void InitializeHealthUI(int initialHealth) {

        for (int i = 0; i < initialHealth; i++) {
            Image healthImage = Instantiate(healthImagePrefab, healthBarParent);
            healthImages.Add(healthImage);
        }

        UpdateHealthUI(initialHealth);
    }

    private void UpdateHealthUI(int currentHealth) {
        currentHealth = Mathf.Max(currentHealth, 0); // Ensure health is non-negative

        for (int i = 0; i < healthImages.Count; i++) {
            healthImages[i].gameObject.SetActive(i >= healthImages.Count - currentHealth);
        }

        ArrangeHealthImages();
    }

    private void ArrangeHealthImages() {
        for (int i = 0; i < healthImages.Count; i++) {
            float yOffset = (i - (healthImages.Count - 1) / 2.0f) * (healthImagePrefab.rectTransform.sizeDelta.y + spacing);
            healthImages[i].rectTransform.anchoredPosition = new Vector2(0f, yOffset);
        }
    }
}
