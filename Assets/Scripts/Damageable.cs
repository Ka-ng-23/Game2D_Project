using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damagableHit;
    public UnityEvent damageableDeath;
    public UnityEvent<int, int> healthChanged;
    Animator animator;

    public UIManager manager;

    [SerializeField]
    private int _maxHealth = 100;

    public int MaxHealth
    {
        get { return _maxHealth;}
        set { _maxHealth = value;}
    }

    [SerializeField]
    private int _health = 100;

    public int Health
    {
        get { return _health; }
        set 
        { 
            _health = value; 
            healthChanged?.Invoke(_health, MaxHealth);
            // if health <= 0, character is no longer alive
            if (_health <= 0)
            {
                IsAlive = false;
            }
        }

        
    }

    [SerializeField]
    private bool _isAlive = true;

    [SerializeField]
    private bool isInvincible = false;

    /*public bool IsHit 
    {
        get
        {
            return animator.GetBool(AnimationStrings.isHit);
        }
        
        private set
        {
            animator.SetBool(AnimationStrings.isHit, value);
        } 
    }*/
    private float timeSinceHit = 0;
    public float invincibilityTimer = 0.25f;

    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("Is Alive set " + value);

            if (value == false)
            {
                damageableDeath.Invoke();
            }
        }
    }

    //The Velocity should be changed while this is true but need to be respected by other physic components like the PlayerController
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

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invincibilityTimer)
            {
                // Remmove Invincibility
                isInvincible = false ;
                timeSinceHit = 0;
            }
            timeSinceHit += Time.deltaTime;
        }

        if (Health <=0 && !IsAlive)
        {
            manager.gameOver();
        }
    }


    public bool Hit(int damage, Vector2 knockback)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;

            //Notify other subcribed components that damageable was hit to handle the knowback and such 
            animator.SetTrigger(AnimationStrings.hitTrigger);

            LockVelocity = true;
            damagableHit?.Invoke(damage, knockback); // '.invoke': to call the event
            CharacterEvents.characterDamaged.Invoke(gameObject, damage);
            return true;
        }
        //Unable to be hit
        return false;
    }

    //Returns whether the character was healed or not
    public bool Heal(int healthRestore)
    {
        if (IsAlive && Health < MaxHealth)
        {
            int maxHeal = Mathf.Max(MaxHealth - Health, 0);
            int actualHeal = Mathf.Min(maxHeal, healthRestore);
            Health += actualHeal;

            CharacterEvents.characterHealed(gameObject, actualHeal);

            return true;
        }

        return false ;
    }

}
