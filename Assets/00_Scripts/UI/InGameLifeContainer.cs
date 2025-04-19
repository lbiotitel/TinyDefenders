using UnityEngine;
using UnityEngine.UI;

public class InGameLifeContainer : MonoBehaviour
{
    [SerializeField] private Image[] lifeNodes;




    public void ShowLifeNode(int life)
    {
        life = Mathf.Clamp(life, 0, lifeNodes.Length - 1);
        lifeNodes[life].gameObject.SetActive(false);
    }

}
