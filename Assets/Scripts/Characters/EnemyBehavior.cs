using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : Damageable
{
    public Animator animator;
    public Rigidbody2D rb;
    public GameObject splatterEffect;

    private GameObject player;

    public float speed;
    public float attackSpeed;
    public float attackDuration;

    public int health;
    public bool attacking = false;
    public string direction = " "; 

    public float attackCoolDown;

    private Vector2 attackDirection;

    private float nextAction = 0f;
    
    private float attackDistance = 2f;
    private float distance = 0f;
    private bool directionLock = false;

    //private bool isAlive = true;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        player = PlayerSystem.Inst.GetPlayer();

        distance = Vector2.Distance(transform.position, player.transform.position);
        //attackSpeed = 1.5f;
        attackDirection = player.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        player = PlayerSystem.Inst.GetPlayer();
        if (player == null)
        {
            return;
        }

        distance = Vector2.Distance(transform.position, player.transform.position);
        updateDirection();

        //update sprite 
        Vector3 characterScale = transform.localScale;
        if(direction.Equals("right") && !directionLock){
            characterScale.x = -(Mathf.Abs(transform.localScale.x));
        }
        if(direction.Equals("left") && !directionLock){
            characterScale.x = Mathf.Abs(transform.localScale.x);
        }
        transform.localScale = characterScale;

        //Check attack state
        if(attacking == false){
        //update movement
            if(distance > attackDistance){
                transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);

            }else{
                //if cooldown time if over then attack
                if(Time.time > nextAction){
                    attackDirection = player.transform.position - transform.position;
                    attacking = true;
                    nextAction = Time.time + attackDuration;
                    rb.velocity =  attackDirection * attackSpeed;
                    directionLock = true;
                    animator.SetBool("attacking", true);
                }
            }
        }else{
            //stop attacking if attack duration is over
            if(Time.time > nextAction){
                attacking = false;
                directionLock = false;
                animator.SetBool("attacking", false);
                rb.velocity = new Vector2(0, 0); 
                nextAction = Time.time + attackCoolDown;
            }
        }
        
    }

    // Update the direction string to indicate whatch direction we are primarily moving
    void updateDirection(){
        Vector2 currDirection = player.transform.position - transform.position;
        float x = currDirection.x;
        float y = currDirection.y;

        if(Mathf.Abs(x) >= Mathf.Abs(y) ){
            animator.SetInteger("direction", 1);
            if(x > 0){
                direction = "right";
            }else{
                direction = "left";
            }

        }else{
            if(y > 0){
                animator.SetInteger("direction", 3);
                direction = "up";
            }else{
                animator.SetInteger("direction", 2);
                direction = "down";
            }
        }

    }

    // Damagable method implementations
    public override void Kill()
    {
        // Stub, add death animation
        Destroy(gameObject);
    }
    public override void RecieveDamage(HitboxData damageInfo, GameObject collider = null)
    {
        health -= damageInfo.Damage;

        if (collider != null && splatterEffect != null)
        {
            GameObject effect = Instantiate(splatterEffect);
            effect.transform.position = collider.transform.position;
            effect.transform.right = collider.transform.right;
        }

        if (health <= 0)
        {
            Kill();
        }
    }
}
