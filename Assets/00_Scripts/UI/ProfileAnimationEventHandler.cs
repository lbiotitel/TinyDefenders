using UnityEngine;

public class ProfileAnimationEventHandler : MonoBehaviour
{
    public void AnimationEnd()
    {
        MainUI.instance.profileAnimationReady = true;
    }

    public void DisableGameObject() => gameObject.SetActive(false);
}
