using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Match-3/Item")]
public class Item : ScriptableObject
{
    public int value;

    public Sprite sprite;

    public string appName;
}
