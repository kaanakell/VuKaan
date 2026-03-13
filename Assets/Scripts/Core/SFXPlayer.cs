using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    public static SFXPlayer Instance { get; private set; }

    [Tooltip("Drag he SFX group from your GameMixer asset here")]
    public AudioMixerGroup sfxMixerGroup;

    private AudioSource _source;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _source = GetComponent<AudioSource>();
        _source.playOnAwake = false;
        _source.loop = false;

        if(sfxMixerGroup != null)
            _source.outputAudioMixerGroup = sfxMixerGroup;
    }

    public static void Play(AudioClip clip, float volume = 1f)
    {
        if(clip == null) return;

        if(Instance != null)
        {
            Instance._source.PlayOneShot(clip, volume);
        }
        else
        {
            AudioSource.PlayClipAtPoint(clip, Vector3.zero, volume);
            Debug.LogWarning("[SFXPlayer] No instance found - using fallback PlayClipAtPoint");
        }
    }
}
