using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroCardUI : MonoBehaviour
{
    [Header("References")]
    public GameObject cardBox;
    public TMP_Text titleText;
    public TMP_Text bodyText;
    public Button startButton;

    [Header("Item List (optional)")]
    public Transform itemListParent;
    public GameObject itemRowPrefab;

    [Header("Content")]
    [TextArea(3, 8)]
    public string titleString = "Time to Get Ready!";

    [TextArea(5, 12)]
    public string bodyString =
        "You're late for a party and your room is a mess.\n\n" +
        "Use the <b>Nutik App</b> on your phone to scan the room " +
        "and find all your missing items.\n\n" +
        "<size=85%><color=#AAAAAA>" +
        "Move your phone with the <b>mouse</b>.\n" +
        "Click an item to collect it.\n" +
        "Watch out for pop-up ads!" +
        "</color></size>";

    [Header("Animation")]
    public float animDuration = 0.4f;

    private void Awake()
    {
        if (cardBox != null)
            cardBox.transform.localScale = Vector3.zero;

        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);
    }

    private void OnEnable()
    {
        GameEvents.OnGameStart += HandleGameStart;
        GameEvents.OnGameReset += HandleGameReset;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStart -= HandleGameStart;
        GameEvents.OnGameReset -= HandleGameReset;
    }

    private void HandleGameStart()
    {
        gameObject.SetActive(true);
        Cursor.visible = true;
        PopulateText();
        PopulateItemList();
        AnimateIn();
    }

    private void HandleGameReset()
    {
        LeanTween.cancel(cardBox);
        if (cardBox != null) cardBox.transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
        Cursor.visible = false;
    }

    private void OnStartClicked()
    {
        AnimateOut(() =>
        {
            gameObject.SetActive(false);
            Cursor.visible = false;
            GameEvents.IntroEnd();
        });
    }

    private void PopulateText()
    {
        if (titleText) titleText.text = titleString;
        if (bodyText) bodyText.text = bodyString;
    }

    private void PopulateItemList()
    {
        if (itemListParent == null || itemRowPrefab == null) return;
        foreach (Transform child in itemListParent) Destroy(child.gameObject);

        var gm = GameManager.Instance;
        if (gm == null) return;

        foreach (var item in gm.allItems)
        {
            if (item.itemData == null) continue;
            var row = Instantiate(itemRowPrefab, itemListParent);
            var icon = row.GetComponentInChildren<Image>();
            var nameText = row.GetComponentInChildren<TMP_Text>();
            if (icon && item.itemData.colorSprite) icon.sprite = item.itemData.colorSprite;
            if (nameText) nameText.text = item.itemData.itemName;
        }
    }

    private void AnimateIn()
    {
        if (cardBox == null) return;
        LeanTween.cancel(cardBox);
        cardBox.transform.localScale = Vector3.zero;
        LeanTween.scale(cardBox, Vector3.one * 1.04f, animDuration * 0.75f)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
                LeanTween.scale(cardBox, Vector3.one, animDuration * 0.25f)
                    .setEase(LeanTweenType.easeInQuad));
    }

    private void AnimateOut(System.Action onComplete)
    {
        if (cardBox == null) { onComplete?.Invoke(); return; }
        LeanTween.cancel(cardBox);
        LeanTween.scale(cardBox, Vector3.zero, animDuration * 0.6f)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() => onComplete?.Invoke());
    }
}
