using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LFE
{
    public class HeightMeasurePlugin : MVRScript
    {
        public DAZSkinV2 Skin;

        readonly IVertexPosition[] _verticesBust = new IVertexPosition[] {
            new VertexPositionMiddle(7213, 17920), // midchest 1/2 way between the nipples at bust height
            new VertexPositionExact(17920), // bust -- right nipple just to the left
            new VertexPositionExact(10939), // bust -- right nipple just to the right
            new VertexPositionExact(19588),
            new VertexPositionExact(19617),
            new VertexPositionExact(13233),
            new VertexPositionExact(11022), // bust -- right back
            new VertexPositionExact(10495), // bust -- back center
        };

        readonly IVertexPosition[] _verticesUnderbust = new IVertexPosition[] {
            new VertexPositionMiddle(10822, 10820), // mid chest
            new VertexPositionExact(21469), // right breast under nipple
            new VertexPositionExact(21470), // right breast under nipple
            new VertexPositionExact(21394), // right side 
            new VertexPositionMiddle(11022, 21508, 0.4f),
            new VertexPositionExact(2100), // back
        };

        readonly IVertexPosition[] _verticesWaist = new IVertexPosition[] {
            new VertexPositionExact(8152), // front and center
            new VertexPositionExact(19663), // front right 1
            new VertexPositionExact(13675), // front right 2
            new VertexPositionExact(13715), // front right 3
            new VertexPositionExact(13727), // right side
            new VertexPositionExact(13725), // back curve 1
            new VertexPositionExact(2921), // back
        };

        readonly IVertexPosition[] _verticesHip = new IVertexPosition[] {
            new VertexPositionExact(22843), // front and center
            new VertexPositionExact(13750), // front right 1
            new VertexPositionExact(18460), // front right 2
            new VertexPositionMiddle(11234, 18491, 0.8f), // front right 3
            new VertexPositionExact(18512), // right side
            new VertexPositionExact(18529), // glute curve 1
            new VertexPositionExact(18562), // glute curve 2
            new VertexPositionMiddle(18562, 7878), // glute middle
        };

        readonly IVertexPosition _vertexHead = new VertexPositionExact(2087);
        readonly IVertexPosition _vertexChin = new VertexPositionExact(2079);
        readonly IVertexPosition _vertexEarLeft = new VertexPositionExact(3236);
        readonly IVertexPosition _vertexEarRight = new VertexPositionExact(20646);
        readonly IVertexPosition _vertexUnderbust = new VertexPositionExact(21469); // right breast under nipple
        readonly IVertexPosition _vertexNipple = new VertexPositionExact(10939);
        readonly IVertexPosition _vertexNavel = new VertexPositionMiddle(18824, 8147);
        readonly IVertexPosition _vertexGroin = new VertexPositionExact(22208);
        readonly IVertexPosition _vertexKnee = new VertexPositionMiddle(8508, 19179);

        JSONStorableFloat _fullHeightStorable;
        JSONStorableFloat _headHeightStorable;
        JSONStorableFloat _heightInHeadsStorable;
        JSONStorableFloat _markerLeftRightStorable;
        JSONStorableFloat _markerFrontBackStorable;
        JSONStorableBool _showTapeMarkersStorable;
        JSONStorableBool _showHeadHeightMarkersStorable;
        JSONStorableBool _showFeatureMarkersStorable;
        JSONStorableBool _showFeatureMarkerLabelsStorable;
        JSONStorableStringChooser _cupAlgorithmStorable;

        DAZCharacter _dazCharacter;

        readonly ICupCalculator[] _cupCalculators = new ICupCalculator[] {
            new SizeChartCupCalculator(),
            new KnixComCupCalculator(),
            new ChateLaineCupCalculator()
        };

        HorizontalMarker _markerHead;
        HorizontalMarker _markerChin;
        HorizontalMarker _markerNipple;
        HorizontalMarker _markerUnderbust;
        HorizontalMarker _markerNavel;
        HorizontalMarker _markerGroin;
        HorizontalMarker _markerKnee;
        HorizontalMarker _markerHeel;

        public override void Init()
        {
            _dazCharacter = containingAtom.GetComponentInChildren<DAZCharacter>();
            Skin = _dazCharacter.skin;

            // initialize the line markers
            _markerHead = gameObject.AddComponent<HorizontalMarker>();
            _markerHead.Name = "Head";
            _markerHead.Color = Color.green;

            _markerChin = gameObject.AddComponent<HorizontalMarker>();
            _markerChin.Name = "Chin";
            _markerChin.Color = Color.green;

            _markerNipple = gameObject.AddComponent<HorizontalMarker>();
            _markerNipple.Name = "Nipple";
            _markerNipple.Color = Color.green;

            _markerUnderbust = gameObject.AddComponent<HorizontalMarker>();
            _markerUnderbust.Name = "Underbust";
            _markerUnderbust.Color = Color.green;

            _markerNavel = gameObject.AddComponent<HorizontalMarker>();
            _markerNavel.Name = "Navel";
            _markerNavel.Color = Color.green;

            _markerGroin = gameObject.AddComponent<HorizontalMarker>();
            _markerGroin.Name = "Groin";
            _markerGroin.Color = Color.green;

            _markerKnee = gameObject.AddComponent<HorizontalMarker>();
            _markerKnee.Name = "Knee";
            _markerKnee.Color = Color.green;

            _markerHeel = gameObject.AddComponent<HorizontalMarker>();
            _markerHeel.Name = "Heel";
            _markerHeel.Color = Color.green;

            // initialize storables
            _cupAlgorithmStorable = new JSONStorableStringChooser(
                "Cup Size Method",
                _cupCalculators.Select(cc => cc.Name).ToList(),
                _cupCalculators[0].Name,
                "Cup Size Method"
            );

            _markerLeftRightStorable = new JSONStorableFloat("Marker Left/Right", 0.02f, -1, 1);
            RegisterFloat(_markerLeftRightStorable);

            _markerFrontBackStorable = new JSONStorableFloat("Marker Front/Back", 0.15f, -1, 1);
            RegisterFloat(_markerFrontBackStorable);

            _showTapeMarkersStorable = new JSONStorableBool("Show Tape Measure Markers", false);
            RegisterBool(_showTapeMarkersStorable);

            _showHeadHeightMarkersStorable = new JSONStorableBool("Show Head Height Markers", true);
            RegisterBool(_showHeadHeightMarkersStorable);

            _showFeatureMarkersStorable = new JSONStorableBool("Show Feature Markers", true);
            RegisterBool(_showFeatureMarkersStorable);

            _showFeatureMarkerLabelsStorable = new JSONStorableBool("Show Feature Marker Labels", true);
            RegisterBool(_showFeatureMarkerLabelsStorable);

            _fullHeightStorable = new JSONStorableFloat("Full Height In Meters", 0, 0, 100);
            RegisterFloat(_fullHeightStorable);

            _headHeightStorable = new JSONStorableFloat("Head Height In Meters", 0, 0, 100);
            RegisterFloat(_headHeightStorable);

            _heightInHeadsStorable = new JSONStorableFloat("Full Height In Heads", 0, 0, 100);
            RegisterFloat(_heightInHeadsStorable);

            // initialize the ui components
            CreateScrollablePopup(_cupAlgorithmStorable);
            CreateSlider(_markerLeftRightStorable, rightSide: true);
            CreateSlider(_markerFrontBackStorable, rightSide: true);
            CreateToggle(_showTapeMarkersStorable);
            CreateToggle(_showHeadHeightMarkersStorable);
            CreateToggle(_showFeatureMarkersStorable);
            CreateToggle(_showFeatureMarkerLabelsStorable);
        }

        public void OnDestroy() {
            // destroy the markers
            foreach(var m in gameObject.GetComponentsInChildren<HorizontalMarker>()) {
                Destroy(m);
            }

            foreach(var h in _bustMarkersFromMorph) {
                Destroy(h);
            }
            _bustMarkersFromMorph = new List<GameObject>();

            foreach(var h in _underbustMarkersFromMorph) {
                Destroy(h);
            }
            _underbustMarkersFromMorph = new List<GameObject>();

            foreach(var h in _waistMarkersFromMorph) {
                Destroy(h);
            }
            _waistMarkersFromMorph = new List<GameObject>();

            foreach(var h in _hipMarkersFromMorph) {
                Destroy(h);
            }
            _hipMarkersFromMorph = new List<GameObject>();
        }

        public void Update() {
            _dazCharacter = containingAtom.GetComponentInChildren<DAZCharacter>();
            Skin = _dazCharacter.skin;

            if(SuperController.singleton.freezeAnimation) {
                return;
            }

            try {

                Vector3 pos;
                // head
                pos = _vertexHead.Position(this);
                pos.x -= _markerLeftRightStorable.val;
                pos.z += _markerFrontBackStorable.val;
                _markerHead.Origin = pos;
                _markerHead.Enabled = _showFeatureMarkersStorable.val;
                _markerHead.LabelEnabled = _showFeatureMarkerLabelsStorable.val;

                // chin
                pos = _vertexChin.Position(this);
                pos.x = _markerHead.Origin.x;
                pos.z = _markerHead.Origin.z;
                _markerChin.Origin = pos;
                _markerChin.Enabled = _showFeatureMarkersStorable.val;
                _markerChin.LabelEnabled = _showFeatureMarkerLabelsStorable.val;

                // nipple
                pos = _vertexNipple.Position(this);
                pos.x = _markerHead.Origin.x;
                pos.z = _markerHead.Origin.z;
                _markerNipple.Origin = pos;
                _markerNipple.Enabled = _dazCharacter.isMale ? false : _showFeatureMarkersStorable.val; // TODO: better male nipple detection
                _markerNipple.LabelEnabled = _showFeatureMarkerLabelsStorable.val;

                // underbust
                pos = _vertexUnderbust.Position(this);
                pos.x = _markerHead.Origin.x;
                pos.z = _markerHead.Origin.z;
                _markerUnderbust.Origin = pos;
                _markerUnderbust.Enabled = _dazCharacter.isMale ? false : _showFeatureMarkersStorable.val;
                _markerUnderbust.LabelEnabled = _showFeatureMarkerLabelsStorable.val;

                // navel
                pos = _vertexNavel.Position(this);
                pos.x = _markerHead.Origin.x;
                pos.z = _markerHead.Origin.z;
                _markerNavel.Origin = pos;
                _markerNavel.Enabled = _showFeatureMarkersStorable.val;
                _markerNavel.LabelEnabled = _showFeatureMarkerLabelsStorable.val;

                // groin
                pos = _vertexGroin.Position(this);
                pos.x = _markerHead.Origin.x;
                pos.z = _markerHead.Origin.z;
                _markerGroin.Origin = pos;
                _markerGroin.Enabled = _showFeatureMarkersStorable.val;
                _markerGroin.LabelEnabled = _showFeatureMarkerLabelsStorable.val;

                // knee
                pos = _vertexKnee.Position(this);
                pos.x = _markerHead.Origin.x;
                pos.z = _markerHead.Origin.z;
                _markerKnee.Origin = pos;
                _markerKnee.Enabled = _showFeatureMarkersStorable.val;
                _markerKnee.LabelEnabled = _showFeatureMarkerLabelsStorable.val;

                // heel - can not find vertex for heel - using colliders
                var rFoot = containingAtom.GetComponentsInChildren<CapsuleCollider>().FirstOrDefault(c => ColliderName(c).Equals("rFoot/_Collider1")); 
                if(rFoot) {
                    pos = rFoot.transform.position;
                    pos.x = _markerHead.Origin.x;
                    pos.z = _markerHead.Origin.z;
                    _markerHeel.Origin = pos;
                    _markerHeel.Enabled = _showFeatureMarkersStorable.val;
                    _markerHeel.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
                }
             
                UpdateHeadHeightMarkers();

                if(!_dazCharacter.isMale) {
                    UpdateBustMarkersFromMorphVertex();
                    UpdateUnderbustMarkersFromMorphVertex();
                }
                UpdateWaistMarkersFromMorphVertex();
                UpdateHipMarkersFromMorphVertex();

                _fullHeightStorable.val = GetHeight();
                _headHeightStorable.val = GetHeadHeight();
                _heightInHeadsStorable.val = GetHeightInHeads();
                var cupInfo = GetCupInfo();
                var crotchToKnee = Vector3.Distance(_markerGroin.Origin, _markerKnee.Origin);
                var kneeToHeel = Vector3.Distance(_markerKnee.Origin, _markerHeel.Origin);
                var headWidth = GetHeadWidth();

                _markerHead.Label = $"Head (Head To Heel {(int)(_fullHeightStorable.val * 100)} cm / {UnitUtils.FeetInchString(UnitUtils.UnityToFeet(_fullHeightStorable.val))} / {_heightInHeadsStorable.val:0.0} heads)";
                _markerChin.Label = $"Neck (Head Height {(int)(_headHeightStorable.val * 100)} cm / {UnitUtils.FeetInchString(UnitUtils.UnityToFeet(_headHeightStorable.val))}; Width {(int)(headWidth * 100)} cm / {UnitUtils.FeetInchString(UnitUtils.UnityToFeet(headWidth))})";
                if(cupInfo == null) {
                    _markerNipple.Label = "Nipple";
                    _markerUnderbust.Label = "Underbust";
                }
                else {
                    _markerNipple.Label = $"Nipple (Cup {cupInfo}; Around {cupInfo.BustToCentimeters} cm / {cupInfo.BustToInches} in)";
                    _markerUnderbust.Label = $"Underbust (Around {cupInfo.UnderbustToCentimeters} cm / {cupInfo.UnderbustToInches} in)";
                }
                _markerNavel.Label = $"Navel (Waist {(int)(_circumferenceWaist * 100)} cm / {Mathf.RoundToInt(UnitUtils.UnityToFeet(_circumferenceWaist) * 12)} in)";
                _markerGroin.Label = $"Crotch (Hip {(int)(_circumferenceHip * 100)} cm / {Mathf.RoundToInt(UnitUtils.UnityToFeet(_circumferenceHip) * 12)} in)";
                _markerKnee.Label = $"Knee Bottom (Crotch to Knee {(int)(crotchToKnee * 100)} cm / {UnitUtils.FeetInchString(UnitUtils.UnityToFeet(crotchToKnee))} / {crotchToKnee / _headHeightStorable.val:0.0} heads)";
                _markerHeel.Label = $"Heel (Knee to Heel {(int)(kneeToHeel * 100)} cm / {UnitUtils.FeetInchString(UnitUtils.UnityToFeet(kneeToHeel))} / {kneeToHeel / _headHeightStorable.val:0.0} heads)";
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

        List<GameObject> _bustMarkersFromMorph = new List<GameObject>();
        float _circumferenceBust = 0;
        private void UpdateBustMarkersFromMorphVertex() {
            if(Skin == null) {
                return;
            }

            if(_showTapeMarkersStorable.val) {
                if(_bustMarkersFromMorph.Count != _verticesBust.Length)
                {
                    foreach(var m in _bustMarkersFromMorph) {
                        Destroy(m);
                    }
                    _bustMarkersFromMorph.Clear();
                    foreach(var m in _verticesBust){
                        _bustMarkersFromMorph.Add(CreateMarker(Color.red));
                    }
                }

                for(var i = 0; i < _verticesBust.Length; i++) {
                    _bustMarkersFromMorph[i].transform.position = _verticesBust[i].Position(this);
                }
            }
            else {
                foreach(var m in _bustMarkersFromMorph) {
                    Destroy(m);
                }
                _bustMarkersFromMorph.Clear();
            }


            _circumferenceBust = LineLength(_verticesBust.Select(v => v.Position(this)).ToArray()) * 2;
        }

        List<GameObject> _underbustMarkersFromMorph = new List<GameObject>();
        float _circumferenceUnderbust = 0;
        private void UpdateUnderbustMarkersFromMorphVertex() {
            if(Skin == null) {
                return;
            }

            if(_showTapeMarkersStorable.val){
                if(_underbustMarkersFromMorph.Count != _verticesUnderbust.Length) {
                    foreach(var m in _underbustMarkersFromMorph) {
                        Destroy(m);
                    }
                    _underbustMarkersFromMorph.Clear();
                    foreach(var m in _verticesUnderbust){
                        _underbustMarkersFromMorph.Add(CreateMarker(Color.white));
                    }
                }

                for(var i = 0; i < _verticesUnderbust.Length; i++) {
                    _underbustMarkersFromMorph[i].transform.position = _verticesUnderbust[i].Position(this);
                }
            }
            else {
                foreach(var m in _underbustMarkersFromMorph) {
                    Destroy(m);
                }
                _underbustMarkersFromMorph.Clear();
            }

            _circumferenceUnderbust = LineLength(_verticesUnderbust.Select(v => v.Position(this)).ToArray()) * 2;
        }

        List<GameObject> _waistMarkersFromMorph = new List<GameObject>();
        float _circumferenceWaist = 0;
        private void UpdateWaistMarkersFromMorphVertex() {
            if(Skin == null) {
                return;
            }

            if(_showTapeMarkersStorable.val){
                if(_waistMarkersFromMorph.Count != _verticesWaist.Length) {
                    foreach(var m in _waistMarkersFromMorph) {
                        Destroy(m);
                    }
                    _waistMarkersFromMorph.Clear();
                    foreach(var m in _verticesWaist){
                        _waistMarkersFromMorph.Add(CreateMarker(Color.white));
                    }
                }

                for(var i = 0; i < _verticesWaist.Length; i++) {
                    _waistMarkersFromMorph[i].transform.position = _verticesWaist[i].Position(this);
                }
            }
            else {
                foreach(var m in _waistMarkersFromMorph) {
                    Destroy(m);
                }
                _waistMarkersFromMorph.Clear();
            }

            _circumferenceWaist = LineLength(_verticesWaist.Select(v => v.Position(this)).ToArray()) * 2;
        }

        List<GameObject> _hipMarkersFromMorph = new List<GameObject>();
        float _circumferenceHip = 0;
        private void UpdateHipMarkersFromMorphVertex() {
            if(Skin == null) {
                return;
            }

            if(_showTapeMarkersStorable.val){
                if(_hipMarkersFromMorph.Count != _verticesHip.Length) {
                    foreach(var m in _hipMarkersFromMorph) {
                        Destroy(m);
                    }
                    _hipMarkersFromMorph.Clear();
                    foreach(var m in _verticesHip){
                        _hipMarkersFromMorph.Add(CreateMarker(Color.white));
                    }
                }

                for(var i = 0; i < _verticesHip.Length; i++) {
                    _hipMarkersFromMorph[i].transform.position = _verticesHip[i].Position(this);
                }
            }
            else {
                foreach(var m in _hipMarkersFromMorph) {
                    Destroy(m);
                }
                _hipMarkersFromMorph.Clear();
            }

            _circumferenceHip = LineLength(_verticesHip.Select(v => v.Position(this)).ToArray()) * 2;
        }

        private float GetHeight() {
            return Vector3.Distance(_markerHead.Origin, _markerHeel.Origin);
        }

        private float GetHeadHeight() {
            return Vector3.Distance(_markerHead.Origin, _markerChin.Origin);
        }
        
        private float GetHeadWidth() {
            return Vector3.Distance(_vertexEarLeft.Position(this), _vertexEarRight.Position(this));
        }

        private float GetHeightInHeads() {
            var height = GetHeight();
            var headHeight = GetHeadHeight();
            if(headHeight > 0) {
                return height/headHeight;
            }
            return 0;
        }

        List<HorizontalMarker> _extraHeadMarkers = new List<HorizontalMarker>();
        private void UpdateHeadHeightMarkers() {
            var height = GetHeight();
            var headHeight = GetHeadHeight();
            var heightInHeadsRoundedUp = (int)Mathf.Ceil(GetHeightInHeads());

            if(heightInHeadsRoundedUp != _extraHeadMarkers.Count) {
                if(heightInHeadsRoundedUp > _extraHeadMarkers.Count) {
                    for(var i = _extraHeadMarkers.Count; i < heightInHeadsRoundedUp; i++) {
                        var hm = gameObject.AddComponent<HorizontalMarker>();
                        hm.Name = $"Head{i}";
                        hm.Color = Color.white;
                        hm.LineDirection = Vector3.right;
                        _extraHeadMarkers.Add(hm);
                    }
                }

                for(var i = 0; i < _extraHeadMarkers.Count; i++) {
                    _extraHeadMarkers[i].Enabled = false;
                }
            }

            if(height > 0 && headHeight > 0) {
                for(var i = 0; i < heightInHeadsRoundedUp; i++) {

                    var pos = _vertexHead.Position(this);
                    pos.x += _markerLeftRightStorable.val;
                    pos.z += _markerFrontBackStorable.val;
                    pos.y -= headHeight * i;

                    _extraHeadMarkers[i].Origin = pos;
                    _extraHeadMarkers[i].Enabled = _showHeadHeightMarkersStorable.val;
                    _extraHeadMarkers[i].Label = $"{i}";
                }
            }
        }

        private CupSize GetCupInfo() {
            if(_dazCharacter.isMale) {
                return null;
            }
            var cupCalculator = _cupCalculators.FirstOrDefault(cc => cc.Name.Equals(_cupAlgorithmStorable.val));
            if(cupCalculator == null) {
                return null;
            }
            return cupCalculator.Calculate(_circumferenceBust, _circumferenceUnderbust);
        }

        private GameObject CreateMarker(Color color) {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        
            // random position to help avoid physics problems.
            gameObject.transform.position = new Vector3 ((UnityEngine.Random.value*461)+10, (UnityEngine.Random.value*300)+10, 0F);

            // make it smaller
            gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

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

        private string ColliderName(Collider collider)  {
            var parent = collider.attachedRigidbody != null ? collider.attachedRigidbody.name : collider.transform.parent.name;
            var label = parent == collider.name ? collider.name : $"{parent}/{collider.name}";

            return label;
        }

    }

    public class UnitUtils {
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
        readonly int _indexA;

        public VertexPositionExact(int indexA)
        {
            _indexA = indexA;
        }

        public Vector3 Position(HeightMeasurePlugin plugin) {
            if(plugin.Skin == null) {
                return Vector3.zero;
            }
            if(_indexA < 0 || _indexA >= plugin.Skin.rawSkinnedVerts.Length) {
                return Vector3.zero;
            }
            return plugin.Skin.rawSkinnedVerts[_indexA];
        }
    }

    public class VertexPositionMiddle : IVertexPosition {
        readonly int _indexA;
        readonly int _indexB;
        readonly float _ratio;

        public VertexPositionMiddle(int indexA, int indexB, float ratio = 0.5f)
        {
            _indexA = indexA;
            _indexB = indexB;
            _ratio = Mathf.Clamp01(ratio);
        }

        public Vector3 Position(HeightMeasurePlugin plugin) {
            if(plugin.Skin == null) {
                return Vector3.zero;
            }
            if(_indexA < 0 || _indexA >= plugin.Skin.rawSkinnedVerts.Length) {
                return Vector3.zero;
            }
            if(_indexB < 0 || _indexB >= plugin.Skin.rawSkinnedVerts.Length) {
                return Vector3.zero;
            }

            var vertexA = plugin.Skin.rawSkinnedVerts[_indexA];
            var vertexB = plugin.Skin.rawSkinnedVerts[_indexB];

            return Vector3.Lerp(vertexA, vertexB, _ratio);
        }
    }

    public class CupSize {
        public float Bust;
        public float Underbust;
        public int Band;
        public string Cup;
        public string Units;

        public int BustToCentimeters => Mathf.RoundToInt(Bust * 100);
        public int BustToInches => Mathf.RoundToInt(UnitUtils.UnityToFeet(Bust) * 12);
        public int UnderbustToCentimeters => Mathf.RoundToInt(Underbust * 100);
        public int UnderbustToInches => Mathf.RoundToInt(UnitUtils.UnityToFeet(Underbust) * 12);

        public override string ToString() {
            return $"{Band}{Cup}";
        }
    }

    public interface ICupCalculator {
        string Name { get; }
        CupSize Calculate(float bust, float underbust);
    }

    public class KnixComCupCalculator : ICupCalculator {

        // https://knix.com/blogs/resources/how-to-measure-bra-band-size#:~:text=Finally%2C%20Find%20Your%20Cup%20Size%20%20%20Bust,%20%20C%20%207%20more%20rows%20
        public string Name => "https://knix.com/";

        public CupSize Calculate(float bust, float underbust) {
            var bustIn = Mathf.RoundToInt(UnitUtils.UnityToFeet(bust) * 12);
            var underbustIn = Mathf.RoundToInt(UnitUtils.UnityToFeet(underbust) * 12);

            // bust size + 2 inches - if it is odd, add one more
            var band = underbustIn + 2 + (underbustIn % 2);
            var diff = Mathf.Max(0, bustIn - band);

            var bustBandDiffToCup = new Dictionary<Vector2, string>() {
                { new Vector2(0, 1), "AA"}, { new Vector2(1, 2), "A"}, { new Vector2(2, 3), "B"}, { new Vector2(3, 4), "C"}, { new Vector2(4, 5), "D"}, { new Vector2(5, 6), "DD/E"},
                { new Vector2(6, 7), "DDD/F"}, { new Vector2(7, 8), "G"}, { new Vector2(8, 9), "H"}, { new Vector2(9, 10), "I"}, { new Vector2(10, 11), "J"}, { new Vector2(11, 100000), "HUGE"},
            };
            var cupMapping = bustBandDiffToCup.FirstOrDefault(kv => diff >= kv.Key.x && diff < kv.Key.y);

            return new CupSize { Units = "in", Cup = cupMapping.Value, Band = band, Bust = bust, Underbust = underbust };
        }
    }

    public class SizeChartCupCalculator : ICupCalculator {

        public string Name => "sizechart.com/brasize/us/index.html";

        public CupSize Calculate(float bust, float underbust) {
            var bustIn = Mathf.RoundToInt(UnitUtils.UnityToFeet(bust) * 12);
            var underbustIn = Mathf.RoundToInt(UnitUtils.UnityToFeet(underbust) * 12);

            var bustToBand = new Dictionary<Vector2, int>() {
                { new Vector2(23, 25), 28 }, { new Vector2(25, 27), 30 }, { new Vector2(27, 29), 32 }, { new Vector2(29, 31), 34 }, { new Vector2(31, 33), 36 }, { new Vector2(33, 35), 38 },
                { new Vector2(35, 37), 40 }, { new Vector2(37, 39), 42 }, { new Vector2(39, 40), 44 }, { new Vector2(41, 43), 46 },
            };

            var bustMapping = bustToBand.FirstOrDefault(kv => underbustIn >= kv.Key.x && underbustIn < kv.Key.y);
            var band = bustMapping.Value;

            var bustBandDiffToCup = new Dictionary<Vector2, string>() { { new Vector2(0, 1), "AA"}, { new Vector2(1, 2), "A"}, { new Vector2(2, 3), "B"}, { new Vector2(3, 4), "C"},
                { new Vector2(4, 5), "D"}, { new Vector2(5, 6), "DD/E"}, { new Vector2(6, 7), "DDD/F"}, { new Vector2(7, 8), "G"}, { new Vector2(8, 9), "H"}, { new Vector2(9, 10), "I"},
                { new Vector2(10, 11), "J"}, { new Vector2(11, 100000), "HUGE"},
            };
            var cupMapping = bustBandDiffToCup.FirstOrDefault(kv => Mathf.Max(0, bustIn-band) >= kv.Key.x && Mathf.Max(0, bustIn-band) < kv.Key.y);
            return new CupSize { Units = "in", Cup = cupMapping.Value, Band = band, Bust = bust, Underbust = underbust };
        }
    }

    public class ChateLaineCupCalculator : ICupCalculator {

        // https://www.chatelaine.com/style/fashion/bra-size-calculator/ 
        public string Name => "https://www.chatelaine.com/";

        public CupSize Calculate(float bust, float underbust) {
            var bustIn = Mathf.RoundToInt(UnitUtils.UnityToFeet(bust) * 12);
            var underbustIn = Mathf.RoundToInt(UnitUtils.UnityToFeet(underbust) * 12);

            // if you measure an even number, add 2, if you measure odd, add 1
            var band = underbustIn;
            if(band % 2 == 0) { band += 2; } else { band += 1; }
            var diff = Mathf.Max(0, bustIn - band);
            var bustBandDiffToCup = new Dictionary<Vector2, string>() {
                { new Vector2(0, 1), "AA"}, { new Vector2(1, 2), "A"}, { new Vector2(2, 3), "B"}, { new Vector2(3, 4), "C"}, { new Vector2(4, 5), "D"}, { new Vector2(5, 6), "DD/E"},
                { new Vector2(6, 7), "DDD/F"}, { new Vector2(7, 8), "G"}, { new Vector2(8, 9), "H"}, { new Vector2(9, 10), "I"}, { new Vector2(10, 11), "J"}, { new Vector2(11, 100000), "HUGE"},
            };
            var cupMapping = bustBandDiffToCup.FirstOrDefault(kv => diff >= kv.Key.x && diff < kv.Key.y);
            return new CupSize { Units = "in", Cup = cupMapping.Value, Band = band, Bust = bust, Underbust = underbust };
        }
    }

    public class HorizontalMarker : MonoBehaviour {
        public string Name { get; set; } = "Name";
        public string Label { get; set; }
        public Color Color { get; set; } = Color.green;
        public float Length { get; set; } = 0.5f;
        public Vector3 Origin { get; set; } = Vector3.zero;
        public Vector3 LineDirection { get; set; } = Vector3.left;
        public bool Enabled { get; set; } = true;
        public bool LabelEnabled { get; set; } = true;

        LineRenderer _lineRenderer;
        Canvas _canvas;

        public void Awake() {
            // new GameObject because you can only have one renderer per gameobject
            var gameObject = new GameObject();
            // gameObject.transform.SetParent(transform);
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.startWidth = 0.005f;
            _lineRenderer.endWidth = 0.004f;
            _lineRenderer.material.color = Color;
            _lineRenderer.SetPositions(new Vector3[] {
                Origin,
                CalculateEndpoint(Origin)
            });
            InitTextLabel();
        }

        public void Update() {
            transform.position = Origin;
            _lineRenderer.material.color = Color;
            _lineRenderer.startColor = Color;
            _lineRenderer.endColor = Color;
            _lineRenderer.SetPositions(new Vector3[] {
                Origin,
                CalculateEndpoint(Origin)
            });
            _lineRenderer.gameObject.SetActive(Enabled);
            // SuperController.LogMessage($"{_lineRenderer.gameObject.layer} - {_canvas.gameObject.layer}");
            _canvas.gameObject.SetActive(_lineRenderer.gameObject.activeInHierarchy);
            if(Enabled) {
                var text = _canvas.GetComponentInChildren<Text>();
                if(text) {
                    _canvas.gameObject.SetActive(LabelEnabled);
                    if(LineDirection == Vector3.left) {
                        text.text = Label;
                        text.transform.parent.transform.position = CalculateEndpoint(Origin);
                        text.color = Color;
                    }
                    if(LineDirection == Vector3.right) {
                        text.text = Label;
                        text.transform.parent.transform.position = CalculateEndpoint(Origin) + new Vector3(0.07f, 0, 0);
                        text.color = Color;
                    }
                }
            }
        }

        public void OnDestroy() {
            Destroy(_lineRenderer);
            _lineRenderer = null;
            if (SuperController.singleton != null)
            {
                SuperController.singleton.RemoveCanvas(_canvas);
            }
            if(_canvas != null) {
                _canvas.transform.SetParent(null, false);
                if (_canvas.gameObject != null)
                {
                    Destroy(_canvas.gameObject);
                }
            }
        }

        private Vector3 CalculateEndpoint(Vector3 start) {
            var direction = LineDirection;
            direction.x *= Length;
            direction.y *= Length;
            direction.z *= Length;
            return start + direction;
        }

        private void InitTextLabel() {
                        // test text input
            GameObject canvasObject = new GameObject();
            _canvas = canvasObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;
            _canvas.pixelPerfect = false;
            SuperController.singleton.AddCanvas(_canvas);

            CanvasScaler cs = canvasObject.AddComponent<CanvasScaler>();
            cs.scaleFactor = 80.0f;
            cs.dynamicPixelsPerUnit = 1f;

            // GraphicRaycaster gr = canvasObject.AddComponent<GraphicRaycaster>();
            _canvas.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);
            //canvas.transform.localPosition = new Vector3(-0.7f, 0, 0);
            _canvas.transform.localPosition = new Vector3(0.26f, -0.14f, 0.0f);
            _canvas.transform.Rotate(new Vector3(0, 180, 0));

            GameObject container = new GameObject();
            container.name = "Placeholder";
            container.transform.SetParent(_canvas.transform, false);

            Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

            Text placeholderText = container.AddComponent<Text>();
            placeholderText.color = new Color(1, 1, 1);
            placeholderText.font = ArialFont;
            placeholderText.fontSize = 20;
            placeholderText.fontStyle = FontStyle.Normal;
            placeholderText.supportRichText = false;
            placeholderText.horizontalOverflow = HorizontalWrapMode.Overflow;
            placeholderText.text = "";
            placeholderText.alignment = TextAnchor.MiddleLeft;


            container.transform.position = new Vector3(0, 0, 0);

            RectTransform rt = container.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(260, 0);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 500);
        }
    }
}
