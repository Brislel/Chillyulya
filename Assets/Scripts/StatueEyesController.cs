using UnityEngine;
using System.Collections;

public class StatueEyesController : MonoBehaviour
{
    [Header("Eyes Transforms")]
    public Transform[] eyes; // два глаза

    [Header("Timer Settings")]
    public float minInterval = 3f;
    public float maxInterval = 10f;

    [Header("Angle Options")]
    public float[] possibleAngles = new float[] { -25f, 0f, 25f };

    [Header("Rotation Speed")]
    public float rotationSpeed = 30f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip moveSound;

    private Quaternion[] initialRotations; // сохран€ем начальный поворот каждого глаза
    private Quaternion[] targetRotations;
    private float previousAngle = float.NaN;

    void Start()
    {
        if (eyes == null || eyes.Length < 2)
        {
            Debug.LogError("StatueEyesController: Assign two eye transforms.");
            return;
        }

        initialRotations = new Quaternion[eyes.Length];
        targetRotations = new Quaternion[eyes.Length];

        for (int i = 0; i < eyes.Length; i++)
        {
            if (eyes[i] != null)
            {
                initialRotations[i] = eyes[i].localRotation;
                targetRotations[i] = initialRotations[i];
            }
        }

        StartCoroutine(RandomLookRoutine());
    }

    IEnumerator RandomLookRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            // ¬ыбираем угол, отличный от предыдущего
            float newAngle;
            if (possibleAngles.Length == 0)
            {
                newAngle = 0f;
            }
            else if (possibleAngles.Length == 1 || float.IsNaN(previousAngle))
            {
                newAngle = possibleAngles[Random.Range(0, possibleAngles.Length)];
            }
            else
            {
                do
                {
                    newAngle = possibleAngles[Random.Range(0, possibleAngles.Length)];
                } while (Mathf.Approximately(newAngle, previousAngle));
            }

            previousAngle = newAngle;

            // ѕримен€ем вращение к каждому глазу относительно его начального поворота
            for (int i = 0; i < eyes.Length; i++)
            {
                if (eyes[i] == null) continue;
                // ”множаем начальный поворот на поворот вокруг локальной Y на newAngle
                targetRotations[i] = initialRotations[i] * Quaternion.Euler(0, newAngle, 0);
            }

            // «вук
            if (audioSource != null && moveSound != null)
            {
                audioSource.PlayOneShot(moveSound);
            }
        }
    }

    void Update()
    {
        if (eyes == null) return;

        for (int i = 0; i < eyes.Length; i++)
        {
            if (eyes[i] == null) continue;
            eyes[i].localRotation = Quaternion.RotateTowards(
                eyes[i].localRotation,
                targetRotations[i],
                rotationSpeed * Time.deltaTime
            );
        }
    }
}