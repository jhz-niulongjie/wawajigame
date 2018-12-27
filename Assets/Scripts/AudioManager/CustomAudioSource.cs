using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class CustomAudioSource : MonoBehaviour
{
    private IEnumerator ie = null;
    private AudioSource continuousAds = null;
    private AudioSource newAds = null;
    private AudioSource fiexdAds = null;
    private AudioSource backGroundAds = null;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(StartCheckIE());
    }

    private IEnumerator StartCheckIE()
    {
        while (true)
        {
            if (ie != null && ie.MoveNext())
                yield return ie.Current;
            else
                yield return null;
        }
    }
    public void Play(AudioType adtype, AudioSource audioSource, AudioClip clip, Transform ads_parent, float volume, float pitch, bool loop, Action action = null)
    {
        if (adtype == AudioType.Fixed)
            StartCoroutine(PlayI(audioSource, clip, volume, pitch, loop, action));
        else if (adtype == AudioType.Continuous)
        {
            if (ie == null)
                ie = PlayII(clip, ads_parent, volume, pitch, loop, () => ie = null);
        }
        else if (adtype == AudioType.New)
        {
            PlayIII(clip, ads_parent, volume, pitch, loop);
        }
        else if(adtype==AudioType.BackGround)
        {
            PlayBackGround(clip, ads_parent,0.7f,pitch,loop);
        }
    }


    private IEnumerator PlayI(AudioSource audioSource, AudioClip clip, float volume, float pitch, bool loop, Action action = null)
    {
        fiexdAds = fiexdAds ?? audioSource;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.loop = loop;
        audioSource.Play();
        yield return new WaitForSeconds(clip.length);
        if (action != null)
            action();
    }
    private IEnumerator PlayII(AudioClip clip, Transform ads_parent, float volume, float pitch, bool loop, Action action = null)
    {
        continuousAds = CreateAudioSource(ads_parent, clip.name);
        continuousAds.clip = clip;
        continuousAds.volume = volume;
        continuousAds.pitch = pitch;
        continuousAds.loop = loop;
        continuousAds.Play();
        if (!loop)
        {
            Destroy(continuousAds.gameObject, clip.length);
            yield return new WaitForSeconds(clip.length);
            if (action != null)
                action();
        }

    }
    private void PlayIII(AudioClip clip, Transform ads_parent, float volume, float pitch, bool loop)
    {
        newAds = CreateAudioSource(ads_parent, clip.name);
        newAds.clip = clip;
        newAds.volume = volume;
        newAds.pitch = pitch;
        newAds.loop = loop;
        newAds.Play();
        if (!loop)
        {
            Destroy(newAds.gameObject, clip.length);
        }
    }

    private void PlayBackGround(AudioClip clip, Transform _adsParent, float volume, float pitch, bool loop)
    {
        backGroundAds = backGroundAds ?? CreateAudioSource(_adsParent,clip.name);
        backGroundAds.clip = clip;
        backGroundAds.volume = volume;
        backGroundAds.pitch = pitch;
        backGroundAds.loop = loop;
        backGroundAds.Play();
    }

    private AudioSource CreateAudioSource(Transform ads_parent, string ads_name)
    {
        GameObject go = new GameObject("Audio: " + ads_name);
        go.transform.SetParent(ads_parent);
        go.transform.localPosition = Vector3.zero;
        return go.AddComponent<AudioSource>();
    }


    public void StopAudio(AudioType adtype)
    {
        switch (adtype)
        {
            case AudioType.Continuous:
                ie = null;
                if (continuousAds != null) continuousAds.volume = 0;
                break;
            case AudioType.Fixed:
                if (fiexdAds != null) fiexdAds.volume = 0;
                break;
            case AudioType.New:
                if (newAds != null) newAds.volume = 0;
                break;
            case AudioType.BackGround:
                if (backGroundAds != null) backGroundAds.volume = 0;
                break;
        }
    }

}
