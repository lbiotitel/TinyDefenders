using UnityEngine;

public class ProfileAnimationEventHandler : MonoBehaviour
{
    public void AnimationEnd()
    {
        InGameUI.instance.profileAnimationReady = true;
    }

    public void DisableGameObject() => gameObject.SetActive(false);
}
