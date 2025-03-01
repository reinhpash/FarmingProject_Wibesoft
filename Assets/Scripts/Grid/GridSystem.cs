using System;
using UnityEngine;

namespace Aeterponis.Grid
{
    public class GridSystem
    {
        private int width;
        private int height;
        private float cellSize;
        private GridObject[,] gridObjects;

        public float CellSize { get => cellSize; private set => cellSize = value; }

        public GridSystem(int _width, int _height, float _cellSize)
        {
            this.width = _width;
            this.height = _height;
            this.cellSize = _cellSize;

            gridObjects = new GridObject[this.width, this.height];

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GridPosition gridPosition = new GridPosition(x,z);
                    gridObjects[x,z] = new GridObject(gridPosition, this);
                }
            }
        }

        public Vector3 GetWorldPosition(GridPosition gridPosition) => new Vector3(gridPosition.x, 0, gridPosition.z) * this.cellSize;

        public GridPosition GetGridPosition(Vector3 worldPosition)
        {
            return new GridPosition(
                Mathf.RoundToInt(worldPosition.x / cellSize),
                Mathf.RoundToInt(worldPosition.z / cellSize)
                );
        }

        public void CreateDebugObjects(Transform debugPrefab)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GridPosition gridPosition = new GridPosition(x,z);
                    Transform debugObject = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                    debugObject.GetComponent<GridDebugObject>().SetGridObject(GetGridObject(gridPosition));
                }
            }
        }

        public GridObject GetGridObject(GridPosition gridPosition)
        {
            return gridObjects[gridPosition.x, gridPosition.z];
        }

        public void TriggerGridObjectChanged(int x, int z)
        {
        }
    }

}