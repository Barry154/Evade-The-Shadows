using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoorManager : MonoBehaviour
{
    [SerializeField] Sprite closedState;
    [SerializeField] Sprite openState;
    private bool isOpen;
    private SpriteRenderer renderer;

    private void Start()
    {
        isOpen = false;
        renderer = GetComponent<SpriteRenderer>();

        if (renderer != null)
            renderer.sprite = closedState;
    }

    // Change state of the door to open
    public void openDoor(int keysCollected)
    {
        if (keysCollected < 3)
            return;

        if (isOpen)
            return;

        isOpen = true;
        if (renderer != null)
            renderer.sprite = openState;

        foreach (BoxCollider2D collider in GetComponents<BoxCollider2D>())
            collider.enabled = false;
    }
}
