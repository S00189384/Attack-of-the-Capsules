using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionParticleEffect : MonoBehaviour
{
    //Components.
    AudioSource audioSource;

    public float timeBeforeDestroying;
    public AudioClip[] possibleExplosionSounds;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        Destroy(gameObject, timeBeforeDestroying);
        audioSource.PlayOneShot(possibleExplosionSounds[Random.Range(0, possibleExplosionSounds.Length)]);
    }
}
