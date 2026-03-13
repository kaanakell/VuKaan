using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SmartphoneUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject idlePanel;
    public GameObject infoPanel;
    public GameObject foundPanel;

    [Header("Info Panel")]
    public Image infoPreview;
    public TextMeshProUGUI infoItemName;
    public TextMeshProUGUI infoDescription;
    public TextMeshProUGUI infoBrandName;
    public Image infoDealImage;

    [Header("Found Panel")]
    public Image foundImage;
    public TextMeshProUGUI foundLabel;

    [Header("Timing")]
    public float foundDisplayTime = 2.5f;

    [Header("Panel Transitions")]
    [Tooltip("How long each panel takes to scale in.")]
    public float transitionDuration = 0.2f;

    private Coroutine _returnCoroutine;
    private GameObject _activePanel;

    private void OnEnable()
    {
        GameEvents.OnGameStart += ShowIdle;
        GameEvents.OnGameReset += ShowIdle;
        GameEvents.OnItemHovered += ShowInfo;
        GameEvents.OnItemHoverExit += ShowIdle;
        GameEvents.OnItemFound += ShowFound;
        GameEvents.OnAdOpened += HideAll;
        GameEvents.OnAdClosed += ShowIdle;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStart -= ShowIdle;
        GameEvents.OnGameReset -= ShowIdle;
        GameEvents.OnItemHovered -= ShowInfo;
        GameEvents.OnItemHoverExit -= ShowIdle;
        GameEvents.OnItemFound -= ShowFound;
        GameEvents.OnAdOpened += HideAll;
        GameEvents.OnAdClosed += ShowIdle;
    }

    private void Start() => ShowIdle();

    private void ShowIdle() => TransitionTo(idlePanel);
    private void HideAll() => TransitionTo(null);

    private void ShowInfo(HiddenItem item)
    {
        var d = item?.itemData;
        if (d == null) return;

        if (infoItemName) infoItemName.text = d.itemName;
        if (infoDescription) infoDescription.text = d.description;
        if (infoBrandName) infoBrandName.text = d.brandName;
        if (infoPreview) infoPreview.sprite = d.colorSprite;

        if (infoDealImage != null)
        {
            infoDealImage.sprite = d.dealImage;
            infoDealImage.gameObject.SetActive(d.dealImage != null);
        }

        TransitionTo(infoPanel);
    }

    private void ShowFound(HiddenItem item)
    {
        var d = item?.itemData;
        if (d == null) return;

        if (foundImage) foundImage.sprite = d.colorSprite;
        if (foundLabel) foundLabel.text = $"Found: {d.itemName}";

        TransitionTo(foundPanel);

        if (_returnCoroutine != null) StopCoroutine(_returnCoroutine);
        _returnCoroutine = StartCoroutine(ReturnToIdle());
    }

    private IEnumerator ReturnToIdle()
    {
        yield return new WaitForSeconds(foundDisplayTime);
        ShowIdle();
        _returnCoroutine = null;
    }

    private void TransitionTo(GameObject target)
    {
        if (target == _activePanel) return;

        LeanTween.cancel(idlePanel);
        LeanTween.cancel(infoPanel);
        LeanTween.cancel(foundPanel);

        GameObject outgoing = _activePanel;
        _activePanel = target;

        if (outgoing != null)
        {
            LeanTween.scale(outgoing, Vector3.zero, transitionDuration * 0.5f)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() =>
            {
                outgoing.SetActive(false);
                if (target != null) ScaleIn(target);
            });
        }
        else
        {
            if (target != null) ScaleIn(target);
        }

        if (idlePanel && idlePanel != target && idlePanel != outgoing) idlePanel.SetActive(false);
        if (infoPanel && infoPanel != target && infoPanel != outgoing) infoPanel.SetActive(false);
        if (foundPanel && foundPanel != target && foundPanel != outgoing) foundPanel.SetActive(false);
    }

    private void ScaleIn(GameObject panel)
    {
        panel.SetActive(true);
        panel.transform.localScale = Vector3.zero;

        LeanTween.scale(panel, Vector3.one * 1.05f, transitionDuration * 0.7f)
        .setEase(LeanTweenType.easeOutQuad)
        .setOnComplete(() =>
        {
            LeanTween.scale(panel, Vector3.one, transitionDuration * 0.3f)
            .setEase(LeanTweenType.easeInQuad);
        });
    }
}
