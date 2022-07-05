using System;
using DG.Tweening;
using UnityEngine;

namespace deVoid.UIFramework.Examples
{
    public class ScaleScreenTransition : ATransitionComponent
    {
        [SerializeField] protected bool isOutAnimation;
        [SerializeField] protected float duration = 0.5f;
        [SerializeField] protected bool doFade;
        [SerializeField] protected float fadeDurationPercent = 0.5f;
        [SerializeField] protected Ease ease = Ease.Linear;
        [SerializeField] [Range(0f, 1f)] 
        protected float xYSplit = 0.25f;

        public override void Animate(Transform target, Action callWhenFinished) {
            RectTransform rTransform = target as RectTransform;
            CanvasGroup canvasGroup = null;
            if (doFade) {
                canvasGroup = rTransform.GetComponent<CanvasGroup>();
                if (canvasGroup == null) {
                    canvasGroup = rTransform.gameObject.AddComponent<CanvasGroup>();
                }

                canvasGroup.DOFade(isOutAnimation ? 0f : 1f, duration * fadeDurationPercent);
            }

            rTransform.DOKill();
            if (isOutAnimation) {
                rTransform.DOScale(0f, duration).SetEase(ease)
                    .OnComplete(() => Cleanup(callWhenFinished, rTransform, canvasGroup))
                    .SetUpdate(true);
            }
            else {
                Sequence scaleSequence = DOTween.Sequence();
                scaleSequence.SetUpdate(true);
                rTransform.localScale = new Vector3(0f, 0.02f, 0f);

                var xScale = rTransform.DOScaleX(1f, duration * xYSplit).SetEase(ease);
                var yScale = rTransform.DOScaleY(1f, duration * 1f - xYSplit).SetEase(ease);
                scaleSequence.Append(xScale).Append(yScale).OnComplete(
                    () => Cleanup(callWhenFinished, rTransform, canvasGroup)
                ).SetUpdate(true);

                scaleSequence.Play();
            }
        }

        private void Cleanup(Action callWhenFinished, RectTransform rTransform, CanvasGroup canvasGroup) {
            callWhenFinished();
            rTransform.localScale = Vector3.one;
            if (canvasGroup != null) {
                canvasGroup.alpha = 1f;
            }
        }
    }
}