using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


public class CameraRay : NetworkBehaviour
{
    private Camera cam;
    HeroHolder heroHolder = null;
    HeroHolder moveHolder = null;
    string hostOrClient;

    private void Start()
    {
        cam = GetComponent<Camera>();
        hostOrClient = NetUtils.GetLocalD() == 0 ? "HOST" : "CLIENT";
    }
    //ray cast
    private void Update()
    {
        CheckTouchHero();
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
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;  // Ray가 나갈 고정 거리

        // Orthographic 카메라 기준으로 월드 좌표 변환
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);

        // 2D Raycast 실행 (FOV 영향 없음!)
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        return hit;
    }

    private void MouseButtonDown()
    {
        RaycastHit2D hit = GetRayObject();

        if (heroHolder != null)
        {
            heroHolder.HideRange();
            heroHolder = null;
            InGameUI.instance.HideHeroInfoUI();
        }

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
            if(value == 0) isOwn = heroHolder.holderId.Contains("HOST");
            else if(value == 1) isOwn = heroHolder.holderId.Contains("CLIENT");

            if (!isOwn)
            {
                heroHolder = null;
                return;
            }
            InGameUI.instance.ShowHeroInfoUI(heroHolder.ownHero);
        }
    }

    private void MouseButton()
    {
        if (heroHolder != null)
        {
            heroHolder.ShowCircle();
            RaycastHit2D hit = GetRayObject();

            if (hit.collider != null && hit.collider.transform != heroHolder.transform)
            {
                if (hit.collider.GetComponent<HeroHolder>() == null) return;
                if (hit.collider.GetComponent<HeroHolder>().holderId.Contains(hostOrClient) == false)
                {
                    return;
                }
                if (moveHolder != null)
                {
                    moveHolder.HideSquare();
                }
                moveHolder = hit.collider.GetComponent<HeroHolder>();
                moveHolder.ShowSquare();
            }
        }
    }

    private void MouseButtonUp()
    {
        if (heroHolder == null) return;
        if (moveHolder == null)
        {
            RaycastHit2D hit = GetRayObject();

            if (hit.collider != null)
            {
                if (heroHolder.transform == hit.collider.transform)
                {
                    heroHolder.ShowRange();
                }
            }
        }
        else
        {
            moveHolder.HideSquare();
            Spawner.instance.HeroHolderSwap(heroHolder.holderId,moveHolder.holderId);
        }

        if (heroHolder != null)
            heroHolder.HideCircle();

        moveHolder = null;
    }


}
