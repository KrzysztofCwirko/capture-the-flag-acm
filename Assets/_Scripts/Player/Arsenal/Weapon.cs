using System;
using UnityEngine;

namespace _Scripts.Player.Arsenal
{
    [Serializable]
    public class Weapon
    {
        public GameObject prefab;
        public float fireRate;
        public float maxDistance;
        public Vector3 moveOnAttack;
        public Vector3 rotateOnAttack;
        public Transform shootingEffect;
        public float TimeToFire { get; set; }

        public bool CanFire()
        {
            return TimeToFire > 1f / fireRate;
        }
    }
}