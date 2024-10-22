using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

// AudioManager class is responsible for managing audio playback in the scene.
namespace Managers
{
    /*************************************************************************************************
     *    Title: AudioManager
     *    Author: John Young
     *    Date: 2023
     *    Code version: 1.0
     *    Notes: Code has been modified to suit the needs of the project
    **************************************************************************************************/
    public class AudioManager : MonoBehaviour
    {
        // Instance property to make AudioManager a singleton.
        public static AudioManager Instance { get; private set; }

        // AudioItem is a struct that contains all necessary information for an audio clip.
        [Serializable]
        public struct AudioItem
        {
            public string Name; // Unique name identifier for the audio clip.
            public AudioClip Clip; // The actual audio clip to be played.
            [Range(0f, 1f)] public float Volume; // The volume of the audio clip.
            public bool Loop; // Determines if the audio clip should loop when played.
            [Range(0f, 1f)] public float SpatialBlend; // Controls how much the audio is affected by 3D positioning.
            public float MinDistance; // The minimum distance for attenuating the audio source.
            public float MaxDistance; // The maximum distance for attenuating the audio source.
            [Range(0f, 10f)] public float RolloffFactor; // Controls how fast the sound volume decreases as the listener moves away from the source.
            public AudioMixerGroup AudioMixerGroup;

            // AudioItem constructor initializes all properties with default or passed values.
            public AudioItem(string name, AudioClip clip, AudioMixerGroup audioMixerGroup, float volume = 1f, bool loop = false, float spatialBlend = 1f, 
                float minDistance = 1f, float maxDistance = 500f, float rolloffFactor = 1f)
            {
                Name = name;
                Clip = clip;
                AudioMixerGroup = audioMixerGroup;
                Volume = volume;
                Loop = loop;
                SpatialBlend = spatialBlend;
                MinDistance = minDistance;
                MaxDistance = maxDistance;
                RolloffFactor = rolloffFactor;
            }
        }

        // List of AudioItems that can be played in the scene.
        public List<AudioItem> AudioItems = new List<AudioItem>();

        // Dictionary of AudioSource components, each associated with an AudioItem's unique name.
        private readonly Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();

        private Dictionary<string, GameObject> spawnedAudioSources;

        // Awake is called when the script instance is being loaded.
        private void Awake()
        {
            // Check if there is already an AudioManager instance and if it's different from this instance.
            if (Instance != null && Instance != this)
            {
                // Destroy this game object if there's already an AudioManager instance.
                Destroy(gameObject);
                return;
            }

            // Set the AudioManager instance to this instance.
            Instance = this;
            // Make sure the AudioManager instance is not destroyed when loading a new scene.
            DontDestroyOnLoad(gameObject);

            // Loop through the list of AudioItems.
            foreach (var item in AudioItems)
            {
                // Create a new game object for each AudioItem and add an AudioSource component to it.
                var obj = new GameObject(item.Name, typeof(AudioSource));
                // Set the new game object's parent to the AudioManager game object.
                obj.transform.SetParent(transform);
                // Get the AudioSource component from the new game object.
                var source = obj.GetComponent<AudioSource>();

                try
                {
                    // Set the AudioSource properties based on the AudioItem properties.
                    source.clip = item.Clip;
                    source.outputAudioMixerGroup = item.AudioMixerGroup;
                    source.volume = item.Volume;
                    source.loop = item.Loop;
                    source.spatialBlend = item.SpatialBlend;
                    source.minDistance = item.MinDistance;
                    source.maxDistance = item.MaxDistance;
                    source.playOnAwake = false;
                    // Add the AudioSource component to the audioSources dictionary using the AudioItem's name as the key.
                    audioSources.Add(item.Name, source);
                }
                catch (Exception ex)
                {
                    // Log an error message if there was an issue adding the AudioSource to the game object.
                    Debug.LogError($"Failed to add AudioSource to game object '{item.Name}': {ex.Message}");
                }
            }

            spawnedAudioSources = new Dictionary<string, GameObject>();
        }

        private void Start()
        {
            Play("music", transform);
        }

        // Play method is used to play an audio clip by its name and attach it to a specified parent transform.
        public void Play(string name, Transform parent)
        {
            // Check if an AudioItem with the specified name exists in the AudioItems list.
            if (AudioItems.Exists(item => item.Name == name))
            {
                // Find the AudioItem with the specified name.
                AudioItem item = AudioItems.Find(item => item.Name == name);
                // Create a new temporary GameObject with an AudioSource component, named as "(Temp)".
                GameObject obj = new GameObject(item.Name + " (Temp)", typeof(AudioSource));
                // Set the parent of the temporary GameObject to the specified parent transform.
                obj.transform.SetParent(parent);

                // Set the local position of the AudioSource GameObject to be the same as the parent.
                obj.transform.localPosition = Vector3.zero;

                // Get the AudioSource component from the temporary GameObject.
                var source = obj.GetComponent<AudioSource>();

                // Set the AudioSource properties based on the AudioItem properties.
                source.clip = item.Clip;
                source.outputAudioMixerGroup = item.AudioMixerGroup;
                source.volume = item.Volume;
                source.loop = item.Loop;
                source.spatialBlend = item.SpatialBlend;
                source.minDistance = item.MinDistance;
                source.maxDistance = item.MaxDistance;
                source.playOnAwake = false;
                // Set the AudioSource rolloff mode to custom.
                source.rolloffMode = AudioRolloffMode.Custom;

                // Create a custom rolloff curve for the AudioSource.
                AnimationCurve customCurve = new AnimationCurve();
                // Add keys to the custom curve.
                customCurve.AddKey(0f, 1f);
                customCurve.AddKey(item.RolloffFactor, 0f);
                // Set the custom rolloff curve to the AudioSource.
                source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, customCurve);

                // Play the AudioSource.
                source.Play();
            }
        }

        // PlayOneShot method plays a non-looping audio clip once by its name.
        public void PlayOneShot(string name)
        {
            // Check if the specified AudioSource exists in the audioSources dictionary.
            if (audioSources.TryGetValue(name, out var source))
            {
                // Play the audio clip once with the given volume if the source is found.
                source?.PlayOneShot(source.clip, source.volume);
            }
        }

        // Stop method stops the playback of the specified audio clip by its name.
        public void Stop(string name)
        {
            // Check if the specified AudioSource exists in the audioSources dictionary.
            if (audioSources.TryGetValue(name, out var source))
            {
                // Stop the playback of the audio clip if the source is found.
                source?.Stop();
            }
        }

        // StopAndRemove method stops the playback of the specified audio clip by its name and removes it from the audioSources dictionary.
        public void StopAndRemove(string name)
        {
            // Check if the specified AudioSource exists in the audioSources dictionary.
            if (audioSources.TryGetValue(name, out var source))
            {
                // Stop the playback of the audio clip if the source is found.
                source?.Stop();
                // Remove the AudioSource from the dictionary.
                audioSources.Remove(name);
                // Destroy the AudioSource GameObject.
                Destroy(source.gameObject);
            }
        }

        // FadeIn method gradually increases the volume of the specified audio clip by its name over a specified duration.
        public void FadeIn(string name, float duration)
        {
            // Check if the specified AudioSource exists in the audioSources dictionary.
            if (audioSources.TryGetValue(name, out var source))
            {
                // Start a coroutine to gradually increase the volume of the audio clip over the specified duration.
                StartCoroutine(FadeAudio(source, true, duration));
            }
        }

        // FadeOut method gradually decreases the volume of the specified audio clip by its name over a specified duration.
        public void FadeOut(string name, float duration)
        {
            // Check if the specified AudioSource exists in the audioSources dictionary.
            if (audioSources.TryGetValue(name, out var source))
            {
                // Start a coroutine to gradually decrease the volume of the audio clip over the specified duration.
                StartCoroutine(FadeAudio(source, false, duration));
            }
        }

        // FadeAudio is a private coroutine used to fade in or fade out an audio clip over a specified duration.
        private IEnumerator FadeAudio(AudioSource source, bool fadeIn, float duration)
        {
            // Determine the starting and target volumes based on whether it's a fade-in or fade-out.
            var startVolume = fadeIn ? 0f : source.volume;
            var targetVolume = fadeIn ? source.volume : 0f;

            // Set the initial volume of the AudioSource.
            source.volume = startVolume;
            // Play the AudioSource.
            source.Play();

            // Initialize the currentTime variable.
            var currentTime = 0f;

            // Execute the loop until the currentTime is greater than or equal to the duration.
            while (currentTime < duration)
            {
                // Increment the currentTime variable by the time passed since the last frame.
                currentTime += Time.deltaTime;
                // Interpolate the volume of the AudioSource based on the currentTime and duration.
                source.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);

                // Yield null to wait for the next frame before executing the loop again.
                yield return null;
            }

            // Stop the AudioSource if it's a fade-out.
            if (!fadeIn)
            {
                source.Stop();
            }

            // Reset the AudioSource volume to its original value if the volume reaches zero.
            if (source.volume <= 0f)
            {
                source.volume = startVolume;
            }
        }

        // PlayOneShot method plays a non-looping audio clip once by its name.
        public void Play(string name)
        {
            // Check if the specified AudioSource exists in the audioSources dictionary.
            if (audioSources.TryGetValue(name, out var source))
            {
                if(source.isPlaying) return;
                // Play the audio clip once with the given volume if the source is found.
                source?.Play();
            }
        }
        
        public void PlayOneShotWithRandomPitch(string name, float minPitch, float maxPitch)
        {
            // Check if the specified AudioSource exists in the audioSources dictionary.
            if (audioSources.TryGetValue(name, out var source))
            {
                var oldPitch = source.pitch;
                
                source.pitch = Random.Range(minPitch, maxPitch);
                
                // Play the audio clip once with the given volume if the source is found.
                source.PlayOneShot(source.clip, source.volume);
                
                StartCoroutine(ResetPitch(source, oldPitch));
            }
        }

        public void PlayWithRandomPitch(string name, float minPitch, float maxPitch)
        {
            // Check if the specified AudioSource exists in the audioSources dictionary.
            if (audioSources.TryGetValue(name, out var source))
            {
                if(source.isPlaying) return;
                
                var oldPitch = source.pitch;
                
                source.pitch = Random.Range(minPitch, maxPitch);
                
                // Play the audio clip once with the given volume if the source is found.
                source.Play();
                
                StartCoroutine(ResetPitch(source, oldPitch));
            }
        }

        private IEnumerator ResetPitch(AudioSource source, float oldPitch)
        {
            yield return new WaitForSeconds(source.clip.length);
            source.pitch = oldPitch;
        }

        public void StopAllAudio()
        {
            foreach (var audioSource in audioSources)
            {
                audioSource.Value.Stop();
            }
        }
    }
}