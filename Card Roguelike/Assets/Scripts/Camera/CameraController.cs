using UnityEngine;

namespace Camera
{
    public class CameraController : MonoBehaviour
    {
        private Rigidbody2D _rigidbody2d;

        [SerializeField]
        UnityEngine.Camera camera;

        private int _cameraSpeed = 30;

        private Transform _target;

        private void Start()
        {
            _rigidbody2d = GetComponent<Rigidbody2D>();
            _target = camera.transform;
        }

        private void Update()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                if(camera.orthographicSize > 4) 
                {
                    camera.orthographicSize--;
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
            {
                camera.orthographicSize++;
            }

            camera.transform.position = new Vector3(_target.position.x, _target.position.y, transform.position.z);
        }

        private void Move() 
        {
            _rigidbody2d.AddForce(new Vector2(Input.GetAxis("Horizontal") * _cameraSpeed * camera.orthographicSize/2, Input.GetAxis("Vertical") * _cameraSpeed * camera.orthographicSize/2));
            _rigidbody2d.velocity = Vector2.zero;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }
    
    }
}
