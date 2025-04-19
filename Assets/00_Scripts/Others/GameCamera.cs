using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;


public class GameCamera : MonoBehaviour
{

    private Camera cam;
    private HeroHolder heroHolder = null;
    private HeroHolder moveHolder = null;
    private string hostOrClient = string.Empty;
    [SerializeField] private float screenShakeTime;
    [SerializeField] private float screenShakeForce;


    private void Start()
    {
        cam = GetComponent<Camera>();
        hostOrClient = NetUtils.IsHost() ? "HOST" : "CLIENT";
        EventManager.instance.onWaveNotClearAction += ScreenShake;
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

#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Time.timeScale += 0.5f;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Time.timeScale -= 0.5f;
        }
#endif
    }

    private void ScreenShake()
    {
        StartCoroutine(ScreenShakeCouroutine());
    }

    private IEnumerator ScreenShakeCouroutine()
    {
        float shakeTime = 0f;
        float shakeForce = screenShakeForce;
        Vector3 originPos = transform.position;
        while (shakeTime < screenShakeTime)
        { 
            shakeTime += Time.deltaTime;
            shakeForce -= Time.deltaTime;
            transform.position = originPos + Random.insideUnitSphere * shakeForce;

            yield return null;
        }

        transform.position = originPos;
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
