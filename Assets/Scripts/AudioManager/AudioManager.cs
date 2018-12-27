using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Video;

public sealed class AudioManager : Singleton<AudioManager>
{
    private Dictionary<string, AudioClip> soundDic = new Dictionary<string, AudioClip>();
    private AudioSource _source;
    private CustomAudioSource _cas;
    private GameObject emitter;
    private AudioManager()
    {
        GameObject audioSource = new GameObject("Audios");
        _source = audioSource.AddComponent<AudioSource>();
        _cas = audioSource.AddComponent<CustomAudioSource>();
        emitter = new GameObject("audios---");
        emitter.transform.SetParent(audioSource.transform);
    }
    public void PlayByName(AssetFolder _foldre, AudioType adtype, AudioNams clipName, bool loop, Action acion = null)
    {
        AudioClip clip = FindAudioClip(_foldre, clipName.ToString());
        //Debug.Log("播放声音---" + clipName);
        if (adtype == AudioType.Fixed)
        {
            _cas.Play(adtype, _source, clip, null, 1, 1, loop, acion);
        }
        else
        {
            _cas.Play(adtype, null, clip, emitter.transform, 1, 1, loop, acion);
        }
    }
    public float GetClipLength(AssetFolder _folder, AudioNams clipName)
    {
        AudioClip clip = FindAudioClip(_folder, clipName.ToString());
        return clip.length;
    }

    public void StopPlayAds(AudioType adtype)
    {
        Debug.Log("停止音效---" + adtype);
        _cas.StopAudio(adtype);
    }

    private AudioClip FindAudioClip(AssetFolder _folder, string clipName)
    {
        AudioClip clip;
        soundDic.TryGetValue(clipName, out clip);
        if (clip == null)
        {
            clip = Resources.Load<AudioClip>("Audio/" + _folder + "/" + clipName);
            soundDic.Add(clipName, clip);
        }
        return clip;
    }
    public void Clear()
    {
        foreach (var item in soundDic.Values)
        {
            Resources.UnloadAsset(item);
        }
        soundDic.Clear();
        Dispose();
    }

}
