using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


public delegate void OnGoldGetEventHandler();
public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { 
            Destroy(this.gameObject);
        }
    }


    //config
    [field:SerializeField]public int maxMonsterCount { get; private set; }
    [field: SerializeField] public int gold { get; set; }
    [field: SerializeField] public int summonGold { get; set; }
    [field: SerializeField] public int summonGoldInc { get; set; }

   //event
    public event OnGoldGetEventHandler onGoldGet;

    private List<Monster> monsterList = new List<Monster>();




    public void AddMonster(Monster monster) { monsterList.Add(monster); }
    public void RemoveMonster(Monster monster) { monsterList.Remove(monster); }
    public int GetMonsterCount() { return monsterList.Count; }
    public void GetGold(int gold)
    {
        onGoldGet?.Invoke();
        this.gold += gold;
    }
    

}
