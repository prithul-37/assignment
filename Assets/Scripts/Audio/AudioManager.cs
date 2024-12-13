using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton instance
    public static AudioManager Instance;

    [SerializeField] AudioClip BackGroundAudio;
    [SerializeField] AudioSource SfXobj;

    private void Awake()
    {
        // Ensure only one instance of AudioManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayAudio(AudioClip clip, float volume = 1.0f)
    {
        AudioSource audioSource = Instantiate(SfXobj, transform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource.gameObject, audioSource.clip.length);
    }



    private void Start()
    {
        StartCoroutine(PlayBackgroundAudio());
    }


    IEnumerator PlayBackgroundAudio()
    {
        while (true)
        {
            PlayAudio(BackGroundAudio,.05f);
            yield return new WaitForSeconds(BackGroundAudio.length);
        }
    }

}

