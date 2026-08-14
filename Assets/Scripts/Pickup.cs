using UnityEngine;

public class Pickup : MonoBehaviour, Iinteractable
{
    [Header("Item")]
    public string itemName;

    [Header("Audio")]
    public AudioSource pickupAudioSource;

    [Header("Pulse Settings")]
    public float pulseSpeed = 1.5f;      
    public float minIntensity = 0f;
    public float maxIntensity = 0.6f;

    private Material material;           
    private Color baseEmissionColor;     
    private bool isPulsing = true;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;

            if (!material.IsKeywordEnabled("_EMISSION"))
                material.EnableKeyword("_EMISSION");

            baseEmissionColor = material.GetColor("_EmissionColor");

        }
        else
        {
            Debug.LogWarning("Pickup: No Renderer found on " + gameObject.name);
        }
    }

    void Update()
    {
        if (!isPulsing || material == null) return;


        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, t);


        Color finalColor = baseEmissionColor * intensity;
        material.SetColor("_EmissionColor", finalColor);


        // material.SetFloat("_EmissiveIntensity", intensity);
    }

    public string GetDescription()
    {
        return $"Press E to pick up {itemName}";
    }

    public void Interact()
    {
        isPulsing = false;

        if (pickupAudioSource != null)
            pickupAudioSource.Play();

        InventorySystem.Instance.AddItem(itemName);

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null) renderer.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        float delay = pickupAudioSource != null ? pickupAudioSource.clip.length : 0.3f;
        Destroy(gameObject, delay + 0.1f);
    }
}