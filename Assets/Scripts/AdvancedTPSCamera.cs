using UnityEngine;

namespace AdvancedRogueLikeandPuzzleSystem
{
    public class AdvancedTPSCamera : MonoBehaviour
    {
        public Transform viewTarget;
        public LayerMask collisionLayers;
        public float distance = 6.0f;
        public float distanceSpeed = 150.0f;
        public float collisionOffset = 0.3f;
        public float minDistance = 4.0f;
        public float maxDistance = 12.0f;
        public float height = 1.5f;
        public float horizontalRotationSpeed = 250.0f;
        public float verticalRotationSpeed = 150.0f;
        public float rotationDampening = 0.75f;
        public float minVerticalAngle = -60.0f;
        public float maxVerticalAngle = 60.0f;
        public bool useRMBToAim = false;
        private float h, v, smoothDistance;
        private Vector3 newPosition;
        private Quaternion newRotation, smoothRotation;
        private Transform cameraTransform;
        public float fCamShakeImpulse = 0.0f;
        public static AdvancedTPSCamera Instance;
        void Start()
        {
            Initialize();
        }

        private void Awake()
        {
            Instance = this;
        }

        void Initialize()
        {
            h = this.transform.eulerAngles.x;
            v = this.transform.eulerAngles.y;

            cameraTransform = this.transform;
            smoothDistance = distance;
            NullErrorCheck();
        }

        void NullErrorCheck()
        {
            if (!viewTarget)
            {

            }
            if (collisionLayers == 0)
            {

            }
        }

        public float MoveDamping = 0.1f;

        void LateUpdate()
        {
            if (!viewTarget)
                return;

            if (GameManager.Instance.controllerType == ControllerType.Mobile)
            {
                h += Touchpad.Instance.HorizontalValue * horizontalRotationSpeed * Time.deltaTime;
                v -= Touchpad.Instance.VerticalValue * verticalRotationSpeed * Time.deltaTime;
            }
            else
            {
                h += Input.GetAxis("Mouse X") * horizontalRotationSpeed * Time.deltaTime;
                v -= Input.GetAxis("Mouse Y") * verticalRotationSpeed * Time.deltaTime;
            }

            h = ClampAngle(h, -360.0f, 360.0f);
            v = ClampAngle(v, minVerticalAngle, maxVerticalAngle);

            newRotation = Quaternion.Euler(v, h, 0.0f);

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 10, minDistance, maxDistance);
            smoothDistance = Mathf.Lerp(smoothDistance, distance, TimeSignature(distanceSpeed));

            smoothRotation = Quaternion.Slerp(smoothRotation, newRotation, TimeSignature((1 / rotationDampening) * 100.0f));

            newPosition = viewTarget.position;
            newPosition += smoothRotation * new Vector3(0.0f, height, -smoothDistance);

            CheckSphere();

            smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

            cameraTransform.position = Vector3.Lerp(cameraTransform.position, newPosition, MoveDamping);
            cameraTransform.rotation = smoothRotation;
            if (fCamShakeImpulse > 0.0f)
                shakeCamera();
        }

        void CheckSphere()
        {
            Vector3 tmpVect = viewTarget.position;
            tmpVect.y += height;
            RaycastHit hit;
            Vector3 dir = (newPosition - tmpVect).normalized;
            if (Physics.SphereCast(tmpVect, 0.3f, dir, out hit, distance, collisionLayers))
            {
                newPosition = hit.point + (hit.normal * collisionOffset);
            }
        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;

            if (angle > 360)
                angle -= 360;

            return Mathf.Clamp(angle, min, max);
        }

        private float TimeSignature(float speed)
        {
            return 1.0f / (1.0f + 80.0f * Mathf.Exp(-speed * 0.02f));
        }

        public void shakeCamera()
        {
            Camera.main.transform.position += new Vector3(Random.Range(-fCamShakeImpulse, fCamShakeImpulse) / 4, Random.Range(-fCamShakeImpulse, fCamShakeImpulse) / 4, Random.Range(-fCamShakeImpulse, fCamShakeImpulse) / 4);
            fCamShakeImpulse -= Time.deltaTime * fCamShakeImpulse * 4.0f;
            if (fCamShakeImpulse < 0.01f)
            {
                fCamShakeImpulse = 0.0f;
            }
        }
    }
}