using Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyHealthPickup : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        audioSource.Play();

        Destroy(gameObject);
    }
}
