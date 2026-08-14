using UnityEngine;

public class FootstepController : MonoBehaviour
{
    public AudioSource audioSource;          
    public AudioClip[] footstepSounds;      
    public void PlayFootstep()
    {
        if (footstepSounds.Length == 0 || audioSource == null)
            return;

        int index = Random.Range(0, footstepSounds.Length);
        audioSource.clip = footstepSounds[index];
        audioSource.Play();
    }
}