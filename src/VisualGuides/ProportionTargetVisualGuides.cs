using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LFE {
    public class ProportionTargetVisualGuides : BaseVisualGuides {

        LabeledLine _markerHead;
        LabeledLine _markerChin;
        LabeledLine _markerShoulder;
        LabeledLine _markerNipple;
        LabeledLine _markerNavel;
        LabeledLine _markerGroin;
        LabeledLine _markerKnee;
        LabeledLine _markerHeel;

        public Proportions TargetProportion { get; set; } = null;

        public void Awake() {

            _markerHead = CreateLineMarker("Head", LineColor);
            _markerChin = CreateLineMarker("Chin", LineColor);
            _markerShoulder = CreateLineMarker("Shoulder", LineColor);
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

            var targetHeads = targetProportion.FigureHeightInHeads;

            var parentPos = transform.parent.transform.position;
            var parentRot = transform.parent.transform.rotation;
            var parentRotEuler = Quaternion.Euler(parentRot.eulerAngles);

            // set the head height to use for all unit conversions
            UnitUtils.HeadHeightInUnity = Measurements.Height ?? 0;

            var height = (Measurements.Height ?? 0) - (Measurements.HeelHeight ?? 0);
            var unitsPerHead = height / targetHeads;
            var proportionDisplayName = TargetProportion != null ? $"{targetProportion.ProportionName} Proportions" : $"{targetProportion.ProportionName}";
            var title = $"{proportionDisplayName} - Target {targetHeads} h.u.";
            _markerHead.Label = $"{title}";
            SetMainMarkerProperties(_markerHead, height);
            _markerHead.transform.position = parentRotEuler * (new Vector3(0, height, 0) + Offset) + parentPos;

            var headHeight = 1 * unitsPerHead;
            var chinHeight = height - headHeight;
            _markerChin.Label = $"Head Size";
            SetMainMarkerProperties(_markerChin, chinHeight);
            _markerChin.transform.position = parentRotEuler * (new Vector3(0, chinHeight, 0) + Offset) + parentPos;

            var shoulderHeight = chinHeight - (targetProportion.FigureChinToShoulder * unitsPerHead);
            _markerShoulder.Label = $"Chin To Shoulder - Target {targetProportion.FigureChinToShoulder} h.u.";
            SetMainMarkerProperties(_markerShoulder, shoulderHeight);
            _markerShoulder.transform.position = parentRotEuler * (new Vector3(0, shoulderHeight, 0) + Offset) + parentPos;

            var nippleHeight = shoulderHeight - (targetProportion.FigureShoulderToNipples * unitsPerHead);
            _markerNipple.Label = $"Shoulder To Nipple - Target {targetProportion.FigureShoulderToNipples} h.u.";
            SetMainMarkerProperties(_markerNipple, nippleHeight);
            _markerNipple.transform.position = parentRotEuler * (new Vector3(0, nippleHeight, 0) + Offset) + parentPos;

            var navelHeight = shoulderHeight - (targetProportion.FigureShoulderToNavel * unitsPerHead);
            _markerNavel.Label = $"Shoulder To Navel - Target {targetProportion.FigureShoulderToNavel} h.u.";
            SetMainMarkerProperties(_markerNavel, navelHeight);
            _markerNavel.transform.position = parentRotEuler * (new Vector3(0, navelHeight, 0) + Offset) + parentPos;

            var crotchHeight = shoulderHeight - (targetProportion.FigureShoulderToCrotch * unitsPerHead);
            _markerGroin.Label = $"Shoulder To Crotch - Target {targetProportion.FigureShoulderToNavel} h.u.";
            SetMainMarkerProperties(_markerGroin, crotchHeight);
            _markerGroin.transform.position = parentRotEuler * (new Vector3(0, crotchHeight, 0) + Offset) + parentPos;

            var kneeHeight = crotchHeight - (targetProportion.FigureCrotchToBottomOfKnees * unitsPerHead);
            _markerKnee.Label = $"Crotch To Knee - Target {targetProportion.FigureCrotchToBottomOfKnees} h.u.";
            SetMainMarkerProperties(_markerKnee, kneeHeight);
            _markerKnee.transform.position = parentRotEuler * (new Vector3(0, kneeHeight, 0) + Offset) + parentPos;

            _markerHeel.Label = $"Knee to Heel - Target {targetProportion.FigureBottomOfKneesToHeels}";
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