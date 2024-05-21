using System;
using UnityEngine;

namespace _Scripts.Player.Arsenal
{
    [Serializable]
    public class Weapon
    {
        public GameObject prefab;
        public float fireRate;
        public bool isManual;
        [HideInInspector] public float timeToFire;

        public bool CanFire()
        {
            return timeToFire > 1f / fireRate;
        }
    }
}