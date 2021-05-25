using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LFE
{
    public class HeightMeasurePlugin : MVRScript
    {
        readonly IVertexPosition[] verticesBust = new IVertexPosition[] {
            new VertexPositionMiddle(7213, 17920), // midchest 1/2 way between the nipples at bust height
            new VertexPositionExact(17920), // bust -- right nipple just to the left
            new VertexPositionExact(10939), // bust -- right nipple just to the right
            new VertexPositionExact(19588),
            new VertexPositionExact(19617),
            new VertexPositionExact(13233),
            new VertexPositionExact(11022), // bust -- right back
            new VertexPositionExact(10495), // bust -- back center
        };

        readonly IVertexPosition[] verticesUnderbust = new IVertexPosition[] {
            new VertexPositionMiddle(10822, 10820), // mid chest
            new VertexPositionExact(21469), // right breast under nipple
            new VertexPositionExact(21470), // right breast under nipple
            new VertexPositionExact(21394), // right side 
            new VertexPositionMiddle(11022, 21508, 0.4f),
            new VertexPositionExact(2100), // back
        };

        readonly IVertexPosition vertexHeadTop = new VertexPositionExact(2087);
        readonly IVertexPosition vertexHeadLeft = new VertexPositionExact(3236);
        readonly IVertexPosition vertexHeadRight = new VertexPositionExact(20646);
        readonly IVertexPosition vertexHeadBottom = new VertexPositionExact(2079);

        JSONStorableFloat fullHeightStorable;
        JSONStorableFloat headHeightStorable;
        JSONStorableFloat heightInHeadsStorable;
        JSONStorableString textStorable;
        JSONStorableFloat markerLeftRightStorable;
        JSONStorableFloat markerFrontBackStorable;
        JSONStorableBool showBreastMarkersStorable;
        JSONStorableStringChooser cupAlgorithmStorable;

        DAZCharacter dazCharacter;
        public DAZSkinV2 skin;


        Atom sign;
        string signAtomName;

        ICupCalculator[] cupCalculators = new ICupCalculator[] {
            new SizeChartCupCalculator(),
            new KnixComCupCalculator()
        };

        public override void Init()
        {
            dazCharacter = containingAtom.GetComponentInChildren<DAZCharacter>();
            skin = dazCharacter.skin;

            cupAlgorithmStorable = new JSONStorableStringChooser(
                "Cup Size Method",
                cupCalculators.Select(cc => cc.Name).ToList(),
                cupCalculators[0].Name,
                "Cup Size Method"
            );
            CreateScrollablePopup(cupAlgorithmStorable);

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
            var headHeight = headHeightStorable.val;
            var text = "";
            text += $"Body: {height:0.##} meters tall\n"
                + $"Body: {UnityToFeet(height):0.##} feet tall ({FeetInchString(UnityToFeet(height))})\n"
                + $"Body: {heightInHeads:0.##} heads tall\n\n"
                + $"Head: {headHeight:0.##} meters tall\n"
                + $"Head: {UnityToFeet(headHeight):0.##} feet tall ({FeetInchString(UnityToFeet(headHeight))})\n\n";

            var cupCalculator = cupCalculators.FirstOrDefault(cc => cc.Name.Equals(cupAlgorithmStorable.val));
            if(!dazCharacter.isMale && cupCalculator != null) {
                var cupInfo = cupCalculator.Calculate(circumferenceBust, circumferenceUnderbust);
                text += $"Bust: {circumferenceBust * 100:0} cm / {Mathf.RoundToInt(UnityToFeet(circumferenceBust) * 12)} in\n"
                + $"Underbust: {circumferenceUnderbust * 100:0} cm / {Mathf.RoundToInt(UnityToFeet(circumferenceUnderbust) * 12)} in\n"
                + $"Cup : {cupInfo.Band}{cupInfo.Cup}\n";
            }
            return text;
        }

        public void OnEnable() {
            if(signAtomName == null) {
                signAtomName = Guid.NewGuid().ToString();
            }
            sign = SuperController.singleton.GetAtomByUid(signAtomName);
            if(sign == null) {
                StartCoroutine(SuperController.singleton.AddAtomByType("SimpleSign", useuid: signAtomName));
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

            foreach(var h in bustMarkersFromMorph) {
                Destroy(h);
            }
            bustMarkersFromMorph = new List<GameObject>();

            foreach(var h in underbustMarkersFromMorph) {
                Destroy(h);
            }
            underbustMarkersFromMorph = new List<GameObject>();

        }

        TextStorable signStorable;
        public void Update() {
            dazCharacter = containingAtom.GetComponentInChildren<DAZCharacter>();
            skin = dazCharacter.skin;

            if(SuperController.singleton.freezeAnimation) {
                return;
            }

            try {
                if(sign == null) {
                    sign = SuperController.singleton.GetAtomByUid(signAtomName);
                }
                UpdateHeadMarker();
                UpdateFootMarker();
                UpdateChinMarker();
                UpdateHeadHeightMarkers();

                if(!dazCharacter.isMale) {
                    UpdateBustMarkersFromMorphVertex();
                    UpdateUnderbustMarkersFromMorphVertex();
                }

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

        private float LineLength(Vector3[] vertices) {
            float total = 0;
            var distances = new List<string>();
            for(var i = 1; i < vertices.Length; i++) {
                var distance = Mathf.Abs(Vector3.Distance(vertices[i-1], vertices[i]));
                distances.Add((distance * 2).ToString("0.000"));
                total += distance;
            }
            var s = string.Join(",", distances.ToArray());
            return total;
        }

        List<GameObject> bustMarkersFromMorph = new List<GameObject>();
        float circumferenceBust = 0;
        private void UpdateBustMarkersFromMorphVertex() {
            if(skin == null) {
                return;
            }

            if(showBreastMarkersStorable.val) {
                if(bustMarkersFromMorph.Count != verticesBust.Length)
                {
                    foreach(var m in bustMarkersFromMorph) {
                        Destroy(m);
                    }
                    bustMarkersFromMorph.Clear();
                    foreach(var m in verticesBust){
                        bustMarkersFromMorph.Add(CreateMarker(Color.red, "Sphere"));
                    }
                }

                for(var i = 0; i < verticesBust.Length; i++) {
                    bustMarkersFromMorph[i].transform.position = verticesBust[i].Position(this);
                }
            }
            else {
                foreach(var m in bustMarkersFromMorph) {
                    Destroy(m);
                }
                bustMarkersFromMorph.Clear();
            }


            circumferenceBust = LineLength(verticesBust.Select(v => v.Position(this)).ToArray()) * 2;
        }

        List<GameObject> underbustMarkersFromMorph = new List<GameObject>();
        float circumferenceUnderbust = 0;
        private void UpdateUnderbustMarkersFromMorphVertex() {
            if(skin == null) {
                return;
            }

            if(showBreastMarkersStorable.val){
                if(underbustMarkersFromMorph.Count != verticesUnderbust.Length) {
                    foreach(var m in underbustMarkersFromMorph) {
                        Destroy(m);
                    }
                    underbustMarkersFromMorph.Clear();
                    foreach(var m in verticesUnderbust){
                        underbustMarkersFromMorph.Add(CreateMarker(Color.white, "Sphere"));
                    }
                }

                for(var i = 0; i < verticesUnderbust.Length; i++) {
                    underbustMarkersFromMorph[i].transform.position = verticesUnderbust[i].Position(this);
                }
            }
            else {
                foreach(var m in underbustMarkersFromMorph) {
                    Destroy(m);
                }
                underbustMarkersFromMorph.Clear();
            }

            circumferenceUnderbust = LineLength(verticesUnderbust.Select(v => v.Position(this)).ToArray()) * 2;
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

        public static float UnityToFeet(float unit) {
            return unit/0.3048f;
        }

        public static string FeetInchString(float feet) {
            int f = (int)feet;
            int inches = (int)((feet - f) * 12);
            return $"{f}'{inches}\"";
        }
    }

    public interface IVertexPosition {
        Vector3 Position(HeightMeasurePlugin plugin);
    }

    public class VertexPositionExact : IVertexPosition {
        int _indexA;
        public VertexPositionExact(int indexA)
        {
            _indexA = indexA;
        }

        public Vector3 Position(HeightMeasurePlugin plugin) {
            if(plugin.skin == null) {
                return Vector3.zero;
            }
            if(_indexA < 0 || _indexA >= plugin.skin.rawSkinnedVerts.Length) {
                return Vector3.zero;
            }

            return plugin.skin.rawSkinnedVerts[_indexA];
        }
    }

    public class VertexPositionMiddle : IVertexPosition {
        int _indexA;
        int _indexB;
        float _ratio;

        public VertexPositionMiddle(int indexA, int indexB, float ratio = 0.5f)
        {
            _indexA = indexA;
            _indexB = indexB;
            _ratio = Mathf.Clamp01(ratio);
        }

        public Vector3 Position(HeightMeasurePlugin plugin) {
            if(plugin.skin == null) {
                return Vector3.zero;
            }
            if(_indexA < 0 || _indexA >= plugin.skin.rawSkinnedVerts.Length) {
                return Vector3.zero;
            }
            if(_indexB < 0 || _indexB >= plugin.skin.rawSkinnedVerts.Length) {
                return Vector3.zero;
            }

            var vertexA = plugin.skin.rawSkinnedVerts[_indexA];
            var vertexB = plugin.skin.rawSkinnedVerts[_indexB];

            return Vector3.Lerp(vertexA, vertexB, _ratio);
        }
    }

    public class CupSize {
        public int Band;
        public string Cup;
        public string Units;
    }

    public interface ICupCalculator {
        string Name { get; }
        CupSize Calculate(float bust, float underbust);
    }


    public class KnixComCupCalculator : ICupCalculator {

        // https://knix.com/blogs/resources/how-to-measure-bra-band-size#:~:text=Finally%2C%20Find%20Your%20Cup%20Size%20%20%20Bust,%20%20C%20%207%20more%20rows%20
        public string Name => "https://knix.com/";

        public CupSize Calculate(float bust, float underbust) {
            var bustIn = Mathf.RoundToInt(HeightMeasurePlugin.UnityToFeet(bust) * 12);
            var underbustIn = Mathf.RoundToInt(HeightMeasurePlugin.UnityToFeet(underbust) * 12);

            // bust size + 2 inches - if it is odd, add one more
            var band = underbustIn + 2 + (underbustIn % 2);
            var diff = Mathf.Max(0, bustIn - band);

            var bustBandDiffToCup = new Dictionary<Vector2, string>() {
                { new Vector2(0, 1), "AA"},
                { new Vector2(1, 2), "A"},
                { new Vector2(2, 3), "B"},
                { new Vector2(3, 4), "C"},
                { new Vector2(4, 5), "D"},
                { new Vector2(5, 6), "DD/E"},
                { new Vector2(6, 7), "DDD/F"},
                { new Vector2(7, 8), "G"},
                { new Vector2(8, 9), "H"},
                { new Vector2(9, 10), "I"},
                { new Vector2(10, 11), "J"},
                { new Vector2(11, 100000), "HUGE"},
            };
            var cupMapping = bustBandDiffToCup.FirstOrDefault(kv => diff >= kv.Key.x && diff < kv.Key.y);

            return new CupSize {
                Units = "in",
                Cup = cupMapping.Value,
                Band = band
            };
        }
    }

    public class SizeChartCupCalculator : ICupCalculator {

        public string Name => "sizechart.com/brasize/us/index.html";

        public CupSize Calculate(float bust, float underbust) {
            var bustIn = Mathf.RoundToInt(HeightMeasurePlugin.UnityToFeet(bust) * 12);
            var underbustIn = Mathf.RoundToInt(HeightMeasurePlugin.UnityToFeet(underbust) * 12);

            var bustToBand = new Dictionary<Vector2, int>() {
                { new Vector2(23, 25), 28 },
                { new Vector2(25, 27), 30 },
                { new Vector2(27, 29), 32 },
                { new Vector2(29, 31), 34 },
                { new Vector2(31, 33), 36 },
                { new Vector2(33, 35), 38 },
                { new Vector2(35, 37), 40 },
                { new Vector2(37, 39), 42 },
                { new Vector2(39, 40), 44 },
                { new Vector2(41, 43), 46 },
            };

            var bustMapping = bustToBand.FirstOrDefault(kv => underbustIn >= kv.Key.x && underbustIn < kv.Key.y);
            var band = bustMapping.Value;

            var bustBandDiffToCup = new Dictionary<Vector2, string>() {
                { new Vector2(0, 1), "AA"},
                { new Vector2(1, 2), "A"},
                { new Vector2(2, 3), "B"},
                { new Vector2(3, 4), "C"},
                { new Vector2(4, 5), "D"},
                { new Vector2(5, 6), "DD/E"},
                { new Vector2(6, 7), "DDD/F"},
                { new Vector2(7, 8), "G"},
                { new Vector2(8, 9), "H"},
                { new Vector2(9, 10), "I"},
                { new Vector2(10, 11), "J"},
                { new Vector2(11, 100000), "HUGE"},
            };
            var cupMapping = bustBandDiffToCup.FirstOrDefault(kv => Mathf.Max(0, bustIn-band) >= kv.Key.x && Mathf.Max(0, bustIn-band) < kv.Key.y);

            return new CupSize {
                Units = "in",
                Cup = cupMapping.Value,
                Band = band
            };
        }
    }
}
