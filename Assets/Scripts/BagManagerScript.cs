using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagManagerScript : MonoBehaviour
{
    [SerializeField]
    private GameObject BagPrefab;

    [SerializeField]
    private GameObject Player;

    
    void Start()
    {
        
    }
    
    void Update()
    {}

    public void AddBag(int x_pos, int y_pos) {
        GameObject NewBag = Object.Instantiate(BagPrefab, transform);
        NewBag.transform.Translate(new Vector3(x_pos, y_pos,0));
        NewBag.GetComponent<BagLootScript>().SetPlayer(Player);
    }

    public void ClearAllBags() {
        for (int i = transform.childCount - 1; i >= 0; i-- ){
            Destroy(transform.GetChild(i).gameObject);
        }
    }

}
