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

    public GameObject _SEPiece;
    

    public void playSE(string key)
    {
        SE_Piexe sp = Instantiate(_SEPiece, Vector3.zero,Quaternion.identity).GetComponent<SE_Piexe>();
        sp.playClip(findClipByKey(key));
    }

    public void playSE(string key, float stereoPan)
    {
        SE_Piexe sp = Instantiate(_SEPiece, Vector3.zero, Quaternion.identity).GetComponent<SE_Piexe>();
        sp.playClip(findClipByKey(key),stereoPan);
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
