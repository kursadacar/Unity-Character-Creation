using UnityEngine;

namespace Michsky.UI.FieldCompleteMainMenu
{
    public class AnimatorDisableHelper : MonoBehaviour
    {
        private Animator animator;
        private AnimatorDisableHelper script;
        public bool disableScript;

        void Start()
        {
            animator = GetComponent<Animator>();
            animator.enabled = false;

            if (disableScript)
            {
                script = GetComponent<AnimatorDisableHelper>();
                script.enabled = false;
            }
        }
    }
}