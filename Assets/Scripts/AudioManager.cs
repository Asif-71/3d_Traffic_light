using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class SoundEntry
    {
        public string key;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [Header("Sound Library")]
    [SerializeField] private SoundEntry[] soundEntries;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private Dictionary<string, SoundEntry> _soundMap;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _soundMap = new Dictionary<string, SoundEntry>();
        foreach (var entry in soundEntries)
            if (!string.IsNullOrEmpty(entry.key))
                _soundMap[entry.key] = entry;
    }

    public void PlaySFX(string key)
    {
        if (_soundMap.TryGetValue(key, out SoundEntry entry))
            sfxSource.PlayOneShot(entry.clip, entry.volume);
        else
            Debug.LogWarning($"[AudioManager] SFX key not found: {key}");
    }

    public void PlayMusic(string key)
    {
        if (_soundMap.TryGetValue(key, out SoundEntry entry))
        {
            musicSource.clip = entry.clip;
            musicSource.loop = true;
            musicSource.volume = entry.volume;
            musicSource.Play();
        }
    }

    public void StopMusic() => musicSource.Stop();
}