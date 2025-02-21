using UnityEngine;
using UnityEngine.UI;

public class CombineUI : MonoBehaviour
{
    public CombineSciptable[] combineDatas;

    public Image mainCharacterImage;

    private void Start()
    {
        combineDatas = Resources.LoadAll<CombineSciptable>("CombineHeroData");
    }


}
