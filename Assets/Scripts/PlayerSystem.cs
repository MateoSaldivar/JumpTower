using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSystem : MonoBehaviour
{
    public static PlayerSystem instance;
    [HideInInspector]
    public bool isJumping = false;
    [HideInInspector]
    public Transform originalParent;
    public float jumpForce = 10f;         
    public float moveSpeed = 5f;
    public bool startedGame = false;

    private int health = 5;
    private Rigidbody2D rb;
    private Camera mainCamera;
    private float yTop = 50f;
    private Animator anim;
    private SpriteRenderer sprite;
    private bool FlippedX = false;
    private float damageCoolDown = 0;

    public int Health {
        get => health;
        private set {
            if (damageCoolDown <= 0) {
                anim?.SetTrigger("DamageTaken");
                health = value;
                OnHealthUpdated?.Invoke(health);
                damageCoolDown = 2;
            }
        }
    }
    public bool Falling { get => rb.velocity.y <= 0; }
    public UnityEvent OnGameOver;
    public UnityEvent OnGameWon;
    public Action<int> OnHealthUpdated;
    private void Awake()
    {
        if(instance == null) {
            instance = this;
		} else {
            Destroy(instance);
            instance = this;
		}
        originalParent = transform.parent;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
        OnHealthUpdated = new Action<int>(GameOverCheck);
        yTop = MapGenerator.numberOfBackgrounds * 5;
    }

	private void OnDestroy() {
        OnHealthUpdated -= GameOverCheck;
    }

	
	private void Update() {
        MovePlayer();
        JumpInputCheck();
        UpdateCameraPosition();
        CheckDamageCoolDown();
    }

    private void CheckDamageCoolDown() {
        if(damageCoolDown > 0) damageCoolDown -= Time.deltaTime;

    }

    private void MovePlayer() {
        float moveDirection = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
        anim.SetFloat("VelocityY", rb.velocity.y);
        anim.SetBool("Walking", Mathf.Abs(rb.velocity.x) > 1f);
        if (rb.velocity.x < 0 && FlippedX) FlippedX = false;
        else if(rb.velocity.x > 0 && !FlippedX) FlippedX=true;

        sprite.flipX = FlippedX;
        
    }

    private void JumpInputCheck() {
        if (Input.GetKeyDown(KeyCode.UpArrow) && !isJumping && Mathf.Abs(rb.velocity.y)<0.1f){
            anim.SetTrigger("jump");
            Jump();
        }
    }

    private void Jump(float jumpStrength = -1) {
        if (jumpStrength < 0) {
            jumpStrength = jumpForce;
        }

        rb.velocity = new Vector2(rb.velocity.x, jumpStrength);
        isJumping = true;
        startedGame = true;
        
        anim.SetBool("isJumping",isJumping);
    }

    private void UpdateCameraPosition() {
        if (transform.position.y > mainCamera.transform.position.y) {
            float newY = Mathf.Clamp(transform.position.y, 0, yTop);
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, newY, mainCamera.transform.position.z);
        }
    }

    public void HitEnemy() {
        Health -= 1; 
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isJumping = false;
            anim.SetBool("isJumping", isJumping);
        } else if (collision.gameObject.CompareTag("DeathPlane")) {
            HitEnemy();
            Jump();
		}
    }

	private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Finish")) {
            OnGameWon?.Invoke();
        }
    }

	private void GameOverCheck(int currentHealth) {
        if (currentHealth <= 0) {
            GameOver();
        }
    }

    private void GameOver() {
        OnGameOver?.Invoke();
    }
}
