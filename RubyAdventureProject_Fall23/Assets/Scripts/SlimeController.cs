using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour // Brianna D. I Created this new character controller script for the Slime Monsters.
{
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;


    Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;
    

    Animator animator;

    

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();

    }

    void Update()
    {

        timer -= Time.deltaTime; //Do I need this??????

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    void FixedUpdate() //This lets the slimes move up/down or right/left
    {

        Vector2 position = rigidbody2D.position;

        if (vertical)
        {
            position.y = position.y + Time.deltaTime * .8f * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * .8f * direction;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        rigidbody2D.MovePosition(position);

    }

    void OnTriggerStay2D(Collider2D other) //This is similar to the damage zone script, triggers reduceSpeed on Ruby
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            controller.ChangeSpeed(-2);  //Added this to Reduce Ruby's speed!
        }
    }

}
