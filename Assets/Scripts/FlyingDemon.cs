using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingDemon : MonoBehaviour
{
    public float flySpeed = 5f;
    public float wpReachedDistance = 0.1f;
    public DetectionZone biteDetectionZone;
    public Collider2D deathCollider;
    public List<Transform> waypoints;
    

    Animator animator;
    Rigidbody2D rb;
    Damageable damageable;

    Transform nextWP;
    int wpNum = 0;

    public bool _hasTarget = false;
    

    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
    }

    private void Start()
    {
        nextWP = waypoints[wpNum];
    }

    /*private void OnEnable()
    {
        damageable.damageableDeath += OnDeath();
    }*/


    // Update is called once per frame
    void Update()
    {
        HasTarget = biteDetectionZone.detectedColiders.Count > 0;

    }

    private void FixedUpdate()
    {
        if (damageable.IsAlive)
        {
            if (CanMove)
            {
                Flight();
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }
    }

    private void Flight()
    {
        // Fly to the next WP
        Vector2 directionToWaypoint = (nextWP.position - transform.position).normalized;

        //check if the Demon has reached wp already
        float distance = Vector2.Distance(nextWP.position, transform.position);

        rb.velocity = directionToWaypoint * flySpeed;
        UpdateDirection();

        //See if we need to switch Wps
        if (distance <= wpReachedDistance)
        {
            //Switch to next waypoint
            wpNum++;
            if (wpNum >= waypoints.Count)
            {
                //Loop back to the original waypoint
                wpNum = 0;
            }

            nextWP = waypoints[wpNum];
        }

    }

    private void UpdateDirection() //Demon Flip right/left
    {
        Vector3 locScale = transform.localScale;
        if (transform.localScale.x > 0)
        {
            //facing the right
            if (rb.velocity.x < 0)
            {
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
        else
        {
            //facinng the left
            if (rb.velocity.x > 0)
            {
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
    }

    public void OnDeath()
    {
        //when demon die, it will fall on the ground
        rb.gravityScale = 3f;
        rb.velocity = new Vector2(0, rb.velocity.y);
        deathCollider.enabled = true;
    }
}
