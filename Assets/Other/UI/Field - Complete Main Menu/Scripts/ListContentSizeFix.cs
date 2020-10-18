using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.FieldCompleteMainMenu
{
    public class ListContentSizeFix : MonoBehaviour
    {
        public Scrollbar scrollbar;
        public bool isReversed;

        void Start()
        {
            if (isReversed)
            {
                scrollbar.value = 1;
            }

            else
            {
                scrollbar.value = 0;
            }
        }

        public void FixListSize()
        {
            if (isReversed)
            {
                scrollbar.value = 1;
            }

            else
            {
                scrollbar.value = 0;
            }
        }
    }
}