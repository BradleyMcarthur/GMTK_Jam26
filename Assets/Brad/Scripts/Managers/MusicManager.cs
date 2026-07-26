using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioClip musicTrack;
    [SerializeField, Range(0f, 1f)] private float volume = 0.5f;

    private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = musicTrack;
        _audioSource.loop = true;
        _audioSource.volume = volume;
        _audioSource.playOnAwake = false;
    }

    private void Start()
    {
        if (musicTrack != null)
            _audioSource.Play();
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        _audioSource.volume = volume;
    }

    public void Pause() => _audioSource.Pause();
    public void Resume() => _audioSource.UnPause();
    public void Stop() => _audioSource.Stop();
}
