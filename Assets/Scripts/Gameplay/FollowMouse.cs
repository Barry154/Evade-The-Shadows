using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class FollowMouse : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image icon;
        [SerializeField] private RectTransform rectTransform;

        private void Awake()
        {
            Cursor.visible = false;
        }

        void Update()
        {
            var mousePos = Input.mousePosition;
            var mousePosTransformed = canvas.transform.InverseTransformPoint(mousePos);
            rectTransform.localPosition = mousePosTransformed;
        }
    }
}
