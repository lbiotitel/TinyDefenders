using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    public void DisableGameObject() => this.gameObject.SetActive(false);
    public void ReturnObject() => PoolingManager.instance.ReturnObject(gameObject);

    public void GameOverUIEnd() => GameManager.instance.ChangeAllPlayerToResultScene();
}
