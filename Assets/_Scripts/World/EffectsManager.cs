using System;
using System.Collections.Generic;
using _Scripts.Utility;
using Cinemachine;
using UnityEngine;

namespace _Scripts.World
{
    public class EffectsManager : StaticInstance<EffectsManager>
    {
        [SerializeField] private List<CinemachineImpulseSource> impulseSources;
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
        public void ShowParticle(string key, Vector3 position)
        {
            var particle = particleSystems.Find(p => p.name == key);
            if(particle == default) return;
            _emitParams.position = position;
            particle.Emit(_emitParams, 1);
        }

        /// <summary>
        /// Shake the camera
        /// </summary>
        /// <param name="key">Name of the effect</param>
        public void MakeImpulse(string key)
        {
            var impulse = impulseSources.Find(p => p.name == key);
            if(impulse == default) return;
            impulse.GenerateImpulse();
        }
    }
}