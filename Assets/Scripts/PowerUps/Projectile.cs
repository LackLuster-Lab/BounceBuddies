using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Projectile : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float speed = 30;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, .5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVelocity(Vector3 Velocity) {
        rb.velocity = Velocity.normalized * speed;
    }

	private void OnTriggerEnter(Collider other) {
		PowerUpFunctions.instance.onProjectileCallback?.Invoke(this, new PowerUpFunctions.ProjectileCallbackEventArgs {
			position = this.gameObject.transform.position
		});
	}

	private void OnCollisionEnter(Collision collision) {
		PowerUpFunctions.instance.onProjectileCallback?.Invoke(this, new PowerUpFunctions.ProjectileCallbackEventArgs {
			position = this.gameObject.transform.position
		});
	}

	private void OnDestroy() {
        PowerUpFunctions.instance.onProjectileCallback?.Invoke(this, new PowerUpFunctions.ProjectileCallbackEventArgs {
            position = this.gameObject.transform.position
        });
	}
}
