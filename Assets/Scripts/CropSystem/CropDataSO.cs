using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crop", menuName = "Crops /Create a new Crop")]
public class CropDataSO : ScriptableObject
{
    public string nameString;
    public Transform prefab;
    public Transform visual;
    public int width;
    public int height;

    public List<Vector2Int> GetGridPositionList(Vector2Int offset)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridPositionList.Add(offset +
                    new Vector2Int(x, y));
            }
        }
        return gridPositionList;
    }
}
