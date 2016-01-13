using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Audio Clip Provider.
/// This script allows you to store a pool of audioclips and play them inside other 
/// GameObjects with AudioSources components attached to them.
/// Put this script inside some game manager object acting as a folder to other game objects.
/// </summary>
public class AudioClipProvider : Provider<AudioClip>
{
    public AudioClip[] audioClips;

    public override AudioClip Get(string audioClipName)
    {
        if (_cache.ContainsKey(audioClipName))
            return _cache[audioClipName];

        throw new UnityException("No " + audioClipName + " audio clip registered!");
    }

    public override void Register()
    {
        _cache = new Dictionary<string, AudioClip>();

        for (int i = 0; i < audioClips.Length; i++)
        {
            _cache.Add(audioClips[i].name, audioClips[i]);
        }
    }
}
