using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crop List", menuName = "Crops /Create Crop List")]
public class CropDataList : ScriptableObject
{
    public List<CropDataSO> datas = new List<CropDataSO>();   
}
