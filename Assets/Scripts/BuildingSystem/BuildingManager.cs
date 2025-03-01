using Aeterponis.Grid;
using Aeterponis.InputHandler;
using System;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;
    public PlacableObjectSO.Direction currentDirection = PlacableObjectSO.Direction.Down;
    public PlacedObject selectedPlacedObject;

    private Transform ghostObject;
    private Camera mainCamera;
    private bool isInBuildingState = false;
    private bool isEditingState = false;
    private bool isReplacement = false;
    private Material ghostMaterialInstance;
    private Vector3 selectedObjectLastPosition;
    private Vector2Int selectedObjectLastOrigin;
    private Harvest replaceHarvestAreaCrop;
    private float replaceHarvestTime;
    private int replaceHarvestTick;

    [SerializeField] TouchManager touchManager;
    [SerializeField] GameGrid targetGrid;
    [SerializeField] private LayerMask mousePlaneLayerMask;
    [SerializeField] private PlacableObjectSO selectedPlacable;
    [SerializeField] private Material ghostMaterial;
    public bool IsInBuildingState { get => isInBuildingState; set => isInBuildingState = value; }
    public PlacableObjectSO SelectedPlacable { get => selectedPlacable; set => selectedPlacable = value; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        else
            Instance = this;


        mainCamera = Camera.main;
        ghostMaterialInstance = Instantiate(ghostMaterial);

        touchManager.OnScreenTouched += PlayerTouchedTheScreen;
        touchManager.OnScreenToucheReleased += PlayerToucheReleased;
        touchManager.OnHoldHappend += PlayerHoldHappend;
    }


    private void OnDisable()
    {
        touchManager.OnScreenTouched -= PlayerTouchedTheScreen;
        touchManager.OnScreenToucheReleased -= PlayerToucheReleased;
        touchManager.OnHoldHappend -= PlayerHoldHappend;
    }

    private void PlayerToucheReleased()
    {
        if (IsInBuildingState)
        {
            targetGrid.GetGridWorldPosition(GetTouchWorldPosition(), out float x, out float z);
            GridPosition gp = targetGrid.GetGridPositon(GetTouchWorldPosition());

            GridObject gridObject = targetGrid.GetGridObject(gp.x, gp.z);

            List<Vector2Int> occupiedAreas = SelectedPlacable.GetGridPositionList(new Vector2Int(gp.x,
                                                                                                gp.z),
                                                                                                currentDirection);

            bool canBuild = true;
            foreach (Vector2Int gridPosition in occupiedAreas)
            {
                if (!targetGrid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                {
                    canBuild = false;
                    break;
                }
            }

            if (IsOverWaterLayer())
            {
                canBuild = false;
            }

            if (gridObject != null && canBuild)
            {
                Vector3 placedObjectWorldPosition = new Vector3(x, .1f, z);

                PlacedObject _placedObject = PlacedObject.Create(placedObjectWorldPosition,
                    new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(z)),
                    currentDirection,
                    SelectedPlacable);

                foreach (Vector2Int gridPosition in occupiedAreas)
                {
                    targetGrid.GetGridObject(gridPosition.x, gridPosition.y).SetTransform(_placedObject);
                }

                if (replaceHarvestAreaCrop != null)
                {
                    Transform _placedHarvest = Instantiate(replaceHarvestAreaCrop.cropData.prefab, placedObjectWorldPosition, Quaternion.identity);
                    _placedObject.GetComponent<HarvestArea>().RegisterHarvest(_placedHarvest.GetComponent<Harvest>());
                    Destroy(replaceHarvestAreaCrop.gameObject);

                    replaceHarvestAreaCrop = _placedHarvest.GetComponent<Harvest>();
                    replaceHarvestAreaCrop.SetHarvestData(replaceHarvestTime,replaceHarvestTick);
                    replaceHarvestAreaCrop.transform.position = _placedObject.transform.position + new Vector3(0,1,0);
                    replaceHarvestAreaCrop.transform.parent = _placedObject.transform;
                }

                Destroy(ghostObject.gameObject);
                replaceHarvestAreaCrop = null;
                replaceHarvestTime = 0;
                replaceHarvestTick = 0;
                ghostObject = null;
                isInBuildingState = false;
                isReplacement = false;

            }
            else
            {
                Debug.Log("Can't build");
                if (isReplacement)
                {
                    PlacedObject _placedObject = PlacedObject.Create(selectedObjectLastPosition,
                    selectedObjectLastOrigin,
                    currentDirection,
                    SelectedPlacable);


                    foreach (Vector2Int gridPosition in occupiedAreas)
                    {
                        targetGrid.GetGridObject(gridPosition.x, gridPosition.y).SetTransform(_placedObject);
                    }
                }
                Destroy(ghostObject.gameObject);
                ghostObject = null;
                isInBuildingState = false;
                isReplacement = false;
            }
        }
    }
    private void PlayerTouchedTheScreen()
    {
        if (!isInBuildingState)
        {
            GetPlacedObject();
            if (selectedPlacedObject != null)
            {
                if (!selectedPlacedObject.HoldUI.activeSelf)
                {
                    selectedPlacedObject.HoldUI.SetActive(true);
                    isEditingState = true;
                }
            }

        }
    }
    public void GetPlacedObject()
    {
        GridPosition gp = targetGrid.GetGridPositon(GetTouchWorldPosition());

        GridObject gridObject = targetGrid.GetGridObject(gp.x, gp.z);

        PlacedObject tempCheck = gridObject.GetSettedObject();

        if (selectedPlacedObject != null && !ClickedOnUI())
        {
            if (tempCheck != selectedPlacedObject)
                selectedPlacedObject.HoldUI.SetActive(false);
        }

        selectedPlacedObject = gridObject.GetSettedObject();
    }

    private bool ClickedOnUI()
    {

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Input.GetTouch(0).position;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        // return results.Count > 0;
        foreach (var item in results)
        {
            if (item.gameObject.CompareTag("UI"))
            {
                return true;
            }
        }
        return false;
    }

    public void SetPlacedObject(PlacedObject placedObject)
    {
        selectedPlacedObject = placedObject;
    }

    public void RemoveBuilding()
    {
        //GetPlacedObject();
        if (selectedPlacedObject != null)
        {
            selectedPlacedObject.DestroySelf();
            List<Vector2Int> occupiedAreas = selectedPlacedObject.GetGridPositionList();
            foreach (Vector2Int gridPosition in occupiedAreas)
            {
                targetGrid.GetGridObject(gridPosition.x, gridPosition.y).ClearTransform();
            }
        }
    }

    public Vector3 GetTouchWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(touchManager.TouchPosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, mousePlaneLayerMask);
        return raycastHit.point;
    }

    private void PlayerHoldHappend()
    {
        if (selectedPlacedObject != null)
        {
            selectedPlacedObject.HoldUI.SetActive(false);
            StartReplacement();
        }
    }


    private void Update()
    {
        if (ghostObject != null)
        {
            targetGrid.GetGridWorldPosition(GetTouchWorldPosition(), out float _x, out float _z);
            ghostObject.transform.position = new Vector3(_x, .1f, _z);

            GridPosition gp = targetGrid.GetGridPositon(GetTouchWorldPosition());
            List<Vector2Int> occupiedAreas = SelectedPlacable.GetGridPositionList(new Vector2Int(gp.x,
                                                                                    gp.z),
                                                                                    currentDirection);
            bool canBuild = true;
            foreach (Vector2Int gridPosition in occupiedAreas)
            {
                if (!targetGrid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                {
                    canBuild = false;
                    break;
                }
            }

            if (IsOverWaterLayer())
            {
                canBuild = false;
            }

            ghostMaterialInstance.color = canBuild ? Color.green : Color.red;
        }
    }

    private bool IsOverWaterLayer()
    {
        Ray ray = new Ray(ghostObject.position + Vector3.up * 0.5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1f, LayerMask.GetMask("Water")))
        {
            return true;
        }
        return false;
    }

    public void StartReplacement()
    {
        //Save Last Position
        selectedObjectLastPosition = selectedPlacedObject.transform.position;
        selectedObjectLastOrigin = selectedPlacedObject.Origin;
        selectedPlacable = selectedPlacedObject.Data;
        if(selectedPlacedObject.transform.TryGetComponent(out HarvestArea _harvestArea))
        {
            replaceHarvestAreaCrop = _harvestArea.GetHarvest();
            (replaceHarvestTime, replaceHarvestTick) = _harvestArea.GetHarvest().GetHarvestData();
        }

        //Create a new Ghost at location
        targetGrid.GetGridWorldPosition(GetTouchWorldPosition(), out float x, out float z);
        Vector3 placedObjectWorldPosition = new Vector3(x, .1f, z);

        ghostObject = Instantiate(selectedPlacable.visual.transform,
            placedObjectWorldPosition,
            Quaternion.Euler(0, PlacableObjectSO.GetRotationAngle(currentDirection), 0));

        if(replaceHarvestAreaCrop != null)
            replaceHarvestAreaCrop.transform.SetParent(ghostObject.transform);

        MeshRenderer[] meshRenderers = ghostObject.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in meshRenderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = ghostMaterialInstance;
            }
            renderer.materials = materials;
        }


        isReplacement = true;
        //Remove Old Placed Object
        RemoveBuilding();

        IsInBuildingState = true;
    }

    public void StartGhost()
    {
        if (IsInBuildingState && selectedPlacable != null)
        {
            targetGrid.GetGridWorldPosition(GetTouchWorldPosition(), out float x, out float z);
            Vector3 placedObjectWorldPosition = new Vector3(x, .1f, z);

            ghostObject = Instantiate(selectedPlacable.visual.transform,
                placedObjectWorldPosition,
                Quaternion.Euler(0, PlacableObjectSO.GetRotationAngle(currentDirection), 0));

            MeshRenderer[] meshRenderers = ghostObject.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer renderer in meshRenderers)
            {
                Material[] materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = ghostMaterialInstance;
                }
                renderer.materials = materials;
            }
        }
    }

}