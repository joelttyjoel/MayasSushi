using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioController : MonoBehaviour
{
    public List<AudioClip> songs;
    public AudioClip switchSound;
    public AudioClip offSound;
    public float switchCoolDown;
    public float maxTimeForward;
    public ParticleSystem noteParticles;
    public float timeForDubbleClick;

    private AudioSource radioSource;
    private int radioIndex;
    private bool canSwitch;
    bool mouseClicksStarted;
    int mouseClicks;

    // Start is called before the first frame update
    void Start()
    {
        radioSource = GetComponent<AudioSource>();
        
        radioIndex = (int)Random.Range(0, songs.Count);

        SwitchClipAndPlay();

        noteParticles.Play();

        canSwitch = true;

        mouseClicksStarted = false;
        mouseClicks = 0;
    }

    private void OnMouseUp()
    {
        if (!InteractionManager.Instance.canInteract) return;

        mouseClicks++;
        if (mouseClicksStarted)
        {
            return;
        }
        mouseClicksStarted = true;
        Invoke("checkMouseDoubleClick", timeForDubbleClick);
    }

    private void checkMouseDoubleClick()
    {
        if (mouseClicks > 1)
        {
            PauseMusic();
            Debug.Log("Double Clickedd");

        }
        else if(canSwitch)
        {
            SwitchSong();
            Debug.Log("Single Clicked");

        }
        mouseClicksStarted = false;
        mouseClicks = 0;
    }

    private void PauseMusic()
    {
        noteParticles.Stop();

        radioSource.Stop();
        radioSource.time = 0;
        radioSource.PlayOneShot(offSound);
    }

    private void SwitchSong()
    {
        noteParticles.Play();

        StartCoroutine(CoolDownRadioSwitch());

        radioIndex += 1;
        if (radioIndex >= songs.Count) radioIndex = 0;

        //cooldown plays next clip after done, here play scratch
        radioSource.Stop();
        radioSource.clip = switchSound;
        radioSource.time = 0;
        radioSource.Play();
    }

    private void SwitchClipAndPlay()
    {
        radioSource.Stop();
        radioSource.clip = songs[radioIndex];
        radioSource.time += Random.Range(0, maxTimeForward);
        radioSource.Play();
    }

    private IEnumerator CoolDownRadioSwitch()
    {
        canSwitch = false;
        yield return new WaitForSeconds(switchCoolDown);
        canSwitch = true;

        SwitchClipAndPlay();
    }
}
