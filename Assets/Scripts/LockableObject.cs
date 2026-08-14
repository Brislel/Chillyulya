using UnityEngine;

public class LockableObject : MonoBehaviour, Iinteractable
{
    [Header("Object Settings")]
    public string objectType = "door";          // что это: дверь, €щик, шкаф
    public string actionOpen = "open";          // глагол дл€ открыти€
    public string actionClose = "close";        // глагол дл€ закрыти€
    public string actionUnlock = "unlock";      // глагол дл€ отпирани€

    [Header("State")]
    public bool isOpen;
    public bool isUnlocked = false;

    [Header("Key Requirement (optional)")]
    public string requiredItemName = "";

    [Header("Animation")]
    public Animator objectAnimator;
    public string openBoolParam = "isOpen";     // параметр-булево в аниматоре

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] soundCollection = new AudioClip[2]; // [0] Ц открыть, [1] Ц закрыть
    public AudioClip unlockSound;

    void Start()
    {
        if (string.IsNullOrEmpty(requiredItemName))
            isUnlocked = true;

        if (objectAnimator != null)
            objectAnimator.SetBool(openBoolParam, isOpen);

        if (isOpen)
            isUnlocked = true;
    }

    public string GetDescription()
    {
        if (isOpen)
            return $"Press E to {actionClose} the {objectType}.";

        if (isUnlocked)
            return $"Press E to {actionOpen} the {objectType}.";

        if (!string.IsNullOrEmpty(requiredItemName) && InventorySystem.Instance.HasItem(requiredItemName))
            return $"Press E to {actionUnlock} with {requiredItemName}.";

        if (!string.IsNullOrEmpty(requiredItemName))
            return $"You need {requiredItemName}.";

        return $"Press E to {actionOpen} the {objectType}.";
    }

    public void Interact()
    {
        if (isOpen)
        {
            SetObjectState(false);
            return;
        }

        if (isUnlocked)
        {
            SetObjectState(true);
            return;
        }

        if (!string.IsNullOrEmpty(requiredItemName) && InventorySystem.Instance.HasItem(requiredItemName))
        {
            InventorySystem.Instance.RemoveItem(requiredItemName);
            isUnlocked = true;

            Debug.Log($"{objectType} unlocked with {requiredItemName}.");

            if (audioSource != null && unlockSound != null)
                audioSource.PlayOneShot(unlockSound);
        }
        else
        {
            if (!string.IsNullOrEmpty(requiredItemName))
                Debug.Log($"You don't have {requiredItemName}.");
            else
                Debug.Log("Object is locked without key? This shouldn't happen.");
        }
    }

    private void SetObjectState(bool open)
    {
        isOpen = open;
        if (objectAnimator != null)
            objectAnimator.SetBool(openBoolParam, isOpen);

        if (audioSource != null && soundCollection.Length >= 2)
        {
            int index = isOpen ? 0 : 1;
            audioSource.clip = soundCollection[index];
            audioSource.Play();
        }
    }
}