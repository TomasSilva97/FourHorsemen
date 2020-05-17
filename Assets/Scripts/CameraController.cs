
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float speedH = 2.0f;
    public float speedV = 2.0f;
    public float yaw = 0.0f;
    public float pitch = 0.0f;

    [System.Serializable]
    public class PositionSettings
    {
        public Vector3 target_pos_offset = new Vector3(0, 3.4f, 0);
        public float look_smooth = 80f;
        public float distance_from_traget = -8;
        public float zoom_smooth = 50;
        public float max_zoom = -2;
        public float min_zoom = -15;
    }
    [System.Serializable]
    public class OrbitSettings
    {
        public float x_rotation = -150;
        public float y_rotation = 0;
        public float max_x_rotation = 25;
        public float min_x_rotation = -85;
        public float v_orbit_smooth = 0.1f;
        public float h_orbit_smooth = 0.01f;
    }
    [System.Serializable]
    public class InputSettings
    {
        public string ORBIT_HORIZONTAL_SNAP = "OrbitHorizontalSnap";
        public string ORBIT_HORIZONTAL = "OrbitHorizontal";
        public string ORBIT_VERTICAL = "OrbitVertical";
        public string ZOOM = "Mouse ScrollWheel";
    }
    public PositionSettings position = new PositionSettings();
    public OrbitSettings orbit = new OrbitSettings();
    public InputSettings input = new InputSettings();

    Vector3 target_pos = Vector3.zero;
    Vector3 destination = Vector3.zero;
    PlayerController pc;
    float v_orbit_input, h_orbit_input, zoom_input, h_orbit_snap_input;

    // Start is called before the first frame update
    void Start()
    {
        SetCameraTarget(target);
    }

    void SetCameraTarget(Transform t)
    {
        target = t;
        if (target != null)
        {
            if (target.GetComponent<PlayerController>())
            {
                pc = target.GetComponent<PlayerController>();
            }
        }
        else
        {
            
        }
    }

    void GetInput()
    {
        v_orbit_input = Input.GetAxisRaw(input.ORBIT_VERTICAL);
        h_orbit_input = Input.GetAxisRaw(input.ORBIT_HORIZONTAL);
        h_orbit_snap_input = Input.GetAxisRaw(input.ORBIT_HORIZONTAL_SNAP);
        zoom_input = Input.GetAxisRaw(input.ZOOM);
    }

    void Update()
    {
        GetInput();
        OrbitTarget();
        ZoomOnTarget();
        if (Input.GetKeyDown(KeyCode.V))
        {
            target_pos = Vector3.zero;
            orbit.x_rotation = -150;
            orbit.y_rotation = 0;
            position.distance_from_traget = -8.67f;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        MoveTarget();
        LookAtTarget();
    }

    void MoveTarget()
    {
        target_pos = target.position + position.target_pos_offset;
        destination = Quaternion.Euler(orbit.x_rotation, orbit.y_rotation, 0) * -Vector3.forward * position.distance_from_traget;
        destination += target_pos;
        transform.position = destination;
    }

    void LookAtTarget()
    {
        Quaternion target_rotation = Quaternion.LookRotation(target_pos - transform.position);

        transform.rotation = Quaternion.Lerp(transform.rotation, target_rotation, position.look_smooth * Time.deltaTime);
    }

    void OrbitTarget()
    {
        if (h_orbit_snap_input > 0)
        {
            orbit.y_rotation = -180;
        }
        orbit.x_rotation += -v_orbit_input * orbit.v_orbit_smooth * Time.deltaTime;
        orbit.y_rotation += -h_orbit_input * orbit.h_orbit_smooth * Time.deltaTime;

        if (orbit.x_rotation > orbit.max_x_rotation)
        {
            orbit.x_rotation = orbit.max_x_rotation;
        }

        if (orbit.x_rotation > orbit.min_x_rotation)
        {
            orbit.x_rotation = orbit.min_x_rotation;
        }
    }

    void ZoomOnTarget()
    {
        position.distance_from_traget += zoom_input * position.zoom_smooth * Time.deltaTime;
        if (position.distance_from_traget > position.max_zoom)
        {
            position.distance_from_traget = position.max_zoom;
        }
        if (position.distance_from_traget < position.min_zoom)
        {
            position.distance_from_traget = position.min_zoom;
        }
    }
}
