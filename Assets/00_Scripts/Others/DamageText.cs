using System.Collections;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private float floatSpeed;
    [SerializeField] private float riseDuration = 1.0f;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, 0);
    [SerializeField] private float randX;
    [SerializeField] private TextMeshPro damageText;
    [SerializeField] private GameObject criticalParticle;
    private Color textColor;

    public void InitDamageText(double damage, bool cri = false)
    {
        damageText.text = string.Format("{0:0}", damage);
        Color color = damageText.color;
        color = Color.white;
        color.a = 1f;
        damageText.color = color;
        textColor = damageText.color;
        if (cri)
        {
            damageText.color = Color.red;
            criticalParticle.SetActive(true);
        }
        StartCoroutine(ShowAndFade());
    }


    IEnumerator ShowAndFade()
    {
        offset.x = UnityEngine.Random.Range(-randX,randX);
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + offset;

        float elapsedTime = 0;

        while (elapsedTime < riseDuration)
        {
            transform.position = Vector3.Lerp(startPosition,endPosition,elapsedTime / riseDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            textColor.a = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            damageText.color = textColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        criticalParticle.SetActive(false);
        PoolingManager.instance.ReturnObject(gameObject);
    }

}
