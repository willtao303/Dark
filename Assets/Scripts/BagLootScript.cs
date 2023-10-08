using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagLootScript : MonoBehaviour
{

    private CircleCollider2D col;

    private Collider2D PlayerCollider;
    private PlayerRaycastScript PlayerRsScript;

    [SerializeField]
    GameObject dialog; 
    
    void Start()
    {
        col = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (col.IsTouching(PlayerCollider)) {
            if (!dialog.activeSelf) {
                dialog.SetActive(true);
            }
            if (Input.GetKey("q")) {
                PlayerRsScript.AddOil(0.3f);
                Destroy(gameObject);
            }
        } else {
            if (dialog.activeSelf) {
                dialog.SetActive(false);
            }
        }

    }

    public void SetPlayer(GameObject Player){
        PlayerCollider = Player.GetComponent<Collider2D>();
        PlayerRsScript = Player.transform.GetChild(0).GetComponent<PlayerRaycastScript>();
    }


}
