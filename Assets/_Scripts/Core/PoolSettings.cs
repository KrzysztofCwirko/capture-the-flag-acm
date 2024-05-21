using System;
using UnityEngine;

namespace _Scripts.World
{
    [Serializable]
    public struct PoolSettings
    {
        public MonoBehaviour target;
        public int count;
    }
}