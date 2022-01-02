using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;



///<remarks>Based On: https://github.com/TrinityCore/TrinityCore/blob/8492c273dd50227ca01ead785eda6c4de9361e74/src/server/game/Spells/SpellScript.cpp</remarks>
[CreateAssetMenu(fileName = "NewCast", menuName = "ScriptableObjects/SpellScripts", order = 0)]
public class SpellScript : ScriptableObject {

    public int        _SpellID;
    public GameObject _SpellPrefab;
    public Sprite     _SpellSprite;
    public Alliance[] _Alliances;
    public Schools    _Type1;
    public Schools    _Type2;
    public Schools    _Type3;

    public float _Damage;
    public float _DamageMin;
    public float _DamageMax;

    public float _Healing;
    public float _HealingMin;
    public float _HealingMax;

    public float _ManaCost;
    public float _ManaCostMin;
    public float _ManaCostMax;

    public float _EnergyCost;
    public float _EnergyCostMin;
    public float _EnergyCostMax;

    public float _Cooldown;
    public float _CooldownMin;
    public float _CooldownMax;

    public float _Range;
    public float _RangeMin;
    public float _RangeMax;

    public string _Name;
    public string _Detail;
    public string _Description;

    private bool _Active = true;
    public  bool isActive { get { return _Active; } set { _Active = value; } }



    public float Damage {
        get {
            Damage = UnityEngine.Random.Range(_DamageMin, _DamageMax);
            return _Damage;
        }
        set {
            if (value <= 0) {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            _Damage = Mathf.Clamp(value, _DamageMin, _DamageMax);
        }
    }


    public float Healing {
        get {
            Healing = UnityEngine.Random.Range(_HealingMin, _HealingMax);
            return _Healing;
        }
        set {
            if (value <= 0) {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            _Healing = Mathf.Clamp(value, _HealingMin, _HealingMax);
        }
    }


    public float ManaCost {
        get {
            ManaCost = UnityEngine.Random.Range(_ManaCostMin, _ManaCostMax);
            return _ManaCost;
        }
        set {
            if (value <= 0) {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            _ManaCost = Mathf.Clamp(value, _ManaCostMin, _ManaCostMax);
        }
    }


    public float EnergyCost {
        get {
            EnergyCost = UnityEngine.Random.Range(_EnergyCostMin, _EnergyCostMax);
            return _EnergyCost;
        }
        set {
            if (value <= 0) {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            _EnergyCost = Mathf.Clamp(value, _EnergyCostMin, _EnergyCostMax);
        }
    }


    public float Cooldown {
        get {
            Cooldown = UnityEngine.Random.Range(_CooldownMin, _CooldownMax);
            return _Cooldown;
        }
        set {
            if (value <= 0) {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            _Cooldown = Mathf.Clamp(value, _CooldownMin, _CooldownMax);
        }
    }


    public float Range {
        get {
            Range = UnityEngine.Random.Range(_RangeMin, _RangeMax);
            return _Range;
        }
        set {
            if (value <= 0) {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            _Range = Mathf.Clamp(value, _RangeMin, _RangeMax);
        }
    }


    // [ExecuteInEditMode]
    // void OnValidate() {
    //     _Range = Mathf.Clamp(_Range, _RangeMin, _RangeMax);
    // }

}
