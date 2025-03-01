using System;
using TMPro;
using UnityEngine;

namespace Aeterponis.Grid
{
    public class GridDebugObject : MonoBehaviour
    {
        GridObject gridObject;
        [SerializeField] private TextMeshPro _gridText;

        private void Start()
        {
            gridObject.SettedTransformChanged += UpdateText;
        }

        private void OnDisable()
        {
            gridObject.SettedTransformChanged -= UpdateText;
        }

        private void UpdateText()
        {
            _gridText.SetText(gridObject.ToString());

        }

        public void SetGridObject(GridObject _gridObject)
        {
            gridObject = _gridObject;
            _gridText.SetText(gridObject.ToString());
        }
    }
}