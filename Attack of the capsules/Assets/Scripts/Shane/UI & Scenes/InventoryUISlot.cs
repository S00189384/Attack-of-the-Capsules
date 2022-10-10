using UnityEngine;
using UnityEngine.UI;

//UI Slot Item on bottom of screen as part of UI.
//On equipping weapon its image is added to its inventory slot.
//When weapon is equipped the background changes to green.
//UIBehaviour script has an array of this object - when weapon is equipped or picked up - this array at index of weapon index is updated.
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
