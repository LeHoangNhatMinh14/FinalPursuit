using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cam;

    void Start()
    {
        // Find the main camera once, at runtime
        if (cam == null && Camera.main != null)
        {
            cam = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (cam != null)
        {
            transform.LookAt(transform.position + cam.forward);
        }
    }
}
