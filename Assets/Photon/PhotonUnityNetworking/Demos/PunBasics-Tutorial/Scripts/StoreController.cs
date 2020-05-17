
using UnityEngine;

public class StoreController : MonoBehaviour
{
    public static Vector3 newPose;
    public static bool selectMove;
    public Transform storeContainer;
    public float lerpTime;

    // Update is called once per frame
    void Update()
    {
        if (storeContainer.position != newPose && selectMove)
        {
            storeContainer.position = Vector3.Lerp(storeContainer.position, newPose, lerpTime * Time.deltaTime);
        }
        if (Vector3.Distance(storeContainer.position, newPose) < 0.1f)
        {
            storeContainer.position = newPose;
            selectMove = false;
        }
    }
}
