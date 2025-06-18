using UnityEngine;
using System.Collections;

namespace AdvancedRogueLikeandPuzzleSystem
{
    public class ThirdPersonController : MonoBehaviour
    {
        public float MoveSpeed = 2.0f;
        public float SprintSpeed = 5.335f;
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;
        public float SpeedChangeRate = 10.0f;
        [Space(10)]
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;
        private Vector2 vectorMove = Vector2.zero;
        [Space(10)]
        public float JumpTimeout = 0.50f;
        public float FallTimeout = 0.15f;

        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;

        public float _speed;
        public float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        public int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        public Animator _animator;
        private CharacterController _controller;
        private GameObject _mainCamera;
        private const float _threshold = 0.01f;
        private bool _hasAnimator;
        private LadderScript currentLadder;
        public static ThirdPersonController Instance;

        private void Awake()
        {
            Instance = this;
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();

            AssignAnimationIDs();

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            if (HeroController.instance.Health <= 0) return;

            // Salto desactivado
            // JumpAndGravity();

            GroundedCheck();
            Move();
        }

        public void RunInToArea() => isAutoWalking = true;
        public void ActivatePlayer() => isAutoWalking = false;

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // Desactivado (opcional)
        }

        float lastTime = 0;
        Vector3 inputDirection;
        float inputMagnitude = 0;
        float targetSpeed = 0;

        public ThirdPersonController(bool isAutoWalking)
        {
            this.isAutoWalking = isAutoWalking;
        }

        private bool isAutoWalking = false;

        private void Move()
        {
            if (GameManager.Instance.controllerType == ControllerType.KeyboardMouse)
            {
                vectorMove = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                targetSpeed = Input.GetKey(GameManager.Instance.Keycode_Sprint) ? Mathf.Lerp(targetSpeed, SprintSpeed, 0.2f) : Mathf.Lerp(targetSpeed, MoveSpeed, 0.2f);
            }
            else if (GameManager.Instance.controllerType == ControllerType.Mobile)
            {
                vectorMove = new Vector2(SimpleJoystick.Instance.HorizontalValue, SimpleJoystick.Instance.VerticalValue);
                if (Mathf.Abs(SimpleJoystick.Instance.VerticalValue) > 0.75f || Mathf.Abs(SimpleJoystick.Instance.HorizontalValue) > 0.75f)
                {
                    targetSpeed = Mathf.Lerp(targetSpeed, SprintSpeed, 0.1f);
                }
                else
                {
                    targetSpeed = Mathf.Lerp(targetSpeed, MoveSpeed, 0.1f);
                }
            }

            if (vectorMove == Vector2.zero) targetSpeed = 0.0f;
            float currentHorizontalSpeed = new Vector3(vectorMove.x, 0.0f, vectorMove.y).magnitude;
            float speedOffset = Time.deltaTime;
            inputMagnitude = 1;
            _speed = targetSpeed;
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);

            if (vectorMove != Vector2.zero)
            {
                inputDirection = new Vector3(vectorMove.x, 0.0f, vectorMove.y).normalized;
                if (Time.time > lastTime + 0.35f && !HeroController.instance.inDefendMode)
                {
                    lastTime = Time.time;
                    HeroController.instance.audio_AyakSesi.clip = AudioManager.instance.audioClip_Footsteps[Random.Range(0, AudioManager.instance.audioClip_Footsteps.Length)];
                    HeroController.instance.audio_AyakSesi.Play();
                }
            }
            else
            {
                HeroController.instance.audio_AyakSesi.Stop();
            }

            if (vectorMove != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }

            if (_speed == 0 && (_verticalVelocity < 0 && Grounded)) return;
            if (HeroController.instance.isHitting || HeroController.instance.inDefendMode) return;

            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // Se desactiva el dash con consumo de maná al presionar shift
            // if ((Input.GetKeyDown(GameManager.Instance.Keycode_Sprint) || Input.GetButtonDown("Sprint")) && HeroController.instance.Mana >= ManaSpending_Sprint)
            // {
            //     GameCanvas_Controller.instance.Update_Mana_Bar(ManaSpending_Sprint);
            //     StartCoroutine(TrailRendererShow());
            // }
        }

        // Desactivado sprint especial
        public void Sprint_Now()
        {
            // Desactivado por ahora
            // if (HeroController.instance.Mana >= ManaSpending_Sprint && vectorMove != Vector2.zero)
            // {
            //     GameCanvas_Controller.instance.Update_Mana_Bar(ManaSpending_Sprint);
            //     StartCoroutine(TrailRendererShow());
            // }
        }

        public TrailRenderer SprinttrailRenderer;
        public ParticleSystem SprintParticle;
        public int ManaSpending_Sprint = 20;

        // Desactivado el efecto visual del sprint especial
        IEnumerator TrailRendererShow()
        {
            // SprinttrailRenderer.enabled = true;
            // SprintParticle.Play();
            // AudioManager.instance.Play_Sprint();
            // yield return new WaitForSeconds(0.1f);
            // HeroController.instance.characterController.Move(transform.forward * 5);
            // yield return new WaitForSeconds(0.2f);
            // SprintParticle.Stop();
            // SprinttrailRenderer.enabled = false;
            yield break;
        }

        private void JumpAndGravity()
        {
            // Método de salto desactivado completamente
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = Grounded ? transparentGreen : transparentRed;

            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }

        public void JumpNow()
        {
            // Método de salto desactivado completamente
        }

        private void OnTriggerEnter(Collider other) { }
        private void OnTriggerExit(Collider other) { }
    }

}