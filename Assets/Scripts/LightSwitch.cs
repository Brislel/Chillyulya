using UnityEngine;

public class LightSwitch : MonoBehaviour, Iinteractable
{
    public Light[] lights;   
    public Animator switcherAnimator;
    public AudioSource switcherAudioSource;
    public bool isOn;
    void Start()
    {
        SetLightsState(isOn);
    }

    public string GetDescription()
    {
        if (isOn) return "Press E to turn off the light.";
        return "Press E to turn on the light";
    }

    public void Interact()
    {
        isOn = !isOn;
        SetLightsState(isOn);
        switcherAudioSource.Play();

        if (switcherAnimator != null)
            switcherAnimator.SetBool("isOn", isOn);
    }

    private void SetLightsState(bool state)
    {
        if (lights == null) return;

        foreach (Light light in lights)
        {
            if (light != null)          
                light.enabled = state;
        }
    }
}
