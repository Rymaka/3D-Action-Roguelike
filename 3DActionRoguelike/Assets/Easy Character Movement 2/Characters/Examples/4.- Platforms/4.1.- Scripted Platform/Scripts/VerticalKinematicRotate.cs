using UnityEngine;

namespace EasyCharacterMovement.Examples.Components
{
        [RequireComponent(typeof(Rigidbody))]
        public class VerticalKinematicRotate : MonoBehaviour
        {
            #region FIELDS

            [SerializeField]
            private float _rotationSpeed = 30.0f;
            private float _angle;

            // Новое публичное поле для выбора оси вращения
            [SerializeField]
            private Vector3 _rotationAxis = Vector3.right;

            private Quaternion _startRotation;

            #endregion

            #region PRIVATE FIELDS

            private Rigidbody _rigidbody;

            #endregion

            #region PROPERTIES

            public float rotationSpeed
            {
                get => _rotationSpeed;
                set => _rotationSpeed = value;
            }

            public float angle
            {
                get => _angle;
                set => _angle = MathLib.Clamp0360(value);
            }

            // Новое публичное свойство для выбора оси вращения
            public Vector3 rotationAxis
            {
                get => _rotationAxis;
                set => _rotationAxis = value.normalized;
            }

            #endregion

            #region MONOBEHAVIOUR

            public void OnValidate()
            {
                rotationSpeed = _rotationSpeed;
            }

            public void Awake()
            {
                _rigidbody = GetComponent<Rigidbody>();
                _rigidbody.isKinematic = true;

                // Сохраняем начальную ориентацию объекта
                _startRotation = transform.rotation;
            }

            public void FixedUpdate()
            {
                angle += rotationSpeed * Time.deltaTime;

                // Используем сохраненную начальную ориентацию объекта и выбранную ось вращения
                Quaternion rotation = _startRotation * Quaternion.AngleAxis(angle, rotationAxis);
                _rigidbody.MoveRotation(rotation);
            }

            #endregion
        }
     
}

