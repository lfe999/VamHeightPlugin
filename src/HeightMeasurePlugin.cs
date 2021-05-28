using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LFE
{
    public class HeightMeasurePlugin : MVRScript
    {
        public DAZSkinV2 Skin;
        public readonly ICupCalculator[] cupCalculators = new ICupCalculator[] {
            new KnixComCupCalculator(),
            new SizeChartCupCalculator(),
            new ChateLaineCupCalculator()
        };

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
        readonly IVertexPosition _vertexShoulder = new VertexPositionMiddle(11110, 182);
        readonly IVertexPosition _vertexEyeLeftTopHeight = new VertexPositionMiddle(7478, 1930);
        // readonly IVertexPosition _vertexEyeLeftMidHeight = new VertexPositionMiddle(14006, 18050, 0.1f);
        readonly IVertexPosition _vertexEyeLeftMidHeight = new VertexPositionExact(14006);
        readonly IVertexPosition _vertexEyeLeftInner = new VertexPositionExact(7575); // a little past the lacrimal -- this is actually part of the nose
        readonly IVertexPosition _vertexEyeLeftBottom = new VertexPositionExact(3187);
        readonly IVertexPosition _vertexEyeLeftOuterHeight = new VertexPositionExact(7351);
        readonly IVertexPosition _vertexEyeRightTopHeight = new VertexPositionMiddle(18175, 12858);
        readonly IVertexPosition _vertexEyeRightMidHeight = new VertexPositionMiddle(3223, 7351, 0.2f);
        readonly IVertexPosition _vertexEyeRightInner = new VertexPositionExact(18267); // a little past the lacrimal -- this is actually part of the nose
        readonly IVertexPosition _vertexEyeRightBottom = new VertexPositionExact(13972);
        readonly IVertexPosition _vertexEyeRightOuter = new VertexPositionExact(18050); 
        readonly IVertexPosition _vertexNoseTip = new VertexPositionExact(2111);
        readonly IVertexPosition _vertexNoseBottom = new VertexPositionExact(3252);
        readonly IVertexPosition _vertexMouthLeftSideMiddle = new VertexPositionExact(1655);
        readonly IVertexPosition _vertexMouthRightSideMiddle = new VertexPositionExact(12319);
        readonly IVertexPosition _vertexMouthMidHeight = new VertexPositionMiddle(2136, 2145);
        readonly IVertexPosition _vertexPenisTip = new VertexPositionExact(21627); // not the exact tip - it is a little off to the left
        // readonly IVertexPosition _vertexPenisBase = new VertexPositionMiddle(22825, 22269);
        readonly IVertexPosition _vertexPenisBase = new VertexPositionMiddle(22270, 22865);
        readonly IVertexPosition _vertexPenisShaftLeft = new VertexPositionExact(22608);
        readonly IVertexPosition _vertexPenisShaftRight = new VertexPositionExact(22616);

        DAZBone _eyeLeft;
        DAZCharacter _dazCharacter;

        HorizontalMarker _markerHead;
        HorizontalMarker _markerChin;
        HorizontalMarker _markerShoulder;
        HorizontalMarker _markerNipple;
        HorizontalMarker _markerUnderbust;
        HorizontalMarker _markerNavel;
        HorizontalMarker _markerGroin;
        HorizontalMarker _markerKnee;
        HorizontalMarker _markerHeel;

        HorizontalMarker _markerEyeMidHeight;
        HorizontalMarker _markerEyeRightOuter;
        HorizontalMarker _markerEyeLeftOuter;
        HorizontalMarker _markerMouthMidHeight;
        HorizontalMarker _markerMouthLeft;
        HorizontalMarker _markerMouthRight;
        HorizontalMarker _markerNoseBottomHeight;
        HorizontalMarker _markerChinSmall;
        HorizontalMarker _markerHeadSmall;
        HorizontalMarker _markerHeadLeft;
        HorizontalMarker _markerHeadRight;
        HorizontalMarker _markerFaceCenter;

        private UI _ui;

        public override void Init()
        {
            _ui = new UI(this);
            _ui.Draw();

            _dazCharacter = containingAtom.GetComponentInChildren<DAZCharacter>();
            Skin = _dazCharacter.skin;

            // initialize the line markers
            InitLineMarkers();

        }

        public void InitLineMarkers() {
            var featureColor = HSVToColor(_ui.featureMarkerColor.val);
            var faceColor = HSVToColor(_ui.faceMarkerColor.val);

            // this does not position them, just creates them
            _markerHead = gameObject.AddComponent<HorizontalMarker>();
            _markerHead.Name = "Head";
            _markerHead.Color = featureColor;

            _markerChin = gameObject.AddComponent<HorizontalMarker>();
            _markerChin.Name = "Chin";
            _markerChin.Color = featureColor;

            _markerShoulder = gameObject.AddComponent<HorizontalMarker>();
            _markerShoulder.Name = "Shoulder";
            _markerShoulder.Color = featureColor;

            _markerNipple = gameObject.AddComponent<HorizontalMarker>();
            _markerNipple.Name = "Nipple";
            _markerNipple.Color = featureColor;

            _markerUnderbust = gameObject.AddComponent<HorizontalMarker>();
            _markerUnderbust.Name = "Underbust";
            _markerUnderbust.Color = featureColor;

            _markerNavel = gameObject.AddComponent<HorizontalMarker>();
            _markerNavel.Name = "Navel";
            _markerNavel.Color = featureColor;

            _markerGroin = gameObject.AddComponent<HorizontalMarker>();
            _markerGroin.Name = "Groin";
            _markerGroin.Color = featureColor;

            _markerKnee = gameObject.AddComponent<HorizontalMarker>();
            _markerKnee.Name = "Knee";
            _markerKnee.Color = featureColor;

            _markerHeel = gameObject.AddComponent<HorizontalMarker>();
            _markerHeel.Name = "Heel";
            _markerHeel.Color = featureColor;

            _eyeLeft = containingAtom.GetStorableByID("lEye") as DAZBone;

            _markerEyeMidHeight = gameObject.AddComponent<HorizontalMarker>();
            _markerEyeMidHeight.Name = "Eye Height";
            _markerEyeMidHeight.Color = faceColor;

            _markerEyeRightOuter = gameObject.AddComponent<HorizontalMarker>();
            _markerEyeRightOuter.Name = "Eye Right Outer";
            _markerEyeRightOuter.Color = faceColor;
            _markerEyeRightOuter.LineDirection = Vector3.up;

            _markerEyeLeftOuter = gameObject.AddComponent<HorizontalMarker>();
            _markerEyeLeftOuter.Name = "Eye Left Outer";
            _markerEyeLeftOuter.Color = faceColor;
            _markerEyeLeftOuter.LineDirection = Vector3.up;

            _markerNoseBottomHeight = gameObject.AddComponent<HorizontalMarker>();
            _markerNoseBottomHeight.Name = "Nose Bottom Height";
            _markerNoseBottomHeight.Color = faceColor;

            _markerMouthMidHeight = gameObject.AddComponent<HorizontalMarker>();
            _markerMouthMidHeight.Name = "Mouth Height";
            _markerMouthMidHeight.Color = faceColor;

            _markerMouthLeft = gameObject.AddComponent<HorizontalMarker>();
            _markerMouthLeft.Name = "Mouth Left";
            _markerMouthLeft.Color = faceColor;
            _markerMouthLeft.LineDirection = Vector3.up;

            _markerMouthRight = gameObject.AddComponent<HorizontalMarker>();
            _markerMouthRight.Name = "Mouth Right";
            _markerMouthRight.Color = faceColor;
            _markerMouthRight.LineDirection = Vector3.up;

            _markerChinSmall = gameObject.AddComponent<HorizontalMarker>();
            _markerChinSmall.Name = "Chin Small";
            _markerChinSmall.Color = faceColor;

            _markerHeadSmall = gameObject.AddComponent<HorizontalMarker>();
            _markerHeadSmall.Name = "Head Small";
            _markerHeadSmall.Color = faceColor;

            _markerHeadRight = gameObject.AddComponent<HorizontalMarker>();
            _markerHeadRight.Name = "Head Right";
            _markerHeadRight.Color = faceColor;
            _markerHeadRight.LineDirection = Vector2.up;

            _markerHeadLeft = gameObject.AddComponent<HorizontalMarker>();
            _markerHeadLeft.Name = "Head Left";
            _markerHeadLeft.Color = faceColor;
            _markerHeadLeft.LineDirection = Vector3.up;

            _markerFaceCenter = gameObject.AddComponent<HorizontalMarker>();
            _markerFaceCenter.Name = "Face Center";
            _markerFaceCenter.Color = faceColor;
            _markerFaceCenter.LineDirection = Vector3.up;

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

        private readonly float _updateEverySeconds = 0.05f;
        private float _updateCountdown = 0;
        public void Update() {
            _dazCharacter = containingAtom.GetComponentInChildren<DAZCharacter>();
            Skin = _dazCharacter.skin;

            if(_ui == null) {
                return;
            }

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

            Color featureColor = HSVToColor(_ui.featureMarkerColor.val);

            Vector3 pos;
            // head
            pos = _vertexHead.Position(this);
            pos.x -= _ui.markerSpreadStorable.val + _ui.markerLeftRightStorable.val;
            pos.z += _ui.markerFrontBackStorable.val;
            _markerHead.Origin = pos;
            _markerHead.Enabled = _ui.showFeatureMarkersStorable.val;
            _markerHead.LabelEnabled = _ui.showFeatureMarkerLabelsStorable.val;
            _markerHead.Thickness = _ui.lineThicknessFigureStorable.val * 0.001f;
            _markerHead.Color = featureColor;

            // chin
            pos = _vertexChin.Position(this);
            pos.x = _markerHead.Origin.x;
            pos.z = _markerHead.Origin.z;
            _markerChin.Origin = pos;
            _markerChin.Enabled = _ui.showFeatureMarkersStorable.val;
            _markerChin.LabelEnabled = _ui.showFeatureMarkerLabelsStorable.val;
            _markerChin.Thickness = _ui.lineThicknessFigureStorable.val * 0.001f;
            _markerChin.Color = featureColor;

            // shoulder
            pos = _vertexShoulder.Position(this);
            pos.x = _markerHead.Origin.x;
            pos.z = _markerHead.Origin.z;
            _markerShoulder.Origin = pos;
            _markerShoulder.Enabled = _ui.showFeatureMarkersStorable.val;
            _markerShoulder.LabelEnabled = _ui.showFeatureMarkerLabelsStorable.val;
            _markerShoulder.Thickness = _ui.lineThicknessFigureStorable.val * 0.001f;
            _markerShoulder.Color = featureColor;

            // nipple
            pos = _vertexNipple.Position(this);
            pos.x = _markerHead.Origin.x;
            pos.z = _markerHead.Origin.z;
            _markerNipple.Origin = pos;
            _markerNipple.Enabled = _dazCharacter.isMale ? false : _ui.showFeatureMarkersStorable.val; // TODO: better male nipple detection
            _markerNipple.LabelEnabled = _ui.showFeatureMarkerLabelsStorable.val;
            _markerNipple.Thickness = _ui.lineThicknessFigureStorable.val * 0.001f;
            _markerNipple.Color = featureColor;

            // underbust
            pos = _vertexUnderbust.Position(this);
            pos.x = _markerHead.Origin.x;
            pos.z = _markerHead.Origin.z;
            _markerUnderbust.Origin = pos;
            _markerUnderbust.Enabled = _dazCharacter.isMale ? false : _ui.showFeatureMarkersStorable.val;
            _markerUnderbust.LabelEnabled = _ui.showFeatureMarkerLabelsStorable.val;
            _markerUnderbust.Thickness = _ui.lineThicknessFigureStorable.val * 0.001f;
            _markerUnderbust.Color = featureColor;

            // navel
            pos = _vertexNavel.Position(this);
            pos.x = _markerHead.Origin.x;
            pos.z = _markerHead.Origin.z;
            _markerNavel.Origin = pos;
            _markerNavel.Enabled = _ui.showFeatureMarkersStorable.val;
            _markerNavel.LabelEnabled = _ui.showFeatureMarkerLabelsStorable.val;
            _markerNavel.Thickness = _ui.lineThicknessFigureStorable.val * 0.001f;
            _markerNavel.Color = featureColor;

            // groin
            pos = _vertexGroin.Position(this);
            pos.x = _markerHead.Origin.x;
            pos.z = _markerHead.Origin.z;
            _markerGroin.Origin = pos;
            _markerGroin.Enabled = _ui.showFeatureMarkersStorable.val;
            _markerGroin.LabelEnabled = _ui.showFeatureMarkerLabelsStorable.val;
            _markerGroin.Thickness = _ui.lineThicknessFigureStorable.val * 0.001f;
            _markerGroin.Color = featureColor;

            // knee
            pos = _vertexKnee.Position(this);
            pos.x = _markerHead.Origin.x;
            pos.z = _markerHead.Origin.z;
            _markerKnee.Origin = pos;
            _markerKnee.Enabled = _ui.showFeatureMarkersStorable.val;
            _markerKnee.LabelEnabled = _ui.showFeatureMarkerLabelsStorable.val;
            _markerKnee.Thickness = _ui.lineThicknessFigureStorable.val * 0.001f;
            _markerKnee.Color = featureColor;

            // heel - can not find vertex for heel - using colliders
            var rFoot = containingAtom.GetComponentsInChildren<CapsuleCollider>().FirstOrDefault(c => ColliderName(c).Equals("rFoot/_Collider1")); 
            if(rFoot) {
                pos = rFoot.transform.position;
                pos.x = _markerHead.Origin.x;
                pos.z = _markerHead.Origin.z;
                _markerHeel.Origin = pos;
                _markerHeel.Enabled = _ui.showFeatureMarkersStorable.val;
                _markerHeel.LabelEnabled = _ui.showFeatureMarkerLabelsStorable.val;
                _markerHeel.Thickness = _ui.lineThicknessFigureStorable.val * 0.001f;
                _markerHeel.Color = featureColor;
            }
            
            UpdateHeadHeightMarkers();

            if(_dazCharacter.isMale) {
                UpdatePenisMarkers();
            }
            else {
                UpdateBustMarkersFromMorphVertex();
                UpdateUnderbustMarkersFromMorphVertex();
            }
            UpdateWaistMarkersFromMorphVertex();
            UpdateHipMarkersFromMorphVertex();

            var midpoint = _vertexHead.Position(this);
            var midpointX = midpoint.x + (_ui.headSizeWidthStorable.val / 2) - _ui.markerLeftRightStorable.val;

            var faceColor = HSVToColor(_ui.faceMarkerColor.val);
            var faceLineThickness = _ui.lineThicknessFaceStorable.val * 0.001f;

            // eye midline
            pos = _eyeLeft.transform.position; // comes from a daz bone not a vertex
            pos.x = midpointX;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerEyeMidHeight.Length = _ui.headSizeWidthStorable.val;
            _markerEyeMidHeight.Origin = pos;
            _markerEyeMidHeight.Enabled = _ui.showFaceMarkersStorable.val;
            _markerEyeMidHeight.LabelEnabled = _ui.showFaceMarkersStorable.val;
            _markerEyeMidHeight.Thickness = faceLineThickness;
            _markerEyeMidHeight.Color = faceColor;

            // eye right outer
            pos = _vertexEyeRightOuter.Position(this);
            pos.x = pos.x - _ui.markerLeftRightStorable.val;
            pos.y = _markerEyeMidHeight.Origin.y - _ui.headSizeHeightStorable.val / 4 / 2;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerEyeRightOuter.Length = _ui.headSizeHeightStorable.val / 4;
            _markerEyeRightOuter.Origin = pos;
            _markerEyeRightOuter.Enabled = _ui.showFaceMarkersStorable.val;
            _markerEyeRightOuter.LabelEnabled = _ui.showFaceMarkersStorable.val;
            _markerEyeRightOuter.Thickness = faceLineThickness;
            _markerEyeRightOuter.Color = faceColor;

            // eye left outer
            pos = _vertexEyeLeftOuterHeight.Position(this);
            pos.x = pos.x - _ui.markerLeftRightStorable.val;
            pos.y = _markerEyeMidHeight.Origin.y - _ui.headSizeHeightStorable.val / 4 / 2;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerEyeLeftOuter.Length = _ui.headSizeHeightStorable.val / 4;
            _markerEyeLeftOuter.Origin = pos;
            _markerEyeLeftOuter.Enabled = _ui.showFaceMarkersStorable.val;
            _markerEyeLeftOuter.LabelEnabled = _ui.showFaceMarkersStorable.val;
            _markerEyeLeftOuter.Thickness = faceLineThickness;
            _markerEyeLeftOuter.Color = faceColor;

            // nose bottom
            pos = _vertexNoseBottom.Position(this);
            pos.x = midpointX;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerNoseBottomHeight.Length = _ui.headSizeWidthStorable.val;
            _markerNoseBottomHeight.Origin = pos;
            _markerNoseBottomHeight.Enabled = _ui.showFaceMarkersStorable.val;
            _markerNoseBottomHeight.LabelEnabled = _ui.showFaceMarkersStorable.val;
            _markerNoseBottomHeight.Thickness = faceLineThickness;
            _markerNoseBottomHeight.Color = faceColor;

            // mouth middle
            pos = _vertexMouthMidHeight.Position(this);
            pos.x = midpointX;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerMouthMidHeight.Length = _ui.headSizeWidthStorable.val;
            _markerMouthMidHeight.Origin = pos;
            _markerMouthMidHeight.Enabled = _ui.showFaceMarkersStorable.val;
            _markerMouthMidHeight.LabelEnabled = _ui.showFaceMarkersStorable.val;
            _markerMouthMidHeight.Thickness = faceLineThickness;
            _markerMouthMidHeight.Color = faceColor;

            // mouth left
            pos = _vertexMouthLeftSideMiddle.Position(this);
            pos.x = pos.x - _ui.markerLeftRightStorable.val;
            pos.y = _markerMouthMidHeight.Origin.y - (_ui.headSizeHeightStorable.val / 8 / 2);
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerMouthLeft.Length = _ui.headSizeHeightStorable.val / 8;
            _markerMouthLeft.Origin = pos;
            _markerMouthLeft.Enabled = _ui.showFaceMarkersStorable.val;
            _markerMouthLeft.LabelEnabled = _ui.showFaceMarkersStorable.val;
            _markerMouthLeft.Thickness = faceLineThickness;
            _markerMouthLeft.Color = faceColor;

            // mouth right
            pos = _vertexMouthRightSideMiddle.Position(this);
            pos.x = pos.x - _ui.markerLeftRightStorable.val;
            pos.y = _markerMouthMidHeight.Origin.y - (_ui.headSizeHeightStorable.val / 8 / 2);
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerMouthRight.Length = _ui.headSizeHeightStorable.val / 8;
            _markerMouthRight.Origin = pos;
            _markerMouthRight.Enabled = _ui.showFaceMarkersStorable.val;
            _markerMouthRight.LabelEnabled = _ui.showFaceMarkersStorable.val;
            _markerMouthRight.Thickness = faceLineThickness;
            _markerMouthRight.Color = faceColor;

            // chin small and blue
            pos = _vertexHead.Position(this);
            pos.x = pos.x - _ui.markerLeftRightStorable.val + _ui.headSizeWidthStorable.val / 2;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerChinSmall.Length = _ui.headSizeWidthStorable.val;
            _markerChinSmall.Origin = pos;
            _markerChinSmall.Enabled = _ui.showFaceMarkersStorable.val;
            _markerChinSmall.LabelEnabled = _ui.showFaceMarkersStorable.val;
            _markerChinSmall.Thickness = faceLineThickness;
            _markerChinSmall.Color = faceColor;

            // head small and blue
            pos = _vertexHead.Position(this);
            pos.y = pos.y - _ui.headSizeHeightStorable.val;
            pos.x = pos.x - _ui.markerLeftRightStorable.val + _ui.headSizeWidthStorable.val / 2;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerHeadSmall.Length = _ui.headSizeWidthStorable.val;
            _markerHeadSmall.Origin = pos;
            _markerHeadSmall.Enabled = _ui.showFaceMarkersStorable.val;
            _markerHeadSmall.LabelEnabled = _ui.showFaceMarkersStorable.val;
            _markerHeadSmall.Thickness = faceLineThickness;
            _markerHeadSmall.Color = faceColor;

            // head left
            pos = _vertexHead.Position(this);
            pos.y = pos.y - _ui.headSizeHeightStorable.val;
            pos.x = pos.x - _ui.markerLeftRightStorable.val + _ui.headSizeWidthStorable.val / 2;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerHeadLeft.Length = _ui.headSizeHeightStorable.val;
            _markerHeadLeft.Origin = pos;
            _markerHeadLeft.Enabled = _ui.showFaceMarkersStorable.val;
            _markerHeadLeft.LabelEnabled = _ui.showFaceMarkersStorable.val;
            _markerHeadLeft.Thickness = faceLineThickness;
            _markerHeadLeft.Color = faceColor;

            // head right
            pos = _vertexHead.Position(this);
            pos.y = pos.y - _ui.headSizeHeightStorable.val;
            pos.x = pos.x - _ui.markerLeftRightStorable.val - _ui.headSizeWidthStorable.val / 2;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerHeadRight.Length = _ui.headSizeHeightStorable.val;
            _markerHeadRight.Origin = pos;
            _markerHeadRight.Enabled = _ui.showFaceMarkersStorable.val;
            _markerHeadRight.LabelEnabled = _ui.showFaceMarkersStorable.val;
            _markerHeadRight.Thickness = faceLineThickness;
            _markerHeadRight.Color = faceColor;

            // face center
            pos = _vertexNoseTip.Position(this);
            pos.y = pos.y - (pos.y - _markerChin.Origin.y);
            pos.x = pos.x - _ui.markerLeftRightStorable.val;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerFaceCenter.Length = _ui.headSizeHeightStorable.val;
            _markerFaceCenter.Origin = pos;
            _markerFaceCenter.Enabled = _ui.showFaceMarkersStorable.val;
            _markerFaceCenter.LabelEnabled = _ui.showFaceMarkersStorable.val;
            _markerFaceCenter.Thickness = faceLineThickness;
            _markerFaceCenter.Color = faceColor;
        }



        private void UpdateMeasurements() {
            // basic body heights
            _ui.headSizeHeightStorable.val = Vector3.Distance(_markerHead.Origin, _markerChin.Origin);
            _ui.headSizeWidthStorable.val = Vector3.Distance(_vertexEarLeft.Position(this), _vertexEarRight.Position(this));

            _ui.fullHeightStorable.val = Vector3.Distance(_markerHead.Origin, _markerHeel.Origin);
            _ui.heightInHeadsStorable.val = _ui.headSizeHeightStorable.val == 0 ? 0 : _ui.fullHeightStorable.val / _ui.headSizeHeightStorable.val;

            _ui.chinHeightStorable.val = Vector3.Distance(_markerChin.Origin, _markerHeel.Origin);
            _ui.shoulderHeightStorable.val = Vector3.Distance(_markerShoulder.Origin, _markerHeel.Origin);
            _ui.nippleHeightStorable.val = Vector3.Distance(_markerNipple.Origin, _markerHeel.Origin);
            _ui.underbustHeightStorable.val = Vector3.Distance(_markerUnderbust.Origin, _markerHeel.Origin);
            _ui.navelHeightStorable.val = Vector3.Distance(_markerNavel.Origin, _markerHeel.Origin);
            _ui.crotchHeightStorable.val = Vector3.Distance(_markerGroin.Origin, _markerHeel.Origin);
            _ui.kneeBottomHeightStorable.val = Vector3.Distance(_markerKnee.Origin, _markerHeel.Origin);

            // measure things around (breast, waist, hip)
            _ui.waistSizeStorable.val = _circumferenceWaist;
            _ui.hipSizeStorable.val = _circumferenceHip;

            var cupInfo = GetCupInfo();
            if(cupInfo == null) {
                _ui.breastBustStorable.val = 0;
                _ui.breastUnderbustStorable.val = 0;
                _ui.breastBandStorable.val = 0;
                _ui.breastCupStorable.val = "";
            }
            else {
                _ui.breastBustStorable.val = cupInfo.Bust;
                _ui.breastUnderbustStorable.val = cupInfo.Underbust;
                _ui.breastBandStorable.val = cupInfo.Band;
                _ui.breastCupStorable.val = cupInfo.Cup;
            }

            if(_dazCharacter.isMale) {
                _ui.penisLength.val = _penisLength;
                _ui.penisWidth.val = _penisWidth;
                _ui.penisGirth.val = _penisGirth;
            }
        }

        private void UpdateMarkerLabels() {

            if(!_ui.showFeatureMarkerLabelsStorable.val) {
                return;
            }

            // update marker labels
            _markerHead.Label = "Head (Head To Heel "
                + $"{(int)(_ui.fullHeightStorable.val * 100)} cm / "
                + $"{UnitUtils.MetersToFeetString(_ui.fullHeightStorable.val)} / "
                + $"{_ui.heightInHeadsStorable.val:0.0} heads)";

            var chinToShoulder = _ui.chinHeightStorable.val - _ui.shoulderHeightStorable.val;
            _markerChin.Label = "Chin (Head Height "
                + $"{(int)(_ui.headSizeHeightStorable.val * 100)} cm / "
                + $"{UnitUtils.MetersToFeetString(_ui.headSizeHeightStorable.val)}; "
                + $"Width {(int)(_ui.headSizeWidthStorable.val * 100)} cm / {UnitUtils.MetersToFeetString(_ui.headSizeWidthStorable.val)})\n"
                + $"Chin To Shoulder "
                + $"{(int)(chinToShoulder * 100)} cm / "
                + $"{UnitUtils.MetersToFeetString(chinToShoulder)} / "
                + $"{chinToShoulder / _ui.headSizeHeightStorable.val:0.0} heads";

            if(_ui.breastBustStorable.val == 0) {
                _markerNipple.Label = "Nipple";
                _markerUnderbust.Label = "Underbust";
            }
            else {
                _markerNipple.Label = $"Nipple "
                + $"(Cup {_ui.breastBandStorable.val}{_ui.breastCupStorable.val}; "
                + $"Around {(int)(_ui.breastBustStorable.val * 100)} cm / "
                + $"{(int)UnitUtils.UnityToInches(_ui.breastBustStorable.val)} in)";

                _markerUnderbust.Label = "Underbust (Around "
                    + $"{(int)(_ui.breastUnderbustStorable.val * 100)} cm / "
                    + $"{(int)UnitUtils.UnityToInches(_ui.breastUnderbustStorable.val)} in)";
            }

            var shoulderToNavel = _ui.shoulderHeightStorable.val - _ui.navelHeightStorable.val;
            _markerNavel.Label = "Navel (Waist "
                + $"{(int)(_ui.waistSizeStorable.val * 100)} cm / "
                + $"{Mathf.RoundToInt(UnitUtils.UnityToInches(_ui.waistSizeStorable.val))} in)\n"
                + $"Shoulder to Navel "
                + $"{(int)(shoulderToNavel * 100)} cm / "
                + $"{UnitUtils.MetersToFeetString(shoulderToNavel)} / "
                + $"{shoulderToNavel / _ui.headSizeHeightStorable.val:0.0} heads";

            var shoulderToCrotch = _ui.shoulderHeightStorable.val - _ui.crotchHeightStorable.val;
            _markerGroin.Label = "Crotch (Hip "
                + $"{(int)(_ui.hipSizeStorable.val * 100)} cm / "
                + $"{Mathf.RoundToInt(UnitUtils.UnityToInches(_ui.hipSizeStorable.val))} in)\n";
            if(_dazCharacter.isMale) {
                _markerGroin.Label = _markerGroin.Label + $"Penis "
                    + $"Length {(int)(_ui.penisLength.val * 100)} cm / {UnitUtils.UnityToInches(_ui.penisLength.val):0.0} in, "
                    + $"Width {(int)(_ui.penisWidth.val * 100)} cm / {UnitUtils.UnityToInches(_ui.penisWidth.val):0.0} in, "
                    + $"Girth {(int)(_ui.penisGirth.val * 100)} cm / {UnitUtils.UnityToInches(_ui.penisGirth.val):0.0} in\n";
            }
            _markerGroin.Label = _markerGroin.Label + $"Shoulder to Crotch "
                + $"{(int)(shoulderToCrotch * 100)} cm / "
                + $"{UnitUtils.MetersToFeetString(shoulderToCrotch)} / "
                + $"{shoulderToCrotch / _ui.headSizeHeightStorable.val:0.0} heads";

            var crotchToKnee = _ui.crotchHeightStorable.val - _ui.kneeBottomHeightStorable.val;
            _markerKnee.Label = $"Knee Bottom (Crotch to Knee "
                + $"{(int)(crotchToKnee * 100)} cm / "
                + $"{UnitUtils.MetersToFeetString(crotchToKnee)} / "
                + $"{crotchToKnee / _ui.headSizeHeightStorable.val:0.0} heads)";

            _markerHeel.Label = $"Heel (Knee to Heel "
                + $"{(int)(_ui.kneeBottomHeightStorable.val * 100)} cm / "
                + $"{UnitUtils.MetersToFeetString(_ui.kneeBottomHeightStorable.val)} / "
                + $"{_ui.kneeBottomHeightStorable.val / _ui.headSizeHeightStorable.val:0.0} heads)";

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

            if(_ui.showTapeMarkersStorable.val) {
                var color = Color.red;
                if(_bustMarkersFromMorph.Count != _verticesBust.Length)
                {
                    foreach(var m in _bustMarkersFromMorph) {
                        Destroy(m);
                    }
                    _bustMarkersFromMorph.Clear();
                    foreach(var m in _verticesBust){
                        var marker = CreateMarker(color);
                        _bustMarkersFromMorph.Add(marker);
                    }
                    var a = SuperController.singleton.GetAtomByUid("Person");
                }

                for(var i = 0; i < _verticesBust.Length; i++) {
                    _bustMarkersFromMorph[i].transform.position = _verticesBust[i].Position(this);
                    var r = _bustMarkersFromMorph[i].GetComponent<Renderer>();
                    if(r) {
                        r.material.color = _colorWheel[i % _colorWheel.Length];
                    }
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

        readonly Color[] _colorWheel = new Color[] {
            Color.red,
            new Color(1, 0.5f, 0),
            Color.yellow,
            Color.green,
            Color.blue,
            new Color(0.5f, 0, 0.5f)
        };
        private void UpdateUnderbustMarkersFromMorphVertex() {
            if(Skin == null) {
                return;
            }

            if(_ui.showTapeMarkersStorable.val){
                var color = Color.white;
                if(_underbustMarkersFromMorph.Count != _verticesUnderbust.Length) {
                    foreach(var m in _underbustMarkersFromMorph) {
                        Destroy(m);
                    }
                    _underbustMarkersFromMorph.Clear();
                    foreach(var m in _verticesUnderbust){
                        _underbustMarkersFromMorph.Add(CreateMarker(color));
                    }
                }

                for(var i = 0; i < _verticesUnderbust.Length; i++) {
                    _underbustMarkersFromMorph[i].transform.position = _verticesUnderbust[i].Position(this);
                    var r = _underbustMarkersFromMorph[i].GetComponent<Renderer>();
                    if(r) {
                        r.material.color = _colorWheel[i % _colorWheel.Length];
                    }
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

            if(_ui.showTapeMarkersStorable.val){
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
                    var r = _waistMarkersFromMorph[i].GetComponent<Renderer>();
                    if(r) {
                        r.material.color = _colorWheel[i % _colorWheel.Length];
                    }
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

            if(_ui.showTapeMarkersStorable.val){
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
                    var r = _hipMarkersFromMorph[i].GetComponent<Renderer>();
                    if(r) {
                        r.material.color = _colorWheel[i % _colorWheel.Length];
                    }
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

        List<GameObject> _penisMarkersFromMorph = new List<GameObject>();
        float _penisLength = 0;
        float _penisGirth = 0;
        float _penisWidth = 0;
        private void UpdatePenisMarkers() {
            if(Skin == null) {
                return;
            }
            if(_dazCharacter == null){
                return;
            }

            if(!_dazCharacter.isMale) {
                foreach(var m in _penisMarkersFromMorph) {
                    Destroy(m);
                }
                _penisMarkersFromMorph.Clear();
                return;
            }

            var penisTipPos = _vertexPenisTip.Position(this);
            var penisBasePos = _vertexPenisBase.Position(this);
            var penisShaftLeftPos = _vertexPenisShaftLeft.Position(this);
            var penisShaftRightPos = _vertexPenisShaftRight.Position(this);

            if(_ui.showTapeMarkersStorable.val){
                if(_penisMarkersFromMorph.Count != 4) {
                    _penisMarkersFromMorph.Add(CreateMarker(Color.red));
                    _penisMarkersFromMorph.Add(CreateMarker(Color.red));
                    _penisMarkersFromMorph.Add(CreateMarker(Color.red));
                    _penisMarkersFromMorph.Add(CreateMarker(Color.red));
                }
                _penisMarkersFromMorph[0].transform.position = penisTipPos;
                _penisMarkersFromMorph[1].transform.position = penisBasePos;
                _penisMarkersFromMorph[2].transform.position = penisShaftLeftPos;
                _penisMarkersFromMorph[3].transform.position = penisShaftRightPos;
            }
            else {
                foreach(var m in _penisMarkersFromMorph) {
                    Destroy(m);
                }
                _penisMarkersFromMorph.Clear();
            }

            _penisLength = Vector3.Distance(penisTipPos, penisBasePos);
            _penisWidth = Vector3.Distance(penisShaftLeftPos, penisShaftRightPos);
            _penisGirth = Mathf.PI * _penisWidth;
        }

        readonly List<HorizontalMarker> _extraHeadMarkers = new List<HorizontalMarker>();
        private void UpdateHeadHeightMarkers() {
            Color headColor = HSVToColor(_ui.headMarkerColor.val);

            var height = _ui.fullHeightStorable.val;
            var headHeight = _ui.headSizeHeightStorable.val;
            var heightInHeadsRoundedUp = (int)Mathf.Ceil(_ui.heightInHeadsStorable.val);

            if(heightInHeadsRoundedUp != _extraHeadMarkers.Count) {
                if(heightInHeadsRoundedUp > _extraHeadMarkers.Count) {
                    for(var i = _extraHeadMarkers.Count; i < heightInHeadsRoundedUp; i++) {
                        var hm = gameObject.AddComponent<HorizontalMarker>();
                        hm.Name = $"Head{i}";
                        hm.Color = headColor;
                        hm.Thickness = _ui.lineThicknessHeadStorable.val;
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
                    pos.x += _ui.markerSpreadStorable.val - _ui.markerLeftRightStorable.val;
                    pos.z += _ui.markerFrontBackStorable.val;
                    pos.y -= headHeight * i;

                    _extraHeadMarkers[i].Origin = pos;
                    _extraHeadMarkers[i].Enabled = _ui.showHeadHeightMarkersStorable.val;
                    _extraHeadMarkers[i].Label = $"{i}";
                    _extraHeadMarkers[i].Thickness = _ui.lineThicknessHeadStorable.val * 0.001f;
                    _extraHeadMarkers[i].Color = headColor;
                }
            }
        }

        private CupSize GetCupInfo() {
            if(_dazCharacter.isMale) {
                return null;
            }
            var cupCalculator = cupCalculators.FirstOrDefault(cc => cc.Name.Equals(_ui.cupAlgorithmStorable.val));
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

        public HSVColor ColorToHSV(Color color) {
            float H, S, V;
            Color.RGBToHSV(color, out H, out S, out V);
            return new HSVColor { H = H, S = S, V = V };
        }

        public Color HSVToColor(HSVColor hsv) {
            return Color.HSVToRGB(hsv.H, hsv.S, hsv.V);
        }
    }
}
