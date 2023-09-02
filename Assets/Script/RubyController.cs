using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

public class RubyController : MonoBehaviour
{   
    public int maxHealth = 5;
    public int health { get { return currentHealth; } }
    int currentHealth;

    public float speed;

    float horizontal, vertical;

    Rigidbody2D rigidbody2d;

    bool invincible;
    float invincibleTimer;
    public float timeInvincible;

    Animator animator;

    Vector2 lookDirection = new Vector2(1, 0);

    public GameObject projectilePrefab;

    private AudioSource audioSource;
    public AudioClip cogBullet;
    public AudioClip playerHit;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        invincible = false;
        animator = GetComponent<Animator>();
        //QualitySettings.vSyncCount = 0;
        //UnityEngine.Application.targetFrameRate = 10;
        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        Vector2 move = new Vector2(horizontal, vertical);
        
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (invincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0)
            {
                invincible = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.C)) Launch();
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position 
                + Vector2.up * 0.2f, lookDirection, 
                1.5f, 
                LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                Debug.Log("Raycast has hit the object " + hit.collider.gameObject);
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialogBox();
                }
            }
        }
    }

    void FixedUpdate()
    {
        //Debug.Log(horizontal + " " + vertical);
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.fixedDeltaTime;
        position.y = position.y + speed * vertical * Time.fixedDeltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (invincible) return;
            animator.SetTrigger("Hit");
            PlaySound(playerHit);
            invincible = true;
            invincibleTimer = timeInvincible;
        }
        currentHealth = Mathf.Clamp(0, currentHealth + amount, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float) maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);
        PlaySound(cogBullet);
        animator.SetTrigger("Launch");
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
