using UnityEngine;

public class SmartphoneCursor : MonoBehaviour
{
    public static SmartphoneCursor Instance { get; private set; }

    [Header("References")]
    public Material revealMaterial;
    public GameObject phoneVisual;
    public GameObject fingerCursor;

    [Header("Reveal")]
    public float revealRadius = 1.8f;
    public float edgeSoftness = 0.3f;

    [Header("Hover Detection")]
    public float hoverRadius = 1f;
    public LayerMask itemLayerMask;

    [Header("Feel")]
    public float smoothSpeed = 16f;
    public float cursorDepth = 1f;

    [Header("Playable Bounds (world space)")]
    [Tooltip("Enable to use explicit bounds instead of camera edges.")]
    public bool useExplicitBounds = true;

    [Tooltip("Left edge of the playable area in world space X.")]
    public float boundsLeft = -8.5f;
    [Tooltip("Right edge of the playable area in world space X.")]
    public float boundsRight = 8.5f;
    [Tooltip("Bottom edge of the playable area in world space Y.")]
    public float boundsBottom = -4.5f;
    [Tooltip("Top edge of the playable area in world space Y.")]
    public float boundsTop = 4.5f;

    private static readonly int _CursorScreenPos = Shader.PropertyToID("_CursorScreenPos");
    private static readonly int _RevealRadius = Shader.PropertyToID("_RevealRadius");
    private static readonly int _EdgeSoftness = Shader.PropertyToID("_EdgeSoftness");
    private static readonly int _AspectRatio = Shader.PropertyToID("_AspectRatio");

    private Camera _cam;
    private HiddenItem _currentHovered;
    private bool _adOpen = false;
    private bool _gameActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.OnAdOpened += HandleAdOpened;
        GameEvents.OnAdClosed += HandleAdClosed;
        GameEvents.OnGameReset += HandleGameReset;
        GameEvents.OnIntroEnd += HandleIntroEnd;
    }

    private void OnDisable()
    {
        GameEvents.OnAdOpened -= HandleAdOpened;
        GameEvents.OnAdClosed -= HandleAdClosed;
        GameEvents.OnGameReset -= HandleGameReset;
        GameEvents.OnIntroEnd -= HandleIntroEnd;
    }

    private void Start()
    {
        _cam = Camera.main;
        SetAdMode(false);
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        if (!_adOpen)
        {
            MoveToMouse();
            PushToShader();

            if (_gameActive)
            {
                UpdateHover();
                if (Input.GetMouseButtonDown(0)) TryClick();
            }
        }
        else
        {
            MoveFingerOnly();
        }
    }

    private void OnDestroy() => Cursor.visible = true;

    private void MoveToMouse()
    {
        Vector3 mouse = Input.mousePosition;
        Vector3 target = _cam.ScreenToWorldPoint(
            new Vector3(mouse.x, mouse.y, Mathf.Abs(_cam.transform.position.z)));
        target.z = cursorDepth;

        target = ClampPosition(target);

        transform.position = Vector3.Lerp(transform.position, target,
                                          Time.deltaTime * smoothSpeed);
    }

    private Vector3 ClampPosition(Vector3 worldPos)
    {
        if (useExplicitBounds)
        {
            worldPos.x = Mathf.Clamp(worldPos.x, boundsLeft, boundsRight);
            worldPos.y = Mathf.Clamp(worldPos.y, boundsBottom, boundsTop);
        }
        else
        {
            float camH = _cam.orthographicSize;
            float camW = camH * _cam.aspect;
            Vector3 c = _cam.transform.position;
            worldPos.x = Mathf.Clamp(worldPos.x, c.x - camW + 1f, c.x + camW - 1f);
            worldPos.y = Mathf.Clamp(worldPos.y, c.y - camH + 1f, c.y + camH - 1f);
        }
        return worldPos;
    }

    private void MoveFingerOnly()
    {
        if (fingerCursor == null || _cam == null) return;
        Vector3 mouse = Input.mousePosition;
        Vector3 target = _cam.ScreenToWorldPoint(
            new Vector3(mouse.x, mouse.y, Mathf.Abs(_cam.transform.position.z)));
        target.z = cursorDepth;
        fingerCursor.transform.position = target;
    }

    private void UpdateHover()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameActive) return;

        var hits = Physics2D.OverlapCircleAll(transform.position, hoverRadius, itemLayerMask);
        HiddenItem closest = null;
        float closestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            var item = hit.GetComponent<HiddenItem>();
            if (item == null || item.IsFound) continue;
            float d = Vector2.Distance(transform.position, item.WorldPosition);
            if (d < closestDist) { closestDist = d; closest = item; }
        }

        if (closest != _currentHovered)
        {
            _currentHovered?.SetHovered(false);
            _currentHovered = closest;
            _currentHovered?.SetHovered(true);
        }
    }

    private void TryClick()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameActive) return;
        _currentHovered?.OnClicked();
    }

    private void PushToShader()
    {
        if (revealMaterial == null || _cam == null) return;

        float screenHeight = _cam.orthographicSize * 2f;
        float normRadius = revealRadius / screenHeight;
        float normEdge = edgeSoftness / screenHeight;
        float aspect = (float)Screen.width / Screen.height;

        Vector3 screenPoint = _cam.WorldToScreenPoint(transform.position);
        Vector2 screenPos = new Vector2(
            screenPoint.x / Screen.width,
            screenPoint.y / Screen.height);

        revealMaterial.SetVector(_CursorScreenPos, screenPos);
        revealMaterial.SetFloat(_RevealRadius, normRadius);
        revealMaterial.SetFloat(_EdgeSoftness, normEdge);
        revealMaterial.SetFloat(_AspectRatio, aspect);
    }

    private void HandleAdOpened()
    {
        _adOpen = true;
        SetAdMode(true);
        _currentHovered?.SetHovered(false);
        _currentHovered = null;
    }

    private void HandleAdClosed()
    {
        _adOpen = false;
        SetAdMode(false);
    }

    private void HandleGameReset()
    {
        _currentHovered?.SetHovered(false);
        _currentHovered = null;
        _adOpen = false;
        _gameActive = false;
        Cursor.visible = true;
        SetAdMode(false);
    }

    private void HandleIntroEnd()
    {
        _gameActive = true;
        Cursor.visible = false;
    }
    private void SetAdMode(bool adOpen)
    {
        if (phoneVisual != null)
        {
            var sr = phoneVisual.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = adOpen ? new Color(1, 1, 1, 0.4f) : Color.white;
        }
        if (fingerCursor != null) fingerCursor.SetActive(adOpen);
    }

    private void OnDrawGizmosSelected()
    {
        if (useExplicitBounds)
        {
            Gizmos.color = Color.red;
            Vector3 centre = new Vector3(
                (boundsLeft + boundsRight) * 0.5f,
                (boundsBottom + boundsTop) * 0.5f, 0);
            Vector3 size = new Vector3(
                boundsRight - boundsLeft,
                boundsTop - boundsBottom, 0);
            Gizmos.DrawWireCube(centre, size);
        }

        Gizmos.color = new Color(0, 1, 1, 0.35f);
        Gizmos.DrawWireSphere(transform.position, hoverRadius);
        Gizmos.color = new Color(1, 1, 0, 0.2f);
        Gizmos.DrawWireSphere(transform.position, revealRadius);
    }
}
