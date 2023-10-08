using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameManagerScript : MonoBehaviour
{
    
    [SerializeField]
    private GameObject Prefab;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void AddNew(float x_pos, float y_pos) {
        GameObject NewFlame = Object.Instantiate(Prefab, transform);
        NewFlame.transform.Translate(new Vector3(x_pos, y_pos,0));
    }

    public void ClearAllFlames() {
        for (int i = transform.childCount - 1; i >= 0; i-- ){
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
