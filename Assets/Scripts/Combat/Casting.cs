using UnityEngine;
using UnityEngine.Events;



    /**/
    public class SpellCharging : ConditionalEffects {
        public  float MaxCastTime;
        public  float MinCastTime;
        public  int   increment;
        private float castingPoint;

        [HideInInspector] private bool initiated;
        [HideInInspector] private bool finished;
        [HideInInspector] private bool abandoned;

        public UnityEvent trackPing;
        public UnityEvent trackInit;
        public UnityEvent trackCancel;



        public override void RgstCast() {}

        public override void RgstHit(GameObject obj) {}

        public override void RgstCleanUp() {}

    }