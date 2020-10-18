using UnityEngine;

namespace Michsky.UI.FieldCompleteMainMenu
{
    public class SplashScreenManager : MonoBehaviour
    {
        [Header("RESOURCES")]
        public GameObject splashScreen;
        public GameObject splashScreenLogin;
        public GameObject splashScreenRegister;
        public GameObject mainPanels;
        private Animator mainPanelsAnimator;

        BlurManager bManager;

        [Header("SETTINGS")]
        public bool isLoggedIn;
        public bool alwaysShowLoginScreen = true;
        public bool disableSplashScreen;
        public bool enableBlurSystem = true;

        void Start()
        {
            if(enableBlurSystem)
            {
                bManager = gameObject.GetComponent<BlurManager>();
                bManager.BlurInAnim();
            }

            if (disableSplashScreen)
            {
                splashScreen.SetActive(false);
                splashScreenLogin.SetActive(false);
                splashScreenRegister.SetActive(false);
                mainPanels.SetActive(true);

                mainPanelsAnimator = mainPanels.GetComponent<Animator>();
                mainPanelsAnimator.Play("Main Panel Opening");
            }

            else if (isLoggedIn == false && alwaysShowLoginScreen)
            {
                splashScreen.SetActive(false);
                splashScreenLogin.SetActive(true);
                splashScreenRegister.SetActive(true);
            }

            else if (isLoggedIn == false && alwaysShowLoginScreen == false)
            {
                splashScreen.SetActive(false);
                splashScreenLogin.SetActive(true);
                splashScreenRegister.SetActive(true);
            }

            else if (isLoggedIn == false && alwaysShowLoginScreen == false)
            {
                splashScreen.SetActive(false);
                splashScreenLogin.SetActive(true);
                splashScreenRegister.SetActive(true);
            }

            else if (isLoggedIn && alwaysShowLoginScreen)
            {
                splashScreen.SetActive(false);
                splashScreenLogin.SetActive(true);
                splashScreenRegister.SetActive(true);
            }

            else if (isLoggedIn && alwaysShowLoginScreen == false)
            {
                splashScreen.SetActive(true);
                splashScreenLogin.SetActive(false);
                splashScreenRegister.SetActive(false);
            }
        }
    }
}