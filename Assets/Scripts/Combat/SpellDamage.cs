using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Unity default damage template
/// </summary>
public class SpellDamage : MonoBehaviour {

    public  float            dmgAmount      = 1;
    public  int              flinchPower;
    public  AudioClip        hitAudio;
    public  bool             IsHit = true;
    private List<GameObject> objectsHit;


    
    public float getDamage() {
        return dmgAmount;
    }

    public float GetDamageF() {
        return dmgAmount;
    }

    public int GetDamageI() {
        return (int)dmgAmount;
    }


    public void setDamage(float i) {
        dmgAmount = i;
    }

    
    public void OnTriggerEnter(Collider other) {
        if (!other.isTrigger) {
            if (other.tag.Equals("Spell")) {
                if (!objectsHit.Contains(GameObject.FindWithTag("Health"))) {}
            }
        }
    }


}