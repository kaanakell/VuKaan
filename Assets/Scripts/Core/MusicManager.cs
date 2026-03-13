using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Music")]
    [Tooltip("Drag your music clip here")]
    public AudioClip musicClip;

    [Tooltip("Master volume for the music")]
    [Range(0f, 1f)]
    public float volume = 0.5f;

    [Header("Fade")]
    [Tooltip("Seconds to fade music in on start")]
    public float fadeInDuration = 2f;

    [Tooltip("Seconds to fade music out when game end (win screen)")]
    public float fadeOutDuration = 1.5f;

    private AudioSource _source;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        _source = GetComponent<AudioSource>();
        _source.clip = musicClip;
        _source.loop = true;
        _source.playOnAwake = false;
        _source.volume = 0f;
    }

    private void Start()
    {
        if(musicClip == null)
        {
            Debug.LogWarning("[MusicManager] No music clip assigned.");
            return;
        }

        _source.Play();
        FadeIn();
    }

    private void OnEnable()
    {
        GameEvents.OnAllItemsFound += HandleWin;
    }

    private void OnDisable()
    {
        GameEvents.OnAllItemsFound -= HandleWin;
    }

    public void FadeIn()
    {
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, _source.volume, volume, fadeInDuration)
        .setOnUpdate(v => _source.volume = v)
        .setEase(LeanTweenType.easeInOutSine);
    }

    public void FadeOut(System.Action onComplete = null)
    {
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, _source.volume, 0f, fadeOutDuration)
        .setOnUpdate(v => _source.volume = v)
        .setEase(LeanTweenType.easeInOutSine)
        .setOnComplete(() => onComplete?.Invoke());
    }

    public void SetVolume(float v)
    {
        volume = v;
        LeanTween.cancel(gameObject);
        _source.volume = v;
    }

    private void HandleWin()
    {
        FadeOut();
    }
    
}
