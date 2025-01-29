using UnityEngine;

namespace Scripts
{
    public class ProtoCameraController : MonoBehaviour
    {
        [SerializeField] private GameObject objectToFollow;

        private Vector3 currentVelocity;

        [Header("Settings")] 
        [SerializeField] private float smoothTime;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float playerHitShakeIntensity = 2f;
        [SerializeField] private float playerHitShakeTotalDuration = 0.5f;

        private float shakeIntensity = 0;
        private float shakeTotalDuration = 0;
        private float shakeTimer = 0;

        public void SetTarget(GameObject target)
        {
            objectToFollow = target;
            var pos = objectToFollow.transform.position;
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        }

        public void DoShake()
        {
            shakeIntensity = playerHitShakeIntensity;
            shakeTotalDuration = playerHitShakeTotalDuration;
            shakeTimer = 0;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            var objectToFollowPos = objectToFollow.transform.position;
            var currentPos = transform.position;

            var targetPosition = new Vector3(objectToFollowPos.x, objectToFollowPos.y, currentPos.z);

            if (shakeTotalDuration > 0)
            {
                float currentIntensity = Mathf.Lerp(shakeIntensity, 0, shakeTimer / shakeTotalDuration);
                targetPosition += Random.insideUnitSphere * currentIntensity;
                shakeTimer += shakeTotalDuration;
                if (shakeTimer >= shakeTotalDuration)
                {
                    shakeTotalDuration = 0;
                }

                smoothTime = 0.02f;
            }
            
            transform.position = Vector3.SmoothDamp(currentPos, targetPosition,
                ref currentVelocity, smoothTime, maxSpeed);
        }
    }
}
