using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;


public class GameCamera : NetworkBehaviour
{
    [SerializeField] private BoxCollider2D cameraClampCollider;
    private Vector2 cameraRange;
    [SerializeField] private float cameraSpeed;

    [SerializeField] private float maxZoomValue;
    [SerializeField] private float minZoomValue;
    [SerializeField] private float zoomSpeed;

    private Camera cam;
    private HeroHolder heroHolder = null;
    private HeroHolder moveHolder = null;
    private string hostOrClient;


    private void Start()
    {
        cam = GetComponent<Camera>();
        hostOrClient = NetUtils.IsServer() ? "HOST" : "CLIENT";

    }
    //ray cast
    private void Update()
    {
        CheckTouchHero();
        CheckCameraRange();
        CameraZoom();
    }

    private void FixedUpdate()
    {
        CameraMovement();
    }


    private void CameraMovement()
    {
        Vector2 newCameraPos = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            newCameraPos.y = 1;
        if (Input.GetKey(KeyCode.S))
            newCameraPos.y = -1;
        if (Input.GetKey(KeyCode.D))
            newCameraPos.x = 1;
        if (Input.GetKey(KeyCode.A))
            newCameraPos.x = -1;

        Vector3 moveDir = newCameraPos.normalized;
        Vector3 newPos = transform.position + moveDir * cameraSpeed;

        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * 2f);
    }

    private void CameraZoom()
    {
        float zoomDir = Input.mouseScrollDelta.y;
        float destination = cam.orthographicSize + (zoomDir * zoomSpeed);
        destination = Mathf.Clamp(destination, minZoomValue, maxZoomValue);
        cam.orthographicSize = destination;
    }

    private void CheckCameraRange()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, cameraClampCollider.bounds.min.x,
            cameraClampCollider.bounds.max.x),
            Mathf.Clamp(transform.position.y, cameraClampCollider.bounds.min.y,
            cameraClampCollider.bounds.max.y),
            transform.position.z);

    }

    private void CheckTouchHero()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            MouseButtonDown();
        }

        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            MouseButton();
        }

        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            MouseButtonUp();
        }
    }

    private RaycastHit2D GetRayObject()
    {
        Vector3 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, transform.forward, 100, LayerMask.GetMask("HeroHolder"));
        return hit;
    }


    #region Hero Control Logic
    private void MouseButtonDown()
    {
        InGameUI.instance.HideHeroInfoUI();

        if (heroHolder != null)
        {
            heroHolder.HideHeroRange();
            heroHolder = null;
        }

        RaycastHit2D hit = GetRayObject();

        if (hit.collider != null)
        {
            if (hit.collider.GetComponent<HeroHolder>() == null)
            {
                heroHolder = null;
                return;
            }

            heroHolder = hit.collider.GetComponent<HeroHolder>();

            if (heroHolder.holderName == "" || heroHolder.holderName == null)
            {
                heroHolder = null;
                return;
            }

            bool isOwn = false;

            ulong value = NetworkManager.Singleton.LocalClientId;
            if (value == 0) isOwn = heroHolder.holderId.Contains("HOST");
            else if (value == 1) isOwn = heroHolder.holderId.Contains("CLIENT");

            if (!isOwn)
            {
                heroHolder = null;
                return;
            }

            heroHolder.ShowOwnMark();
            InGameUI.instance.ShowHeroInfoUI(heroHolder.ownHero);
        }
    }

    private void MouseButton()
    {
        if (heroHolder != null)
        {
            RaycastHit2D hit = GetRayObject();

            if (hit.collider != null && hit.collider.transform != heroHolder.transform)
            {
                if (hit.collider.GetComponent<HeroHolder>() == null) return;
                if (hit.collider.GetComponent<HeroHolder>().holderId.Contains(hostOrClient) == false) return;

                if (moveHolder != null) moveHolder.HideHeroMoveTargetMark();
                moveHolder = hit.collider.GetComponent<HeroHolder>();
                moveHolder.ShowHeroMoveTargetMark();
            }
        }
    }

    private void MouseButtonUp()
    {
        if (heroHolder == null) return;

        if (moveHolder != null)
        {
            moveHolder.HideHeroMoveTargetMark();
            heroHolder.HideHeroRange();
            heroHolder.HideOwnMark();
            Spawner.instance.HeroHolderSwap(heroHolder.holderId, moveHolder.holderId);
        }
        else
        {
            heroHolder.HideOwnMark();
            heroHolder.ShowHeroRange();
            InGameUI.instance.ShowHeroInfoUI(heroHolder.ownHero);
        }

        moveHolder = null;
    }

    #endregion
}
