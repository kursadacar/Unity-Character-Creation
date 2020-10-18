using UnityEngine;

namespace Michsky.UI.FieldCompleteMainMenu
{
    public class SwitchManager : MonoBehaviour
    {
        [Header("SWITCH")]
        public bool isOn;
        public Animator switchAnimator;

        private string onTransition = "Switch On";
        private string offTransition = "Switch Off";

        void Start()
        {
            if (isOn)
            {
                switchAnimator.Play(onTransition);
            }

            else
            {
                switchAnimator.Play(offTransition);
            }
        }

        public void AnimateSwitch()
        {
            if (isOn)
            {
                switchAnimator.Play(offTransition);
                isOn = false;
            }

            else
            {
                switchAnimator.Play(onTransition);
                isOn = true;
            }
        }
    }
}