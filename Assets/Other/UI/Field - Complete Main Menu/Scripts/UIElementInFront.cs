using UnityEngine;

namespace Michsky.UI.FieldCompleteMainMenu
{
    public class UIElementInFront : MonoBehaviour
    {
        void Start()
        {
            transform.SetAsFirstSibling();
        }
    }
}