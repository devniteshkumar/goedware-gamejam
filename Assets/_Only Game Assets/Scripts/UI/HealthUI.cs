using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public float health;
    public float dependentHealth;
    public float maxHealth = 100f;
    public float animTime = 0.5f;

    [Header("UI References")]
    public RectTransform IndependentBar;
    public RectTransform DependentBar;
    public RectTransform Reference;

    private Coroutine animateCoroutine;

    public void SetMaxHealth(float value)
    {
        maxHealth = value;
    }

    public void SetHealth(float value)
    {
        health = Mathf.Clamp(value, 0, maxHealth);
    }

    private void Update()
    {
        if (IndependentBar == null || Reference == null)
            return;

        float clampedHealth = Mathf.Clamp(health, 0, maxHealth);
        float width = (clampedHealth / maxHealth) * Reference.sizeDelta.x;
        IndependentBar.sizeDelta = new Vector2(width, Reference.sizeDelta.y);
    }

    private void LateUpdate()
    {
        if (DependentBar == null || IndependentBar == null || Reference == null)
            return;

        if (!Mathf.Approximately(dependentHealth, health))
        {
            // Stop old animation if it's running
            if (animateCoroutine != null)
                StopCoroutine(animateCoroutine);

            animateCoroutine = StartCoroutine(AnimateWidthToReference());
            dependentHealth = health;
        }
    }

    IEnumerator AnimateWidthToReference()
    {
        float elapsed = 0f;
        float startWidth = DependentBar.sizeDelta.x;
        float targetWidth = IndependentBar.sizeDelta.x;

        while (elapsed < animTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animTime);
            float newWidth = Mathf.Lerp(startWidth, targetWidth, Mathf.SmoothStep(0f, 1f, t));
            DependentBar.sizeDelta = new Vector2(newWidth, Reference.sizeDelta.y);
            yield return null;
        }

        DependentBar.sizeDelta = new Vector2(targetWidth, Reference.sizeDelta.y);
        animateCoroutine = null;
    }
}
