using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Volume2D : MonoBehaviour
{
    public Transform listenerTransform;
    public float minDist = 1;
    public float maxDist = 20;
    public AudioSource audioSource;

    private void Start()
    {
        
    }
    void Update()
    {
        float dist = Vector3.Distance(transform.position, listenerTransform.position);

        Sound2DAjust(audioSource, dist);
    }

    private void Sound2DAjust(AudioSource audioSource, float dist)
    {
        if (dist < minDist)
        {
            audioSource.volume = 0.5f;
        }
        else if (dist > maxDist)
        {
            audioSource.volume = 0;
        }
        else
        {
            audioSource.volume = 1 - ((dist - minDist) / (maxDist - minDist));
        }
        audioSource.Play();
    }
}
