using UnityEngine;

namespace Code.Components.Common
{
    public class AnchorMover : MonoBehaviour
    {
        private Transform _anchor;
        private bool _isActive;

        public void SetAnchor(Transform anchor)
        {
            _anchor = anchor;
        }

        public void Off()
        {
            _isActive = false;
        }

        public void On()
        {
            _isActive = _anchor != null;
        }
    }
}