using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;


public class GameCamera : MonoBehaviour
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
    private string hostOrClient = string.Empty;


    private void Start()
    {
        cam = GetComponent<Camera>();
        hostOrClient = NetUtils.IsHost() ? "HOST" : "CLIENT";
    }
    //ray cast
    private void Update()
    {
        CheckClickHero();
    }


    private void CheckClickHero()
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

            if (!IsOwnHolder(heroHolder))
            {
                heroHolder = null;
                return;
            }

            heroHolder.ShowOwnMark();
        }
    }

    private bool IsOwnHolder(HeroHolder targetHeroHolder)
    {
        bool isOwn = false;

        PlayerNetworkType playerNetworkType = NetUtils.GetPlayerNetworkType();
        if (playerNetworkType == PlayerNetworkType.HOST) isOwn = targetHeroHolder.holderId.Contains("HOST");
        else if (playerNetworkType == PlayerNetworkType.CLIENT) isOwn = targetHeroHolder.holderId.Contains("CLIENT");

        return isOwn;
    }

    private void MouseButton()
    {
        if (heroHolder != null)
        {
            RaycastHit2D hit = GetRayObject();

            if (hit.collider != null && hit.collider.transform != heroHolder.transform)
            {
                InGameUI.instance.HideHeroInfoUI();
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
            SpawnManager.instance.HeroHolderSwap(heroHolder.holderId, moveHolder.holderId);
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
