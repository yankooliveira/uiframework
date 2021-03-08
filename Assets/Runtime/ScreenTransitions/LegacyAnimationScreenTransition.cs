using System;
using System.Collections;
using UnityEngine;

namespace deVoid.UIFramework.Examples
{
    /// <summary>
    /// I have avoided using the Legacy Animation system for ages, but since I know people
    /// will want to have hand-authored animations on their UI and I highly recommend
    /// *not* using Animator for that, both for workflow and performance reasons
    /// (ref: https://www.youtube.com/watch?v=_wxitgdx-UI&t=2883s ),
    /// I decided to add this example using the Legacy system. An alternative you can
    /// look into is the SimpleAnimationComponent 
    /// (ref: https://blogs.unity3d.com/2017/11/28/introducing-the-simple-animation-component/ )
    /// Although it still runs on top of Animator, at least it might have a simpler workflow.
    ///
    /// Word of warning: this seems to work, but was barely tested. Be careful if taking it into
    /// production :D
    /// </summary>
    public class LegacyAnimationScreenTransition : ATransitionComponent
    {
        [SerializeField] private AnimationClip clip = null;
        [SerializeField] private bool playReverse = false;

        private Action previousCallbackWhenFinished;
        
        public override void Animate(Transform target, Action callWhenFinished) {
            FinishPrevious();
            var targetAnimation = target.GetComponent<Animation>();
            if (targetAnimation == null) {
                Debug.LogError("[LegacyAnimationScreenTransition] No Animation component in " + target);
                if (callWhenFinished != null) {
                    callWhenFinished();
                }

                return;
            }

            targetAnimation.clip = clip;
            StartCoroutine(PlayAnimationRoutine(targetAnimation, callWhenFinished));
        }

        private IEnumerator PlayAnimationRoutine(Animation targetAnimation, Action callWhenFinished) {
            previousCallbackWhenFinished = callWhenFinished;
            foreach (AnimationState state in targetAnimation) {
                state.time = playReverse ? state.clip.length : 0f;
                state.speed = playReverse ? -1f : 1f;
            }

            targetAnimation.Play(PlayMode.StopAll);
            yield return new WaitForSeconds(targetAnimation.clip.length);
            FinishPrevious();
        }
        
        private void FinishPrevious() {
            if (previousCallbackWhenFinished != null) {
                previousCallbackWhenFinished();
                previousCallbackWhenFinished = null;
            }

            StopAllCoroutines();
        }
    }
}
