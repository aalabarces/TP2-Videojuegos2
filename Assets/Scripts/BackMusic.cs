using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class BackMusic : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip clip;
    void PlayBackgroundMusic()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
    }

}
