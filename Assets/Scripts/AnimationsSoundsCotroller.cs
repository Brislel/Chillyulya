using UnityEngine;

public class AnimationsSoundsCotroller : MonoBehaviour
{
    public AudioSource audioSource;          
    public AudioClip[] animationSounds;      
    public void PlayFootstep()
    {
        if (animationSounds.Length == 0 || audioSource == null)
            return;

        int index = Random.Range(0, animationSounds.Length);
        audioSource.clip = animationSounds[index];
        audioSource.Play();
    }
}