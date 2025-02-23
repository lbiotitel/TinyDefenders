using UnityEngine;

public class ProfileAnimationEventHandler : MonoBehaviour
{
    public void AnimationEnd()
    { 
        MainUI.instance.profileAnimationReady = true;
    }
}
