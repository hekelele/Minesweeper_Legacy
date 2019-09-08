using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SE_Piexe : MonoBehaviour
{

    private AudioSource _AS;
    public float safeTime = 0.5f;

    private void Awake()
    {
        _AS = GetComponent<AudioSource>();
        safeTime += Time.time;
    }

    public void playClip(AudioClip ac)
    {
        _AS.clip = ac;
        _AS.Play();
    }

    public void playClip(AudioClip ac, float stereoPan)
    {
        _AS.clip = ac;
        _AS.panStereo = stereoPan;
        _AS.Play();
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.time>safeTime && !_AS.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }
}
