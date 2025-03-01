using UnityEngine;

namespace Aeterponis.Grid
{
    public class GameGrid : MonoBehaviour
    {
        private GridSystem gridSystem;
        [SerializeField] private Transform gridDebugObjectPrefab;

        private void Awake()
        {
            gridSystem = new GridSystem(20,20,2f);

            gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
        }

        public GridPosition GetGridPositon(Vector3 touchPosition)
        {
            return gridSystem.GetGridPosition(touchPosition);
        }

        public float GetCellSize() => gridSystem.CellSize;
        public void GetGridWorldPosition(Vector3 touchPosition,out float x , out float z)
        {
            Vector3 worldPosition = gridSystem.GetWorldPosition(gridSystem.GetGridPosition(touchPosition));
            x = worldPosition.x;
            z = worldPosition.z;
        }

        public GridObject GetGridObject(int x, int z)
        {
            GridPosition target = new GridPosition(x,z);
            return gridSystem.GetGridObject(target);
        }

        public Vector3 GetWorldPosition(int x, int z)
        {
            Vector3 worldPosition = gridSystem.GetWorldPosition(new GridPosition(x,z));
            return worldPosition;
        }

    }
}