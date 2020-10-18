using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Michsky.UI.FieldCompleteMainMenu
{
    public class SliderManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("TEXTS")]
        public Text valueText;
        public Text mainValueText;

        [Header("SETTINGS")]
        public bool showValue = true;
        public bool showMainValue = true;
        public bool useRoundValue;

        private Slider mainSlider;
        private Animator sliderAnimator;

        void Start()
        {
            mainSlider = GetComponent<Slider>();
            sliderAnimator = GetComponent<Animator>();

            if (showValue == false)
            {
                Destroy(valueText);
            }
        }

        void Update()
        {
            if (useRoundValue)
            {
                valueText.text = Mathf.Round(mainSlider.value * 1.0f).ToString();
                mainValueText.text = Mathf.Round(mainSlider.value * 1.0f).ToString();
            }
            else
            {
                valueText.text = mainSlider.value.ToString("F1");
                mainValueText.text = mainSlider.value.ToString("F1");
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            sliderAnimator.Play("Value In");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            sliderAnimator.Play("Value Out");
        }
    }
}