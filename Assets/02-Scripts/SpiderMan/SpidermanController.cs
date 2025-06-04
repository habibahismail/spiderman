using UnityEngine;

[RequireComponent(typeof(SpidermanMovement))]
[RequireComponent (typeof(SpidermanClimb))]
[RequireComponent(typeof(SpidermanSwing))]
public class SpidermanController : MonoBehaviour
{
    public SpiderManState CurrentState;
    
    private SpidermanMovement movement;
    private SpidermanSwing swing;
    private SpidermanClimb climb;
    private Animator animator;

    private void Start()
    {
        movement = GetComponent<SpidermanMovement>();
        swing = GetComponent<SpidermanSwing>();
        climb = GetComponent<SpidermanClimb>();

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case SpiderManState.Idle: 
            case SpiderManState.Moving:
                movement.JumpInput();
                break;

            case SpiderManState.Jumping:
                movement.JumpLogic();
                break;

            case SpiderManState.Climbing:
               
                break;

            case SpiderManState.Swinging:
                
                break;

            case SpiderManState.Falling:
             
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case SpiderManState.Idle:
            case SpiderManState.Moving:
                movement.HandleMovement();
                movement.HandleRotation();
                break;

            case SpiderManState.Jumping:
                movement.HandleJumping();
                //movement.HandleFalling();
                break;

            case SpiderManState.Climbing:
                climb.HandleClimbing();
                break;

            case SpiderManState.Swinging:
                swing.HandleWebSwinging();
                break;

            case SpiderManState.Falling:
                //HandleFalling();
                break;
        }
    }
}

public enum SpiderManAnimationState
{
    Idle = 0,
    Run = 1,
    Sprint = 2,
    Jump = 3,
    RunningJump = 4,
    Climbing = 5,
    ClimbingIdle = 6,
    ClimbJump = 7,
    HardLanding = 8,
    Falling = 9,
    Swinging = 10,
    SwingingBothArms = 11,
    Death = 12
}

public enum SpiderManState
{
    Idle,
    Moving,
    Jumping,
    Falling,
    Climbing,
    Swinging
}
