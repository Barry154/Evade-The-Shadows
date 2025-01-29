using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerSFX: MonoBehaviour
{
    //[Header("Combat SFX:")]
    //[SerializeField]
    //AudioClip swordSFX;
    //[SerializeField]
    //AudioClip damageTakenSFX;
    //[SerializeField]
    //AudioClip enemyHitSFX;

    AudioSource sounds;

    // Start is called before the first frame update
    void Start()
    {
        sounds = GetComponent<AudioSource>();
    }

    //public void PlayerSwordSwingSound()
    //{
    //    sounds.PlayOneShot(swordSFX);
    //}

    public void PlayerTakesDamageSound()
    {
        sounds.Play();
    }

    //public void EnemyHitSound()
    //{
    //    sounds.PlayOneShot(enemyHitSFX);
    //}
}
