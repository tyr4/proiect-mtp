using System;
using UnityEngine;

public static class AudioEvents
{
    public static event Action<AudioClip> OnSFXRequested;
    public static event Action<AudioClip> OnMusicRequested;

    public static void RequestSFX(AudioClip clip) => OnSFXRequested?.Invoke(clip);
    public static void RequestMusic(AudioClip clip) => OnMusicRequested?.Invoke(clip);
}