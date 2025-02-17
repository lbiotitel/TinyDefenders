using Unity.Netcode;
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
        cam = Camera.main;
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

    private void MouseButtonDown()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (heroHolder != null)
        {
            heroHolder.HideRange();
            heroHolder = null;
        }

        if (hit.collider != null)
        {
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

            if (!isOwn) heroHolder = null;
        }
    }

    private void MouseButton()
    {
        if (heroHolder != null)
        {
            heroHolder.ShowCircle();
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

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
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

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
