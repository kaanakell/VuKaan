using UnityEngine;
using UnityEngine.UI;

public class AdPopup : MonoBehaviour
{
    [Header("References")]
    public Image adImage;
    public Button closeButton;

    [Header("Animation")]
    public float showDuration = 0.35f;
    public float hideDuration = 0.2f;
    public float bounceOvershoot = 0.15f;

    [Header("Audio")]
    [Tooltip("Fallback volume if no per-ad volume is specified.")]
    [Range(0f, 1f)]
    public float defaultVolume = 0.8f;

    private bool _closing = false;

    private void Awake()
    {
        transform.localScale = Vector3.zero;
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
    }

    private void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(Close);
    }

    public void Show(Sprite dealSprite, AudioClip sound = null)
    {
        if (adImage != null)
            adImage.sprite = dealSprite;

        if (sound != null)
            SFXPlayer.Play(sound, defaultVolume);

        AnimateIn();
        GameEvents.AdOpened();
    }

    public void Close()
    {
        if (_closing) return;
        _closing = true;
        AnimateOut();
    }

    private void AnimateIn()
    {
        LeanTween.scale(gameObject, Vector3.one * (1f + bounceOvershoot), showDuration * 0.7f)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
                LeanTween.scale(gameObject, Vector3.one, showDuration * 0.3f)
                    .setEase(LeanTweenType.easeInQuad));
    }

    private void AnimateOut()
    {
        LeanTween.scale(gameObject, Vector3.zero, hideDuration)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() =>
            {
                GameEvents.AdClosed();
                Destroy(gameObject);
            });
    }
}
