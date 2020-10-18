using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.FieldCompleteMainMenu
{
    public class LayoutGroupPositionFix : MonoBehaviour
    {
        public LayoutGroup layoutGroup;

        void Start()
        {
            StartCoroutine(ExecuteAfterTime(0.5f));
        }

        IEnumerator ExecuteAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            layoutGroup.enabled = false;
            layoutGroup.enabled = true;
        }
    }
}