using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SEPlayer : MonoBehaviour
{
    private AudioSource _AS;

    [Serializable]public struct SE_Note
    {
        public string name;
        public AudioClip clip;
    }

    public SE_Note[] _Notes;


    public void playSE(string soundName)
    {
        for(int i=0; i<_Notes.Length; i++)
        {
            SE_Note sn = _Notes[i];
            if (sn.name == soundName)
            {
                _AS.clip = sn.clip;
                _AS.Play();
            }
        }
    }

    private void Awake()
    {
        _AS = GetComponent<AudioSource>();
    }


}
