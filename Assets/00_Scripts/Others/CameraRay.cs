using UnityEngine;

public class CameraRay : MonoBehaviour
{
    private Camera cam;
    HeroHolder heroHolder = null;
    HeroHolder moveHolder = null;

    private void Start()
    {
        cam = Camera.main;
    }
    //ray cast
    private void Update()
    {
        CheckTouchHero();
    }

    private void CheckTouchHero()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MouseButtonDown();
        }

        if (Input.GetMouseButton(0))
        {
            MouseButton();
        }

        if (Input.GetMouseButtonUp(0))
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
            Spawner.HeroHolderSwap(heroHolder,moveHolder);
        }

        if (heroHolder != null)
            heroHolder.HideCircle();

        moveHolder = null;
    }

}
