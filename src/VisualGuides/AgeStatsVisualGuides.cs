using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LFE {
    public class AgeStatsVisualGuides : BaseVisualGuides {

        List<List<LabeledLine>> _quartileMarkerRows;

        const int minAge = 2;
        const int maxAge = 25;

        public Proportions TargetProportion { get; set; }

        public void Awake() {
            LineColor = Color.cyan;
            _quartileMarkerRows = new List<List<LabeledLine>>();
        }

        bool _enabledPrev = false; // allows for performant disabling
        public void Update()  {
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

            var ageFromHeight = Measurements.AgeFromHeight;
            var ageFromHead = Measurements.AgeFromHead;
            // if(ageFromHeight == null || ageFromHead == null) {
            //     Enabled = false;
            // }

            // hide all exiting markers
            for(var i=0; i < _quartileMarkerRows.Count; i++){
                var row = _quartileMarkerRows[i];
                for(var j=0; j<row.Count; j++) {
                    row[j].Enabled = false;
                }
            }

            // (re)display relevant markers
            bool showBoxplot = true;
            if(showBoxplot) {
                // SuperController.singleton.ClearMessages();
                var heightQuartiles = ageFromHeight?.Quartiles;
                var headQuartiles = ageFromHead?.Quartiles;
                var proportionQuartiles = TargetProportion?.Quartiles;

                var quartiles = new List<Quartiles>() { heightQuartiles, headQuartiles, proportionQuartiles };
                var overlapQuartiles = Quartiles.GroupOverlapQuartile(quartiles);

                int? minAge = (int?)Quartiles.GroupMin(quartiles);
                int? maxAge = (int?)Quartiles.GroupMax(quartiles);

                var width = 0.5f;

                int currentRow = 0;
                var height = UnitUtils.ToUnitString(Measurements.Height??0, UnitDisplay);
                if(RenderRow(currentRow, heightQuartiles, width, minAge.Value, maxAge.Value, LineColor, transform, ShowDocumentation ? $"Height Age Guess {heightQuartiles.RangeString} ({height}) From CDC Growth Charts - Caution: UNRELIABLE" : $"Height Age Guess {heightQuartiles.RangeString} ({height})")) {
                    currentRow++;
                }
                var heightInHeads = (Measurements.Height??0) / (Measurements.HeadHeight??1);
                if(RenderRow(currentRow, headQuartiles, width, minAge.Value, maxAge.Value, LineColor, transform, ShowDocumentation ? $"Head Proportion Age Guess {headQuartiles.RangeString} ({heightInHeads:0.0} heads) From CDC Growth Charts - Caution: UNRELIABLE" : $"Head Proportion Age Guess {headQuartiles.RangeString} ({heightInHeads:0.0} heads)")) {
                    currentRow++;
                }
                var proportionName = TargetProportion?.ProportionName ?? "none";
                if(RenderRow(currentRow, proportionQuartiles, width, minAge.Value, maxAge.Value, LineColor, transform, ShowDocumentation ? $"Body Proportion Age Guess {proportionQuartiles.RangeString} ({proportionName}) From anatomy4sculptors.com Or User Created Custom Proportions - Caution: UNRELIABLE" : $"Body Proportion Age Guess {proportionQuartiles.RangeString} ({proportionName})")) {
                    currentRow++;
                }
                if(RenderRow(currentRow, overlapQuartiles, width, minAge.Value, maxAge.Value, Color.yellow, transform, ShowDocumentation ? $"Age Guess {overlapQuartiles.RangeString} - Caution: UNRELIABLE" : $"Age Guess {overlapQuartiles.RangeString}")) {
                    currentRow++;
                }
            }
        }

        private bool RenderRow(int rowId, Quartiles quartile, float width, float min, float max, Color lineColor, Transform transform, string description) {
            var parentPos = transform.parent.transform.position;
            var parentRot = transform.parent.transform.rotation;
            var parentRotEuler = Quaternion.Euler(parentRot.eulerAngles);

            var lengthPerAge = width/(Mathf.Abs(min - max)+1);
            var zOffSet = 0.2f + (rowId*0.05f) + (rowId*0.01f);
            var xOffSet = Mathf.Abs(quartile.Q0 - min) * lengthPerAge;

            if(quartile == null) {
                return false;
            }

            while(_quartileMarkerRows.Count <= rowId) {
                var rowMarkers = new List<LabeledLine>();
                for(var i = 0; i < 5; i++) {
                    rowMarkers.Add(
                        CreateLineMarker($"", lineColor, Vector3.right)
                    );
                }
                _quartileMarkerRows.Add(rowMarkers);
            }

            var markers = _quartileMarkerRows[rowId];

            // thin 0-25 quartile
            markers[0].Color = lineColor;
            markers[0].Enabled = Enabled;
            markers[0].Length = Mathf.Abs(quartile.Q0 - quartile.Q25) * lengthPerAge;
            markers[0].Thickness = 0.003f;
            markers[0].transform.position = parentRotEuler * (Offset + new Vector3((width/2) - xOffSet - (Mathf.Abs(quartile.Q0 - quartile.Q25)*lengthPerAge), (Measurements.Height ?? 0) + zOffSet, 0)) + parentPos;
            markers[0].Label = quartile.Q0.ToString();
            markers[0].LabelEnabled = Enabled;

            // thick 25-75 quartile
            markers[1].Color = lineColor;
            markers[1].Enabled = Enabled;
            markers[1].Length = (Mathf.Abs(quartile.Q25 - quartile.Q75) + 1) * lengthPerAge;
            markers[1].Thickness = 0.05f;
            markers[1].transform.position = parentRotEuler * (Offset + new Vector3((width/2) - xOffSet - ((Mathf.Abs(quartile.Q0 - quartile.Q75)+1)*lengthPerAge), (Measurements.Height ?? 0) + zOffSet, 0)) + parentPos;

            // // median vertical
            // markers[2].Enabled = Enabled;
            // markers[2].Length = lengthPerAge;
            // markers[2].Thickness = 0.05f;
            // markers[2].transform.position = parentRotEuler * (Offset + new Vector3((width/2) - xOffSet - ((Mathf.Abs(heightQuartiles.Q0 - heightQuartiles.Q50)+1) * lengthPerAge), (Measurements.Height ?? 0) + zOffSet, 0.001f)) + parentPos;
            // markers[2].Color = Color.yellow;
            // markers[2].Label = heightQuartiles.Q50.ToString();
            // markers[2].LabelEnabled = Enabled;
            // markers[2].LabelOffsetX = 24f;
            // markers[2].LabelOffsetY = -25f;

            // thin 0-25 quartile
            markers[3].Color = lineColor;
            markers[3].Enabled = Enabled;
            markers[3].Length = Mathf.Abs(quartile.Q75 - quartile.Q100) * lengthPerAge;
            markers[3].Thickness = 0.003f;
            markers[3].transform.position = parentRotEuler * (Offset + new Vector3((width/2) - xOffSet - ((Mathf.Abs(quartile.Q0 - quartile.Q75)+1) * lengthPerAge), (Measurements.Height ?? 0) + zOffSet, 0)) + parentPos;
            markers[3].Label = quartile.Q100.ToString();
            markers[3].LineDirection = Vector3.left;
            markers[3].LabelEnabled = Mathf.Abs(quartile.Q0 - quartile.Q100) > 0 ? Enabled : false;

            // description
            if(description != null && description != string.Empty) {
                markers[4].Color = lineColor;
                markers[4].Enabled = Enabled;
                markers[4].Length = 0f;
                markers[4].Thickness = 0.003f;
                markers[4].transform.position = parentRotEuler * (Offset + new Vector3((width/2) - width - 3*lengthPerAge, (Measurements.Height ?? 0) + zOffSet, 0)) + parentPos;
                markers[4].Label = description;
                markers[4].LineDirection = Vector3.left;
                markers[4].LabelEnabled = Enabled;
            }

            return true;
        }

        public void OnDestroy() {
            foreach(var go in _lineMarkerGameObjects) {
                Destroy(go);
            }
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