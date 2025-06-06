using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;

public class BuildSystem : MonoBehaviour
{
    public static BuildSystem Instance;

    [SerializeField] private Animator buildPanelAnimator;
    [SerializeField] private ModuleInfoPanel moduleInfoPanel;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private Color validColor = new Color(0, 1, 0, 0.7f);
    [SerializeField] private Color invalidColor = new Color(1, 0, 0, 0.7f);
    [SerializeField] private Color destroyHighlightColor = new Color(1, 0.5f, 0.5f, 0.7f);

    private PhotonView photonView;
    private PostProcessVolume volume;
    private bool isBuildMode = false;
    private bool isDestroyMode = false;
    private ModuleInfo currentModule;
    private GameObject previewObject;
    private SpriteRenderer previewRenderer;
    private Collider2D previewCollider;
    private GameObject highlightedObject;

    private void Awake()
    {
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    private Vector3 mouseDownPosition;
    private float mouseDownTime;
    private const float MAX_CLICK_DISTANCE = 15f;
    private const float MIN_CLICK_TIME = 0.15f;

    private void Update()
    {
        if (isBuildMode && currentModule != null)
        {
            UpdateBuildMode();
        }
        else if (isDestroyMode)
        {
            UpdateDestroyMode();
        }
    }

    private void UpdateBuildMode()
    {
        Vector3 gridPosition = GetGridPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (previewObject == null)
        {
            CreatePreviewObject();
        }

        previewObject.transform.position = gridPosition;

        bool canPlace = CheckPlacementValid();
        UpdatePreviewColor(canPlace && EnoughMaterials(currentModule));

        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPosition = Input.mousePosition;
            mouseDownTime = Time.time;
        }

        if (Input.GetMouseButtonUp(0) && !IsPointerOverBlockingUI() && EnoughMaterials(currentModule))
        {
            float mouseMoveDistance = Vector3.Distance(mouseDownPosition, Input.mousePosition);
            float clickDuration = Time.time - mouseDownTime;

            if (mouseMoveDistance <= MAX_CLICK_DISTANCE && clickDuration <= MIN_CLICK_TIME)
            {
                if (canPlace)
                {
                    photonView.RPC("InstantiateRoomObject", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.UserId, currentModule.prefab.name, gridPosition, Quaternion.identity);
                    PayForBuilding(currentModule);
                }
                else
                    CameraEffects.Instance.ShakeCamera(0.075f, 0.2f);
            }
        }
    }

    [PunRPC]
    private void InstantiateRoomObject(string userId, string prefabName, Vector3 position, Quaternion rotation)
    {
        Module moduleComponent = PhotonNetwork.InstantiateRoomObject(prefabName, position, rotation).GetComponent<Module>();
        moduleComponent.Info = currentModule;
        moduleComponent.Initialize(userId);
    }

    private void UpdateDestroyMode()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Destroyable"))
        {
            if (highlightedObject != hit.collider.gameObject)
            {
                ClearHighlight();
                highlightedObject = hit.collider.gameObject;
                var renderer = highlightedObject.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.color = destroyHighlightColor;
                }
            }
        }
        else
        {
            ClearHighlight();
        }

        if (Input.GetMouseButtonDown(0) && !IsPointerOverBlockingUI() && highlightedObject != null)
        {
            DestroyModule(highlightedObject);
            ClearHighlight();
        }
    }

    private void ClearHighlight()
    {
        if (highlightedObject != null)
        {
            var renderer = highlightedObject.GetComponent<SpriteRenderer>();
            var module = highlightedObject.GetComponent<Module>();

            if (renderer != null && module != null)
            {
                module.NormalizeColor();
            }
            highlightedObject = null;
        }
    }

    private void DestroyModule(GameObject moduleObject)
    {
        Module moduleComponent = moduleObject.GetComponent<Module>();
        if (moduleComponent != null && moduleComponent.Info != null)
        {
            if (moduleComponent.OwnerId != PhotonNetwork.LocalPlayer.UserId)
                return;

            foreach (var material in moduleComponent.Info.PriceList)
            {
                int refundAmount = Mathf.CeilToInt(material.Value * 0.70f);
                GameInventory.Balance.IncreaseCount(material.Key.ToString(), refundAmount);
            }
            
            GameInventory.Instance.UpdateUI();
        }

        PhotonNetwork.Destroy(moduleObject);
        AvtoColors.Instance.AutoColor();
    }

    private void PayForBuilding(ModuleInfo module)
    {
        foreach (var material in module.PriceList)
            GameInventory.Balance.DecreaseCount(material.Key.ToString(), material.Value);

        GameInventory.Instance.UpdateUI();
    }

    private bool EnoughMaterials(ModuleInfo module)
    {
        foreach (var material in module.PriceList)
            if (!GameInventory.Balance.HasEnough(material.Key.ToString(), material.Value))
                return false;

        return true;
    }

    private void CreatePreviewObject()
    {
        previewObject = Instantiate(currentModule.prefab);
        previewObject.GetComponent<SpriteRenderer>().sortingOrder = 10;
        previewRenderer = previewObject.GetComponent<SpriteRenderer>();
        previewCollider = previewObject.GetComponent<Collider2D>();

        if (previewCollider != null) previewCollider.enabled = false;
        if (previewRenderer != null) previewRenderer.color = validColor;
    }

    private bool CheckPlacementValid()
    {
        if (previewCollider == null) return true;

        previewCollider.enabled = true;
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        Collider2D[] results = new Collider2D[10];

        int overlapCount = previewCollider.Overlap(filter, results);
        previewCollider.enabled = false;

        return overlapCount == 0;
    }

    private void UpdatePreviewColor(bool isValid)
    {
        if (previewRenderer != null)
        {
            previewRenderer.color = isValid ? validColor : invalidColor;
        }
    }

    private Vector3 GetGridPosition(Vector3 rawPosition)
    {
        float x = Mathf.Round(rawPosition.x / gridSize) * gridSize;
        float y = Mathf.Round(rawPosition.y / gridSize) * gridSize;
        return new Vector3(x, y, 0);
    }

    public void SwitchBuildMode()
    {
        isBuildMode = !isBuildMode;
        isDestroyMode = false;
        volume.profile.TryGetSettings(out ColorGrading colorGrading);
        colorGrading.active = isBuildMode;

        if (isBuildMode)
        {
            buildPanelAnimator.SetTrigger("OPEN");
        }
        else
        {
            buildPanelAnimator.SetTrigger("CLOSE");
            CancelModuleSelection();
        }
    }

    public void ChooseModule(ModuleInfo module)
    {
        if(currentModule == module)
        {
            CancelModuleSelection();
            return;
        }
        CancelModuleSelection();
        ModuleInfoPanel.Instance.gameObject.SetActive(true);
        currentModule = module;
        ModuleInfoPanel.Instance.SetInfo(module, GameInventory.Balance);
    }

    private void CancelModuleSelection()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
            previewRenderer = null;
            previewCollider = null;
        }

        ModuleInfoPanel.Instance.gameObject.SetActive(false);
        currentModule = null;
    }

    public void SwitchDestroyMode()
    {
        isDestroyMode = !isDestroyMode;
        CancelModuleSelection();
        ClearHighlight();
    }

    private void Start()
    {
        volume = Camera.main.GetComponent<PostProcessVolume>();
    }

    private bool IsPointerOverBlockingUI()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            return false;

        PointerEventData pointerData = new(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (!result.gameObject.CompareTag("AllowBuildClick"))
                return true;
        }

        return false;
    }
}