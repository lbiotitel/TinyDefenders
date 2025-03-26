using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CombineSciptable", menuName = "Scriptable Objects/CombineSciptable")]
public class CombineSciptable : ScriptableObject
{
    public HeroScriptable mainHeroData;
    public List<HeroScriptable> subHeroDatas;

}
