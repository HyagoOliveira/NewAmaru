using System.Collections;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour, ISingletonManager
{
    public AudioSource audioSource;
    public AudioClip backgroundMusic;
    public float maxVolume = 0.5f;

    public static BackgroundMusicManager Instance { get; private set; }

    public void SetSingleton()
    {
        if (Instance != null)
        {
            print("Instancia já inicializada. Essa será destruida.");
            DestroyImmediate(this);
        }

        Instance = this;
    }

    private void Awake()
    {
        SetSingleton();
    }

    public void StartPlay()
    {
        if(audioSource == null)
        {
            throw new UnityException("Audio Source esta nulo");
        }

        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.Play();
        audioSource.enabled = true;
        FadeIn();
    }

    public void FindAudioSource()
    {
        audioSource = Camera.main.GetComponent<AudioSource>();
    }

    public void FadeIn()
    {
        audioSource.volume = 0f;
        StartCoroutine(VolumeFadeInCoroutine(0.01f));
    }
    
    public void FadeOut()
    {
        audioSource.volume = maxVolume;
        StartCoroutine(VolumeFadeOutCoroutine(0.01f));
    }

    public void Pause(float time)
    {
        audioSource.Pause();
        Invoke("Play", time);
    }

    public void Play()
    {
        audioSource.Play();
    }

    private IEnumerator VolumeFadeInCoroutine(float speed)
    {
        while(audioSource.volume < maxVolume)
        {
            audioSource.volume += speed;
            yield return new WaitForSeconds(0.1f);
        }

        audioSource.volume = maxVolume;
    }

    private IEnumerator VolumeFadeOutCoroutine(float speed)
    {
        while (audioSource.volume > 0f)
        {
            audioSource.volume -= speed;
            yield return new WaitForSeconds(0.1f);
        }

        audioSource.volume = 0f;
        audioSource.Pause();
    }
}
