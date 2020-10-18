using UnityEngine;

namespace Michsky.UI.FieldCompleteMainMenu
{
    public class SwitchToMainPanels : MonoBehaviour
    {
        [Header("RESOURCES")]
        private Animator loginScreenAnimator;
        public Animator mainPanelAnimator;
        public Animator shadowsAnimator;

        [Header("SETTINGS")]
        public bool isLoginScreen;

        void Start()
        {
            loginScreenAnimator = GetComponent<Animator>();

            if (isLoginScreen == false)
            {
                loginScreenAnimator.Play("SS Fade-out");
                mainPanelAnimator.Play("Main Panel Opening");
                shadowsAnimator.Play("CG Fade-in");
            }
        }

        public void Animate()
        {
            loginScreenAnimator = GetComponent<Animator>();

            if (isLoginScreen)
            {
                loginScreenAnimator.Play("SS w Login Fade-out");
            }

            mainPanelAnimator.Play("Main Panel Opening");
            shadowsAnimator.Play("CG Fade-in");
        }
    }
}