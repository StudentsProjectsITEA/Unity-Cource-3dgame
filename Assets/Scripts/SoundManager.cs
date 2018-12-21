using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private AudioClip[] AudioClips;
    public AudioSource audioSource;

    private SoundManager()
    {

    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        AudioClips = new AudioClip[6];
        AudioClips[0] = Resources.Load<AudioClip>("Music/" + "music");
        AudioClips[1] = Resources.Load<AudioClip>("Sounds/" + "cannon");
        AudioClips[2] = Resources.Load<AudioClip>("Sounds/" + "dryfire");
        AudioClips[3] = Resources.Load<AudioClip>("Sounds/" + "ammo");
        AudioClips[4] = Resources.Load<AudioClip>("Sounds/" + "playerwalk");
        AudioClips[5] = null;
        audioSource.PlayOneShot(AudioClips[0]);
        audioSource.loop = true;
    }

    public void PlayShootSound()
    {
        audioSource.PlayOneShot(AudioClips[1]);
    }

    public void PlayClickSound()
    {
        audioSource.PlayOneShot(AudioClips[2]);
    }

    public void PlayReloadSound()
    {
        audioSource.PlayOneShot(AudioClips[3]);
    }

    public void PlayHitSound()
    {
        audioSource.PlayOneShot(AudioClips[4]);
    }

    public void PLaySoundEffect(int clip)
    {
        audioSource.PlayOneShot(AudioClips[clip]);
    }
}

