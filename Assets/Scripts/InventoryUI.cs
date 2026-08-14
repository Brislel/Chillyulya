using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI itemListText;       

    [Header("Settings")]
    public float fadeDuration = 0.3f;
    public float autoHideDelay = 3f;

    private Coroutine currentFadeCoroutine;
    private Coroutine autoHideCoroutine;

    private void Start()
    {

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);         
    }


    public void UpdateList(List<string> items)
    {
        if (items.Count == 0)
        {
            itemListText.text = "Inventory is empty.";
        }
        else
        {

            string display = "";
            for (int i = 0; i < items.Count; i++)
            {
                display += $"{i + 1}. {items[i]}\n";
            }
            itemListText.text = display;
        }
    }

    public void Show()
    {

        if (gameObject.activeSelf)
        {
            ResetAutoHideTimer();
            return;
        }


        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);
        currentFadeCoroutine = StartCoroutine(FadeCanvas(0f, 1f));


        ResetAutoHideTimer();
    }


    public void Hide()
    {
        if (!gameObject.activeSelf)
            return;

        if (autoHideCoroutine != null)
            StopCoroutine(autoHideCoroutine);
        autoHideCoroutine = null;

        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);
        currentFadeCoroutine = StartCoroutine(FadeAndDisable(1f, 0f));
    }

    // Сбросить таймер автозакрытия
    private void ResetAutoHideTimer()
    {
        if (autoHideCoroutine != null)
            StopCoroutine(autoHideCoroutine);
        autoHideCoroutine = StartCoroutine(AutoHide());
    }

    private IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(autoHideDelay);
        Hide();
    }

    // Анимация изменения альфа 
    private IEnumerator FadeCanvas(float from, float to)
    {
        float elapsed = 0f;
        canvasGroup.alpha = from;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        canvasGroup.alpha = to;
        currentFadeCoroutine = null;
    }

    // Анимация с последующим отключением объекта
    private IEnumerator FadeAndDisable(float from, float to)
    {
        yield return FadeCanvas(from, to);
        gameObject.SetActive(false);
        currentFadeCoroutine = null;
    }
}