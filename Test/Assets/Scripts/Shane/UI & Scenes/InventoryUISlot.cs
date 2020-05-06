using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUISlot : MonoBehaviour
{
    [Header("Background Image")]
    public Image backgroundImage;
    public Color equippedBackgroundColour;
    private Color defaultBackgroundColour;

    [Header("Inventory Item Image")]
    public Image inventoryItemImage;

    private void Start()
    {
        defaultBackgroundColour = backgroundImage.color;
    }

    //Update Background colour (on equipping).
    //public void SwitchBackgroundColour()
    //{
    //    backgroundImage.color = backgroundImage.color == defaultBackgroundColour ? equippedBackgroundColour : defaultBackgroundColour;
    //}
    public void ChangeBackgroundColourToEquipped()
    {
        backgroundImage.color = equippedBackgroundColour;
    }
    public void ChangeBackgroundColourToUnequipped()
    {
        backgroundImage.color = defaultBackgroundColour;
    }

    //Update picture.
    public void EnableInventoryItemPicture()
    {
       inventoryItemImage.gameObject.SetActive(true);
    }
    public void DisableInventoryItemPicture()
    {
       inventoryItemImage.gameObject.SetActive(false);
    }
}
