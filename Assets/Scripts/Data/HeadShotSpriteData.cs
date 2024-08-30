using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Data / HeadShotSpritesData")]
public class HeadShotSpriteData : ScriptableObject
{
    public List<HeadShotSprite> headShotSpritesList = new List<HeadShotSprite>();
}

[System.Serializable]
public class HeadShotSprite
{
    public string name;
    public Sprite headShotSprite;
}
