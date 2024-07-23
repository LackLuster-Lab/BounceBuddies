using Cinemachine.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxSpeed = 30f;
    [SerializeField] private float squishChange = 5f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Vector3 squishSize = new Vector3(0.2f, 1.5f, 1f);
    [SerializeField] private Animator mouthAnim;
    [SerializeField] private Animator EyesAnim;

    Vector2 normalVector;
    
    public event EventHandler<UsePowerUPEventArgs> UsePowerUp;
    public class UsePowerUPEventArgs : EventArgs {
        public GameObject gameObject;
        public GameInput gameInput;
    }
    private Rigidbody2D rb;
    public void Start() {
        rb = GetComponent<Rigidbody2D>();
        gameInput.onPowerUpPerformed += GameInput_onPowerUpPerformed;
    }

    private void GameInput_onPowerUpPerformed(object sender, EventArgs e) {
        UsePowerUp?.Invoke(this, new UsePowerUPEventArgs {
            gameObject = gameObject,
            gameInput = gameInput
        });
        UsePowerUp = null;
    }

    public void Update() {
        HandleMovement();
    }
    public void OnCollisionEnter2D(Collision2D collision) {
        normalVector = transform.position -  new Vector3(collision.GetContact(0).point.x, collision.GetContact(0).point.y, 0);
        float rotation = Vector3.Angle(normalVector, Vector3.right);
        transform.localScale = Quaternion.Euler(0, 0, rotation) * squishSize;
        transform.localScale = transform.localScale.Abs();
        
        //do this on dealt damage
        //EyesAnim.SetBool("hit", true);
        //mouthAnim.SetBool("Hit", true);
	}
	public void OnCollisionExit2D(Collision2D collision) {
		EyesAnim.SetBool("hit", false);
		mouthAnim.SetBool("Hit", false);
	}

	public void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("Trigger");
        if (collision.TryGetComponent(out PowerUpItem powerUpItem) && UsePowerUp == null) {
            UsePowerUp += powerUpItem.Use;
            powerUpItem.collect();
        }
    }



    private void HandleMovement() {
        rb.velocity += gameInput.getMovementVectorNormalized() * moveSpeed;

        transform.right = Vector3.right;
        if (rb.velocity.magnitude > maxSpeed) {
            rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity.normalized * maxSpeed, Time.deltaTime * 3);
        }

        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1, 1), Time.deltaTime * squishChange);
    }

    #region gets/sets
    public float getMoveSpeed() {
        return moveSpeed;
    }
    #endregion

}
