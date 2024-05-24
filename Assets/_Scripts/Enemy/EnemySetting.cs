using System;
using UnityEngine;

namespace _Scripts.Enemy
{
    [Serializable]
    public struct EnemySetting
    {
        public Enemy type;
        public Transform parent;
    }
}