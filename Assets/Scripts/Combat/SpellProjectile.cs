using UnityEngine;



/*Spell Pack/Original*/
[RequireComponent(typeof(Rigidbody))]
public class SpellProjectile : ConditionalEffects {
    public  GameObject spellFabrication;
    private GameObject spellProjectile;
    private SpellCast  combatProjectile;
    // public  float      destroyAfter = 5;
    // public  float      force;



    // public override void OnStartServer() {
    //     Invoke(nameof(DestroySelf), destroyAfter);
    // }

    // set velocity for server and client. this way we don't have to sync the
    // position, because both the server and the client simulate it.
    public override void Register(Spell spellCast) {
        ObjectPool.RegisterPrefab(spellFabrication.name, 1);
        base.Register(spellCast);

        // rigidBody = GetComponent<Rigidbody>();
        // rigidBody.AddForce(transform.forward * force);
        // Invoke(nameof(DestroySelf), destroyAfter);
    }


    private void CastProjectile() {
        spellProjectile = ObjectPool.pingleton.GetFromPool(spellFabrication.name, transform.position, transform.rotation);
        combatProjectile = spellProjectile.GetComponent<SpellCast>();
        combatProjectile.triggerCondition = this;
        // combatProjectile.spellBody = GetComponent<Rigidbody>();
        // combatProjectile.spellBody.AddForce(transform.forward * force);
        // Invoke(nameof(DestroySelf), destroyAfter);
    }


    public override void RgstCast() { 
        /*base.RgstCast();*/
        CastProjectile();
    }


    /*
    // destroy for everyone on the server
    [Server]
    void DestroySelf() {
        NetworkServer.Destroy(gameObject);
    }

    
    // ServerCallback because we don't want a warning if OnTriggerEnter is
    // called on the client
    [ServerCallback]
    void OnTriggerEnter(Collider co) {
        NetworkServer.Destroy(gameObject);
    }
    */


}
