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
        // public const float BUST_FUDGE_FACTOR = 0.75f;
        // public const float BAND_FUDGE_FACTOR = 0.80f;


        JSONStorableFloat fullHeightStorable;
        JSONStorableFloat headHeightStorable;
        JSONStorableFloat heightInHeadsStorable;
        JSONStorableString textStorable;
        JSONStorableFloat markerLeftRightStorable;
        JSONStorableFloat markerFrontBackStorable;
        JSONStorableBool showBreastMarkersStorable;

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

            showBreastMarkersStorable = new JSONStorableBool("Show Breast Measure Markers", false);
            CreateToggle(showBreastMarkersStorable);
            RegisterBool(showBreastMarkersStorable);

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
            // var bustCircumference = GetBustCircumference();
            // var bustCircumferenceInch = Mathf.RoundToInt(UnityToFeet(bustCircumference) * 12);
            // var bandCircumference = GetBandCircumference();
            // var bandCircumferenceInch = Mathf.RoundToInt(UnityToFeet(bandCircumference) * 12);
            // var cupUs = GetCupSizeUS(bustCircumference, bandCircumference);
            var cupUsV2 = GetCupSizeUS(circumferenceBust, circumferenceBand);

            return $"{UnityToMeters(height):0.##} meters tall\n"
                + $"{UnityToFeet(height):0.##} feet tall ({FeetInchString(UnityToFeet(height))})\n"
                + $"{heightInHeads:0.##} heads tall\n\n"
                // + $"Bust: {(bustCircumference * 100):0} cm / {bustCircumferenceInch} in\n"
                // + $"Band : {(bandCircumference * 100):0} cm / {bandCircumferenceInch} in\n"
                // + $"Cup : {cupUs} US\n"
                + $"Bust: {(circumferenceBust * 100):0} cm / {Mathf.RoundToInt(UnityToFeet(circumferenceBust) * 12)} in\n"
                + $"Band : {(circumferenceBand * 100):0} cm / {Mathf.RoundToInt(UnityToFeet(circumferenceBand) * 12)} in\n"
                + $"Cup : {cupUsV2} US";
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

            // foreach(var h in bustMarkers) {
            //     Destroy(h);
            // }
            // bustMarkers = new List<GameObject>();

            // foreach(var h in underbustMarkers) {
            //     Destroy(h);
            // }
            // underbustMarkers = new List<GameObject>();

            if(sign != null) {
                SuperController.singleton.RemoveAtom(sign);
                sign = null;
            }
            signStorable = null;

            foreach(var h in bustMarkersFromMorph) {
                Destroy(h);
            }
            bustMarkersFromMorph = new List<GameObject>();

            foreach(var h in bandMarkersFromMorph) {
                Destroy(h);
            }
            bandMarkersFromMorph = new List<GameObject>();

        }

        TextStorable signStorable;
        public void Update() {

            if(SuperController.singleton.freezeAnimation) {
                return;
            }

            try {
                if(sign == null) {
                    sign = SuperController.singleton.GetAtomByUid("CharacterHeightSign");
                }
                UpdateHeadMarker();
                UpdateFootMarker();
                UpdateChinMarker();
                UpdateHeadHeightMarkers();

                UpdateBustMarkersFromMorphVertex();
                UpdateBandMarkersFromMorphVertex();
                // UpdateBustMarkers(); // TODO: check male for bug
                // UpdateUnderbustMarkers(); // TODO: check male for bug

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

        List<GameObject> bustMarkersFromMorph = new List<GameObject>();
        float circumferenceBust = 0;
        private void UpdateBustMarkersFromMorphVertex() {
            var vertexIndexes = new List<int> {
                15, // bust - left nipple flattened
                10939, // bust -- right nipple flattened
                // 13233, // bust -- right breast side 1
                13234, // bust -- right breast side 1
                //13674, // bust -- right breast side 2
                11022, // bust -- right back
                10495, // bust -- back center
                // 10895, // bust -- back center
                // 8922, // bust - left breast side ??
                // 8928, // bust - left breast side ??
                // 8951, // bust -- left breast side ??
                // 11021, // bust -- right breast side
            };

            DAZCharacter dazCharacter = containingAtom.GetComponentInChildren<DAZCharacter>();
            DAZSkinV2 skin = dazCharacter.skin;

            if(showBreastMarkersStorable.val) {
                if(bustMarkersFromMorph.Count != vertexIndexes.Count) {
                    foreach(var m in bustMarkersFromMorph) {
                        Destroy(m);
                    }
                    bustMarkersFromMorph.Clear();
                    foreach(var m in vertexIndexes){
                        bustMarkersFromMorph.Add(CreateMarker(Color.red, "Sphere"));
                    }
                }

                for(var i = 0; i < vertexIndexes.Count; i++) {
                    bustMarkersFromMorph[i].transform.position = skin.rawSkinnedVerts[vertexIndexes[i]];
                }
            }
            else {
                foreach(var m in bustMarkersFromMorph) {
                    Destroy(m);
                }
                bustMarkersFromMorph.Clear();
            }


            var circumference = 0f;
            // we are only calculating 1/2 of the measurements around the bust so double most measurements
            for(var i = 0; i < vertexIndexes.Count; i++) {
                if(i == 0) {
                    continue;
                }

                var distance = Vector3.Distance(skin.rawSkinnedVerts[vertexIndexes[i-1]], skin.rawSkinnedVerts[vertexIndexes[i]]);
                distance = i == 1 ? distance : distance * 2;

                circumference += distance;
            }
            circumferenceBust = circumference;
        }

        List<GameObject> bandMarkersFromMorph = new List<GameObject>();
        float circumferenceBand = 0;
        private void UpdateBandMarkersFromMorphVertex() {
            var vertexIndexes = new List<int> {
                7221,  // band - left breast under nipple
                13240, // band - right breast under nipple
                21472,
                21384,
                11022, // bust -- right back
                10495, // bust -- back center
            };

            DAZCharacter dazCharacter = containingAtom.GetComponentInChildren<DAZCharacter>();
            DAZSkinV2 skin = dazCharacter.skin;

            if(showBreastMarkersStorable.val){
                if(bandMarkersFromMorph.Count != vertexIndexes.Count) {
                    foreach(var m in bandMarkersFromMorph) {
                        Destroy(m);
                    }
                    bandMarkersFromMorph.Clear();
                    foreach(var m in vertexIndexes){
                        bandMarkersFromMorph.Add(CreateMarker(Color.white, "Sphere"));
                    }
                }

                for(var i = 0; i < vertexIndexes.Count; i++) {
                    bandMarkersFromMorph[i].transform.position = skin.rawSkinnedVerts[vertexIndexes[i]];
                }
            }
            else {
                foreach(var m in bandMarkersFromMorph) {
                    Destroy(m);
                }
                bandMarkersFromMorph.Clear();
            }

            var circumference = 0f;
            // we are only calculating 1/2 of the measurements around the bust so double most measurements
            for(var i = 0; i < vertexIndexes.Count; i++) {
                if(i == 0) {
                    continue;
                }

                var distance = Vector3.Distance(skin.rawSkinnedVerts[vertexIndexes[i-1]], skin.rawSkinnedVerts[vertexIndexes[i]]);
                distance = i == 1 ? distance : distance * 2;

                circumference += distance;
            }
            circumferenceBand = circumference;
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

        // string bustLog = "";

        // private float GetBustCircumference() {

        //     float circumference = 0;

        //     // order: left nipple, right nipple, right side, back right, back left, left side

        //     var distances = new List<string>();
        //     for(var i = 0; i < bustMarkers.Count; i++) {
        //         var prev = i == 0 ? bustMarkers[bustMarkers.Count - 1] : bustMarkers[i - 1];
        //         var me = bustMarkers[i];

        //         var distance = Mathf.Abs(Vector3.Distance(prev.transform.position, me.transform.position));
        //         if(i == 0 || i == 2 || i == 3 || i == 5) {
        //             distance *= BUST_FUDGE_FACTOR;
        //         }

        //         distances.Add(distance.ToString("0.000"));

        //         circumference += distance;
        //     }

        //     // var s = string.Join(",", distances.ToArray());
        //     // if(!s.Equals(bustLog)) {
        //     //     SuperController.LogMessage($"Bust: {s}");
        //     // }
        //     // bustLog = s;

        //     return circumference;
        // }

        // private float GetBandCircumference() {
        //     float circumference = 0;

        //     // order: left nipple, right nipple, right side, back right, back left, left side

        //     var distances = new List<string>();
        //     for(var i = 0; i < underbustMarkers.Count; i++) {
        //         var prev = i == 0 ? underbustMarkers[underbustMarkers.Count - 1] : underbustMarkers[i - 1];
        //         var me = underbustMarkers[i];

        //         var distance = Mathf.Abs(Vector3.Distance(prev.transform.position, me.transform.position));
        //         if(i == 0 || i == 2 || i == 3 || i == 5) {
        //             distance *= BAND_FUDGE_FACTOR;
        //         }

        //         distances.Add(distance.ToString("0.0000"));

        //         circumference += distance;
        //     }

        //     return circumference;
        // }

        private string GetCupSizeUS(float bust, float band) {
            var bustIn = Mathf.RoundToInt(UnityToFeet(bust) * 12);
            var bandIn = Mathf.RoundToInt(UnityToFeet(band) * 12);
            var diff = Mathf.Clamp(Mathf.RoundToInt(bustIn - bandIn), 0, 100);

            switch(diff) {
                case 0:
                    return $"{bandIn}AA";
                case 1:
                    return $"{bandIn}A";
                case 2:
                    return $"{bandIn}B";
                case 3:
                    return $"{bandIn}C";
                case 4:
                    return $"{bandIn}D";
                case 5:
                    return $"{bandIn}E/DD";
                case 6:
                    return $"{bandIn}F/DDD";
                case 7:
                    return $"{bandIn}G/DDDD";
                case 8:
                    return $"{bandIn}H";
                case 9:
                    return $"{bandIn}I";
                case 10:
                    return $"{bandIn}J";
                case 11:
                    return $"{bandIn}K";
                case 12:
                    return $"{bandIn}L";
                case 13:
                    return $"{bandIn}M";
                default:
                    return $"{bandIn}N";
            }
        }

        // List<GameObject> underbustMarkers = new List<GameObject>();
        // CapsuleCollider chest1b;
        // private void UpdateUnderbustMarkers() {
        //     if(chest1b == null) {
        //         chest1b = containingAtom.GetComponentsInChildren<CapsuleCollider>().FirstOrDefault(c => ColliderName(c).Equals("AutoColliderFemaleAutoColliderschest1bJoint"));
        //     }

        //     if(!bustFinished) {
        //         return;
        //     }

        //     int i = 0;
        //     if(chest1b != null) {
        //         GameObject m;
        //         Vector3 pos;

        //         var p = chest1b.GetComponentInParent<AutoCollider>();

        //         // underbust - below left nipple
        //         if(underbustMarkers.Count == i) {
        //             m = CreateMarker(Color.red, "Sphere");
        //             underbustMarkers.Add(m);
        //         }
        //         pos = chest1b.transform.position;
        //         pos.z = pos.z + (chest1b.radius * p.scale) - 0.01f;
        //         pos.x = pos.x - (chest1b.height * p.scale / 2) + (chest1b.radius * p.scale / 3.14f);
        //         underbustMarkers[i].transform.position = pos;
        //         i++;

        //         // underbust - below right nipple
        //         if(underbustMarkers.Count == i) {
        //             m = CreateMarker(Color.white, "Sphere");
        //             underbustMarkers.Add(m);
        //         }
        //         pos = chest1b.transform.position;
        //         pos.z = pos.z + (chest1b.radius * p.scale) - 0.01f;
        //         pos.x = pos.x + (chest1b.height * p.scale / 2) - (chest1b.radius * p.scale / 3.14f);
        //         underbustMarkers[i].transform.position = pos;
        //         i++;

        //         // underbust - right side marker
        //         if(underbustMarkers.Count == i) {
        //             m = CreateMarker(Color.yellow, "Sphere");
        //             underbustMarkers.Add(m);
        //         }
        //         pos = chest1b.transform.position;
        //         pos.x = pos.x + (chest1b.height * p.scale / 2);
        //         underbustMarkers[i].transform.position = pos;
        //         i++;

        //         // underbust - right back marker
        //         if(underbustMarkers.Count == i) {
        //             m = CreateMarker(Color.green, "Sphere");
        //             underbustMarkers.Add(m);
        //         }
        //         pos = chest1b.transform.position;
        //         pos.z = pos.z - (chest1b.radius * p.scale);
        //         pos.x = pos.x + (chest1b.height * p.scale / 2) - (chest1b.radius * p.scale / 3.14f);
        //         underbustMarkers[i].transform.position = pos;
        //         i++;

        //         // underbust - left back marker
        //         if(underbustMarkers.Count == i) {
        //             m = CreateMarker(Color.blue, "Sphere");
        //             underbustMarkers.Add(m);
        //         }
        //         pos = chest1b.transform.position;
        //         pos.z = pos.z - (chest1b.radius * p.scale);
        //         pos.x = pos.x - (chest1b.height * p.scale / 2) + (chest1b.radius * p.scale / 3.14f);
        //         underbustMarkers[i].transform.position = pos;
        //         i++;

        //         // underbust - left side marker
        //         if(underbustMarkers.Count == i) {
        //             m = CreateMarker(Color.magenta, "Sphere");
        //             underbustMarkers.Add(m);
        //         }
        //         pos = chest1b.transform.position;
        //         pos.x = pos.x - (chest1b.height * p.scale / 2);
        //         underbustMarkers[i].transform.position = pos;
        //         i++;
        //     }

        // }

        // Collider lNipple;
        // Collider rNipple;
        // CapsuleCollider chest3;
        // List<GameObject> bustMarkers = new List<GameObject>();
        // bool bustFinished = false;
        // private void UpdateBustMarkers() {
        //     if(lNipple == null) {
        //         lNipple = containingAtom.GetComponentsInChildren<Collider>().FirstOrDefault(c => ColliderName(c).Equals("lNipple"));
        //     }
        //     if(rNipple == null) {
        //         rNipple = containingAtom.GetComponentsInChildren<Collider>().FirstOrDefault(c => ColliderName(c).Equals("rNipple"));
        //     }
        //     if(chest3 == null) {
        //         chest3 = containingAtom.GetComponentsInChildren<CapsuleCollider>().FirstOrDefault(c => ColliderName(c).Equals("AutoColliderFemaleAutoColliderschest3Joint"));
        //     }

        //     int i = 0;
        //     if(lNipple != null) {
        //         if(bustMarkers.Count == i) {
        //             var m = CreateMarker(Color.red, "Sphere");
        //             bustMarkers.Add(m);
        //         }
        //         bustMarkers[i].transform.position = lNipple.transform.position;
        //         i++;
        //     }

        //     if(rNipple != null) {
        //         if(bustMarkers.Count == i) {
        //             var m = CreateMarker(Color.white, "Sphere");
        //             bustMarkers.Add(m);
        //         }
        //         bustMarkers[i].transform.position = rNipple.transform.position;
        //         i++;
        //     }

        //     if(chest3 != null) {
        //         GameObject m;
        //         Vector3 pos;

        //         AutoCollider p;

        //         // bust - right side marker
        //         if(bustMarkers.Count == i) {
        //             m = CreateMarker(Color.yellow, "Sphere");
        //             bustMarkers.Add(m);
        //         }
        //         p = chest3.GetComponentInParent<AutoCollider>();
        //         pos = chest3.transform.position;
        //         pos.x = pos.x + (chest3.height * p.scale / 2) - 0.005f;
        //         bustMarkers[i].transform.position = pos;
        //         i++;

        //         // bust - right back marker
        //         if(bustMarkers.Count == i) {
        //             m = CreateMarker(Color.green, "Sphere");
        //             bustMarkers.Add(m);
        //         }
        //         pos = chest3.transform.position;
        //         pos.z = pos.z - (chest3.radius * p.scale) + 0.01f;
        //         pos.x = pos.x + (chest3.height * p.scale / 2) - (chest3.radius * p.scale / 3.14f);
        //         bustMarkers[i].transform.position = pos;
        //         i++;

        //         // bust - left back marker
        //         if(bustMarkers.Count == i) {
        //             m = CreateMarker(Color.blue, "Sphere");
        //             bustMarkers.Add(m);
        //         }
        //         pos = chest3.transform.position;
        //         pos.z = pos.z - (chest3.radius * p.scale) + 0.01f;
        //         pos.x = pos.x - (chest3.height  * p.scale / 2) + (chest3.radius * p.scale / 3.14f);
        //         bustMarkers[i].transform.position = pos;
        //         i++;

        //         // bust - left side marker
        //         if(bustMarkers.Count == i) {
        //             m = CreateMarker(Color.magenta, "Sphere");
        //             bustMarkers.Add(m);
        //         }
        //         pos = chest3.transform.position;
        //         pos.x = pos.x - (chest3.height * p.scale / 2) + 0.005f;
        //         bustMarkers[i].transform.position = pos;
        //         i++;
        //     }

        //     bustFinished = true;
        // }

        GameObject markerHead;
        AutoCollider acHeadHard6;
        private void UpdateHeadMarker() {
            if(markerHead == null) {
                markerHead = CreateMarker(Color.red, "Cube");
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
                markerFoot = CreateMarker(Color.red, "Cube");
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
                markerChin = CreateMarker(Color.red, "Cube");
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
                        extraHeadMarkers.Add(CreateMarker(Color.green, "Cube"));
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

        private GameObject CreateMarker(Color color, string type) {

            GameObject gameObject;
            if(type == "Cube") {
                gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            else {
                gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            }
        
            // random position to help avoid physics problems.
            gameObject.transform.position = new Vector3 ((UnityEngine.Random.value*461)+10, (UnityEngine.Random.value*300)+10, 0F);

            // make it smaller
            if(type == "Cube") {
                gameObject.transform.localScale = new Vector3(0.75f, 0.005f, 0.005f);
            }
            else {
                gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            }

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
