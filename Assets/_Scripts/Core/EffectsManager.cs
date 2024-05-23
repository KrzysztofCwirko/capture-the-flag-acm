using System.Collections.Generic;
using _Scripts.Utility;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.Core
{
    public class EffectsManager : StaticInstance<EffectsManager>
    {
        [SerializeField] private List<ParticleSystem> particleSystems;
        [SerializeField] private List<AudioClip> audioClips;
        [SerializeField] private AudioSource sourcePrefab;
        
        private ParticleSystem.EmitParams _emitParams;
        /// <summary>
        /// Playing, pooled GameObject, target parent, key
        /// </summary>
        private List<(Tween, Transform, object, Behaviour)> _soundTweens = new List<(Tween, Transform, object, Behaviour)>();

        private void Start()
        {
            _emitParams = new ParticleSystem.EmitParams();
        }

        protected override void OnDestroy()
        {
            foreach (var tween in _soundTweens)
            {
                tween.Item1?.Kill();
            }

            base.OnDestroy();
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

        public void PlaySoundEffect(string key, Vector3 position, Transform parent = null, float volume = 1f)
        {
            var clip = audioClips.Find(c => c.name == key);
            if(clip == default) return;
            var pooledSource = (AudioSource)PrefabPooler.Instance.Pool(sourcePrefab, position, parent:parent);
            pooledSource.volume = volume;
            pooledSource.clip = clip;
            pooledSource.Play();

            var shared = _soundTweens.Find(t => (string)t.Item3 == key && t.Item2 == parent);
            if (shared != default)
            {
                shared.Item1?.Kill();
                _soundTweens.Remove(shared);
            }
            
            var delay = DOTween.Sequence().AppendInterval(clip.length + 0.1f);
            delay.onComplete += () =>
            {
                pooledSource.gameObject.SetActive(false);
                var t = _soundTweens.Find(t => (string)t.Item3 == key && t.Item2 == parent);
                if (t != default)
                {
                    _soundTweens.Remove(t);
                }
            };
            _soundTweens.Add((delay, parent, key, pooledSource));
        }

        public void ClearSoundEffect(string key, Transform parent)
        {
            var clip = audioClips.Find(c => c.name == key);
            if(clip == default) return;
            
            var tween = _soundTweens.Find(t => (string)t.Item3 == key && t.Item2 == parent);
            if (tween == default) return;
            tween.Item1?.Kill();
            ((AudioSource)tween.Item4).Stop();
            tween.Item4.gameObject.SetActive(false);
            _soundTweens.Remove(tween);
        }
    }
}