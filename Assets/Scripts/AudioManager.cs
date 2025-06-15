using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource bgMusicSource;
    public AudioSource gameMusicSource;

    public AudioClip bgMusicClip;
    public AudioClip flipClip;
    public AudioClip matchClip;
    public AudioClip wrongClip;
    public AudioClip winClip;
    public AudioClip clickClip;

    private const string BGM_KEY = "BGMVolume";
    private const string GAME_KEY = "GameMusicVolume";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolumes();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBGM();
    }

    public void PlayBGM()
    {
        if (bgMusicSource != null && bgMusicClip != null)
        {
            bgMusicSource.clip = bgMusicClip;
            bgMusicSource.loop = true;
            bgMusicSource.Play();
        }
    }

    public void PlayFlip() => gameMusicSource?.PlayOneShot(flipClip);
    public void PlayMatch() => gameMusicSource?.PlayOneShot(matchClip);
    public void PlayWrong() => gameMusicSource?.PlayOneShot(wrongClip);
    public void PlayWin() => gameMusicSource?.PlayOneShot(winClip);
    public void ClickSound() => gameMusicSource?.PlayOneShot(clickClip);

    public void SetBGMVolume(float volume)
    {
        if (bgMusicSource != null)
        {
            bgMusicSource.volume = volume;
            PlayerPrefs.SetFloat(BGM_KEY, volume);
        }
    }

    public void SetGameMusicVolume(float volume)
    {
        if (gameMusicSource != null)
        {
            gameMusicSource.volume = volume;
            PlayerPrefs.SetFloat(GAME_KEY, volume);
        }
    }

    private void LoadVolumes()
    {
        float bgmVol = PlayerPrefs.GetFloat(BGM_KEY, 1f);
        float gameVol = PlayerPrefs.GetFloat(GAME_KEY, 1f);

        if (bgMusicSource != null) bgMusicSource.volume = bgmVol;
        if (gameMusicSource != null) gameMusicSource.volume = gameVol;
    }
}
