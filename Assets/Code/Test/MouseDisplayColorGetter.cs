using Code.Infrastructure.DI;
using Code.Services;
using UnityEngine;
using UnityEngine.UI;

namespace uWindowCapture
{
    public class MouseDisplayColorGetter : MonoBehaviour
    {
        [SerializeField] UwcWindowTexture uwcTexture;

        Material material_;
        private PositionService _positionService;

        void Start()
        {
            material_ = GetComponent<Renderer>().material;
            _positionService = Container.Instance.FindService<PositionService>();
        }

        void Update()
        {
            var window = uwcTexture.window;
            if (window == null) return;

            //if (UwcManager.cursorWindow == window) {
            var cursorPos = Lib.GetCursorPosition();
            Debug.Log($"{window.width}x{window.height}: {cursorPos.x}x{cursorPos.y}");
            Debug.Log($"{Input.mousePosition}");
 
            var x = cursorPos.x - window.x;
            var y = cursorPos.y - window.y;
            material_.color = window.GetPixel(x, y);
            //}
        }
    }
}