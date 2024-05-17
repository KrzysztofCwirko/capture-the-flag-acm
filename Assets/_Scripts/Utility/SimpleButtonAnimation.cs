using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Utility
{
    [RequireComponent(typeof(Button))]
    public class SimpleButtonAnimation : MonoBehaviour
    {
        public Transform target;
        [SerializeField] private float maxJump = 1.1f;
        [SerializeField] private float minJump = 0.8f;

        private const float SimpleAnimationTime = .1f;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(PressMe);
            if (!target) target = transform;
        }

        private void PressMe()
        {
            DOTween.Sequence()
            .Append(target.DOScale(new Vector3(minJump, minJump), SimpleAnimationTime).SetEase(Ease.Linear))
            .Append(target.DOScale(new Vector3(maxJump, maxJump), SimpleAnimationTime).SetEase(Ease.Linear))
            .Append(target.DOScale(Vector3.one, SimpleAnimationTime).SetEase(Ease.Linear)).SetEase(Ease.Linear);
        }
    }
}
