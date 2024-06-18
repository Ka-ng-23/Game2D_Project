using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;
// Formula for player: Input actions *  TimeFrame * Speed Actions
[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))] // call component and datatypes 
public class PlayerController : MonoBehaviour // PlayController is baseclass in Monobehavior
{
    public float walkSpeed = 8f;            // velocity of "walk"      
    public float runSpeed = 11f;             // velocity of "run" 
    public float airWalkSpeed = 6f;         // velocity of "fly" 
    private float jumpImpulse = 11f;        // velocity of "iump" 
    
    Vector2 moveInput; // Input *  TimeFrame * SpeedAction
    TouchingDirections touchingDirections; 
    Damageable damageable;

    public float CurrentMoveSpeed 
    { 
        get
        {
            if (CanMove)
            {
                if (IsMoving && !touchingDirections.IsOnWall)
                {
                    if (touchingDirections.IsGrounded)
                    {
                        if (IsRunning)
                        {
                            return runSpeed;
                        }else 
                        {
                            return walkSpeed;
                        }
                    }
                    else
                    {
                        // air move 
                        return airWalkSpeed;
                    }
                }
                else
                {
                    // idle speed 
                    return 0;
                }
            }
            else
            {
                //Movement locked
                return 0;
            }
            
        }
    } 

    [SerializeField]                    // display the value of the variable on the prefab
    private bool _isMoving = false;     // set parameter for animator transition 

    public bool IsMoving {  get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving,value);
        }
    }
    
    [SerializeField]
    private bool _isRunning = false;   

    public bool IsRunning { 
        get 
        {
            return _isRunning;
        }
        set
        {
            _isRunning = value;
            animator.SetBool(AnimationStrings.isRunning, value);
        }
    }

    public bool _isFacingRight = true;
    public bool IsFacingRight 
    {   get { return _isFacingRight; } 
        private set {
            if(_isFacingRight != value)
            {
                // flip the local scale to make the player face the opposite direction
                transform.localScale *= new Vector2(-1, 1);
            } 
            _isFacingRight = value;
        } }

    public bool CanMove { get
        {
            return animator.GetBool(AnimationStrings.canMove);
        } }

    public bool IsAlive
    {
        get
        {
            return animator.GetBool(AnimationStrings.isAlive);
        }
    }

    public bool LockVelocity 
    { 
        get
        {
            return animator.GetBool(AnimationStrings.lockVelocity);
        }
        set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }

    Rigidbody2D rb;     // the "physic" body of player in game
    Animator animator;  // animations of player

    private void Awake() // "awake" is used to initialize the "player" after "start"
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
    }

    private void FixedUpdate() // add default settings
    {
        if (!damageable.LockVelocity)
        {
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y); // velocity x control by movespeed but y control by gravity
        }
        
        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);

    }
    public void OnMove(InputAction.CallbackContext context) // using the object "context" to class Input... read value vetor in player
    {
        moveInput = context.ReadValue<Vector2>();

        if (IsAlive )
        {
            IsMoving = moveInput != Vector2.zero; // Is moving = 1 or 0; set statement (or value) for move of player

            SetFacingDirection(moveInput);
        }
        else
        {
            IsMoving = false;
        }

        
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if(moveInput.x > 0 && !IsFacingRight)
        {
            //face the right
            IsFacingRight = true;
        }
        else if(moveInput.x < 0 && IsFacingRight)
        {
            // face the left
            IsFacingRight= false;
        }
    }

    public void OnRun(InputAction.CallbackContext context) 
    {
        if(context.started)
        {
            IsRunning = true;
        } else if (context.canceled)
        {
            IsRunning = false;
        }

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // to do check alive as well 
        if(context.started && touchingDirections.IsGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
    }
     
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }

    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.rangedAttackTrigger);
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        LockVelocity = true;
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
}
