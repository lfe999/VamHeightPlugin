using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        // measurement storables
        JSONStorableFloat _fullHeightStorable;
        JSONStorableFloat _headSizeHeightStorable;
        JSONStorableFloat _headSizeWidthStorable;
        JSONStorableFloat _chinHeightStorable;
        JSONStorableFloat _shoulderHeightStorable;
        JSONStorableFloat _nippleHeightStorable;
        JSONStorableFloat _underbustHeightStorable;
        JSONStorableFloat _navelHeightStorable;
        JSONStorableFloat _crotchHeightStorable;
        JSONStorableFloat _kneeBottomHeightStorable;
        JSONStorableFloat _heightInHeadsStorable;
        JSONStorableFloat _breastBustStorable;
        JSONStorableFloat _breastUnderbustStorable;
        JSONStorableFloat _breastBandStorable;
        JSONStorableString _breastCupStorable;
        JSONStorableFloat _waistSizeStorable;
        JSONStorableFloat _hipSizeStorable;

        // TODO: chin to shoulder
        // TOOD: shoulder width

        // other storables
        JSONStorableFloat _markerSpreadStorable;
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

        public void InitStorables() {
            // Cup algorithm choice
            _cupAlgorithmStorable = new JSONStorableStringChooser(
                "Cup Size Method",
                _cupCalculators.Select(cc => cc.Name).ToList(),
                _cupCalculators[0].Name,
                "Cup Size Method"
            );

            // Float: How far apart markers are spread apart
            _markerSpreadStorable = new JSONStorableFloat("Spread Markers", 0.02f, -1, 1);
            RegisterFloat(_markerSpreadStorable);

            // Float: Move markers forward or backward relative to center depth of the head
            _markerFrontBackStorable = new JSONStorableFloat("Move Markers Forward/Backward", 0.15f, -5, 5);
            RegisterFloat(_markerFrontBackStorable);

            // Float: Move markers left or right 
            _markerLeftRightStorable = new JSONStorableFloat("Move Markers Left/Right", 0, -5, 5);
            RegisterFloat(_markerLeftRightStorable);

            // Bool: Show the tape measures for circumference?
            _showTapeMarkersStorable = new JSONStorableBool("Show Tape Measure Markers", false);
            RegisterBool(_showTapeMarkersStorable);

            // Bool: Show head hight markers (white ones)
            _showHeadHeightMarkersStorable = new JSONStorableBool("Show Head Height Markers", true);
            RegisterBool(_showHeadHeightMarkersStorable);

            // Bool: Show the main feature markers for the figure
            _showFeatureMarkersStorable = new JSONStorableBool("Show Figure Feature Markers", true);
            RegisterBool(_showFeatureMarkersStorable);

            // Bool: Show the feature marker labels
            _showFeatureMarkerLabelsStorable = new JSONStorableBool("Show Feature Marker Labels", true);
            RegisterBool(_showFeatureMarkerLabelsStorable);


            // calculated positions and distances for other plugins to use if wanted
            _fullHeightStorable = new JSONStorableFloat("figureHeight", 0, 0, 100);
            RegisterFloat(_fullHeightStorable);

            _heightInHeadsStorable = new JSONStorableFloat("figureHeightHeads", 0, 0, 100);
            RegisterFloat(_heightInHeadsStorable);

            _headSizeHeightStorable = new JSONStorableFloat("headSizeHeight", 0, 0, 100);
            RegisterFloat(_headSizeHeightStorable);

            _chinHeightStorable = new JSONStorableFloat("chinHeight", 0, 0, 100);
            RegisterFloat(_chinHeightStorable);

            _shoulderHeightStorable = new JSONStorableFloat("shoulderHeight", 0, 0, 100);
            RegisterFloat(_shoulderHeightStorable);

            _nippleHeightStorable = new JSONStorableFloat("nippleHeight", 0, 0, 100);
            RegisterFloat(_nippleHeightStorable);

            _underbustHeightStorable = new JSONStorableFloat("underbustHeight", 0, 0, 100);
            RegisterFloat(_underbustHeightStorable);

            _navelHeightStorable = new JSONStorableFloat("navelHeight", 0, 0, 100);
            RegisterFloat(_navelHeightStorable);

            _crotchHeightStorable = new JSONStorableFloat("crotchHeight", 0, 0, 100);
            RegisterFloat(_crotchHeightStorable);

            _kneeBottomHeightStorable = new JSONStorableFloat("kneeHeight", 0, 0, 100);
            RegisterFloat(_kneeBottomHeightStorable);

            _headSizeWidthStorable = new JSONStorableFloat("headSizeWidth", 0, 0, 100);
            RegisterFloat(_headSizeWidthStorable);

            _breastBustStorable = new JSONStorableFloat("breastBustSize", 0, 0, 100);
            RegisterFloat(_breastBustStorable);

            _breastUnderbustStorable = new JSONStorableFloat("breastUnderbustSize", 0, 0, 100);
            RegisterFloat(_breastUnderbustStorable);

            _breastBandStorable = new JSONStorableFloat("breastBandSize", 0, 0, 100);
            RegisterFloat(_breastBandStorable);

            _breastCupStorable = new JSONStorableString("breastCupSize", "");
            RegisterString(_breastCupStorable);

            _waistSizeStorable = new JSONStorableFloat("waistSize", 0, 0, 100);
            RegisterFloat(_waistSizeStorable);

            _hipSizeStorable = new JSONStorableFloat("hipSize", 0, 0, 100);
            RegisterFloat(_hipSizeStorable);

        }

        public override void Init()
        {
            _dazCharacter = containingAtom.GetComponentInChildren<DAZCharacter>();
            Skin = _dazCharacter.skin;

            // initialize the line markers
            InitLineMarkers();

            // initialize storables
            InitStorables();

            // initialize the ui components
            CreateScrollablePopup(_cupAlgorithmStorable);
            CreateSlider(_markerSpreadStorable, rightSide: true);
            CreateSlider(_markerFrontBackStorable, rightSide: true);
            CreateSlider(_markerLeftRightStorable, rightSide: true);
            CreateToggle(_showTapeMarkersStorable);
            CreateToggle(_showHeadHeightMarkersStorable);
            CreateToggle(_showFeatureMarkersStorable);
            CreateToggle(_showFeatureMarkerLabelsStorable);
        }

        public void InitLineMarkers() {
            // this does not position them, just creates them
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

        private float _updateEverySeconds = 0.05f;
        private float _updateCountdown = 0;
        public void Update() {
            _dazCharacter = containingAtom.GetComponentInChildren<DAZCharacter>();
            Skin = _dazCharacter.skin;

            if(SuperController.singleton.freezeAnimation) {
                return;
            }

            // throttle the update loop
            _updateCountdown -= Time.deltaTime;
            if(_updateCountdown > 0) {
                return;
            }
            _updateCountdown = _updateEverySeconds;


            try {
                UpdateMeasurements();
                UpdateMarkerPositions();
                UpdateMarkerLabels();
            }
            catch(Exception e) {
                SuperController.LogError(e.ToString());
            }
        }

        private void UpdateMarkerPositions() {
            Vector3 pos;
            // head
            pos = _vertexHead.Position(this);
            pos.x -= _markerSpreadStorable.val + _markerLeftRightStorable.val;
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

        }

        private void UpdateMeasurements() {
            // basic body heights
            _headSizeHeightStorable.val = Vector3.Distance(_markerHead.Origin, _markerChin.Origin);
            _headSizeWidthStorable.val = Vector3.Distance(_vertexEarLeft.Position(this), _vertexEarRight.Position(this));

            _fullHeightStorable.val = Vector3.Distance(_markerHead.Origin, _markerHeel.Origin);
            _heightInHeadsStorable.val = _headSizeHeightStorable.val == 0 ? 0 : _fullHeightStorable.val / _headSizeHeightStorable.val;

            _chinHeightStorable.val = Vector3.Distance(_markerChin.Origin, _markerHeel.Origin);
            // TODO: shoulder
            _nippleHeightStorable.val = Vector3.Distance(_markerNipple.Origin, _markerHeel.Origin);
            _underbustHeightStorable.val = Vector3.Distance(_markerUnderbust.Origin, _markerHeel.Origin);
            _navelHeightStorable.val = Vector3.Distance(_markerNavel.Origin, _markerHeel.Origin);
            _crotchHeightStorable.val = Vector3.Distance(_markerGroin.Origin, _markerHeel.Origin);
            _kneeBottomHeightStorable.val = Vector3.Distance(_markerKnee.Origin, _markerHeel.Origin);

            // measure things around (breast, waist, hip)
            _waistSizeStorable.val = _circumferenceWaist;
            _hipSizeStorable.val = _circumferenceHip;

            var cupInfo = GetCupInfo();
            if(cupInfo == null) {
                _breastBustStorable.val = 0;
                _breastUnderbustStorable.val = 0;
                _breastBandStorable.val = 0;
                _breastCupStorable.val = "";
            }
            else {
                _breastBustStorable.val = cupInfo.Bust;
                _breastUnderbustStorable.val = cupInfo.Underbust;
                _breastBandStorable.val = cupInfo.Band;
                _breastCupStorable.val = cupInfo.Cup;
            }
        }

        private void UpdateMarkerLabels() {

            if(!_showFeatureMarkerLabelsStorable.val) {
                return;
            }

            // update marker labels
            _markerHead.Label = "Head (Head To Heel "
                + $"{(int)(_fullHeightStorable.val * 100)} cm / "
                + $"{UnitUtils.MetersToFeetString(_fullHeightStorable.val)} / "
                + $"{_heightInHeadsStorable.val:0.0} heads)";

            _markerChin.Label = "Neck (Head Height "
                + $"{(int)(_headSizeHeightStorable.val * 100)} cm / "
                + $"{UnitUtils.MetersToFeetString(_headSizeHeightStorable.val)}; "
                + $"Width {(int)(_headSizeWidthStorable.val * 100)} cm / {UnitUtils.MetersToFeetString(_headSizeWidthStorable.val)})";

            if(_breastBustStorable.val == 0) {
                _markerNipple.Label = "Nipple";
                _markerUnderbust.Label = "Underbust";
            }
            else {
                _markerNipple.Label = $"Nipple "
                + $"(Cup {_breastBandStorable.val}{_breastCupStorable.val}; "
                + $"Around {(int)(_breastBustStorable.val * 100)} cm / "
                + $"{(int)UnitUtils.UnityToInches(_breastBustStorable.val)} in)";

                _markerUnderbust.Label = "Underbust (Around "
                    + $"{(int)(_breastUnderbustStorable.val * 100)} cm / "
                    + $"{(int)UnitUtils.UnityToInches(_breastUnderbustStorable.val)} in)";
            }

            _markerNavel.Label = "Navel (Waist "
                + $"{(int)(_waistSizeStorable.val * 100)} cm / "
                + $"{Mathf.RoundToInt(UnitUtils.UnityToInches(_waistSizeStorable.val))} in)";

            _markerGroin.Label = "Crotch (Hip "
                + $"{(int)(_hipSizeStorable.val * 100)} cm / "
                + $"{Mathf.RoundToInt(UnitUtils.UnityToInches(_hipSizeStorable.val))} in)";

            var crotchToKnee = _crotchHeightStorable.val - _kneeBottomHeightStorable.val;
            _markerKnee.Label = $"Knee Bottom (Crotch to Knee "
                + $"{(int)(crotchToKnee * 100)} cm / "
                + $"{UnitUtils.MetersToFeetString(crotchToKnee)} / "
                + $"{crotchToKnee / _headSizeHeightStorable.val:0.0} heads)";

            _markerHeel.Label = $"Heel (Knee to Heel "
                + $"{(int)(_kneeBottomHeightStorable.val * 100)} cm / "
                + $"{UnitUtils.MetersToFeetString(_kneeBottomHeightStorable.val)} / "
                + $"{_kneeBottomHeightStorable.val / _headSizeHeightStorable.val:0.0} heads)";

        }

        private float LineLength(Vector3[] vertices) {
            float total = 0;
            for(var i = 1; i < vertices.Length; i++) {
                var distance = Mathf.Abs(Vector3.Distance(vertices[i-1], vertices[i]));
                total += distance;
            }
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

        readonly List<HorizontalMarker> _extraHeadMarkers = new List<HorizontalMarker>();
        private void UpdateHeadHeightMarkers() {
            var height = _fullHeightStorable.val;
            var headHeight = _headSizeHeightStorable.val;
            var heightInHeadsRoundedUp = (int)Mathf.Ceil(_heightInHeadsStorable.val);

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
                    pos.x += _markerSpreadStorable.val - _markerLeftRightStorable.val;
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
}
