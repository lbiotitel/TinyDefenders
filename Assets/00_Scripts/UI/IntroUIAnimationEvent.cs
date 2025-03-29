using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroUIAnimationEvent : MonoBehaviour
{
    public void OnIntroAnimationEnd()
    {
        SceneManager.LoadScene(SceneName.MainScene);
    }
}
