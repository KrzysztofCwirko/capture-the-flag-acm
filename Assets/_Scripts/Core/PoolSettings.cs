using System;
using UnityEngine;

namespace _Scripts.Core
{
    [Serializable]
    public struct PoolSettings
    {
        public Behaviour target;
        public int count;
    }
}