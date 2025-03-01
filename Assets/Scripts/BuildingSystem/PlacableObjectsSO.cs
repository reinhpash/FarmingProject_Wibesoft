using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Placable Object List", menuName = "Placable Object/Create a new List")]
public class PlacableObjectsSO : ScriptableObject
{
    public List<PlacableObjectSO> objectSOs = new List<PlacableObjectSO>();
}
