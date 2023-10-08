using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    
	public float x = 0.0F;
    public float y = 0.0F;
    private Vector2 pos;

    [SerializeField]
    private float spd = 3.5F;

    private Rigidbody2D rb2d;
    private Animator animator;
    private const string paramDirection = "Direction";


    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator  = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {        
        float spFactor = 1.0F;
        int keysdown = 0;
        float dt = Time.deltaTime;
        Vector3 translation = new Vector3(0.0F , 0.0F);

        bool up   =  Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow);
        if (up){keysdown++;}
        bool down =  Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow);
        if (down){keysdown++;}
        bool left  = Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow);
        if (left){keysdown++;}
        bool right = Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow);
        if (right){keysdown++;}

        if (keysdown > 1){
            spFactor = 0.7F;
        }
        

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftControl)) {
            spFactor *= 2;
        }

        if (up){
            translation.y += spFactor;
        }
        if (down){
            translation.y -= spFactor;
        }
        if (right){
            translation.x += spFactor;
        }
        if (left){
            translation.x -= spFactor;
        }

        if (translation.x < 0){
            animator.SetInteger(paramDirection, 3);
        } else if (translation.x > 0) {
            animator.SetInteger(paramDirection, 1);
        } else if (translation.y > 0) {
            animator.SetInteger(paramDirection, 0);
        } else if (translation.y < 0) {
            animator.SetInteger(paramDirection, 2);
        }

    	rb2d.velocity = (translation*spd);
    }

    public void resetPos() {
        transform.position = Vector3.zero;
    }
}
