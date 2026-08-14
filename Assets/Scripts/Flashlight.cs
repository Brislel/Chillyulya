using UnityEngine;

public class Flashlight : MonoBehaviour
{

    private Light flashLight;
    public AudioSource flashlightSound;
    private void Awake()
    {
        flashLight = GetComponent<Light>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashLight.enabled = !flashLight.enabled;
            flashlightSound.Play();
        }
    }
}
