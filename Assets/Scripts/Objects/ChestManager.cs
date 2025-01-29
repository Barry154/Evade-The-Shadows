using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts; 

public class ChestManager : MonoBehaviour
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

    /*
     * Change state of the chest to open
     * 
     * Returns 1 if chest is opened
     * Returns 0 if chest was already open
     */
    public int openChest()
    {
        if (isOpen)
            return 0;

        isOpen = true;
        if (renderer != null)
            renderer.sprite = openState;

        FindObjectOfType<GameplayManager>().OnChestOpened();

        return 1;
    }
}
