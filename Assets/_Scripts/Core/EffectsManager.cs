using System.Collections.Generic;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.Core
{
    public class EffectsManager : StaticInstance<EffectsManager>
    {
        [SerializeField] private List<ParticleSystem> particleSystems;

        private ParticleSystem.EmitParams _emitParams;

        private void Start()
        {
            _emitParams = new ParticleSystem.EmitParams();
        }

        /// <summary>
        /// Emit once at position
        /// </summary>
        /// <param name="key">Name of the effect</param>
        /// <param name="position">Spawn point</param>
        /// <param name="count">;)</param>
        /// <param name="rotation">Rotation</param>
        public void ShowParticle(string key, Vector3 position, int count = 1, Vector3 rotation = new Vector3())
        {
            var particle = particleSystems.Find(p => p.name == key);
            if(particle == default) return;
            _emitParams.position = position;
            _emitParams.rotation3D = rotation;
            particle.Emit(_emitParams, count);
        }
    }
}