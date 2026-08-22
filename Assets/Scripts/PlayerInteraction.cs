using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    public Camera mainCam;
    public float interactionDistance = 10f;


    public TextMeshProUGUI interactionText;
    public float fadeDuration = 0.3f;

    private Coroutine currentFadeCoroutine;

    void Start()
    {
        // Прогрев UI (можно оставить, не мешает)
        interactionText.alpha = 1.0f;
        interactionText.text = " ";
        Canvas.ForceUpdateCanvases();

    }

    void Update()
    {
        InteractionRay();
    }

    void InteractionRay()
    {
        Ray ray = mainCam.ViewportPointToRay(Vector3.one / 2f);
        RaycastHit hit;

        bool hitSomething = false;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            Iinteractable interactable = hit.collider.GetComponent<Iinteractable>();

            if (interactable != null)
            {
                hitSomething = true;
                interactionText.text = interactable.GetDescription();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }
            }
        }

        if (hitSomething)
            FadeTo(1f);   
        else
            FadeTo(0f);   
    }

    private void FadeTo(float targetAlpha)
    {
        if (Mathf.Approximately(interactionText.alpha, targetAlpha))
            return;

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
            currentFadeCoroutine = null;
        }

        currentFadeCoroutine = StartCoroutine(FadeAlpha(targetAlpha));
    }

    private IEnumerator FadeAlpha(float target)
    {
        float startAlpha = interactionText.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            interactionText.alpha = Mathf.Lerp(startAlpha, target, t);
            yield return null;
        }

        interactionText.alpha = target;
        currentFadeCoroutine = null;
    }
}