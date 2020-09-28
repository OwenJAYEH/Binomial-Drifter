using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] music;
    private AudioSource audioSource;
    int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = FindObjectOfType<AudioSource>();
        audioSource.loop = false;
    }
    
    private AudioClip GetNextClip()
    {
        return music[i];
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = GetNextClip();
            i++;
            audioSource.Play();
        }

        if (i == 6)
        {
            i = 0;
        }
    }
}
