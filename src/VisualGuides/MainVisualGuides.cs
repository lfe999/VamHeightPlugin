using UnityEngine;
using System.Collections.Generic;

namespace LFE {
    public class MainVisualGuides : BaseVisualGuides {

        LabeledLine _markerHead;
        LabeledLine _markerChin;
        LabeledLine _markerShoulder;
        LabeledLine _markerShoulderWidth;
        LabeledLine _markerArmLength;
        LabeledLine _markerNipple;
        LabeledLine _markerUnderbust;
        LabeledLine _markerNavel;
        LabeledLine _markerGroin;
        LabeledLine _markerKnee;
        LabeledLine _markerHeel;

        public bool ShowHeight { get; set; } = true;
        public bool ShowChin { get; set; } = true;
        public bool ShowBust { get; set; } = true;
        public bool ShowUnderbust { get; set; } = true;
        public bool ShowNavel { get; set; } = true;
        public bool ShowCrotch { get; set; } = true;
        public bool ShowKnee { get; set; } = true;
        public bool ShowHeel { get; set; } = true;
        public bool ShowShoulder { get; set; } = true;
        public bool ShowShoulderWidth { get; set; } = true;
        public bool ShowArm { get; set; } = true;

        public void Awake() {

            _markerHead = CreateLineMarker("Head", LineColor);
            _markerChin = CreateLineMarker("Chin", LineColor);
            _markerShoulder = CreateLineMarker("Shoulder", LineColor);
            _markerShoulderWidth = CreateLineMarker("ShoulderWidth", LineColor);
            _markerArmLength = CreateLineMarker("ArmLength", LineColor);
            _markerNipple = CreateLineMarker("Nipple", LineColor);
            _markerUnderbust = CreateLineMarker("Underbust", LineColor);
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
            var parentPos = transform.parent.transform.position;
            var parentRot = transform.parent.transform.rotation;
            var parentRotEuler = Quaternion.Euler(parentRot.eulerAngles);

            // set the head height to use for all unit conversions
            UnitUtils.HeadHeightInUnity = Measurements.HeadHeight ?? 0;

            var offset = Offset;
            if(FlipDirection) {
                offset = new Vector3(offset.x * -1f, offset.y, offset.z);
            }

            _markerHead.Label = $"Height - {UnitUtils.ToUnitString(Measurements.Height ?? 0, UnitDisplay)}";
            SetMainMarkerProperties(_markerHead, Measurements.Height);
            _markerHead.transform.position = parentRotEuler * (new Vector3(0, Measurements.Height ?? 0, 0) + offset) + parentPos;
            _markerHead.LineDirection = FlipDirection ? Vector3.right: Vector3.left;
            _markerHead.Enabled = _markerHead.Enabled && ShowHeight;

            var headWidth = Measurements.HeadWidth ?? 0;
            _markerChin.Label = $"Head - Height {UnitUtils.ToUnitString(Measurements.HeadHeight ?? 0, UnitDisplay)}";
            if(headWidth != 0) {
                _markerChin.Label += $" Width {UnitUtils.ToUnitString(headWidth, UnitDisplay)}";
            }
            SetMainMarkerProperties(_markerChin, Measurements.ChinHeight);
            _markerChin.transform.position = parentRotEuler * (new Vector3(0, Measurements.ChinHeight ?? 0, 0) + offset) + parentPos;
            _markerChin.LineDirection = FlipDirection ? Vector3.right: Vector3.left;
            _markerChin.Enabled = _markerChin.Enabled && ShowChin;

            var chinToShoulder = (Measurements.ChinHeight ?? 0) - (Measurements.ShoulderHeight ?? 0);
            var shoulderWidth = Measurements.ShoulderWidth ?? 0;
            var shoulderLineBuffer = 0;
            // var shoulderLineBuffer = _markerChin.Length;
            _markerShoulder.Label = $"Chin To Shoulder - {UnitUtils.ToUnitString(chinToShoulder, UnitDisplay)} Width - {UnitUtils.ToUnitString(shoulderWidth, UnitDisplay)}";
            SetMainMarkerProperties(_markerShoulder, Measurements.ShoulderHeight);
            _markerShoulder.transform.position = parentRotEuler * (new Vector3(0, Measurements.ShoulderHeight ?? 0, 0) + new Vector3(offset.x - shoulderLineBuffer, offset.y, offset.z)) + parentPos;
            _markerShoulder.LineDirection = FlipDirection ? Vector3.right: Vector3.left;
            _markerShoulder.Length = _markerChin.Length - shoulderLineBuffer;
            _markerShoulder.Enabled = _markerShoulder.Enabled && ShowShoulder;

            _markerShoulderWidth.Label = "";
            SetMainMarkerProperties(_markerShoulderWidth, Measurements.ShoulderWidth);
            _markerShoulderWidth.Length = shoulderWidth;
            _markerShoulderWidth.transform.position = parentRotEuler * (new Vector3(0, Measurements.ShoulderHeight ?? 0, 0) + new Vector3(offset.x + (OffsetSpread.x - shoulderWidth/2) * (FlipDirection ? -1 : 1), offset.y, offset.z + 0.04f)) + parentPos;
            _markerShoulderWidth.LineDirection = FlipDirection ? Vector3.left : Vector3.right;
            _markerShoulderWidth.Enabled = _markerShoulderWidth.Enabled && ShowShoulderWidth;

            var armLength = Measurements.ArmLength ?? 0;
            _markerArmLength.Label = $"Arm - {UnitUtils.ToUnitString(armLength, UnitDisplay)}";
            SetMainMarkerProperties(_markerArmLength, armLength);
            _markerArmLength.Length = armLength;
            _markerArmLength.transform.position = parentRotEuler * (new Vector3(0, Measurements.ShoulderHeight ?? 0, 0) + new Vector3(offset.x + (OffsetSpread.x + shoulderWidth/2 + 0.15f) * (FlipDirection ? -1 : 1), offset.y, offset.z)) + parentPos;
            _markerArmLength.LineDirection = Vector3.down;
            _markerArmLength.Enabled = _markerArmLength.Enabled && ShowArm;

            if(Measurements.BustSize == null || Measurements.BustSize == 0) {
                _markerNipple.Label = $"Bust";
            }
            else {
                _markerNipple.Label = $"Bust - {UnitUtils.ToUnitString(Measurements.BustSize ?? 0, UnitDisplay)} - ({Measurements.BandSize}{Measurements.CupSize})";
            }
            SetMainMarkerProperties(_markerNipple, Measurements.NippleHeight);
            _markerNipple.transform.position = parentRotEuler * (new Vector3(0, Measurements.NippleHeight ?? 0, 0) + offset) + parentPos;
            _markerNipple.LineDirection = FlipDirection ? Vector3.right: Vector3.left;
            _markerNipple.Enabled = _markerNipple.Enabled && ShowBust;

            if(Measurements.UnderbustSize == null || Measurements.UnderbustSize == 0) {
                _markerUnderbust.Label = "Underbust";
            }
            else {
                _markerUnderbust.Label = $"Underbust - {UnitUtils.ToUnitString(Measurements.UnderbustSize ?? 0, UnitDisplay)}";
            }
            SetMainMarkerProperties(_markerUnderbust, Measurements.UnderbustHeight);
            _markerUnderbust.transform.position = parentRotEuler * (new Vector3(0, Measurements.UnderbustHeight ?? 0, 0) + offset) + parentPos;
            _markerUnderbust.LineDirection = FlipDirection ? Vector3.right: Vector3.left;
            _markerUnderbust.Enabled = _markerUnderbust.Enabled && ShowUnderbust;

            var waist = Measurements.WaistSize ?? 0;
            var shoulderToNavel = (Measurements.ShoulderHeight?? 0) - (Measurements.NavelHeight ?? 0);
            _markerNavel.Label = "Navel";
            if(waist != 0) {
                _markerNavel.Label += $" - Waist {UnitUtils.ToUnitString(waist, UnitDisplay)}";
            }
            _markerNavel.Label += $"\nShoulder to Navel - {UnitUtils.ToUnitString(shoulderToNavel, UnitDisplay)}";
            SetMainMarkerProperties(_markerNavel, Measurements.NavelHeight);
            _markerNavel.transform.position = parentRotEuler * (new Vector3(0, Measurements.NavelHeight ?? 0, 0) + offset) + parentPos;
            _markerNavel.LineDirection = FlipDirection ? Vector3.right: Vector3.left;
            _markerNavel.Enabled = _markerNavel.Enabled && ShowNavel;

            var hip = Measurements.HipSize ?? 0;
            var shoulderToCrotch = (Measurements.ShoulderHeight ?? 0) - (Measurements.CrotchHeight ?? 0);
            _markerGroin.Label = $"Crotch";
            if(hip != 0) {
                _markerGroin.Label += $" - Hip {UnitUtils.ToUnitString(hip, UnitDisplay)}";
            }
            _markerGroin.Label += "\n";
            if(Measurements.POI?.IsMale ?? false) {
                _markerGroin.Label = _markerGroin.Label + $"Penis "
                    + $"Length {UnitUtils.ToUnitString(Measurements.PenisLength ?? 0, UnitDisplay)}, "
                    + $"Width {UnitUtils.ToUnitString(Measurements.PenisWidth ?? 0, UnitDisplay)}, "
                    + $"Girth {UnitUtils.ToUnitString(Measurements.PenisGirth ?? 0, UnitDisplay)}\n";
            }
            _markerGroin.Label = _markerGroin.Label + $"Shoulder to Crotch - {UnitUtils.ToUnitString(shoulderToCrotch, UnitDisplay)}";
            SetMainMarkerProperties(_markerGroin, Measurements.CrotchHeight);
            _markerGroin.transform.position = parentRotEuler * (new Vector3(0, Measurements.CrotchHeight ?? 0, 0) + offset) + parentPos;
            _markerGroin.LineDirection = FlipDirection ? Vector3.right: Vector3.left;
            _markerGroin.Enabled = _markerGroin.Enabled && ShowCrotch;

            var crotchToKnee = (Measurements.CrotchHeight ?? 0) - (Measurements.KneeHeight ?? 0);
            _markerKnee.Label = $"Knee - Crotch to Knee {UnitUtils.ToUnitString(crotchToKnee, UnitDisplay)}";
            SetMainMarkerProperties(_markerKnee, Measurements.KneeHeight);
            _markerKnee.transform.position = parentRotEuler * (new Vector3(0, Measurements.KneeHeight ?? 0, 0) + offset) + parentPos;
            _markerKnee.LineDirection = FlipDirection ? Vector3.right: Vector3.left;
            _markerKnee.Enabled = _markerKnee.Enabled && ShowKnee;

            var kneeToHeel = (Measurements.KneeHeight ?? 0) - (Measurements.HeelHeight ?? 0);
            _markerHeel.Label = $"Heel - Knee to Heel {UnitUtils.ToUnitString(kneeToHeel, UnitDisplay)}";
            SetMainMarkerProperties(_markerHeel, Measurements.HeelHeight);
            _markerHeel.transform.position = parentRotEuler * (new Vector3(0, Measurements.HeelHeight ?? 0, 0) + offset) + parentPos;
            _markerHeel.LineDirection = FlipDirection ? Vector3.right: Vector3.left;
            _markerHeel.Enabled = _markerHeel.Enabled && ShowHeel;

        }

        private void SetMainMarkerProperties(LabeledLine marker, float? measurement) {
            marker.transform.rotation = transform.parent.transform.rotation;
            marker.Enabled = measurement != null && Enabled && measurement > 0;
            marker.LabelEnabled = LabelsEnabled;
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