using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] Sprite defaultState;
    [SerializeField] Sprite pressedState;

    int state = 0;
    SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    public int getState() {return state;}

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (state == 0) { audioSource.Play(); }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("PickUp"))
        {
            state = 1;
            spriteRenderer.sprite = pressedState;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("PickUp"))
        {
            state = 0;
            spriteRenderer.sprite = defaultState;
        }
    }
}
