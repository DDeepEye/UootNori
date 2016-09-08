using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UootNori;

public class SoundPlayer : MonoBehaviour{
    static SoundPlayer s_instance;
    static public SoundPlayer Instance {
        get
        {
            if (s_instance == null)
            {
                s_instance = GameObject.Find("Flow").GetComponent<SoundPlayer>();
            }
            return s_instance;
        }
    }

    public void Play(string resourcePath)
    {
        if (GameData.s_isDemo)
            return;
        
        AudioClip clip;
        if (_audioClips.ContainsKey(resourcePath))
        {
            clip = _audioClips[resourcePath];
        }
        else
        {
            clip = Resources.Load(resourcePath) as AudioClip;
            _audioClips.Add(resourcePath, clip);
        }
        AudioSource player;

        if (_audioSourcesPool.Count > 0)
            player = _audioSourcesPool.Dequeue();
        else
            player = gameObject.AddComponent<AudioSource>();

        player.clip = clip;
        player.Play();
        _audioPlayingSources.Add(player);
    }

    public void BGMPlay(string resourcePath)
    {
        if (GameData.s_isDemo)
            return;
        AudioClip clip;
        if (_audioClips.ContainsKey(resourcePath))
        {
            clip = _audioClips[resourcePath];
        }
        else
        {
            clip = Resources.Load(resourcePath) as AudioClip;
            _audioClips.Add(resourcePath, clip);
        }
        if (_bgmPlayer == null)
            _bgmPlayer = gameObject.AddComponent<AudioSource>();
        
        _bgmPlayer.clip = clip;
        _bgmPlayer.loop = true;
        _bgmPlayer.Play();
    }

    public void BGMStop()
    {
        if (_bgmPlayer != null)
            _bgmPlayer.Stop();
            
    }

    void Update()
    {   
        for (int i = 0; i < _audioPlayingSources.Count; ++i)
        {
            if (!_audioPlayingSources[i].isPlaying)
                _audioEndPlayingSources.Enqueue(_audioPlayingSources[i]);
        }

        while (_audioEndPlayingSources.Count > 0)
        {
            AudioSource player = _audioEndPlayingSources.Dequeue();
            _audioPlayingSources.Remove(player);
            _audioSourcesPool.Enqueue(player);
        }
    }

    Queue<AudioSource> _audioEndPlayingSources = new Queue<AudioSource>();

    List<AudioSource> _audioPlayingSources = new List<AudioSource>();
    Queue<AudioSource> _audioSourcesPool = new Queue<AudioSource>();
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
    AudioSource _bgmPlayer;
}
