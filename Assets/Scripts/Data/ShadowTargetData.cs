using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data / ShadowTargetData")]
public class ShadowTargetData : ScriptableObject
{
    public List<ShadowTarget> shadowTargetsList = new List<ShadowTarget>(); 
}

[System.Serializable]
public class ShadowTarget
{
    public string shadowTargetName;
    public GameObject shadowTargetPrefab;
}

