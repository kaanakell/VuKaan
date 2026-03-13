using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class HiddenItem : MonoBehaviour
{
    [Header("Data")]
    public ItemData itemData;

    [Header("Found FX")]
    public GameObject foundParticlesPrefab;
    public float foundAnimDuration = 0.45f;

    [Header("Found Sound")]
    [Tooltip("Clip played when this item is collected.")]
    public AudioClip foundSoundClip;

    [Tooltip("Volume for the collect sound.")]
    [Range(0f, 1f)]
    public float foundSoundVolume = 1f;

    public bool IsFound { get; private set; }
    public bool IsBeingHovered { get; private set; }
    public Vector3 WorldPosition => transform.position;

    private SpriteRenderer _sr;
    private Collider2D _col;
    private Vector3 _originalScale;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _col = GetComponent<Collider2D>();
        _originalScale = transform.localScale;

        if (itemData?.colorSprite != null)
            _sr.sprite = itemData.colorSprite;
    }

    public void SetHovered(bool state)
    {
        if (IsFound) return;
        IsBeingHovered = state;
        if (state) GameEvents.ItemHovered(this);
        else GameEvents.ItemHoverExit();
    }

    public void OnClicked()
    {
        if (IsFound) return;
        StartCoroutine(FoundSequence());
    }

    public void ResetItem()
    {
        StopAllCoroutines();
        IsFound = IsBeingHovered = false;
        _col.enabled = true;
        transform.localScale = _originalScale;
        _sr.color = Color.white;
        SetLayerRecursive(gameObject, LayerMask.NameToLayer("HiddenItems"));

        if (itemData?.colorSprite != null)
            _sr.sprite = itemData.colorSprite;
    }

    private IEnumerator FoundSequence()
    {
        IsFound = true;
        _col.enabled = false;

        GameEvents.ItemFound(this);

        if (foundParticlesPrefab != null)
            Instantiate(foundParticlesPrefab, transform.position, Quaternion.identity);

        if (foundSoundClip != null)
            SFXPlayer.Play(foundSoundClip, foundSoundVolume);

        float elapsed = 0f;
        while (elapsed < foundAnimDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / foundAnimDuration;
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.3f;
            transform.localScale = _originalScale * scale;
            yield return null;
        }
        transform.localScale = _originalScale;

        SetLayerRecursive(gameObject, LayerMask.NameToLayer("FoundItem"));
    }

    private static void SetLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursive(child.gameObject, layer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if (TryGetComponent<Collider2D>(out var c))
            Gizmos.DrawWireCube(transform.position, c.bounds.size);
    }
}
