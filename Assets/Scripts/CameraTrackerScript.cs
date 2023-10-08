using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrackerScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Transform trackedObject;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(trackedObject.position.x, trackedObject.position.y, transform.position.z);
    }
}
