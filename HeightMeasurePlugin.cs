using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Uri;
using UnityEngine;

namespace LFE
{
    public class HeightMeasurePlugin : MVRScript
    {
        JSONStorableFloat fullHeightStorable;
        JSONStorableFloat headHeightStorable;
        JSONStorableFloat heightInHeadsStorable;
        JSONStorableString textStorable;
        JSONStorableFloat markerLeftRightStorable;
        JSONStorableFloat markerFrontBackStorable;

        Atom sign;

        public override void Init()
        {

            textStorable = new JSONStorableString("Text", "", (string text) => {
                if(sign != null) {
                    var t = sign.GetStorableByID("Sign") as TextStorable;
                    t.SetStringParamValue("text", text);
                }
            });
            CreateTextField(textStorable);

            markerLeftRightStorable = new JSONStorableFloat("Marker Left/Right", 0.25f, -1, 1);
            CreateSlider(markerLeftRightStorable, rightSide: true);
            RegisterFloat(markerLeftRightStorable);

            markerFrontBackStorable = new JSONStorableFloat("Marker Front/Back", 0.15f, -1, 1);
            CreateSlider(markerFrontBackStorable, rightSide: true);
            RegisterFloat(markerFrontBackStorable);

            fullHeightStorable = new JSONStorableFloat("Full Height In Meters", 0, 0, 100);
            RegisterFloat(fullHeightStorable);
            headHeightStorable = new JSONStorableFloat("Head Height In Meters", 0, 0, 100);
            RegisterFloat(headHeightStorable);
            heightInHeadsStorable = new JSONStorableFloat("Full Height In Heads", 0, 0, 100);
            RegisterFloat(heightInHeadsStorable);




        }

        private string CalculateText() {
            var height = fullHeightStorable.val;
            var heightInHeads = heightInHeadsStorable.val;
            return $"{UnityToMeters(height):0.##} meters tall\n{UnityToFeet(height):0.##} feet tall ({FeetInchString(UnityToFeet(height))})\n{heightInHeads:0.##} heads tall";
        }

        public void OnEnable() {
            sign = SuperController.singleton.GetAtomByUid("CharacterHeightSign");
            if(sign == null) {
                StartCoroutine(SuperController.singleton.AddAtomByType("SimpleSign", useuid: "CharacterHeightSign"));
            }
        }

        public void OnDisable()
        {
            if(markerHead != null) {
                Destroy(markerHead);
                markerHead = null;
            }
            if(markerFoot != null) {
                Destroy(markerFoot);
                markerFoot = null;
            }
            if(markerChin != null) {
                Destroy(markerChin);
                markerChin = null;
            }

            foreach(var h in extraHeadMarkers) {
                Destroy(h);
            }
            extraHeadMarkers = new List<GameObject>();

            if(sign != null) {
                SuperController.singleton.RemoveAtom(sign);
                sign = null;
            }
            signStorable = null;
        }

        TextStorable signStorable;
        public void Update() {

            try {
                if(sign == null) {
                    sign = SuperController.singleton.GetAtomByUid("CharacterHeightSign");
                }
                UpdateHeadMarker();
                UpdateFootMarker();
                UpdateChinMarker();
                UpdateHeadHeightMarkers();
                fullHeightStorable.val = GetHeight();
                headHeightStorable.val = GetHeadHeight();
                heightInHeadsStorable.val = GetHeightInHeads();
                textStorable.val = CalculateText();
                if(sign != null) {
                    if(signStorable == null) {
                        signStorable = sign.GetStorableByID("Sign") as TextStorable;
                    }
                    signStorable.SetStringParamValue("text", textStorable.val);
                }
            }
            catch(Exception e) {
                SuperController.LogError(e.ToString());
            }

        }

        private float GetHeight() {
            if(markerHead && markerFoot) {
                return Vector3.Distance(markerHead.transform.position, markerFoot.transform.position);
            }
            else {
                return 0;
            }
        }

        private float GetHeadHeight() {
            if(markerHead && markerChin) {
                return Vector3.Distance(markerHead.transform.position, markerChin.transform.position);
            }
            else {
                return 0;
            }
        }

        private float GetHeightInHeads() {
            var height = GetHeight();
            var headHeight = GetHeadHeight();
            if(headHeight > 0) {
                return height/headHeight;
            }
            return 0;
        }

        GameObject markerHead;
        AutoCollider acHeadHard6;
        private void UpdateHeadMarker() {
            if(markerHead == null) {
                markerHead = CreateMarker(Color.red);
            }
            if(acHeadHard6 == null) {
                acHeadHard6 = containingAtom.GetComponentsInChildren<AutoCollider>().FirstOrDefault(ac => ac.name.Equals("AutoColliderAutoCollidersHeadHard6"));
            }

            if(markerHead && acHeadHard6) {
                var capsuleCollider = acHeadHard6.gameObject.GetComponentInChildren<CapsuleCollider>();

                var rot = Quaternion.Euler(acHeadHard6.transform.rotation.eulerAngles);
                var pos = rot * new Vector3(0, capsuleCollider.radius * acHeadHard6.scale, 0) + capsuleCollider.transform.position;
                pos.x = pos.x - markerLeftRightStorable.val;
                pos.z = pos.z + markerFrontBackStorable.val;

                markerHead.transform.position = pos;
            }
        }

        GameObject markerFoot;
        CapsuleCollider rFoot;
        CapsuleCollider lFoot;
        private void UpdateFootMarker() {
            if(markerFoot == null) {
                markerFoot = CreateMarker(Color.red);
            }

            if(!rFoot || !lFoot) {
                rFoot = containingAtom.GetComponentsInChildren<CapsuleCollider>().FirstOrDefault(c => ColliderName(c).Equals("rFoot/_Collider1"));
                lFoot = containingAtom.GetComponentsInChildren<CapsuleCollider>().FirstOrDefault(c => ColliderName(c).Equals("lFoot/_Collider1"));
            }

            if(markerFoot && markerHead && rFoot && lFoot) {
                var pos = Midpoint(rFoot.transform.position, lFoot.transform.position);
                pos.x = markerHead.transform.position.x;
                pos.z = markerHead.transform.position.z;
                markerFoot.transform.position = pos - new Vector3(0, rFoot.radius, 0);
            }

        }

        GameObject markerChin;
        CapsuleCollider chin;
        private void UpdateChinMarker() {
            if(markerChin == null) {
                markerChin = CreateMarker(Color.red);
            }
            if(!chin) {
                chin = containingAtom.GetComponentsInChildren<CapsuleCollider>().FirstOrDefault(c => ColliderName(c).Equals("lowerJaw/_ColliderL1fb"));
            }

            if(markerChin && chin && markerHead) {
                var pos = chin.transform.position - new Vector3(0, chin.radius, 0);
                pos.z = markerHead.transform.position.z;
                pos.x = markerHead.transform.position.x;

                markerChin.transform.position = pos;
            }
        }

        List<GameObject> extraHeadMarkers = new List<GameObject>();
        private void UpdateHeadHeightMarkers() {
            var height = GetHeight();
            var headHeight = GetHeadHeight();
            var heightInHeadsRoundedUp = (int)Mathf.Ceil(GetHeightInHeads());

            if(heightInHeadsRoundedUp != extraHeadMarkers.Count) {
                if(heightInHeadsRoundedUp > extraHeadMarkers.Count) {
                    for(var i = extraHeadMarkers.Count; i < heightInHeadsRoundedUp; i++) {
                        extraHeadMarkers.Add(CreateMarker(Color.green));
                    }
                }

                for(var i = 0; i < extraHeadMarkers.Count; i++) {
                    extraHeadMarkers[i].SetActive(false);
                }
            }

            if(markerHead && height > 0 && headHeight > 0 && markerHead && markerChin) {

                // skip the first marker
                for(var i = 0; i < heightInHeadsRoundedUp - 2; i++) {

                    var pos = markerChin.transform.position;
                    pos.y = pos.y - (headHeight * (i+1));

                    extraHeadMarkers[i].SetActive(true);
                    extraHeadMarkers[i].transform.position = pos;
                }
            }
        }

        private GameObject CreateMarker(Color color) {

            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // random position to help avoid physics problems.
            gameObject.transform.position = new Vector3 ((UnityEngine.Random.value*461)+10, (UnityEngine.Random.value*300)+10, 0F);

            // make it smaller
            gameObject.transform.localScale = new Vector3(0.75f, 0.005f, 0.005f);

            // make it red
            var r = gameObject.GetComponent<Renderer>();
            if(r) {
                r.material.color = color;
            }

            // remove collisions
            foreach(var c in gameObject.GetComponents<Collider>()) {
                Destroy(c);
            }

            return gameObject;
        }

        private Vector3 Midpoint(Vector3 a, Vector3 b) {
            return new Vector3((a.x + b.x) / 2f, (a.y + b.y) / 2f, (a.z + b.z) / 2f);
        }


        private string ColliderName(Collider collider)  {
            var parent = collider.attachedRigidbody != null ? collider.attachedRigidbody.name : collider.transform.parent.name;
            var label = parent == collider.name ? collider.name : $"{parent}/{collider.name}";

            return label;
        }

        private float UnityToMeters(float unit) {
            return unit;
        }

        private float UnityToFeet(float unit) {
            return UnityToMeters(unit)/0.3048f;
        }

        private string FeetInchString(float feet) {
            int f = (int)feet;
            int inches = (int)((feet - f) * 12);
            return $"{f}'{inches}\"";
        }
    }
}
