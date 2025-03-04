using System;
using System.Collections.Generic;
using UnityEngine;

public class Harvest : MonoBehaviour
{
    MeshFilter meshFilter;
    public Action OnCropHarvested; //Public event for harvesting 
    public GameObject HarvestedEffect;
    public List<Mesh> levelMeshes = new List<Mesh>();
    public CropDataSO cropData;
    public float currentHarvestTime;
    public float harvestTickTime = 10;
    public int maxHarvestTickAmount = 3;
    public int currentTick = 1;
    bool isHarvested = false;

    private void Start()
    {
        meshFilter = this.GetComponent<MeshFilter>();
    }

    private void Update()
    {
        if (currentTick >= maxHarvestTickAmount) return;

        currentHarvestTime += Time.deltaTime;

        if (currentHarvestTime >= harvestTickTime * currentTick)
        {
            currentTick++; //Update level of crop
            meshFilter.mesh = levelMeshes[currentTick - 1]; //Growth Animation
        }
    }

    public bool IsDone() //Check is crop life cycle done
    {
        return currentTick >= maxHarvestTickAmount;
    }

    public void DoHarvest() //Crop Harvesting
    {
        if (!isHarvested)
        {
            isHarvested = true;

            if (HarvestedEffect != null)
                Instantiate(HarvestedEffect.gameObject, this.transform.position, Quaternion.identity);

            OnCropHarvested?.Invoke();
            Destroy(this.gameObject, .25f);
        }
        
    }

    public (float, int) GetHarvestData() // Harvest data Getter for Save/Placement system
    {
        return (currentHarvestTime, currentTick);
    }

    public void SetHarvestData(float _currentHarvestTime, int _currentTick) // Load Harvest Data for Save/Placement system
    {
        currentHarvestTime = _currentHarvestTime;
        currentTick = _currentTick;
        if (meshFilter == null)
            meshFilter = this.GetComponent<MeshFilter>();

        meshFilter.mesh = levelMeshes[currentTick - 1];
    }
}