using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager instance { get; private set; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { 
            Destroy(gameObject);
        }
    }

    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        gameObject.SetActive(false);
    }

    public async void FadeInOut(bool isFadeIn)
    { 
        gameObject.SetActive(true);
        if(isFadeIn)await FadeInOutAsync(true);
        else await FadeInOutAsync(false);
    }

    private async Task FadeInOutAsync(bool fadeIn)
    {
        if (fadeIn)
        {
            float alpha = 0;
            canvasGroup.alpha = alpha;

            while (alpha <= 1.1f)
            {
                canvasGroup.alpha = alpha;
                alpha += Time.deltaTime;
                await Task.Yield();
            }
        }
        else
        {
            float alpha = 1.1f;
            canvasGroup.alpha = alpha;

            while (alpha >= 0)
            {
                canvasGroup.alpha = alpha;
                alpha -= Time.deltaTime;
                await Task.Yield();
            }

            gameObject.SetActive(false);
        }
    }

}
