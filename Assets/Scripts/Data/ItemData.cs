using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "HiddenObject/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string itemName = "Unnamed Item";

    [Header("Visuals")]
    public Sprite colorSprite;
    public Sprite thumbnailSprite;

    [Header("Item Info (shown on phone screen)")]
    [TextArea(2, 5)]
    public string description = "A short flavour text description.";

    [Header("Brand Deal (shown on phone screen)")]
    public string brandName = "";
    
    [Tooltip("Deal image shown on the phone screen")]
    public Sprite dealImage;

    [Header("UI")]
    public Color accentColor = Color.white;
}
