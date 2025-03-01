using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    public static PlacedObject Create(Vector3 _worldPosition, Vector2Int _origin, PlacableObjectSO.Direction _direction, PlacableObjectSO _data)
    {
        Transform placedObjectTransform = Instantiate(_data.prefab, _worldPosition,
            Quaternion.Euler(0, PlacableObjectSO.GetRotationAngle(_direction), 0));

        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();

        placedObject.data = _data;
        placedObject.origin = _origin;
        placedObject.direction = _direction;

        return placedObject;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public List<Vector2Int> GetGridPositionList()
    {
        return data.GetGridPositionList(origin, direction);
    }

    [SerializeField] private GameObject holdUI;
    private PlacableObjectSO data;
    private Vector2Int origin;
    private PlacableObjectSO.Direction direction;

    public GameObject HoldUI { get => holdUI; set => holdUI = value; }
    public PlacableObjectSO Data { get => data; set => data = value; }
    public Vector2Int Origin { get => origin; set => origin = value; }

    public void OnRemoveClicked()
    {
        BuildingManager.Instance.SetPlacedObject(this);
        BuildingManager.Instance.RemoveBuilding();
    }

}
