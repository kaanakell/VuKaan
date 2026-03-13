using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanelUI : MonoBehaviour
{
    public Image thumbnail;
    public TextMeshProUGUI nameText;
    public GameObject foundOverlay;
    public GameObject checkmark;

    public void Setup(ItemData data)
    {
        nameText.text = data.itemName;
        thumbnail.sprite = data.thumbnailSprite != null ? data.thumbnailSprite : data.colorSprite;
        foundOverlay.SetActive(false);
        checkmark.SetActive(false);
    }

    public void MarkFound()
    {
        checkmark.SetActive(true);
        foundOverlay.SetActive(true);
    }
}
