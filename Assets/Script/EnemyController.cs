using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    public bool vertical = false;
    public float speed = 0.3f;
    public float changeDirectTime = 2;
    float changeDirectTimer;
    int changeDirect;
    Animator animator;
    Rigidbody2D rigidbody2d;
    bool broken;
    public float dist = 2;
    public ParticleSystem smokeEffect;
    public AudioClip fix;
    // Start is called before the first frame update
    void Start()
    {
        changeDirectTimer = changeDirectTime;
        changeDirect = 1;
        rigidbody2d = GetComponent<Rigidbody2D>(); 
        animator = GetComponent<Animator>();
        broken = true;
    }

    private void Update()
    {
        if (!broken)
        {
            return;
        }
    }
    private void FixedUpdate()
    {
        if (!broken)
        {
            return;
        }

        if (changeDirectTimer <= 0)
        {
            changeDirect *= -1;
            changeDirectTimer = changeDirectTime;
        }

        Vector2 pos = rigidbody2d.position;
        if (vertical)
        {
            pos.y += speed * Time.deltaTime * changeDirect;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", changeDirect);
        } else 
        { 
            pos.x += speed * Time.deltaTime * changeDirect;
            animator.SetFloat("Move X", changeDirect);
            animator.SetFloat("Move Y", 0);
        }
        rigidbody2d.MovePosition(pos);
        changeDirectTimer -= Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        RubyController controller = collision.gameObject.GetComponent<RubyController>();

        if (controller != null)
        {
            controller.ChangeHealth(-1);
        }
    }

    public void Fix()
    {
        broken = false;
        rigidbody2d.simulated = false;
        animator.SetTrigger("Fixed");
        smokeEffect.Stop();
        AudioSource src = gameObject.GetComponent<AudioSource>();
        src.Stop();
        src.PlayOneShot(fix);
    }

}
