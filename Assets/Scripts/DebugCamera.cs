
using UnityEngine;

public class DebugCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        bool is_going_under_map = transform.position.y - 10.0f < 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0.0f, vertical);
        if (Input.GetKeyDown(KeyCode.Space))
            movement.y += 10.0f;
        else if (Input.GetKeyDown(KeyCode.C) && !is_going_under_map)
            movement.y -= 10.0f;
        else if (movement != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(movement);

        transform.position += movement * Time.deltaTime * 30.0f;
    }




}
