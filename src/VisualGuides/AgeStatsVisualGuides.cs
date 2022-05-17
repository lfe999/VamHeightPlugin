using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LFE {
    public class AgeStatsVisualGuides : BaseVisualGuides {

        List<LabeledLine> _heightAgeMarkers;
        List<LabeledLine> _headAgeMarkers;

        const int minAge = 2;
        const int maxAge = 25;
        const int maxIndex = maxAge-minAge;

        public void Awake() {
            LineColor = Color.cyan;
            _heightAgeMarkers = new List<LabeledLine>();
            _headAgeMarkers = new List<LabeledLine>();
            for(var i=0; i < maxIndex+1; i++) {
                _heightAgeMarkers.Add(
                    CreateLineMarker($"{i+minAge}", LineColor, Vector3.up)
                );
                _headAgeMarkers.Add(
                    CreateLineMarker($"{i+minAge}", LineColor, Vector3.up)
                );
            }
        }

        bool _enabledPrev = false; // allows for performant disabling
        public void Update()  {
            // SuperController.singleton.ClearMessages();
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

            var parentPos = transform.parent.transform.position;
            var parentRot = transform.parent.transform.rotation;
            var parentRotEuler = Quaternion.Euler(parentRot.eulerAngles);
            var halfHeadWidth = (Measurements.HeadWidth?? 0) / 2;
            var halfHeadWidthVector = new Vector3(halfHeadWidth, 0, 0);

            var barThickness = 0.02f;
            var barPadding   = 0.003f;
            var columnWidth  = barThickness + barPadding*2;
            var chartWidth   = _heightAgeMarkers.Count * columnWidth;

            var ageFromHeight = Measurements.AgeFromHeight;
            var ageFromHead = Measurements.AgeFromHead;
            if(ageFromHeight == null || ageFromHead == null) {
                Enabled = false;
            }

            // combine and weight confidences (row[0] is weighted, row[1] is height, row[2] is head)
            var combinedConfidenceByAge = new List<List<float>>();
            var confidenceMax = 0f;
            var confidenceMaxIndex = 0;

            // float weightHead = 1f;
            // float weightHeight = 1f;
            for(var i = 0; i < maxIndex; i++) {
                var age = i+minAge;

                var confidenceHeight = ageFromHeight?.ConfidenceForAge(age) ?? 0;
                var confidenceHead = ageFromHead?.ConfidenceForAge(age) ?? 0;
                float confidenceWeighted;

                // algorithm 1 - add things and weight them
                // var confidenceHeightW = confidenceHeight * weightHeight / (weightHead + weightHeight) / 2;
                // var confidenceHeadW = confidenceHead * weightHead / (weightHead + weightHeight) / 2;
                // var confidenceWeighted = confidenceHeadW + confidenceHeightW;

                // algorithm 2 - just take the max and use that
                if(confidenceHead > confidenceHeight) {
                    confidenceHead -= confidenceHeight;
                }
                else {
                    confidenceHead = 0;
                }

                confidenceWeighted = confidenceHead + confidenceHeight;
                if(confidenceWeighted > confidenceMax) {
                    confidenceMax = confidenceWeighted;
                    confidenceMaxIndex = i;
                }

                combinedConfidenceByAge.Add(new List<float>() {confidenceWeighted, confidenceHeight, confidenceHead});
            }

            // display things
            int firstNonZeroIndex = -1;
            for(var i = 0; i < maxIndex; i++) {
                var age = i+minAge;
                var confidenceCombined = combinedConfidenceByAge[i][0];
                var confidenceHeight = combinedConfidenceByAge[i][1];
                var confidenceHead = combinedConfidenceByAge[i][2];

                if(firstNonZeroIndex < 0 && confidenceCombined > 0) {
                    firstNonZeroIndex = i;
                }

                if(firstNonZeroIndex < 0) {
                    _heightAgeMarkers[i].Enabled = false;
                    _headAgeMarkers[i].Enabled = false;
                    continue;
                }

                var lineHeightForHeight = confidenceHeight == 0 ? 0.002f : confidenceHeight / 5f;
                var lineHeightForHead = confidenceHead == 0 ? 0.002f : confidenceHead / 5f;

                // render height bars
                SetMainMarkerProperties(_heightAgeMarkers[i], lineHeightForHeight);
                _heightAgeMarkers[i].Label = age < 18 && i == confidenceMaxIndex ? $"{age}yo\nWARNING!" : $"{age}yo";
                _heightAgeMarkers[i].LabelEnabled = i == firstNonZeroIndex || i == maxIndex - 1 || i == confidenceMaxIndex;
                if(i != confidenceMaxIndex) {
                    _heightAgeMarkers[i].Color = LineColor;
                }
                else {
                    _heightAgeMarkers[i].Color = (age < 18 ? Color.red : Color.yellow);
                }
                _heightAgeMarkers[i].Enabled = _heightAgeMarkers[i].Enabled && Enabled;
                _heightAgeMarkers[i].Length = lineHeightForHeight;
                _heightAgeMarkers[i].Thickness = barThickness;
                _heightAgeMarkers[i].transform.position = parentRotEuler * (Offset + new Vector3(-1*(columnWidth*(i-confidenceMaxIndex)), (Measurements.Height ?? 0) + 0.2f, 0)) + parentPos;

                // render height bars (stacked)
                SetMainMarkerProperties(_headAgeMarkers[i], lineHeightForHead);
                if(i != confidenceMaxIndex) {
                    _headAgeMarkers[i].Color = LineColor + Color.grey;
                }
                else {
                    _headAgeMarkers[i].Color = (age < 18 ? Color.red : Color.yellow) + Color.grey;
                }
                _headAgeMarkers[i].Enabled = _headAgeMarkers[i].Enabled && Enabled;
                _headAgeMarkers[i].Label = $""; // TODO
                _headAgeMarkers[i].Length = lineHeightForHead;
                _headAgeMarkers[i].Thickness = barThickness;
                _headAgeMarkers[i].LabelEnabled = false; // TODO
                _headAgeMarkers[i].transform.position = parentRotEuler * (Offset + new Vector3(-1*(columnWidth*(i-confidenceMaxIndex)), (Measurements.Height ?? 0) + 0.2f + lineHeightForHeight, 0)) + parentPos;

                
            }
        }

        public void OnDestroy() {
            foreach(var go in _lineMarkerGameObjects) {
                Destroy(go);
            }
        }

        private void SetMainMarkerProperties(LabeledLine marker, float? measurement) {
            marker.transform.rotation = transform.parent.transform.rotation;
            marker.Enabled = measurement != null && Enabled;
            marker.LabelEnabled = LabelsEnabled;
            marker.Thickness = LineThickness * 0.001f;
            marker.Color = LineColor;
        }

        private readonly List<GameObject> _lineMarkerGameObjects = new List<GameObject>();
        private LabeledLine CreateLineMarker(string name, Color color, Vector3 direction) {
            var go = new GameObject();
            go.transform.SetParent(transform);
            _lineMarkerGameObjects.Add(go);
            var marker = go.AddComponent<LabeledLine>();
            marker.transform.SetParent(go.transform);
            marker.Name = name;
            marker.Color = color;
            marker.LineDirection = direction;
            return marker;
        }
    }
}