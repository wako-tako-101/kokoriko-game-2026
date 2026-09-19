using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip marsMusic;

    [Header("Scene Names")]
    public string menuSceneName = "MainMenu";
    public string tutorialSceneName = "Level1Tutorial";
    public string marsSceneName = "Level2Mars";
    public string endSceneName = "EndScene";

    private AudioSource musicSource;

    void Awake()
    {
        // Prevent duplicate AudioManagers
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep music playing when switching scenes
        DontDestroyOnLoad(gameObject);

        musicSource = GetComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // Play music for the scene where the game starts
        ChangeMusic(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ChangeMusic(scene.name);
    }

    void ChangeMusic(string sceneName)
    {
        AudioClip newMusic = null;

        // Main Menu and Tutorial share the same music
        if (sceneName == menuSceneName ||
            sceneName == tutorialSceneName)
        {
            newMusic = menuMusic;
        }

        // Mars and End Scene share the same music
        else if (sceneName == marsSceneName ||
                 sceneName == endSceneName)
        {
            newMusic = marsMusic;
        }

        // No music assigned to this scene
        if (newMusic == null)
            return;

        // Don't restart the music if the correct song is already playing
        if (musicSource.clip == newMusic && musicSource.isPlaying)
            return;

        // Switch to the new music
        musicSource.Stop();
        musicSource.clip = newMusic;
        musicSource.Play();
        Debug.Log("Music started: " + musicSource.isPlaying);
    }
}