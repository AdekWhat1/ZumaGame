using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    // Статичне посилання, щоб звертатися до менеджера звідки завгодно
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Destruction Sounds")]
    [SerializeField] private AudioClip[] destructionClips; // Сюди закинемо всі 5 ballsdestroyed

    [Header("UI Sounds")]
    [SerializeField] private AudioClip buttonClick;

    [Header("Special Effects")]
    public AudioClip bombExplode;
    public AudioClip fireBall;
    public AudioClip coinGrab;
    public AudioClip ballClick;
    public AudioClip choral1;
    public AudioClip chant4;

    private void Awake()
    {
        // Реалізація Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Звук не зникатиме при зміні рівнів
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Універсальний метод для програвання будь-якого кліпу
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    // Спеціальний метод для рандомного звуку руйнування
    public void PlayRandomDestruction()
    {
        if (destructionClips.Length > 0)
        {
            int randomIndex = Random.Range(0, destructionClips.Length);
            sfxSource.PlayOneShot(destructionClips[randomIndex]);
        }
    }

    // Метод для кнопок
    public void PlayButtonClick()
    {
        uiSource.PlayOneShot(buttonClick);
    }
}