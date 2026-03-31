using UnityEngine;

public class BGMmanager : MonoBehaviour
{
    public static BGMmanager instance;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton pattern to ensure only one BGM manager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object when switching scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate BGM managers
            return;
        }

        audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.loop = true;        // Enable looping playback
            audioSource.playOnAwake = true; // Automatically play when the game starts
            audioSource.Play();             // Start playing BGM
        }
        else
        {
            Debug.LogWarning("AudioSource component not found on BGMmanager!");
        }
    }
}

