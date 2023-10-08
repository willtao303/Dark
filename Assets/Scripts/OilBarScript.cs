using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilBarScript : MonoBehaviour
{
    
    [SerializeField]
    private PlayerRaycastScript OilReferance;
    
    private Vector3 HeightVector = new Vector3(1, 1, 1);

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        HeightVector.y = OilReferance.GetOilPercent();
        transform.localScale = HeightVector;
    }
}
