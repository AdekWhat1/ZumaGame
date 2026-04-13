using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Додано для відстеження сцен

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Music Tracks")]
    [SerializeField] private AudioClip menuMusic;   // Музика для MainMenu
    [SerializeField] private AudioClip gameMusic;   // Музика для гри

    [Header("Destruction Sounds")]
    [SerializeField] private AudioClip[] destructionClips;

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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Підписуємося на подію зміни сцени
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Відписуємося від події при знищенні об'єкта
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Автоматичне керування музикою при завантаженні сцени
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Debug.Log допомагає побачити в консолі, як Unity бачить назву сцени
        Debug.Log("Завантажено сцену: " + scene.name);

        // Використовуємо ToLower(), щоб назва була нечутливою до регістру
        string sceneName = scene.name.ToLower();

        if (sceneName == "mainmenu")
        {
            PlayMusic(menuMusic, true);
        }
        else if (sceneName == "samplescene") // Перевір, чи твоя сцена гри в Build Settings називається Game
        {
            PlayMusic(gameMusic, true);
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayRandomDestruction()
    {
        if (destructionClips.Length > 0)
        {
            int randomIndex = Random.Range(0, destructionClips.Length);
            sfxSource.PlayOneShot(destructionClips[randomIndex]);
        }
    }

    public void PlayButtonClick()
    {
        if (buttonClick != null)
            uiSource.PlayOneShot(buttonClick);
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void PlayMusic(AudioClip musicClip, bool loop = true)
    {
        if (musicSource == null || musicClip == null) return;

        // Якщо цей кліп уже встановлений І грає — нічого не робимо
        if (musicSource.clip == musicClip && musicSource.isPlaying) return;

        // Якщо кліп інший — зупиняємо стару музику і вмикаємо нову
        musicSource.Stop();
        musicSource.clip = musicClip;
        musicSource.loop = loop;
        musicSource.Play();

        Debug.Log("Грає нова музика: " + musicClip.name);
    }
}