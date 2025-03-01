using Aeterponis.Grid;
using Aeterponis.InputHandler;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CropManager : MonoBehaviour
{
    public static CropManager Instance;
    public CropDataSO selectedCropData;


    private Material ghostMaterialInstance;
    private Camera mainCamera;
    private Transform ghostObject;
    private bool isOnPlacement;
    private bool isOnHarvestingMode;

    [SerializeField] TouchManager touchManager;
    [SerializeField] GameGrid targetGrid;
    [SerializeField] private LayerMask mousePlaneLayerMask;
    [SerializeField] private LayerMask harvestAreaLayer;
    [SerializeField] private LayerMask harvestLayer;
    [SerializeField] private Material ghostMaterial;

    public bool IsOnPlacement { get => isOnPlacement; set => isOnPlacement = value; }
    public bool IsOnHarvestingMode { get => isOnHarvestingMode; set => isOnHarvestingMode = value; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        else
            Instance = this;


        mainCamera = Camera.main;
        ghostMaterialInstance = Instantiate(ghostMaterial);

        touchManager.OnScreenToucheReleased += PlayerToucheReleased;
    }


    private void OnDisable()
    {
        touchManager.OnScreenToucheReleased -= PlayerToucheReleased;
    }

    private void Update()
    {
        if (isOnHarvestingMode)
        {
            RaycastHit hit = GetTouchWorldHarvestHit();

            if(hit.transform != null)
            {
                if ((harvestLayer.value & (1 << hit.transform.gameObject.layer)) != 0)
                {
                    if (hit.transform.TryGetComponent(out Harvest harvestScript))
                    {
                        if (harvestScript.IsDone())
                        {
                            harvestScript.DoHarvest();
                        }
                    }
                }
            }
        }

        if (!isOnHarvestingMode && ghostObject != null)
        {
            targetGrid.GetGridWorldPosition(GetTouchWorldPosition(), out float _x, out float _z);
            ghostObject.transform.position = new Vector3(_x, 1, _z);

            RaycastHit hit = GetTouchWorldHit();

            //HarvestArea check is has crop


            if ((harvestAreaLayer.value & (1 << hit.transform.gameObject.layer)) != 0)
            {
                if (hit.transform.TryGetComponent(out HarvestArea _harvestArea))
                {
                    if (_harvestArea.GetHarvest() != null)
                    {
                        ghostMaterialInstance.color = Color.red;
                    }
                    else
                    {
                        ghostMaterialInstance.color = Color.green;
                    }
                }
            }
            else
            {
                ghostMaterialInstance.color = Color.red;
            }
        }
    }

    private void PlayerToucheReleased()
    {
        if (!isOnHarvestingMode && isOnPlacement)
        {
            targetGrid.GetGridWorldPosition(GetTouchWorldPosition(), out float x, out float z);
            GridPosition gp = targetGrid.GetGridPositon(GetTouchWorldPosition());

            GridObject gridObject = targetGrid.GetGridObject(gp.x, gp.z);

            bool canBuild = true;

            RaycastHit hit = GetTouchWorldHit();
            if ((harvestAreaLayer.value & (1 << hit.transform.gameObject.layer)) != 0)
                canBuild = true;
            else
                canBuild = false;

            if (hit.transform.TryGetComponent(out HarvestArea _harvestArea))
            {
                if (_harvestArea.GetHarvest() != null)
                {
                    canBuild = false;
                }
            }


            if (gridObject != null && canBuild)
            {
                Vector3 placedObjectWorldPosition = new Vector3(x, .20f, z);

                Transform _placedHarvest = Instantiate(selectedCropData.prefab, placedObjectWorldPosition, Quaternion.identity);
                _harvestArea.RegisterHarvest(_placedHarvest.GetComponent<Harvest>());
                _placedHarvest.transform.SetParent(_harvestArea.transform);
                Destroy(ghostObject.gameObject);
                ghostObject = null;
                isOnPlacement = false;
            }
            else
            {
                Debug.Log("Can't build");
                Destroy(ghostObject.gameObject);
                ghostObject = null;
                isOnPlacement = false;
            }
        }
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

    public void StartGhost()
    {
        if (!isOnHarvestingMode && isOnPlacement && selectedCropData != null)
        {
            targetGrid.GetGridWorldPosition(GetTouchWorldPosition(), out float x, out float z);
            Vector3 placedObjectWorldPosition = new Vector3(x, .18f, z);

            ghostObject = Instantiate(selectedCropData.visual.transform,
                placedObjectWorldPosition,
                Quaternion.identity);

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

    public Vector3 GetTouchWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(touchManager.TouchPosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, mousePlaneLayerMask);
        return raycastHit.point;
    }

    public RaycastHit GetTouchWorldHit()
    {
        Ray ray = mainCamera.ScreenPointToRay(touchManager.TouchPosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, mousePlaneLayerMask);
        return raycastHit;
    }

    public RaycastHit GetTouchWorldHarvestHit()
    {
        Ray ray = mainCamera.ScreenPointToRay(touchManager.TouchPosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, harvestLayer);
        return raycastHit;
    }

}
