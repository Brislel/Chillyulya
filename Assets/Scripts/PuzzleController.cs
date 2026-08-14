using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Player; // если ваш PlayerMovement в пространстве имён Player

public class PuzzleController : MonoBehaviour, Iinteractable
{
    public enum DragMode
    {
        FixedDepth,
        PlaneOffset
    }

    [Header("Puzzle Settings")]
    public PuzzleSlot[] slots;
    public Transform handPivot;
    public Transform puzzleViewPoint;
    public float cameraMoveSpeed = 5f;
    public DragMode dragMode = DragMode.PlaneOffset;
    public float planeOffset = 0f;
    public LayerMask slotLayerMask;

    [Header("Completion Effects")]
    public Image flashImage;          // белый Image на весь экран
    public float flashDuration = 0.3f;
    public float flashHoldTime = 0.5f;

    [Header("Chest Animation")]
    public Animator chestAnimator;
    public string openTriggerName = "Open";

    [Header("Camera Shake")]
    public MonoBehaviour cameraShakeComponent; // например, скрипт тряски камеры

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip insertSound;
    public AudioClip completeSound;

    [Header("References")]
    public Transform cameraHolder;

    private bool isPuzzleActive = false;
    private bool isCompleted = false;

    private Vector3 holderStartPos;
    private Quaternion holderStartRot;
    private Coroutine cameraCoroutine;

    private PuzzleSlot pickedSlot = null;
    private PuzzleItem pickedItem = null;

    void Start()
    {
        if (slots == null || slots.Length == 0)
            slots = GetComponentsInChildren<PuzzleSlot>(true);

        if (cameraHolder == null)
        {
            Camera cam = Camera.main;
            if (cam != null && cam.transform.parent != null)
                cameraHolder = cam.transform.parent;
            else
                Debug.LogError("CameraHolder not found! Assign it in inspector.");
        }

        handPivot.gameObject.SetActive(false);
        if (flashImage != null)
            flashImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isPuzzleActive || isCompleted) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitPuzzle();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }

        if (pickedItem != null)
        {
            MovePickedItem();
        }
    }

    void EnterPuzzle()
    {
        isPuzzleActive = true;
        PlayerMovement.IsInputBlocked = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Отключаем тряску камеры
        if (cameraShakeComponent != null)
            cameraShakeComponent.enabled = false;

        holderStartPos = cameraHolder.position;
        holderStartRot = cameraHolder.rotation;

        if (cameraCoroutine != null) StopCoroutine(cameraCoroutine);
        cameraCoroutine = StartCoroutine(MoveCameraHolderToView());

        handPivot.gameObject.SetActive(false);
        pickedItem = null;
        pickedSlot = null;
    }

    void ExitPuzzle()
    {
        isPuzzleActive = false;
        PlayerMovement.IsInputBlocked = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Включаем тряску камеры обратно
        if (cameraShakeComponent != null)
            cameraShakeComponent.enabled = true;

        if (pickedItem != null)
        {
            if (pickedSlot != null)
                PutItemInSlot(pickedSlot, pickedItem.gameObject);
            else
                Destroy(pickedItem.gameObject);
            pickedItem = null;
            pickedSlot = null;
            handPivot.gameObject.SetActive(false);
        }

        if (cameraCoroutine != null) StopCoroutine(cameraCoroutine);
        cameraCoroutine = null;

        cameraHolder.position = holderStartPos;
        cameraHolder.rotation = holderStartRot;
    }

    IEnumerator MoveCameraHolderToView()
    {
        float t = 0;
        Vector3 startPos = cameraHolder.position;
        Quaternion startRot = cameraHolder.rotation;
        while (t < 1)
        {
            t += Time.deltaTime * cameraMoveSpeed;
            t = Mathf.Clamp01(t);
            cameraHolder.position = Vector3.Lerp(startPos, puzzleViewPoint.position, t);
            cameraHolder.rotation = Quaternion.Slerp(startRot, puzzleViewPoint.rotation, t);
            yield return null;
        }
        cameraCoroutine = null;
    }

    void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, slotLayerMask))
        {
            PuzzleSlot slotHit = hit.collider.GetComponent<PuzzleSlot>();
            if (slotHit != null)
            {
                if (pickedItem != null)
                {
                    PlaceItemInSlot(slotHit);
                    return;
                }
                else
                {
                    if (slotHit.currentItem != null)
                    {
                        PickupFromSlot(slotHit);
                        return;
                    }
                }
            }
        }

        if (Physics.Raycast(ray, out hit))
        {
            PuzzleItem item = hit.collider.GetComponent<PuzzleItem>();
            if (item != null && pickedItem == null)
            {
                PuzzleSlot slot = GetSlotContaining(item.gameObject);
                if (slot != null)
                {
                    PickupFromSlot(slot);
                    return;
                }
            }
        }
    }

    void PickupFromSlot(PuzzleSlot slot)
    {
        if (slot.currentItem == null || pickedItem != null) return;

        pickedSlot = slot;
        pickedItem = slot.currentItem.GetComponent<PuzzleItem>();
        slot.currentItem.transform.SetParent(handPivot);
        slot.currentItem.transform.localPosition = Vector3.zero;
        slot.currentItem.transform.localRotation = Quaternion.identity;
        handPivot.gameObject.SetActive(true);
        slot.currentItem = null;
    }

    void PlaceItemInSlot(PuzzleSlot targetSlot)
    {
        if (pickedItem == null) return;

        if (targetSlot.currentItem != null)
        {
            GameObject targetItemObj = targetSlot.currentItem;
            PuzzleItem targetItem = targetItemObj.GetComponent<PuzzleItem>();

            targetSlot.currentItem = null;
            targetItemObj.transform.SetParent(handPivot);
            targetItemObj.transform.localPosition = Vector3.zero;
            targetItemObj.transform.localRotation = Quaternion.identity;

            PutItemInSlot(targetSlot, pickedItem.gameObject);

            pickedItem = targetItem;
            pickedSlot = targetSlot;

            if (audioSource != null && insertSound != null)
                audioSource.PlayOneShot(insertSound);
        }
        else
        {
            PutItemInSlot(targetSlot, pickedItem.gameObject);
            pickedItem = null;
            pickedSlot = null;
            handPivot.gameObject.SetActive(false);

            if (audioSource != null && insertSound != null)
                audioSource.PlayOneShot(insertSound);

            if (CheckCompletion())
            {
                StartCoroutine(CompletePuzzle());
            }
        }
    }

    void PutItemInSlot(PuzzleSlot slot, GameObject itemObj)
    {
        slot.currentItem = itemObj;
        itemObj.transform.SetParent(slot.transform);
        itemObj.transform.localPosition = Vector3.zero;
        itemObj.transform.localRotation = Quaternion.identity;
    }

    PuzzleSlot GetSlotContaining(GameObject item)
    {
        foreach (var slot in slots)
            if (slot.currentItem == item) return slot;
        return null;
    }

    void MovePickedItem()
    {
        if (pickedItem == null) return;
        Camera cam = Camera.main;
        if (cam == null) return;

        if (dragMode == DragMode.FixedDepth)
        {
            Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                planeOffset
            ));
            handPivot.position = mouseWorld;
        }
        else
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Vector3 planePoint = transform.position + cam.transform.forward * planeOffset;
            Plane plane = new Plane(-cam.transform.forward, planePoint);
            float enter;
            if (plane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                handPivot.position = hitPoint;
            }
        }
    }

    bool CheckCompletion()
    {
        foreach (var slot in slots)
        {
            if (string.IsNullOrEmpty(slot.requiredItemName))
                continue;

            if (slot.currentItem == null)
                return false;

            PuzzleItem item = slot.currentItem.GetComponent<PuzzleItem>();
            if (item == null || item.itemName != slot.requiredItemName)
                return false;
        }
        return true;
    }

    IEnumerator CompletePuzzle()
    {
        isCompleted = true;

        // Звук завершения
        if (audioSource != null && completeSound != null)
            audioSource.PlayOneShot(completeSound);

        // Белая вспышка
        if (flashImage != null)
        {
            flashImage.gameObject.SetActive(true);
            flashImage.color = new Color(1f, 1f, 1f, 0f);

            float t = 0;
            while (t < flashDuration)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, t / flashDuration);
                flashImage.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }
            flashImage.color = new Color(1f, 1f, 1f, 1f);

            yield return new WaitForSeconds(flashHoldTime);

            t = 0;
            while (t < flashDuration)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, t / flashDuration);
                flashImage.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }
            flashImage.color = new Color(1f, 1f, 1f, 0f);
            flashImage.gameObject.SetActive(false);
        }

        // Запуск анимации открытия ящика
        if (chestAnimator != null && !string.IsNullOrEmpty(openTriggerName))
        {
            chestAnimator.SetTrigger(openTriggerName);
            // Ждём, пока анимация "Open" полностью проиграется
            yield return StartCoroutine(WaitForAnimation("Open"));
        }
        else
        {
            // Если аниматор не назначен – небольшая пауза
            yield return new WaitForSeconds(1f);
        }

        // Выход из головоломки
        ExitPuzzle();
    }

    // Корутина, которая ждёт завершения анимации по имени состояния
    IEnumerator WaitForAnimation(string stateName)
    {
        // Ждём, пока аниматор перейдёт в состояние с именем stateName
        while (!chestAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            yield return null;

        // Теперь ждём, пока нормализованное время анимации не станет >= 1 (конец)
        while (chestAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;
    }

    // ---------- Интерфейс Iinteractable ----------
    public string GetDescription()
    {
        if (isCompleted) return "Puzzle is already solved.";
        return "Press E to inspect puzzle.";
    }

    public void Interact()
    {
        if (isCompleted || isPuzzleActive) return;
        EnterPuzzle();
    }
}