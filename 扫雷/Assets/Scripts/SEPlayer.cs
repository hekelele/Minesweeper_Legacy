using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SEPlayer : MonoBehaviour
{
    [Serializable]
    public class MusicLink
    {
        public string key;
        public AudioClip clip;
    }

    [SerializeField] public List<MusicLink> _Dict;
    private AudioSource _SE;

    private void Awake()
    {
        _SE = GetComponent<AudioSource>(); 
    }

    public void playSE(string key)
    {
        _SE.clip = findClipByKey(key);
        _SE.Play();
    }

    public AudioClip findClipByKey(string key)
    {
        for(int i=0; i<_Dict.Count; i++)
        {
            if (_Dict[i].key == key)
            {
                return _Dict[i].clip;
            }
        }
        return null;
    }
}
