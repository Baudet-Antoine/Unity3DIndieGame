using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class GameData
{
    public long lastUpdated;
    public int Balance;

    public List<Item> Inventory;

    public List<Weapon> FastInventory;

    public List<Item> SideInventory;

    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData() 
    {
        this.Balance = 0;
        this.Inventory = Enumerable.Repeat<Item>(null, 25).ToList();
        this.FastInventory = Enumerable.Repeat<Weapon>(null, 8).ToList();
        this.SideInventory = Enumerable.Repeat<Item>(null, 4).ToList();
    }
}