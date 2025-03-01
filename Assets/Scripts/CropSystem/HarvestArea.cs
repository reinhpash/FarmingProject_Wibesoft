using System;
using UnityEngine;

public class HarvestArea : MonoBehaviour
{
    Harvest currentHarvest;
    
    public void RegisterHarvest(Harvest _harvest)
    {
        currentHarvest = _harvest;
        currentHarvest.OnCropHarvested += OnCropHarvested;
    }

    private void OnDisable()
    {
        if(currentHarvest != null)
            currentHarvest.OnCropHarvested -= OnCropHarvested;
    }

    private void OnCropHarvested()
    {
        if (currentHarvest != null)
        {
            currentHarvest.OnCropHarvested -= OnCropHarvested;
            currentHarvest = null;
        }
    }

    public Harvest GetHarvest() => currentHarvest;

}
