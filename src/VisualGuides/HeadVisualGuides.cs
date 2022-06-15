using UnityEngine;
using System.Collections.Generic;

namespace LFE {
    public class HeadVisualGuides : BaseVisualGuides {

        public void Awake() {
            LineColor = Color.white;
        }

        List<LabeledLine> _headMarkers = new List<LabeledLine>();
        List<GameObject> _lineMarkerGameObjects = new List<GameObject>();
        bool _enabledPrev = false;
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

            var height = Measurements.Height ?? 0;
            var headHeight = Measurements.HeadHeight ?? 0;
            var heightInHeads = (float)(height/headHeight);
            var heightInHeadsRoundedUp = headHeight == 0 ? 0 : (int)Mathf.Ceil(heightInHeads);

            // make sure we have enough marker objects created - we don't delete them, but use this more like an object pool
            if(heightInHeadsRoundedUp > _headMarkers.Count) {
                for(var i = _headMarkers.Count - 1; i < heightInHeadsRoundedUp; i++) {
                    var go = new GameObject();
                    go.transform.SetParent(transform);
                    var hm = go.AddComponent<LabeledLine>();
                    _lineMarkerGameObjects.Add(go);
                    hm.transform.SetParent(go.transform);
                    hm.Name = $"Head Marker";
                    hm.Color = LineColor;
                    hm.Thickness = LineThickness;
                    hm.LineDirection = Vector3.right;
                    _headMarkers.Add(hm);
                }
            }

            var offset = Offset;

            if(FlipDirection) {
                offset = new Vector3(offset.x * -1, offset.y, offset.z);
            }

            for(var i = 0; i < _headMarkers.Count; i++) {
                var parentPos = transform.parent.transform.position;
                var parentRot = transform.parent.transform.rotation;
                var parentRotEuler = Quaternion.Euler(parentRot.eulerAngles);

                var marker = _headMarkers[i];
                if(i <= heightInHeadsRoundedUp) {
                    marker.Enabled = Enabled;
                    marker.LabelEnabled = LabelsEnabled;
                    marker.Thickness = LineThickness * 0.001f;
                    marker.Color = LineColor;
                    marker.LineDirection = FlipDirection ? Vector3.left : Vector3.right;

                    marker.transform.rotation = transform.parent.transform.rotation;

                    float lineHeight;
                    if(i == heightInHeadsRoundedUp) {
                        lineHeight = 0;
                        marker.Label = $"{heightInHeads:0.0}";
                    }
                    else {
                        lineHeight = height - (headHeight * i);
                        marker.Label = $"{i}";

                        if(i == heightInHeadsRoundedUp - 1) {
                            // if we are close to the bottom, hide this label to make room
                            if((heightInHeadsRoundedUp - heightInHeads) > 0.8f ) {
                                marker.Label = $"";
                            }
                        }
                    }
                    marker.transform.position = parentRotEuler * (new Vector3(0, lineHeight, 0) + offset) + parentPos;
                }
                else {
                    marker.Enabled = false;
                }
            }

        }

        public void OnDestroy() {
            foreach(var m in _lineMarkerGameObjects) {
                Destroy(m);
            }
            _lineMarkerGameObjects = new List<GameObject>();
            _headMarkers = new List<LabeledLine>();
        }
    }
}