using UnityEngine;

public class PlacableObjectUIManager : MonoBehaviour
{
    public PlacableObjectsSO placableList;

    public void RegisterSelectedBuilding(int index)
    {
        BuildingManager.Instance.SelectedPlacable = placableList.objectSOs[index];
        BuildingManager.Instance.IsInBuildingState = true;  
        BuildingManager.Instance.StartGhost();
    }
}
