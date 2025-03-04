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
    //PUBLIC
    public static BuildingManager Instance;
    public PlacableObjectSO.Direction currentDirection = PlacableObjectSO.Direction.Down;
    public PlacedObject selectedPlacedObject;

    //PRIVATE
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


    //SERIALIZEFIELD
    [SerializeField] TouchManager touchManager;
    [SerializeField] GameGrid targetGrid;
    [SerializeField] private LayerMask mousePlaneLayerMask;
    [SerializeField] private PlacableObjectSO selectedPlacable;
    [SerializeField] private Material ghostMaterial;

    //PROPS
    public bool IsInBuildingState { get => isInBuildingState; set => isInBuildingState = value; }
    public PlacableObjectSO SelectedPlacable { get => selectedPlacable; set => selectedPlacable = value; }

    private void Awake()
    {
        //SINGLETON
        if (Instance != null)
            Destroy(this);
        else
            Instance = this;


        //Caching for optimization
        mainCamera = Camera.main;
        ghostMaterialInstance = Instantiate(ghostMaterial);

        //Subscribe to events
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
            //Get the Grid object where player tries to put the building
            targetGrid.GetGridWorldPosition(GetTouchWorldPosition(), out float x, out float z); //Get touch world grid position
            GridPosition gp = targetGrid.GetGridPositon(GetTouchWorldPosition());
            GridObject gridObject = targetGrid.GetGridObject(gp.x, gp.z);

            //Get Building Occupied Areas
            List<Vector2Int> occupiedAreas = SelectedPlacable.GetGridPositionList(new Vector2Int(gp.x,
                                                                                                gp.z),
                                                                                                currentDirection);
            //Check can Player build here
            bool canBuild = true;
            foreach (Vector2Int gridPosition in occupiedAreas)
            {
                if (!targetGrid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                {
                    canBuild = false;
                    break;
                }
            }

            //Check Is Player Tries to build on the safe area
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

                //Occupie areas
                foreach (Vector2Int gridPosition in occupiedAreas)
                {
                    Debug.Log(gridPosition + "OCCUPIE");
                    targetGrid.GetGridObject(gridPosition.x, gridPosition.y).SetTransform(_placedObject);
                }

                //If area has Crops
                if (replaceHarvestAreaCrop != null)
                {
                    //Spawn new Crop
                    Transform _placedHarvest = Instantiate(replaceHarvestAreaCrop.cropData.prefab, placedObjectWorldPosition, Quaternion.identity);
                    _placedObject.GetComponent<HarvestArea>().RegisterHarvest(_placedHarvest.GetComponent<Harvest>());
                    //Destroy old one
                    Destroy(replaceHarvestAreaCrop.gameObject);

                    //Replace old datas to new crop
                    replaceHarvestAreaCrop = _placedHarvest.GetComponent<Harvest>();
                    replaceHarvestAreaCrop.SetHarvestData(replaceHarvestTime,replaceHarvestTick);
                    replaceHarvestAreaCrop.transform.position = _placedObject.transform.position + new Vector3(0,.18f,0);
                    replaceHarvestAreaCrop.transform.parent = _placedObject.transform;
                }

                //Replace phase done reset the values
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
                //Can't build
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
            //If player touched to placed object
            if (selectedPlacedObject != null)
            {
                if (!selectedPlacedObject.HoldUI.activeSelf)
                {
                    //Enable speacial menu for that object and enter editing state
                    selectedPlacedObject.HoldUI.SetActive(true);
                    isEditingState = true;
                }
            }

        }
    }
    public void GetPlacedObject()
    {
        //Register Placed Building to the SelectedPlacedObject
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
        //Check is Touch on the UI 
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Input.GetTouch(0).position;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
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
        //Building Removing system
        if (selectedPlacedObject != null)
        {
            selectedPlacedObject.DestroySelf();
            List<Vector2Int> occupiedAreas = selectedPlacedObject.GetGridPositionList();
            foreach (Vector2Int gridPosition in occupiedAreas)
            {
                GridObject _gridObject = targetGrid.GetGridObject(gridPosition.x, gridPosition.y);
                if(_gridObject != null)
                    _gridObject.ClearTransform();
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
            //Player touched and hold on the Building start replacement
            selectedPlacedObject.HoldUI.SetActive(false);
            StartReplacement();
        }
    }


    private void Update()
    {
        if (ghostObject != null)
        {
            //Building Ghost checking system
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
            if (_harvestArea.GetHarvest() != null)
            {
                replaceHarvestAreaCrop = _harvestArea.GetHarvest();
                (replaceHarvestTime, replaceHarvestTick) = _harvestArea.GetHarvest().GetHarvestData();

            }
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