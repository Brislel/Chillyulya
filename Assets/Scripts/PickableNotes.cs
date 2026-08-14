using Player;
using UnityEngine;

public class PickableNote : MonoBehaviour, Iinteractable
{
    [SerializeField] private GameObject _notePageUI;

    public AudioSource pageAudioSource;
    private bool _isOpen = false;

    public string GetDescription()
    {
        return _isOpen ? "Press E to close note" : "Press E to read note";
    }

    public void Interact()
    {
        _isOpen = !_isOpen;
        _notePageUI.SetActive(_isOpen);
        pageAudioSource.Play();
        PlayerMovement.IsInputBlocked = _isOpen; 
    }
}