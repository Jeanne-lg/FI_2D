using UnityEngine;

[System.Serializable]
public class ShopItem
{
    public string itemName;
    public int price;
    public int atkBonus;
    public int defBonus;
    public bool isPurchased = false;
}
