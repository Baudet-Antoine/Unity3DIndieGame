using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Upgrade")]
public class Upgrade : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Image;
    public float Value;
    public string Rarity;
}
