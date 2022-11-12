using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AnimalInstance : MonoBehaviour
{

    [SerializeField] private AudioClip animalSound;
    [SerializeField] private AudioClip eatSound;

    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.spatialBlend = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //if (source.clip == null && Random.Range(0, 100) == 0) playAnimalSound();
    }

    private void PlaySound(AudioClip c)
    {
        source.clip = c;
        StartCoroutine(PlaySoundHelper());
    }

    IEnumerator PlaySoundHelper()
    {
        source.Play();
        yield return new WaitForSeconds(source.clip.length);
        source.clip = null;
    }

    public void playAnimalSound()
    {
        PlaySound(animalSound);
    }

    public void playEatSound()
    {
        PlaySound(eatSound);
    }

}
