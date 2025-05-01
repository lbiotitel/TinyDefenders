using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WideAreaProjectile : Projectile
{
    [Header("Wide Area Value")]
    [SerializeField] private float _range;
    [SerializeField] private int _targetCount;
    public int TargetCount { get { return _targetCount; } set { _targetCount = value; } }

  
}
