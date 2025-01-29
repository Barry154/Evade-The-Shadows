using UnityEngine;
using UnityEngine.Assertions;

namespace Scripts.Character
{
    public class CharacterVisual : MonoBehaviour
    {
        [SerializeField] private Transform root;

        [SerializeField] private Transform handsParent;
        [SerializeField] private CharacterHand handRight;
        [SerializeField] private CharacterHand handLeft;

        [SerializeField] private SpriteRenderer renderer;
        [SerializeField] private Animator animator;

        [Header("Settings")] [SerializeField] public float handsPositionRangeSize = 1;
            
        private Vector3 handsInitialLocalPos;

        void Awake()
        {
            handsInitialLocalPos = handsParent.localPosition;
        }

        public void UpdateWalking(bool isWalking, float moveSpeed)
        {
            animator.SetFloat("WalkSpeed", moveSpeed);
            animator.SetBool("IsWalking", isWalking);
        }
        
        public void UpdateOrientation(Vector3 mouseDir)
        {
            Assert.IsTrue(Mathf.Approximately(mouseDir.sqrMagnitude, 1), "[Character Visual] Mouse direction is not normalised!");

            if (mouseDir.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                mouseDir.x *= -1;
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            
            handsParent.localPosition = handsInitialLocalPos + (mouseDir * handsPositionRangeSize);
        }

        public void UpdateHands(bool isWalking, bool isUsingLeftHand, bool isUsingRightHand)
        {
            handLeft.UpdateHand(isWalking, isUsingLeftHand);
            handRight.UpdateHand(isWalking, isUsingRightHand);
        }
    }
}