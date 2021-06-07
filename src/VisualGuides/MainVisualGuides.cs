using UnityEngine;
using System.Collections.Generic;

namespace LFE {
    public class MainVisualGuides : BaseVisualGuides {

        LabeledLine _markerHead;
        LabeledLine _markerChin;
        LabeledLine _markerShoulder;
        LabeledLine _markerNipple;
        LabeledLine _markerUnderbust;
        LabeledLine _markerNavel;
        LabeledLine _markerGroin;
        LabeledLine _markerKnee;
        LabeledLine _markerHeel;

        public void Awake() {

            _markerHead = CreateLineMarker("Head", LineColor);
            _markerChin = CreateLineMarker("Chin", LineColor);
            _markerShoulder = CreateLineMarker("Shoulder", LineColor);
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

            _markerHead.Label = $"Height - {UnitUtils.ToUnitString(Measurements.Height ?? 0, UnitDisplay)}";
            SetMainMarkerProperties(_markerHead, Measurements.Height);
            _markerHead.transform.position = parentRotEuler * (new Vector3(0, Measurements.Height ?? 0, 0) + Offset) + parentPos;

            var headWidth = Measurements.HeadWidth ?? 0;
            _markerChin.Label = $"Head - Height {UnitUtils.ToUnitString(Measurements.HeadHeight ?? 0, UnitDisplay)}";
            if(headWidth != 0) {
                _markerChin.Label += $" Width {UnitUtils.ToUnitString(headWidth, UnitDisplay)}";
            }
            SetMainMarkerProperties(_markerChin, Measurements.ChinHeight);
            _markerChin.transform.position = parentRotEuler * (new Vector3(0, Measurements.ChinHeight ?? 0, 0) + Offset) + parentPos;

            var chinToShoulder = (Measurements.ChinHeight ?? 0) - (Measurements.ShoulderHeight ?? 0);
            _markerShoulder.Label = $"Chin To Shoulder - {UnitUtils.ToUnitString(chinToShoulder, UnitDisplay)}";
            SetMainMarkerProperties(_markerShoulder, Measurements.ShoulderHeight);
            _markerShoulder.transform.position = parentRotEuler * (new Vector3(0, Measurements.ShoulderHeight ?? 0, 0) + Offset) + parentPos;

            if(Measurements.BustSize == null || Measurements.BustSize == 0) {
                _markerNipple.Label = $"Bust";
            }
            else {
                _markerNipple.Label = $"Bust - {UnitUtils.ToUnitString(Measurements.BustSize ?? 0, UnitDisplay)} - ({Measurements.BandSize}{Measurements.CupSize})";
            }
            SetMainMarkerProperties(_markerNipple, Measurements.NippleHeight);
            _markerNipple.transform.position = parentRotEuler * (new Vector3(0, Measurements.NippleHeight ?? 0, 0) + Offset) + parentPos;

            if(Measurements.UnderbustSize == null || Measurements.UnderbustSize == 0) {
                _markerUnderbust.Label = "Underbust";
            }
            else {
                _markerUnderbust.Label = $"Underbust - {UnitUtils.ToUnitString(Measurements.UnderbustSize ?? 0, UnitDisplay)}";
            }
            SetMainMarkerProperties(_markerUnderbust, Measurements.UnderbustHeight);
            _markerUnderbust.transform.position = parentRotEuler * (new Vector3(0, Measurements.UnderbustHeight ?? 0, 0) + Offset) + parentPos;

            var waist = Measurements.WaistSize ?? 0;
            var shoulderToNavel = (Measurements.ShoulderHeight?? 0) - (Measurements.NavelHeight ?? 0);
            _markerNavel.Label = "Navel";
            if(waist != 0) {
                _markerNavel.Label += $" - Waist {UnitUtils.ToUnitString(waist, UnitDisplay)}";
            }
            _markerNavel.Label += $"\nShoulder to Navel - {UnitUtils.ToUnitString(shoulderToNavel, UnitDisplay)}";
            SetMainMarkerProperties(_markerNavel, Measurements.NavelHeight);
            _markerNavel.transform.position = parentRotEuler * (new Vector3(0, Measurements.NavelHeight ?? 0, 0) + Offset) + parentPos;

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
            _markerGroin.transform.position = parentRotEuler * (new Vector3(0, Measurements.CrotchHeight ?? 0, 0) + Offset) + parentPos;

            var crotchToKnee = (Measurements.CrotchHeight ?? 0) - (Measurements.KneeHeight ?? 0);
            _markerKnee.Label = $"Knee - Crotch to Knee {UnitUtils.ToUnitString(crotchToKnee, UnitDisplay)}";
            SetMainMarkerProperties(_markerKnee, Measurements.KneeHeight);
            _markerKnee.transform.position = parentRotEuler * (new Vector3(0, Measurements.KneeHeight ?? 0, 0) + Offset) + parentPos;

            var kneeToHeel = (Measurements.KneeHeight ?? 0) - (Measurements.HeelHeight ?? 0);
            _markerHeel.Label = $"Heel - Knee to Heel {UnitUtils.ToUnitString(kneeToHeel, UnitDisplay)}";
            SetMainMarkerProperties(_markerHeel, Measurements.HeelHeight);
            _markerHeel.Enabled = Measurements.HeelHeight != null && Enabled;
            _markerHeel.transform.position = parentRotEuler * (new Vector3(0, Measurements.HeelHeight ?? 0, 0) + Offset) + parentPos;

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