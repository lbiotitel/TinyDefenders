using UnityEngine;
using UnityEngineInternal;

public class CameraRay : MonoBehaviour
{
    private Camera cam;
    HeroHolder heroHolder;

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
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if(heroHolder != null)
                heroHolder.HideRange();

            if (hit.collider != null)
            {
                heroHolder = hit.collider.GetComponent<HeroHolder>();
                if (heroHolder != null)
                {
                    heroHolder.ShowRange();
                }
            }
        }
    }
}
