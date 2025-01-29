using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPuzzleManager : MonoBehaviour
{
    [SerializeField] List<ClickerPlate> clickerPlates;
    [SerializeField] GameObject barrierPrefab;
    [SerializeField] Vector3 barrierPos;
    [SerializeField] Sprite barriedSprite;
    [SerializeField] Vector3 spriteOffset;

    private GameObject barrier;
    private int currentPlateIndex = 0;

    private void Start()
    {
        spawnBarrier();
    }

    public void setCurrentPlateIndex(int index)
    {
        if (index != currentPlateIndex)
        {
            foreach (ClickerPlate clickerPlate in clickerPlates)
                clickerPlate.resetPlate();

            currentPlateIndex = 0;
        }
        else
        {
            currentPlateIndex = index + 1;
            if (currentPlateIndex == clickerPlates.Count)
            {
                despawnBarrier();
            }
        }
    }

    private void spawnBarrier()
    {
        barrier = Instantiate(barrierPrefab, barrierPos, Quaternion.identity);

        GameObject barrierVis = new GameObject("BarrierVisual");
        SpriteRenderer spriteRenderer = barrierVis.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = barriedSprite;
        spriteRenderer.sortingLayerName = "PressurePlate";
        barrierVis.transform.position = barrier.transform.position + spriteOffset;

        barrierVis.transform.parent = barrier.transform;
    }

    private void despawnBarrier()
    {
        Destroy(barrier);
        barrier = null;
    }
}
