using UnityEngine;

public class MathUtils : MonoBehaviour
{
    public static Vector3 GenerateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + (t * t) * p2;
    }

    public static Vector3 GenerateRandomPoint(Vector3 start, Vector3 end)
    {
        Vector3 midPoint = (start + end) / 2f;
        float randomHeight = Random.Range(1.0f, 3.0f);
        midPoint += Vector3.up * randomHeight;

        midPoint += new Vector3(Random.Range(-1.0f, 1.0f), 0.0f);

        return midPoint;
    }
}
