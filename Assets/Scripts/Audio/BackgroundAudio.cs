using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAudio : MonoBehaviour
{
    [Header("Game Over SFX")]
    [SerializeField]
    AudioSource gameOverSFX;

    [Header("Game Won SFX")]
    [SerializeField]
    AudioSource gameWonSFX;

    [Header("Wave Alert")]
    [SerializeField]
    public AudioSource incomingWaveSFX;

    [Header("Combat Music")]
    [SerializeField]
    public AudioSource combatMusic;

    [Header("Background Music")]
    [SerializeField]
    public AudioSource outOfCombatMusic;

    [Header("Key Collected SFX")]
    [SerializeField]
    public AudioSource keyCollectedChime;

    [Header("Boss Door Open SFX")]
    [SerializeField]
    public AudioSource bossDoorOpensSFX;

    public void gameOverSound()
    {
        //Debug.Log("game over sound triggered");
        combatMusic.Stop();
        outOfCombatMusic.Stop();
        gameOverSFX.Play();
    }

    public void gameWonSound()
    {
        //Debug.Log("game won sound triggered");
        combatMusic.Stop();
        outOfCombatMusic.Stop();
        gameWonSFX.Play();
    }

    public void WaveAlertSound()
    {
        incomingWaveSFX.Play();
    }

    public void playCombatMusic()
    {
        //Debug.Log("Sound triggered");
        outOfCombatMusic.Stop();
        combatMusic.Play();
    }

    public void playBackgroundMusic()
    {
        //Debug.Log("Sound triggered");
        combatMusic.Stop();
        outOfCombatMusic.Play();
    }

    public void playKeyCollected()
    {
        keyCollectedChime.Play();
    }

    public void playBossDoorOpens()
    {
        bossDoorOpensSFX.Play();
    }
}
