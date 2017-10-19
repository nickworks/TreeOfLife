using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PawnAABB))]
public class Ghost : MonoBehaviour {

    public bool inRange = false;
    public Vector3 targetPos;
    float xAcc;
    float yAcc;
    Vector3 currentPos;
    public float enemeyImpulse;
    private Vector3 velocity = new Vector3();
    private PawnAABB pawn;
    private float gravity = 10;
    static public bool isGrounded = false;
    static public bool canDash = true;

    // Use this for initialization
    void Start () {
        pawn = GetComponent<PawnAABB>();
        xAcc = 1;
        yAcc = 1;
	}
	
	// Update is called once per frame
	void Update () {
        DoCollisions();
       // velocity = transform.position;
        if (inRange)
        {
            print("ghost is chasing!");
           // print(targetPos);           
            Dash();
        }
        float gravityScale = 2;
        velocity.y -= gravity * Time.deltaTime * gravityScale;
    }


    private void Dash() {

       // float gravityScale = 2;
       //transform.position = currentPos;



        if (canDash)
                    {
            if (Input.GetButtonDown("Fire2"))
            {

                velocity = DirectionalDash.rot * enemeyImpulse;
                canDash = false;

            }
            }




        
        //dash ground check
        if (isGrounded)
        {
            canDash = true;


        }

        // gravity
       // velocity.y -= gravity * Time.deltaTime * gravityScale;



    }
    private void DoCollisions()
    {
        PawnAABB.CollisionResults results = pawn.Move(velocity * Time.deltaTime);
        if (results.hitTop || results.hitBottom) velocity.y = 0;
        if (results.hitLeft || results.hitRight) velocity.x = 0;
        isGrounded = results.hitBottom || results.ascendSlope;
        transform.position += results.distance;
    }
}
