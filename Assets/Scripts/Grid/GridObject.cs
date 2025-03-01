using System;
using UnityEngine;

namespace Aeterponis.Grid
{
    public class GridObject
    {
        private GridPosition gridPosition;
        private GridSystem masterGridSystem;
        private PlacedObject settedObject;
        public Action SettedTransformChanged;

        public GridObject(GridPosition _gridPosition, GridSystem _masterGridSystem)
        {
            this.gridPosition = _gridPosition;
            this.masterGridSystem = _masterGridSystem;
        }

        public void SetTransform(PlacedObject _placedObject)
        {
            this.settedObject = _placedObject;
            masterGridSystem.TriggerGridObjectChanged(gridPosition.x,gridPosition.z);
            SettedTransformChanged?.Invoke();
        }

        public bool CanBuild()
        {
            return settedObject == null;
        }
        public void ClearTransform()
        {
            settedObject = null;
            SettedTransformChanged?.Invoke();
        }
        public PlacedObject GetSettedObject()
        {
            return this.settedObject;
        }

        public override string ToString()
        {
            string objectName = "";

            if (settedObject != null)
                objectName = settedObject.transform.name;

            return "X: " + GridPosition.x + ", Z:" + GridPosition.z + "\n" + objectName;
        }

        public GridPosition GridPosition { get => gridPosition; set => gridPosition = value; }
    }
}