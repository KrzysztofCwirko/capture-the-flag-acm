using System;
using UnityEngine;

namespace _Scripts.Core
{
    [Serializable]
    public struct PoolSettings
    {
        public MonoBehaviour target;
        public int count;
    }
}