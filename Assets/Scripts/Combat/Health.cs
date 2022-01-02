using System;
using System.Collections.Generic;
using UnityEngine;



/* Tutorials:
 https://kybernetik.com.au/platformer/docs/characters/states/flinch/ 
 https://gamedevacademy.org/how-to-create-a-multiplayer-game-in-unity/
 https://titanwolf.org/Network/Articles/Article?AID=bbfd7597-9471-4a0e-9c80-667ac8528213
 */
/// <summary>
/// A health system similar to the system used by the Divinity Original Sin engine
/// </summary>
public class Health : MonoBehaviour {
    public    float  armor;
    public    float  maxArmor;
    public    float  maxHealth;
    public    float  currentHealth;
    public    bool   IsDead = false;
    public    bool   IsHalfHealth;
    public    float  halfHealth;
    protected Avatar lastDamagedBy;


    public delegate void       OnDamageEvent(float damage);
    public event OnDamageEvent OnDeath;
    public event OnDamageEvent OnDamaged;
    public event OnDamageEvent OnHealthDamaged;
    public event OnDamageEvent OnArmorBroken;
    public event OnDamageEvent OnArmorDamaged;
    public event OnDamageEvent WhenDamaged;

    public delegate void       OnHealthEvent(float healing);
    public event OnHealthEvent OnHealthReplenish;
    public event OnHealthEvent OnRevive;

    public delegate void        ConditionEvent(bool status);
    public event ConditionEvent WhenDebuffApplied;
    public event ConditionEvent WhenDebuffRemoved;
    public event ConditionEvent WhenBuffApplied;
    public event ConditionEvent WhenBuffRemoved;

    // Status/Condition containers
    public List<ConditionInventory> _ActiveDebuffs = new List<ConditionInventory>();
    public List<ConditionInventory> _ActiveBuffs   = new List<ConditionInventory>();



    protected virtual void Start() {
        currentHealth = maxHealth;
        armor = maxArmor;
        halfHealth = maxHealth / 2;
        OnDeath += damage => {};
        WhenDamaged += damage => {};
        OnHealthDamaged += damage => {};
        OnArmorBroken += damage => {};
        OnArmorDamaged += damage => {};
        OnHealthReplenish += healing => {};
        OnRevive += healing => {};
        WhenDebuffApplied += data => {};
        WhenDebuffRemoved += data => {};
        WhenBuffApplied += data => {};
        WhenBuffRemoved += data => {};
    }



    // ---------------------------------------------------- Server Callbacks ---------------------------------------------------- \\
    public virtual void OnDamageTaken(float damage, CharacterSkeleton origin) {
        if (IsDead) { return; }


        if (maxArmor > 0) {
            float damageTaken = damage - maxArmor;
            if (damageTaken > 0) {
                maxArmor = 0;
                OnArmorBroken?.Invoke(damage);
                currentHealth -= damageTaken;
                OnHealthDamaged?.Invoke(damageTaken);
            }
            else {
                maxArmor -= damage;
                OnArmorDamaged?.Invoke(damage);
            }
        }
        else {
            currentHealth -= damage;
            OnHealthDamaged?.Invoke(damage);
        }

        WhenDamaged?.Invoke(damage);
        halfHealth = maxHealth / 2;
        IsHalfHealth = (halfHealth >= currentHealth);

        if (currentHealth <= 0) {
            IsDead = true;
            OnDeath?.Invoke(damage);
            Die();
        }


    }


    public virtual void OnHealingReceived(float healing, CharacterSkeleton origin) {
        if (IsDead) {}
    }

    //
    // Append or update a Debuff condition.
    public void AddDebuff(ConditionInventory condition, CharacterSkeleton origin) {
        if (_ActiveDebuffs.Contains(condition))
            return;
        Condition con = condition.GetCondition(this, origin);
        _ActiveDebuffs.Add(condition);
    
        con.OnDropDebuffCondition += () => {
            _ActiveDebuffs.Remove(condition);
            WhenDebuffRemoved?.Invoke(condition);
        };
        con.AddDebuffCondition();
        WhenDebuffApplied?.Invoke(condition);
    }
    
    
    // Append or update a Buff condition.
    public void AddBuff(ConditionInventory condition, CharacterSkeleton origin) {
        if (_ActiveBuffs.Contains(condition))
            return;
        Condition con = condition.GetCondition(this, origin);
        _ActiveBuffs.Add(condition);
    
        con.OnDropBuffCondition += () => {
            _ActiveBuffs.Remove(condition);
            WhenBuffRemoved?.Invoke(condition);
        };
        con.AddBuffCondition();
        WhenBuffApplied?.Invoke(condition);
    }


    public virtual void OnHealthChanged(float oldVal, float newVal) {
        if (newVal < 1) { Die(); }
        else if (newVal > oldVal) {
            
        }
        else if (newVal < oldVal) {
            
        }
    }


    public void OnMaxHealthChanged(float oldVal, float newVal) {
        if (currentHealth > newVal) currentHealth = newVal;
    }


    public void OnMaxArmorChanged(float oldVal, float newVal) {
        if (maxArmor > newVal) maxArmor = newVal;
    }


    public virtual void Die() {
        _ActiveDebuffs.Clear();
        _ActiveBuffs.Clear();
    }
}
