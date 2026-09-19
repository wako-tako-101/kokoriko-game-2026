using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip levelMusic;
    public AudioClip gameOverMusic;

    [Header("Scene Names")]
    public string menuSceneName = "MainMenu";
    public string gameOverSceneName = "GameOver";

    private AudioSource musicSource;

    void Awake()
    {
        // Make sure only one AudioManager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep this object alive when changing scenes
        DontDestroyOnLoad(gameObject);

        musicSource = GetComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.playOnAwake = false;
    }

    void Start()
    {
        // Set music for the first scene
        ChangeMusic(SceneManager.GetActiveScene().name);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ChangeMusic(scene.name);
    }

    void ChangeMusic(string sceneName)
    {
        AudioClip newMusic;

        if (sceneName == menuSceneName)
        {
            newMusic = menuMusic;
        }
        else if (sceneName == gameOverSceneName)
        {
            newMusic = gameOverMusic;
        }
        else
        {
            // All other scenes use level music
            newMusic = levelMusic;
        }

        // Don't restart music if it's already playing
        if (musicSource.clip == newMusic && musicSource.isPlaying)
            return;

        musicSource.Stop();
        musicSource.clip = newMusic;

        if (newMusic != null)
        {
            musicSource.Play();
        }
    }
}