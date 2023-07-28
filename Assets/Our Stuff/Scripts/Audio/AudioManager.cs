using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume = 1;
    [Range(0, 1)]
    public float musicVolume = 1;
    [Range(0, 1)]
    public float ambienceVolume = 1;
    [Range(0, 1)]
    public float SFXVolume = 1;

    //private Bus masterBus;
    //private Bus musicBus;
    //private Bus ambienceBus;
    //private Bus sfxBus;

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;
    private Dictionary<string, EventInstance> soundInstances = new Dictionary<string, EventInstance>();
    private EventInstance ambienceEventInstance;
    private EventInstance FillerEventInstance;
    //private EventInstance musicEventInstance;
    private EventInstance ChipserEventInstance;

    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene.");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        //masterBus = RuntimeManager.GetBus("bus:/");
        //musicBus = RuntimeManager.GetBus("bus:/Music");
        //ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        //sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    private void Start()
    {
        
        //InitializeMusic(FMODEvents.instance.music);
    }

    private void Update()
    {
    //    masterBus.setVolume(masterVolume);
    //    musicBus.setVolume(musicVolume);
    //    ambienceBus.setVolume(ambienceVolume);
    //    sfxBus.setVolume(SFXVolume);
    }

    public void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateInstance(ambienceEventReference);
        ambienceEventInstance.start();
    }
    public void InitializeFiller (EventReference FillerEventReference)
    {
        FillerEventInstance = CreateInstance(FillerEventReference);
        FillerEventInstance.start();
    }

    private void InitializeMusic(EventReference musicEventReference)
    {
        //musicEventInstance = CreateInstance(musicEventReference);
        //musicEventInstance.start();
    }

    public void SetAmbienceParameter(string parameterName, float parameterValue)
    {
        //ambienceEventInstance.setParameterByName(parameterName, parameterValue);
    }

    //public void SetMusicArea(MusicArea area)
    //{
    //    musicEventInstance.setParameterByName("area", (float)area);
    //}

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
        
    }
    public void PlayOneShotUnique(EventReference sound, Vector3 worldPos)
    {
        string soundPath = sound.Path;
        if (soundInstances.ContainsKey(soundPath))
        {
            EventInstance existingInstance = soundInstances[soundPath];
            PLAYBACK_STATE state;
            existingInstance.getPlaybackState(out state);

            if (state != PLAYBACK_STATE.PLAYING)
            {
                soundInstances.Remove(soundPath);
            }
            else
            {
                // Sound is already playing, do not play it again.
                return;
            }
        }

        EventInstance eventInstance = CreateInstance(sound);
        soundInstances[soundPath] = eventInstance;
        eventInstance.start();
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    private void CleanUp()
    {
        // stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        // stop all of the event emitters, because if we don't they may hang around in other scenes
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}