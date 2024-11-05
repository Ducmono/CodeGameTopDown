using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSFX : MonoBehaviour
{
    public AudioSource source;

    private void Start()
    {
        source.pitch = Random.Range(0.8f, 2.3f);
        source.Play();
    }
}
