using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

[Serializable]
public class Sound
{
    public string Name;
    // public enum SoundType { Normal, Music };
    public enum Activation { Custom, PlayInstantly };
    //  public SoundType soundType;
    public Activation ActivationType;
    public AudioClip Clip;
    public bool Loop;
    public bool HearEveryWhere;
    public Vector2 Range = new Vector2(20, 30);
    [Range(0f, 1f)]
    public float Volume = 1f;
    [Range(.1f, 3f)]
    public float Pitch = 1f;
    [Range(0f, 2f)]
    public float RandomizedPitch;

    [HideInInspector]
    public AudioSource Source;

}

public class AudioManager : MonoBehaviour
{
    // AudioMixerGroup[] AudioMixers;

    public Sound[] Sounds;

    private bool _started;

    // Start is called before the first frame update
    private void Start()
    {
        CustomStart();
    }

    public void CustomStart()
    {
        if (!_started)
        {
            _started = true;
            SetSounds();
        }
    }

    public void PlaySound(Sound.Activation A, string Name = "")
    {
        if (A == Sound.Activation.Custom)
        {
            if (Name != "")
            {
                Sound s = Array.Find(Sounds, S => S.Name == Name);
                if (s == null)
                {
                    Debug.LogWarning($"The Sound '{Name}' not found");
                    return;
                }
                PlaySound(s);
            }
            else Debug.LogWarning($"Custom sounds must be called with a name");
        }
        else
        {
            foreach (Sound.Activation a in (Sound.Activation[])Enum.GetValues(typeof(Sound.Activation)))
            {
                if (A == a)
                {
                    foreach (Sound s in Sounds)
                    {
                        if (s.ActivationType == a)
                        {
                            PlaySound(s);
                        }
                    }
                    break;
                }
            }
        }
    }
    private void PlaySound(Sound s)
    {
        if (s.RandomizedPitch > 0)
        {
            float minPitch = s.Pitch - s.RandomizedPitch;
            if (minPitch < 0.1f) minPitch = 0.1f;
            float maxPitch = s.Pitch + s.RandomizedPitch;
            if (maxPitch > 3) maxPitch = 3;
            s.Source.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        }
        else s.Source.pitch = s.Pitch;
        s.Source.Play();
    }

    private void SetSounds()
    {
        //  AudioMixers = Settings.audiomixergroup;

        foreach (Sound s in Sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.volume = s.Volume;
            s.Source.loop = s.Loop;
            s.Source.playOnAwake = false;
            if (!s.HearEveryWhere)
            {
                s.Source.spatialBlend = 1;
                s.Source.rolloffMode = AudioRolloffMode.Linear;
                s.Source.minDistance = s.Range.x;
                s.Source.maxDistance = s.Range.y;
            }
            //     if (s.soundType == Sound.SoundType.Normal)
            //   {
            //       s.source.outputAudioMixerGroup = AudioMixers[0];
            //   }
            //   else if (s.soundType == Sound.SoundType.Music)
            //   {
            //      s.source.outputAudioMixerGroup = AudioMixers[1];
            //    }
            if (s.ActivationType == Sound.Activation.PlayInstantly)
            {
                PlaySound(s);
            }
        }
    }

    public void StopAllSound()
    {
        foreach (Sound s in Sounds)
        {
            s.Source.Stop();
        }
    }
}