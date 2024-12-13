using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // Reference to the Item scriptable object

    private void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.icon;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        Inventory inventory = collision.GetComponent<Inventory>();
        if (inventory != null && inventory.AddItem(item))
        {
            Destroy(gameObject); // Remove the item from the world

            AchievementManager.Instance.UnlockAchievement("Pick Up Item");
        }
    }
}
