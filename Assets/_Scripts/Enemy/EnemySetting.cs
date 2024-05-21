using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Enemy
{
    [Serializable]
    public struct EnemySetting
    {
        [FormerlySerializedAs("baseType")] public Enemy type;
        public Transform[] positions;
    }
}