using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LFE {
    public class ProportionTargetVisualGuides : BaseVisualGuides {

        LabeledLine _markerHead;
        LabeledLine _markerChin;
        LabeledLine _markerShoulder;
        LabeledLine _markerShoulderWidth;
        LabeledLine _markerArmLength;
        LabeledLine _markerNipple;
        LabeledLine _markerNavel;
        LabeledLine _markerGroin;
        LabeledLine _markerKnee;
        LabeledLine _markerHeel;

        public bool ShowHeight { get; set; } = true;
        public bool ShowChin { get; set; } = true;
        public bool ShowNipple { get; set; } = true;
        public bool ShowNavel { get; set; } = true;
        public bool ShowCrotch { get; set; } = true;
        public bool ShowKnee { get; set; } = true;
        public bool ShowHeel { get; set; } = true;
        public bool ShowShoulder { get; set; } = true;
        public bool ShowShoulderWidth { get; set; } = true;
        public bool ShowArm { get; set; } = true;

        public Proportions TargetProportion { get; set; } = null;

        public void Awake() {

            _markerHead = CreateLineMarker("Head", LineColor);
            _markerChin = CreateLineMarker("Chin", LineColor);
            _markerShoulder = CreateLineMarker("Shoulder", LineColor);
            _markerShoulderWidth = CreateLineMarker("ShoulderWidth", LineColor);
            _markerArmLength = CreateLineMarker("ArmLength", LineColor);
            _markerNipple = CreateLineMarker("Nipple", LineColor);
            _markerNavel = CreateLineMarker("Navel", LineColor);
            _markerGroin = CreateLineMarker("Crotch", LineColor);
            _markerKnee = CreateLineMarker("Knee", LineColor);
            _markerHeel = CreateLineMarker("Heel", LineColor);
        }

        bool _enabledPrev = false; // allows for performant disabling
        public void Update() {
            if(Measurements == null){
                return;
            }

            // allows for performant disabling with OnEnable/Disable
            // if disabling, the Update should still try and shut off
            // components one last time
            if(_enabledPrev == false && !Enabled) {
                return;
            }
            _enabledPrev = Enabled;

            UpdateMainMarkers();

        }

        public void OnDestroy() {
            foreach(var go in _lineMarkerGameObjects) {
                Destroy(go);
            }
        }


        public void UpdateMainMarkers() {
            var targetProportion = TargetProportion;

            if(targetProportion == null) {
                targetProportion = new Proportions();
                Enabled = false;
            }

            var offset = Offset;
            if(FlipDirection) {
                offset = new Vector3(offset.x * -1, offset.y, offset.z);
            }

            var targetHeads = targetProportion.FigureHeightInHeads;

            var parentPos = transform.parent.transform.position;
            var parentRot = transform.parent.transform.rotation;
            var parentRotEuler = Quaternion.Euler(parentRot.eulerAngles);

            // set the head height to use for all unit conversions
            UnitUtils.HeadHeightInUnity = Measurements.HeadHeight ?? 0;

            var height = (Measurements.Height ?? 0) - (Measurements.HeelHeight ?? 0);
            var unitsPerHead = height / targetHeads;
            var proportionDisplayName = TargetProportion != null ? $"{targetProportion.ProportionName} Proportions" : $"{targetProportion.ProportionName}";
            var title = $"{proportionDisplayName} - Target {targetHeads} h.u.";
            _markerHead.Label = $"{title}";
            SetMainMarkerProperties(_markerHead, height);
            _markerHead.transform.position = parentRotEuler * (new Vector3(0, height, 0) + offset) + parentPos;
            _markerHead.LineDirection = FlipDirection ? Vector3.right : Vector3.left;
            _markerHead.Enabled = _markerHead.Enabled && ShowHeight;

            var headHeight = 1 * unitsPerHead;
            var chinHeight = height - headHeight;
            _markerChin.Label = $"Head Size";
            if(ShowCalculate){
                _markerChin.Label += $" ({UnitUtils.ToUnitString(headHeight, UnitDisplay)})";
            }
            SetMainMarkerProperties(_markerChin, chinHeight);
            _markerChin.transform.position = parentRotEuler * (new Vector3(0, chinHeight, 0) + offset) + parentPos;
            _markerChin.LineDirection = FlipDirection ? Vector3.right : Vector3.left;
            _markerChin.Enabled = _markerChin.Enabled && ShowChin;

            var shoulderHeight = chinHeight - (targetProportion.FigureChinToShoulder * unitsPerHead);
            var shoulderWidth = targetProportion.FigureShoulderWidth * unitsPerHead;
            var shoulderLineBuffer = Mathf.Abs(offset.x) < Mathf.Abs(shoulderWidth/2) ? shoulderWidth/2 + 0.02f : 0;
            var neckHeight = targetProportion.FigureChinToShoulder * unitsPerHead;
            _markerShoulder.Label = $"Chin To Shoulder - Target {targetProportion.FigureChinToShoulder} h.u.";
            if(ShowCalculate) {
                _markerShoulder.Label += $" ({UnitUtils.ToUnitString(neckHeight, UnitDisplay)})";
            }
            _markerShoulder.Label += $" | Width Target {targetProportion.FigureShoulderWidth} h.u.";
            if(ShowCalculate){
                _markerShoulder.Label += $" ({UnitUtils.ToUnitString(shoulderWidth, UnitDisplay)})";
            }
            SetMainMarkerProperties(_markerShoulder, shoulderHeight);
            _markerShoulder.transform.position = parentRotEuler * (new Vector3(0, shoulderHeight, 0) + new Vector3(offset.x - (shoulderLineBuffer * (FlipDirection ? -1 : 1)), offset.y, offset.z)) + parentPos;
            _markerShoulder.LineDirection = FlipDirection ? Vector3.right : Vector3.left;
            _markerShoulder.Length = _markerChin.Length - shoulderLineBuffer;
            _markerShoulder.Enabled = _markerShoulder.Enabled && ShowShoulder;

            _markerShoulderWidth.Label = "";
            SetMainMarkerProperties(_markerShoulderWidth, shoulderWidth);
            _markerShoulderWidth.transform.position = parentRotEuler * (new Vector3(0, shoulderHeight, 0) + new Vector3(offset.x + (OffsetSpread.x + shoulderWidth/2) * (FlipDirection ? -1 : 1), offset.y, offset.z + 0.04f)) + parentPos;
            _markerShoulderWidth.LineDirection = FlipDirection ? Vector3.right : Vector3.left;
            _markerShoulderWidth.Length = shoulderWidth;
            _markerShoulderWidth.Enabled = _markerShoulderWidth.Enabled && ShowShoulderWidth;

            var armLength = targetProportion.FigureLengthOfUpperLimb * unitsPerHead;
            var actualShoulderWidth = Measurements.ShoulderWidth ?? 0;
            _markerArmLength.Label = $"Arm - Target {targetProportion.FigureLengthOfUpperLimb} h.u.";
            if(ShowCalculate){
                _markerArmLength.Label += $" ({UnitUtils.ToUnitString(armLength, UnitDisplay)})";
            }
            SetMainMarkerProperties(_markerArmLength, armLength);
            _markerArmLength.Length = armLength;
            _markerArmLength.transform.position = parentRotEuler * (new Vector3(0, Measurements.ShoulderHeight ?? 0, 0) + new Vector3(offset.x + (OffsetSpread.x + actualShoulderWidth/2 + 0.20f) * (FlipDirection ? -1 : 1), offset.y, offset.z)) + parentPos;
            _markerArmLength.LineDirection = Vector3.down;
            _markerArmLength.Enabled = _markerArmLength.Enabled && ShowArm;

            var shoulderToNipple = targetProportion.FigureShoulderToNipples * unitsPerHead;
            var nippleHeight = shoulderHeight - shoulderToNipple;
            _markerNipple.Label = $"Shoulder To Nipple - Target {targetProportion.FigureShoulderToNipples} h.u.";
            if(ShowCalculate){
                _markerNipple.Label += $" ({UnitUtils.ToUnitString(shoulderToNipple, UnitDisplay)})";
            }
            SetMainMarkerProperties(_markerNipple, nippleHeight);
            _markerNipple.transform.position = parentRotEuler * (new Vector3(0, nippleHeight, 0) + offset) + parentPos;
            _markerNipple.LineDirection = FlipDirection ? Vector3.right : Vector3.left;
            _markerNipple.Enabled = _markerNipple.Enabled && ShowNipple;

            var shoulderToNavel = targetProportion.FigureShoulderToNavel * unitsPerHead;
            var navelHeight = shoulderHeight - shoulderToNavel;
            _markerNavel.Label = $"Shoulder To Navel - Target {targetProportion.FigureShoulderToNavel} h.u.";
            if(ShowCalculate){
                _markerNavel.Label += $" ({UnitUtils.ToUnitString(shoulderToNavel, UnitDisplay)})";
            }
            SetMainMarkerProperties(_markerNavel, navelHeight);
            _markerNavel.transform.position = parentRotEuler * (new Vector3(0, navelHeight, 0) + offset) + parentPos;
            _markerNavel.LineDirection = FlipDirection ? Vector3.right : Vector3.left;
            _markerNavel.Enabled = _markerNavel.Enabled && ShowNavel;

            var shoulderToCroth = targetProportion.FigureShoulderToCrotch * unitsPerHead;
            var crotchHeight = shoulderHeight - shoulderToCroth;
            _markerGroin.Label = $"Shoulder To Crotch - Target {targetProportion.FigureShoulderToCrotch} h.u.";
            if(ShowCalculate){
                _markerGroin.Label += $" ({UnitUtils.ToUnitString(shoulderToCroth, UnitDisplay)})";
            }
            SetMainMarkerProperties(_markerGroin, crotchHeight);
            _markerGroin.transform.position = parentRotEuler * (new Vector3(0, crotchHeight, 0) + offset) + parentPos;
            _markerGroin.LineDirection = FlipDirection ? Vector3.right : Vector3.left;
            _markerGroin.Enabled = _markerGroin.Enabled && ShowCrotch;

            var crotchToKnee = targetProportion.FigureCrotchToBottomOfKnees * unitsPerHead;
            var kneeHeight = crotchHeight - crotchToKnee;
            _markerKnee.Label = $"Crotch To Knee - Target {targetProportion.FigureCrotchToBottomOfKnees} h.u.";
            if(ShowCalculate){
                _markerKnee.Label += $" ({UnitUtils.ToUnitString(crotchToKnee, UnitDisplay)})";
            }
            SetMainMarkerProperties(_markerKnee, kneeHeight);
            _markerKnee.transform.position = parentRotEuler * (new Vector3(0, kneeHeight, 0) + offset) + parentPos;
            _markerKnee.LineDirection = FlipDirection ? Vector3.right : Vector3.left;
            _markerKnee.Enabled = _markerKnee.Enabled && ShowKnee;

            var kneeToHeel = targetProportion.FigureBottomOfKneesToHeels * unitsPerHead;
            _markerHeel.Label = $"Knee to Heel - Target {targetProportion.FigureBottomOfKneesToHeels} h.u.";
            if(ShowCalculate){
                _markerHeel.Label += $" ({UnitUtils.ToUnitString(kneeToHeel, UnitDisplay)})";
            }
            var crotchToHeel = targetProportion.FigureLengthOfLowerLimb * unitsPerHead;
            _markerHeel.Label += $" | Crotch To Heel - Target {targetProportion.FigureLengthOfLowerLimb} h.u.";
            if(ShowCalculate){
                _markerHeel.Label += $" ({UnitUtils.ToUnitString(crotchToHeel, UnitDisplay)})";
            }
            SetMainMarkerProperties(_markerHeel, Measurements.HeelHeight);
            _markerHeel.Enabled = Measurements.HeelHeight != null && Enabled;
            _markerHeel.transform.position = parentRotEuler * (new Vector3(0, Measurements.HeelHeight ?? 0, 0) + offset) + parentPos;
            _markerHeel.LineDirection = FlipDirection ? Vector3.right : Vector3.left;
            _markerHeel.Enabled = _markerHeel.Enabled && ShowHeel;

        }

        private void SetMainMarkerProperties(LabeledLine marker, float? measurement) {
            marker.transform.rotation = transform.parent.transform.rotation;
            marker.Enabled = measurement != null && Enabled && measurement > 0;
            marker.LabelEnabled = LabelsEnabled;
            marker.LineEnabled = LinesEnabled;
            marker.Thickness = LineThickness * 0.001f;
            marker.Color = LineColor;
        }

        private readonly List<GameObject> _lineMarkerGameObjects = new List<GameObject>();
        private LabeledLine CreateLineMarker(string name, Color color) {
            var go = new GameObject();
            go.transform.SetParent(transform);
            _lineMarkerGameObjects.Add(go);
            var marker = go.AddComponent<LabeledLine>();
            marker.transform.SetParent(go.transform);
            marker.Name = name;
            marker.Color = color;
            marker.LineDirection = Vector3.left;
            return marker;
        }
    }
}