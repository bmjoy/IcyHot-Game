using UnityEngine;



/*Possibly Delete: Non-magical projectiles*/
// [RequireComponent(typeof(Rigidbody))]
public class CombatProjectile : MonoBehaviour, IPoolableObject {
    public Rigidbody          rigidBody;
    public ConditionalEffects triggerCondition;
    public float              destroyAfter = 5;
    public float              force        = 1000;

    // Required, but unused
    public void Initiate() {}



    // public override void OnStartServer() {
    //     Invoke(nameof(DestroySelf), destroyAfter);
    // }

    // set velocity for server and client. this way we don't have to sync the
    // position, because both the server and the client simulate it.
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.AddForce(transform.forward * force);
        Invoke(nameof(DestroySelf), destroyAfter);
    }


    // destroy 
    void DestroySelf() {
        GameManager.Destroy(gameObject);
    }


    void OnTriggerEnter(Collider co) {
        GameManager.Destroy(gameObject);
    }
}
