using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public static EnemyController instance { get; private set; }

    public float speed { get { return currentSpeed; } }
    float currentSpeed;

    public int maxHealth = 5;

    public GameObject projectilePrefab;

    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip winGame; //Brianna D. first sound
    public AudioClip loseGame; //Hadassah R. first sound
    public AudioClip hitSlime; //Brianna D. second sound
    public AudioClip dialogAppear;//Brianna D. third sound
   


    public GameObject healthIncreasePrefab;
    public GameObject healthDecreasePrefab;

    public int health { get { return currentHealth; } }
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    public float invincibleTimer;

    public float timeAltered = 4.0f; //Brianna D. added this to slow down ruby down
    bool isAlteredSpeed;
    public float alteredSpeedTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;

    public TextMeshProUGUI scoreText;
    public int score = 0;

    public GameObject loseMenu;
    public GameObject winMenu;
    bool gameActive = true;
    public GameObject BGM; //Background Music



    private void Awake()
    {
        Time.timeScale = 1;
        currentHealth = maxHealth;
    }


    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentSpeed = 3.0f; 

        audioSource = GetComponent<AudioSource>();

        scoreText.text = "Fixed Robots: " + score.ToString();


    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("speed = " + currentSpeed.ToString());

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

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (isAlteredSpeed)
        {
            alteredSpeedTimer -= Time.deltaTime;
            if (alteredSpeedTimer < 0)
            { 
              isAlteredSpeed = false;
              currentSpeed = 3.0f;
            }
               
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                    PlaySound(dialogAppear); //Brianna D- third audio change for when dialog box appears
                }
            }

        }

        if (!gameActive)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene
            }
        }


    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + currentSpeed * horizontal * Time.deltaTime;
        position.y = position.y + currentSpeed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            animator.SetTrigger("Hit");
            PlaySound(hitSound);
        }

        if (amount < 0)
        {
            GameObject healthDecrease = Instantiate(healthDecreasePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            animator.SetTrigger("Hit");
            PlaySound(hitSound);
        }


        if (amount > 0)
        {
            GameObject healthIncrease = Instantiate(healthIncreasePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }


        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if (health <= 0)
        {
            loseMenu.SetActive(true);
            gameActive = false;
            Time.timeScale = 0;

            BGM.GetComponent<AudioSource>().mute = true;

            PlaySound(loseGame);    //Hadassah R. Added my first Audio clip for losing game.
        }

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void ChangeScore(int scoreAmount)
    {
        score += scoreAmount;
        scoreText.text = "Fixed Robots: " + score.ToString();

        if (score >= 4)
        {
            winMenu.SetActive(true);
            gameActive = false;
            Time.timeScale = 0;

            BGM.GetComponent<AudioSource>().mute = true;

            PlaySound(winGame); //Brianna D. This is where I was able to add the audio clip for winning the game
        }

    }

    public void ChangeSpeed(float amount)  //This where: Brianna D.- slowed Ruby's speed when she hit the slime. Haddash R.- increased Ruby's speed  when she picks up PowerPickup.
    {
        if (amount < 0) //Brianna D.- When Ruby hits slime, reduce speed
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

            isAlteredSpeed = true;
            alteredSpeedTimer = timeAltered;

            animator.SetTrigger("Hit");
            PlaySound(hitSlime); //Brianna D. added the audio clip for when Ruby hits a Slime Monster
        }

        if (amount > 0) //Hadassah R. - When Ruby picks up the PowerPickup, speed is increased
        {

            isInvincible = true;
            invincibleTimer = timeInvincible;

            isAlteredSpeed = true;
            alteredSpeedTimer = timeAltered;

        }

        currentSpeed = Mathf.Clamp(currentSpeed + amount, 1, 5); 

    }


}