using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;



    public class PostponeCast : ConditionalEffects {
        public float waitTime = 1.0f;

        public UnityEvent trackPing;
        public UnityEvent trackInit;
        public UnityEvent trackCancel;



        public override void RgstCast() { StartCoroutine(WaitThenRgst(waitTime)); }
        private IEnumerator WaitThenRgst(float time) {
            yield return new WaitForSeconds(time);
            base.RgstCast();
        }


        public override void RgstHit(GameObject obj) {}

        public override void RgstCleanUp() {}

    }

