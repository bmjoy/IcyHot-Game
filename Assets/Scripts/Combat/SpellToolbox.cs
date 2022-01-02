using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class SpellToolbox : MonoBehaviour {
    private Avatar    avatar;
    public  Transform CastingPoint;

    [Header("Resources")]
    public float _MaximumMana = 5f;
    public float _CurrentMana;
    public float _ManaRegen = 0.5f;

    [Header("Active")]
    public SpellScript selectedScript;
    public SpellScript basicSpellScript;
    public SpellScript activeScript;
    public float       switchDelay;
    public Spell       activeSpell;


    [Header("Firing")]
    public Transform spellMount;
    public                  AnimationClip currentCastAnimation;
    private static readonly int           _Shoot = Animator.StringToHash("Shoot");


    [Header("Toolbox")]
    public List<SpellScript> scripts;
    public int   spellCount;
    public int   currentIndex;
    public float _Delay;

    [Header("Audio")]
    public AudioSource spellAudio;
    public AudioClip cancelCastAudioClip;
    public AudioClip lowManaAudioClip;
    public AudioClip coolingDownAudioClip;
    
    private List<GameObject> spells;

    public delegate void ToolEvent(SpellScript currentlyEquipped);
    private         bool _Anchoring;
    public          bool IsAnchoring { get { return _Anchoring; } set { _Anchoring = value; } }
    private         bool _Deflecting;
    public          bool IsDeflecting { get { return _Deflecting; } set { _Deflecting = value; } }



    private void Start() {
        avatar = GetComponent<Avatar>();
        avatar.toolbox = this;
        CastingPoint = avatar.aimPoint;
    }


    private void Update() {
        if (activeSpell && activeSpell.Done()) {
            activeSpell.Despawn();
            IsAnchoring = false;
            IsDeflecting = false;
            activeSpell = null;
        }

        // Mana regeneration
        float dT   = Time.deltaTime;
        float mana = _CurrentMana + _ManaRegen * dT;
        _CurrentMana = Mathf.Clamp(mana, 0, _MaximumMana);
    }




    // ----------------------------------------------------Cooldown Control---------------------------------------------------- \\
    //
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="script"></param>
    /// <param name="cooldownType"></param>
    /// <returns></returns>
    public IEnumerator SpellCooldown(SpellScript script, CooldownType cooldownType) {
        DeactivateScript(script);
        yield return new WaitForSeconds(GetCooldownTime(script, cooldownType));
        ActivateScript(script);
    }
    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="script"></param>
    private static void DeactivateScript(SpellScript script) {
        script.isActive = false;
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="script"></param>
    private static void ActivateScript(SpellScript script) {
        script.isActive = true;
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="script"></param>
    /// <param name="cooldownType"></param>
    /// <returns></returns>
    private float GetCooldownTime(SpellScript script, CooldownType cooldownType) {
        return cooldownType switch {
            CooldownType.Global      => GetGCD(),
            CooldownType.Interrupted => GetForcedCancel(script),
            _                        => script.Cooldown
        };
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float GetGCD() { return 1f; }

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="script"></param>
    /// <returns></returns>
    private float GetForcedCancel(SpellScript script) {
        return UnityEngine.Random.Range(script._CooldownMin, script._CooldownMax);
    }

    
    /// <summary>
    /// 
    /// </summary>
    public void BasicFire() {
        // animator.SetTrigger("isCasting");
        if (IsDeflecting) { return; }
        if (CheckMana(basicSpellScript.ManaCost) && basicSpellScript.isActive) {
            _CurrentMana -= basicSpellScript.ManaCost;
            if (_CurrentMana < 0) _CurrentMana = 0;

            string spellfabrication = basicSpellScript._SpellPrefab.GetComponent<GameObject>().name;
            activeScript = basicSpellScript;

            Instantiate(activeScript._SpellPrefab, CastingPoint.position, CastingPoint.rotation);
            StartCoroutine(SpellCooldown(basicSpellScript, CooldownType.Standard));

        }
    }


    /// <summary>
    /// 
    /// </summary>
    public void SpecialFire() {
        // if (activeSpell) { activeSpell.Cast(); }
        if (!selectedScript) {
            return;
        }

        if (CheckMana(selectedScript.ManaCost) && selectedScript.isActive) {
            _CurrentMana -= selectedScript.ManaCost;
            if (_CurrentMana < 0) _CurrentMana = 0;

            string spellfabrication = selectedScript._SpellPrefab.GetComponent<GameObject>().name;
            activeScript = selectedScript;
            
            Instantiate(activeScript._SpellPrefab, CastingPoint.position, CastingPoint.rotation);
            StartCoroutine(SpellCooldown(selectedScript, CooldownType.Standard));

        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="spellScripts"></param>
    public void AssignSpells(List<SpellScript> spellScripts) {
        scripts = spellScripts;
    }


    /// <summary>
    /// 
    /// </summary>
    public void PostEffect() {}


    /// <summary>
    /// 
    /// </summary>
    /// <param name="spellCast"></param>
    public void PostCast(Spell spellCast) {
        spellCast.Initiate();
        spellCast.Cast();

        spellCast.caster = avatar;
        activeSpell = spellCast;

        // Maintain casting stance for an increasing length of time until player released.
        IsAnchoring = spellCast.anchored;

        // Maintain shield for an increasing length of time until player released.
        IsDeflecting = spellCast.deflection;
    }


    /// <summary>
    /// Transfer ownership of scripts after building from imported data.
    /// </summary>
    /// <param name="tools"></param>
    public void BuildToolBox(List<SpellScript> tools) {
        avatar = GetComponent<Avatar>();
        avatar.toolbox = this;
        scripts = tools;
    }
    

    /// <summary>
    /// Casting check. Is the entity in the process of initiating an ability?
    /// </summary>
    /// <returns></returns>
    public bool IsCasting() { return activeSpell != null; }


    /// <summary>
    /// Cancel the player ability cast post initiation.
    /// </summary>
    public void CancelCasting() {
        if (activeSpell) {
            activeSpell = null;
            selectedScript = null;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="cost"></param>
    /// <returns></returns>
    private bool CheckMana(float cost) {
        return (_CurrentMana > cost);
    }

    
    /// <summary>
    /// Select or "equip" the desired specialty spell.  
    /// </summary>
    /// <param name="spellIndex"></param>
    public void ScriptSelection(int spellIndex) {
        if (scripts.Capacity <= spellIndex) { return; }
        if (!scripts[spellIndex]) { return; }
        selectedScript = scripts[spellIndex];
    }


    /// <summary>
    /// 
    /// </summary>
    public void Release() {
        if (activeSpell) { activeSpell.Release(); }
    }


    /// <summary>
    /// 
    /// </summary>
    public void Postpone() {
        if (activeSpell) { activeSpell.Postpone(); }
    }
}
