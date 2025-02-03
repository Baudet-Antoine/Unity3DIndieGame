using UnityEngine;
public abstract class Item : ScriptableObject
{
    public string itemName;
    public GameObject icon;
    public int itemID;
    public float weight;
    public GameObject Model;
    public GameObject ModelGround;

    public virtual void Use()
    {
        
    }
}
