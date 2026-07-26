using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
	public AudioMixer mixer;

	[SerializeField] private AudioClip _GameMusic;
	[SerializeField] private AudioClip _IntroMusic;
	public AudioMixerGroup musicGroup;

	
	private uint _masterVolume;
    private uint _musicVolume;
    private uint _soundVolume;

	[Header("SFX")]
	private int audioSourceCount = 100;
	private int nextSourceIndex = 0;
	public AudioSource musicSource;
	private AudioSource[] sources;
	private int[] overridePriorities;
	public AudioMixerGroup gameSFXGroup;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
        Instance = this;
		DontDestroyOnLoad(gameObject);
		
		StartCoroutine(setupSound());
	}

	IEnumerator setupSound()
	{
		//sfx
		yield return new WaitForEndOfFrame();

		if (gameSFXGroup == null && SoundManager.Instance != null)
		{
			var groups = SoundManager.Instance.mixer.FindMatchingGroups("GameSFX");
			if (groups.Length > 0) gameSFXGroup = groups[0];
		}


		sources = new AudioSource[audioSourceCount];
		for (int i = 0; i < audioSourceCount; i++)
		{
			GameObject go = new GameObject("AudioSource_" + i);
			go.transform.SetParent(transform);
			AudioSource source = go.AddComponent<AudioSource>();
			source.playOnAwake = false;

			if (gameSFXGroup != null)
			{
				source.outputAudioMixerGroup = gameSFXGroup;
			}

			sources[i] = source;
		}

		//music
		if (musicGroup == null && SoundManager.Instance != null)
		{
			var groups = mixer.FindMatchingGroups("Music");
			if (groups.Length > 0) musicGroup = groups[0];
		}

		musicSource.playOnAwake = true;
		musicSource.loop = true;
		musicSource.outputAudioMixerGroup = musicGroup;
		musicSource.clip = _IntroMusic;
		PlayMenuMusic();
		

	}

	// Plays sounds
	public void PlaySound(AudioClip clip, float minPitch = 0.9f, float maxPitch = 1.1f, float volume = 1f, bool networked = true)
	{
		if (clip == null) return;

		// Make the source the next one unused
		AudioSource source = sources[nextSourceIndex];

		// Randomize pitch and play
		source.pitch = UnityEngine.Random.Range(minPitch * Time.timeScale, maxPitch * Time.timeScale);
		source.PlayOneShot(clip, volume);

		// Increment and wrap around using the modulo operator, once you reach the end start using the first source again
		nextSourceIndex = (nextSourceIndex + 1) % sources.Count();
	}

	public void PlayMenuMusic()
	{
		musicSource.clip = _IntroMusic;
		musicSource.Play();
	}

	public void PlayGameMusic()
	{
		musicSource.clip = _GameMusic;
		musicSource.Play();
	}
}
