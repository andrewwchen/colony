using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AnimalInstance : MonoBehaviour
{

    [SerializeField] private AudioClip animalSound;
    //[SerializeField] private AudioClip eatSound;

    private float minWait;
    private float maxWait;

    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.spatialBlend = 1;
        playAnimalSound();

        minWait = animalSound.length + 1;
        maxWait = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (source.clip == null)
        {
            float wait = Random.Range(minWait, maxWait);
            source.clip = animalSound;
            StartCoroutine(PlaySoundHelper(wait));
        }
    }

    private void PlaySound(AudioClip c)
    {
        source.clip = c;
        StartCoroutine(PlaySoundHelper(c.length));
    }

    IEnumerator PlaySoundHelper(float delay)
    {
        source.Play();
        yield return new WaitForSeconds(delay);
        source.clip = null;
    }

    public void playAnimalSound()
    {
        PlaySound(animalSound);
    }

    /*
    public void playEatSound()
    {
        PlaySound(eatSound);
    }
    */

}
