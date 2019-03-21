using UnityEngine;
using System;

namespace deVoid.UIFramework {
    /// <summary>
    /// Screens use ATransitionComponents to animate their in and out transitions.
    /// This can be extended to use Lerps, animations etc.
    /// </summary>
    public abstract class ATransitionComponent : MonoBehaviour {
        /// <summary>
        /// Animate the specified target transform and execute CallWhenFinished when the animation is done.
        /// </summary>
        /// <param name="target">Target transform.</param>
        /// <param name="callWhenFinished">Delegate to be called when animation is finished.</param>
        public abstract void Animate(Transform target, Action callWhenFinished);
    }
}
