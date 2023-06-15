using UnityEngine;

namespace Camera
{
    public class CameraController : MonoBehaviour
    {
        Rigidbody2D rigidbody2d;

        [SerializeField]
        UnityEngine.Camera camera;
        int cameraSpeed = 30;
        bool freeCamera = false;

        private void Start()
        {
            rigidbody2d = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            Move();
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

        void Move() 
        {
            rigidbody2d.AddForce(new Vector2(Input.GetAxis("Horizontal") * cameraSpeed * camera.orthographicSize/2, Input.GetAxis("Vertical") * cameraSpeed * camera.orthographicSize/2));
            rigidbody2d.velocity = Vector2.zero;
        }
    
    }
}
