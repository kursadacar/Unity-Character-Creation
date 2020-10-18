using UnityEngine;

namespace Michsky.UI.FieldCompleteMainMenu
{
    public class ObjectDisableHelper : MonoBehaviour
    {
        private GameObject thisObject;
        private ObjectDisableHelper script;
        public bool disableScript;

        void Start()
        {
            script = GetComponent<ObjectDisableHelper>();

            if (disableScript)
            {
                script.enabled = false;
            }
        }
    }
}