using UnityEngine;

namespace Camera
{
    public class CameraController : MonoBehaviour
    {
        private Rigidbody2D _rigidbody2d;

        [SerializeField]
        UnityEngine.Camera camera;

        private int _cameraSpeed = 30;

        private void Start()
        {
            _rigidbody2d = GetComponent<Rigidbody2D>();
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
        }

        private void Move() 
        {
            _rigidbody2d.AddForce(new Vector2(Input.GetAxis("Horizontal") * _cameraSpeed * camera.orthographicSize/2, Input.GetAxis("Vertical") * _cameraSpeed * camera.orthographicSize/2));
            _rigidbody2d.velocity = Vector2.zero;
        }
    
    }
}
