using Cinemachine.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    //public static Player Instance { get; private set; }

    //GamePlay
    [SerializeField] private int HealthMax;
    private int Health;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxSpeed = 30f;
    [SerializeField] private float squishChange = 5f;
    [SerializeField] private Color BodyColor;
    [SerializeField] private Vector3 squishSize = new Vector3(0.2f, 1.5f, 1f);
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject Parent;
    //Anim
    [SerializeField] private Animator mouthAnim;
    [SerializeField] private Animator EyesAnim;
    [SerializeField] private SpriteRenderer MouthSprite;
    [SerializeField] private SpriteRenderer BodySprite;


    private bool gameStarted = false;
	Vector2 normalVector;
    private Rigidbody2D rb;

    //Events
    public event EventHandler OnPlayerHitWall;

    public event EventHandler<HealthChangeEventArgs> HealthChange;
    public class HealthChangeEventArgs : EventArgs {
        public int newHealth;
    }

    public event EventHandler<UsePowerUPEventArgs> UsePowerUp;
    public class UsePowerUPEventArgs : EventArgs {
        public GameObject gameObject;
        public GameInput gameInput;
    }

    public event EventHandler<UpdateIconArgs> UpdateIcon;
    public class UpdateIconArgs : EventArgs {
        public Sprite Icon;
    }

	public void Awake() {
		//Instance = this;
	}

	public void Start() {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        BodySprite.color = BodyColor;
        MouthSprite.color = BodyColor;
        Health = HealthMax;
        Parent = GameObject.Find("Canvas/PlayersUI");
        GameObject UsedUI = Instantiate(UI, Parent.gameObject.transform);//network issue
        UsedUI.GetComponent<PlayerUI>().setPlayer(this);
		GameInput.instance.onPowerUpPerformed += GameInput_onPowerUpPerformed;
	}

    public void Update() {
        if (!IsOwner) {
            return;
        }
        if (RoundManager.instance.IsGameplaying()) {
            if (!gameStarted) {
                rb.gravityScale = 1;
                gameStarted = true;
            }
            HandleMovement();
        } else {
            gameStarted = false;
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        }
        HandleSquish();
	}

    private void GameInput_onPowerUpPerformed(object sender, EventArgs e) {
        if (RoundManager.instance.IsGameplaying()) {
            UsePowerUp?.Invoke(this, new UsePowerUPEventArgs {
                gameObject = gameObject,
                gameInput = GameInput.instance
            });
            UsePowerUp = null;
            UpdateIcon?.Invoke(this, new UpdateIconArgs {
                Icon = null
            });
        }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        OnPlayerHitWall?.Invoke(this, EventArgs.Empty);
        normalVector = transform.position -  new Vector3(collision.GetContact(0).point.x, collision.GetContact(0).point.y, 0);
        float rotation = Vector3.Angle(normalVector, Vector3.right);
        transform.localScale = Quaternion.Euler(0, 0, rotation) * squishSize;
        transform.localScale = transform.localScale.Abs();
        //do this on dealt damage
        //EyesAnim.SetBool("hit", true);
        //mouthAnim.SetBool("Hit", true);
	}
	public void OnCollisionExit2D(Collision2D collision) {
		//EyesAnim.SetBool("hit", false);
		//mouthAnim.SetBool("Hit", false);
	}

	public void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("Trigger");
        if (collision.TryGetComponent(out PowerUpItem powerUpItem) && UsePowerUp == null) {
            UsePowerUp += powerUpItem.Use;
            UpdateIcon?.Invoke(this, new UpdateIconArgs {
                Icon = powerUpItem.powerUpSO.Sprite
			});
            powerUpItem.collect();
        }
    }


    private void HandleMovement() {
        rb.velocity += GameInput.instance.getMovementVectorNormalized() * moveSpeed; //network issue
    }

    private void HandleSquish() {

		transform.right = Vector3.right;
		if (rb.velocity.magnitude > maxSpeed) {
			rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity.normalized * maxSpeed, Time.deltaTime * 3);
		}

		transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1, 1), Time.deltaTime * squishChange);
	}

    private void changeHealth() {
        HealthChange?.Invoke(this, new HealthChangeEventArgs {
            newHealth = Health
        });
    }

    #region gets/sets
    public float getMoveSpeed() {
        return moveSpeed;
    }
    #endregion
}
