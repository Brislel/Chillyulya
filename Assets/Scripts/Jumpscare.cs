using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Jumpscare : MonoBehaviour
{
    public GameObject jumpscareAsset;
    public AudioSource jumpscareSound;
    public bool jumpscareIsActivated = false;
    void Start()
    {
        jumpscareAsset.SetActive(false);


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && jumpscareIsActivated == false)
        {
            jumpscareAsset.SetActive(true);
            jumpscareSound.Play();
            jumpscareIsActivated = true;
            StartCoroutine(DisableJumpscare());
        }
    }

    IEnumerator DisableJumpscare()
    { 
        yield return new WaitForSeconds(2);
        jumpscareAsset.SetActive(false);
    }
}