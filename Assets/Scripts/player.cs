using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {
    private Rigidbody2D myrigidbody;
    private Animator myanimator;

    [SerializeField] 
        private float mspeed;

    private bool faceRight;
    private bool attack;
    private bool slide;

    [SerializeField]
        private Transform[] groundPoints;

    [SerializeField]
        private float groundRadius;

    [SerializeField]
        private LayerMask whatisGround;

    private bool isGrounded;
    private bool jump;

    [SerializeField]
        private float jumpForce;
    [SerializeField]
        private bool airControl;
    // Use this for initialization
	void Start () {
        faceRight = true;
        myrigidbody = GetComponent<Rigidbody2D>();
        myanimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
    void Update()
    {
        handleinput();
    }
	void FixedUpdate () {
        float horizontal = Input.GetAxis("Horizontal");
        //Debug.Log(horizontal);
        isGrounded = IsGrounded();
        handlemovement(horizontal);
        handleattacks();
        handleLayers();
        flip(horizontal);
        resetvalues();
	}
    private void handlemovement(float horizontal)
    {
        if(myrigidbody.velocity.y <0)
        {
            myanimator.SetBool("land", true);
        }
        if(!this.myanimator.GetBool("slide") && !this.myanimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && (isGrounded || airControl))
        {
            //myrigidbody.velocity = Vector2.left;// x = -1, y =0
            myrigidbody.velocity = new Vector2(horizontal * mspeed, myrigidbody.velocity.y);
        }
        if(isGrounded && jump)
        {
            isGrounded = false;
            myrigidbody.AddForce(new Vector2(0, jumpForce));
            myanimator.SetTrigger("jump");
        }
        
        if(slide && !this.myanimator.GetCurrentAnimatorStateInfo(0).IsName("slide"))
        {
            myanimator.SetBool("slide", true);
        }
        else if(!this.myanimator.GetCurrentAnimatorStateInfo(0).IsName("slide"))
        {
            myanimator.SetBool("slide", false);
        }
        myanimator.SetFloat("speed", Mathf.Abs(horizontal));

    }
    private void handleattacks()
    {
        if(attack && !this.myanimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            myanimator.SetTrigger("attack");
            myrigidbody.velocity = Vector2.zero;
        }
    }
    private void handleinput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            jump = true;
        if (Input.GetKeyDown(KeyCode.LeftShift))
            attack = true;
        if (Input.GetKeyDown(KeyCode.LeftControl))
            slide = true;
    }
    private void flip(float horizontal)
    {
        if(horizontal>0 && !faceRight || horizontal<0 && faceRight)
        {
            faceRight = !faceRight;
            Vector3 theScale = transform.localScale;
            theScale.x = theScale.x * -1; 
            transform.localScale = theScale; 

        }

    }
    private void resetvalues()
    {
        attack = false;
        slide = false;
        jump = false;
    }
    private bool IsGrounded()
    {
        if (myrigidbody.velocity.y <= 0)
        {
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatisGround);
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject != gameObject)
                    {
                        myanimator.ResetTrigger("jump");
                        myanimator.SetBool("land", false);
                        return true;
                    }
                }
            }
        }
            return false;
    }
    private void handleLayers()
    {
        if (!isGrounded)
            myanimator.SetLayerWeight(1, 1);
        else
            myanimator.SetLayerWeight(1, 0);
    }
}
