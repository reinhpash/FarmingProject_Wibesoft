using UnityEngine;

public class CropUIManager : MonoBehaviour
{
    public CropDataList cropList;

    public void RegisterSelectedBuilding(int index)
    {
        CropManager.Instance.selectedCropData = cropList.datas[index];
        CropManager.Instance.IsOnPlacement = true;
        CropManager.Instance.StartGhost();
    }
}
