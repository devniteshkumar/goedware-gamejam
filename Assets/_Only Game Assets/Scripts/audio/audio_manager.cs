using UnityEngine;

public class audio_manager : MonoBehaviour
{
    
    [SerializeField] AudioSource background_music;
    [SerializeField] AudioSource sound_effects;

    public AudioClip level_start_countdown;
    public AudioClip background;
    public AudioClip death;
    public AudioClip enemy_hit;
    public AudioClip enemy_death_incase_found_smtg;
    public AudioClip sword_sound;

    public AudioClip smtg;

    public AudioClip player_hit;
    public AudioClip convert;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        background_music.clip = background;
        background_music.Play();
    }

    // Update is called once per frame
    public void sound_effect(AudioClip clip)
    {
        sound_effects.PlayOneShot(clip);
    }
}
