using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public string description;
    public GameObject prefab;
    public bool isStackable;

    // Define the type of effect
    public enum ItemEffect
    {
        Heal,
        DamageBoost,
        SpeedBoost
    }
    public ItemEffect effectType;

    // Effect strength (e.g., healing amount, boost percentage)
    public float effectValue;

    // Apply the effect (can be extended for custom logic)
    public void Use(GameObject user)
    {
        switch (effectType)
        {
            case ItemEffect.Heal:

                Debug.Log("You healed");
                break;
            case ItemEffect.DamageBoost:
                Debug.Log("Enerry Increased");
                break;
            case ItemEffect.SpeedBoost:
               
                break;
        }
    }
}
