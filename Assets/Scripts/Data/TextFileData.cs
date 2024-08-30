using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data / TextFileData")]
public class TextFileData : ScriptableObject
{
    public List<TextFile> textFilesList = new List<TextFile>();
}

[System.Serializable]
public class TextFile
{
    public string fileName;
    public TextAsset textAsset;
}

