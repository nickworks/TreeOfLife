using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Applies a jumping AI for any object this script is attached to.
/// THIS SCRIPT REQUIRES THE OBJECT TO HAVE AN ENEMY TYPE (this is now determined as a dropdown menu in the editor)
/// ENEMY TYPE CHOICES:
///     "basicJumpingEnemy": an enemy that jumps in an upward velocity when it hits the ground
///     "playerJumpingEnemy": an enemy that jumps at the player when it hits the ground
/// 	-New Enemy Type descriptions added here- (STEP 5)
/// ADDING ENEMY TYPES:
///     1. Add an enemy name to the public enum EnemyType variable.
///     2. Add a case to the switch statement within the private void ApplyJumpType() function.
///     3. Encapsulate enemy logic within the newly made case.
///     4. Choose the new Enemy Type in the editor.
///         -NOTE- New Enemy Types must be added to the custom editor to be seen in the inspector
///                as well as the public variables that affect the enemy.
/// 	5. Add new Enemy Type description to the comments above
/// OTHER NOTES
///     1. New objects using this script will need to choose "Ground" under Collidable With within the editor 
///        of the Pawn AABB (script). This will make it so that enemies will collide with the ground.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AlignWithPath))]
public class JumpingEnemies : MonoBehaviour {
    /// <summary>
    /// holds object tags for the type of enemy you want the object to be.
    /// </summary>
    public enum EnemyType
    {
        basicJumpingEnemy,
        playerJumpingEnemy
        // new Enemy Types added here, dont forget the comma <STEP 1>
    }
    /// <summary>
    /// holds the tag for the type of enemy you want the object to be, this is determined in the editor.
    /// </summary>
    public EnemyType enemyType;
    /// <summary>
    /// delays the object from jumping the first time it hits the ground.
    /// </summary>
    public float startDelay;
    /// <summary>
    /// controls what the jump delay timer of the object is based on time inputted in the editor.
    /// </summary>
    public float jumpDelay;
    /// <summary>
    /// The height, in meters, of the player's vertical jump distance.
    /// </summary>
    public float jumpHeight;
    /// <summary>
    /// The amount of time, in seconds, that it should take the player to reach the peak of their jump arc.
    /// </summary>
    public float jumpTime;
    /// <summary>
    /// This toggle allows the user to control if they want to use activationDistance to control when the object moves.
    /// </summary>
    public bool useActivationDistance;
    /// <summary>
    /// The amount of distance, in meters, that the player has to come within to activate the object.
    /// 15 meters is the minimum distance for the object to always be moving on screen.
    /// </summary>
    public float activationDistance;

    /// <summary>
    /// the Vector3 that is applied to the objects position every frame.
    /// </summary>
    private Vector3 velocity = new Vector3();
    /// <summary>
    /// The acceleration for this objects gravity.
    /// This is determined from (jumpHeight * 2) / (jumpTime * jumpTime) in the OnValidate function.
    /// </summary>
    private float gravity;
    /// <summary>
    /// is true when the object is colliding with the ground on its bottom edge.
    /// </summary>
    private bool isGrounded = false;
    /// <summary>
    /// this float is used to track when the object is allowed to jump again.
    /// </summary>
    private float jumpDelayTimer;
    /// <summary>
    /// the impulse to start this objects jump.
    /// This is determined from Gravity * JumpTime  in the OnValidate function.
    /// </summary>
    private float jumpImpulse;
    /// <summary>
    /// Controls when the object can move.
    /// This is based off of player to object distance.
    /// </summary>
    private bool isActivated = false;
    /// <summary>
    /// used at the beginning of the level so that the enemies don't fall through the floor during the launch/load of the level.
    /// </summary>
    private float startingLagAvoidance = 1;

    /// <summary>
    /// stores a reference to this objects TRANSFORM class.
    /// </summary>
    private Transform enemy;
    /// <summary>
    /// stores a reference to this objects PawnAABB script.
    /// </summary>
    private BoxCollider enemyBoxCollider;
    /// <summary>
    /// stores a reference to the player object in the scene.
    /// </summary>
    private Transform player;

    /// <summary>
    /// This function is called on the first frame the object is spawned
    /// it stores references to this objects Transform and PawnAABB classes
    /// grabs the first object in the scene to have a PlayerController script (since only the player should have a PlayerController script)
    /// it then gets the transform of the player object and stores it in a variable for later use
    /// </summary>
    void Start () {
        enemy = gameObject.GetComponent<Transform>();
        enemyBoxCollider = gameObject.GetComponent<BoxCollider>();
        Player.PlayerController playerControl = (Player.PlayerController)FindObjectOfType(typeof(Player.PlayerController));
        player = playerControl.GetComponent<Transform>();
        if (useActivationDistance == false) isActivated = true;
    }
    /// <summary>
    /// This is called automatically when the values change in the inspector.
    /// </summary>
    void OnValidate()
    {
        gravity = (jumpHeight * 2) / (jumpTime * jumpTime);
        jumpImpulse = gravity * jumpTime;
        jumpDelayTimer = startDelay;
    }

    /// <summary>
    /// Updates the object every frame
    /// This function applies gravity to the object
    /// and applies impulses if the object is colliding with the ground
    /// </summary>
	void Update ()
    {
        if (startingLagAvoidance >= 0)
        {
            startingLagAvoidance -= Time.deltaTime;
            return;
        }

        if (useActivationDistance == true)
        {
            if (CalculateActivationDistance() < activationDistance) isActivated = true;
        }

        if (isGrounded)
        {
            velocity.y = 0;
            velocity.x = 0;
            velocity.z = 0;

            jumpDelayTimer -= Time.deltaTime;

            if (jumpDelayTimer <= 0)
            {
                if(isActivated)
                {
                    ApplyJumpType();
                    jumpDelayTimer = jumpDelay;
                    isGrounded = false;
                }
            }
            if (useActivationDistance == true)
            {
                if (CalculateActivationDistance() > activationDistance)
                {
                    jumpDelayTimer = 0;
                    isActivated = false;
                }
            }
        } else
        {
            velocity.y -= gravity * Time.deltaTime;
        }
        enemy.position += velocity * Time.deltaTime;
        HandleCollisions();
    }
    /// <summary>
    /// calculates the distance between the player and the enemy and returns the distance
    /// </summary>
    /// <returns> the distance between the enemy and the player </returns>
    private float CalculateActivationDistance()
    {
        float aSquared = Mathf.Abs((player.position.x - enemy.position.x) * (player.position.x - enemy.position.x));
        float bSquared = Mathf.Abs((player.position.y - enemy.position.y) * (player.position.y - enemy.position.y));
        float c = Mathf.Sqrt(aSquared + bSquared);
        return c;
    }

    /// <summary>
    /// determines jump velocity based on enemyType tag determined in the editor
    /// </summary>
    private void ApplyJumpType()
    {
        switch (enemyType)
        {

			/// <summary>
			/// this case is called when Enemy Type is equal to Basic Jumping Enemy
			/// this case contains the movement logic for the Basic Jumping Enemy
			/// </summary>
            case JumpingEnemies.EnemyType.basicJumpingEnemy:
                velocity.y = jumpImpulse;
                break;
			/// <summary>
			/// this case is called when Enemy Type is equal to Player Jumping Enemy
			/// this case contains the movement logic for the Player Jumping Enemy
			/// </summary>
            case JumpingEnemies.EnemyType.playerJumpingEnemy:
                velocity.y = jumpImpulse;
                velocity.x = (player.position.x - enemy.position.x) / (jumpTime * 2);
                velocity.z = (player.position.z - enemy.position.z) / (jumpTime * 2);
                break;
            // New Enemy Type logic added here <STEP 2-3>
        }
    }

    /// <summary>
    /// Handles collision for this object
    /// then applies collision detection as needed
    /// </summary>
    private void HandleCollisions()
    {
        enemyBoxCollider = GetComponent<BoxCollider>();
        Vector3 centerPoint = enemyBoxCollider.bounds.center;
        float halfH = enemyBoxCollider.bounds.extents.y;

        RaycastHit hit;
        if (Physics.Raycast(centerPoint, Vector3.down, out hit, halfH, LayerMask.GetMask("Ground")))
        {
            if(hit.distance < halfH)
            {
                enemy.position += new Vector3(0, Mathf.Abs(hit.distance - halfH), 0);
            }
            isGrounded = true;
        }

    }
    /// <summary>
    /// applies damage to the player if colliding
    /// </summary>
    private void ApplyDamage()
    {
        // TODO: implement damage when health system is implemented into the player character
    }
}
/// <summary>
/// Adds a custom editor to the Jumping Enemies script.
/// This will replace the old editor completely.
/// New public variables will need to be added to this class to be viewed in the inspector.
/// </summary>
[CustomEditor(typeof(JumpingEnemies))]
public class JumpingEnemiesEditor : Editor
{
    /// <summary>
    /// creates the custom inspector
    /// all inspector data will be encapsulated inside
    /// </summary>
    public override void OnInspectorGUI()
    {
        JumpingEnemies editor = (JumpingEnemies)target;
        //base.OnInspectorGUI();
        serializedObject.Update();
        editor.enemyType = (JumpingEnemies.EnemyType)EditorGUILayout.EnumPopup("Enemy Type", editor.enemyType);

        switch(editor.enemyType)
        {
			/// <summary>
			/// this case controls what you can see in the inspector for Basic Jumping Enemy
			/// -CAUTION- you will not be able to edit public variables in the inspector without this logic
			/// </summary>
            case JumpingEnemies.EnemyType.basicJumpingEnemy:
                editor.startDelay = EditorGUILayout.FloatField("Start Delay", editor.startDelay);
                editor.jumpDelay = EditorGUILayout.FloatField("Jump Delay", editor.jumpDelay);
                editor.jumpHeight = EditorGUILayout.FloatField("Jump Height", editor.jumpHeight);
                editor.jumpTime = EditorGUILayout.FloatField("Jump Time", editor.jumpTime);
                editor.useActivationDistance = EditorGUILayout.Toggle("Use Activation Distance", editor.useActivationDistance);
                if(editor.useActivationDistance) editor.activationDistance = EditorGUILayout.FloatField("Activation Distance", editor.activationDistance);
                break;
			/// <summary>
			/// this case controls what you can see in the inspector for Player Jumping Enemy
			/// -CAUTION- you will not be able to edit public variables in the inspector without this logic
			/// </summary>
            case JumpingEnemies.EnemyType.playerJumpingEnemy:
                editor.startDelay = EditorGUILayout.FloatField("Start Delay", editor.startDelay);
                editor.jumpDelay = EditorGUILayout.FloatField("Jump Delay", editor.jumpDelay);
                editor.jumpHeight = EditorGUILayout.FloatField("Jump Height", editor.jumpHeight);
                editor.jumpTime = EditorGUILayout.FloatField("Jump Time", editor.jumpTime);
                editor.useActivationDistance = EditorGUILayout.Toggle("Use Activation Distance", editor.useActivationDistance);
                if (editor.useActivationDistance) editor.activationDistance = EditorGUILayout.FloatField("Activation Distance", editor.activationDistance);
                break;
            // New Enemy Type custom inspector added here <STEP 4>
        }
        serializedObject.ApplyModifiedProperties();
    }
}
