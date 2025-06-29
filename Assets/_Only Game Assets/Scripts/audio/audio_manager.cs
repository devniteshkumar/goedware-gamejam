using UnityEngine;

public class audio_manager : MonoBehaviour
{
    public static audio_manager Instance { get; private set; }

    [SerializeField] AudioSource background_music;
    [SerializeField] AudioSource sound_effects;

    public AudioClip level_start_countdown;
    public AudioClip background;
    public AudioClip death;
    public AudioClip enemy_hit;
    public AudioClip player_hit;
    public AudioClip convert;

    void Awake()
    {
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

    void Start()
    {
        //background_music.clip = background;
        //background_music.Play();
    }

    public void PlaySound(AudioClip clip)
    {
        //sound_effects.PlayOneShot(clip);
    }
}
