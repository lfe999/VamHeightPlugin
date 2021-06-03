using UnityEngine;
using System.Collections.Generic;

namespace LFE {
    public class FaceVisualGuides : BaseVisualGuides {

        LabeledLine _markerEyeMidHeight;
        LabeledLine _markerEyeRightOuter;
        LabeledLine _markerEyeLeftOuter;
        LabeledLine _markerMouthMidHeight;
        LabeledLine _markerMouthLeft;
        LabeledLine _markerMouthRight;
        LabeledLine _markerNoseBottomHeight;
        LabeledLine _markerChinSmall;
        LabeledLine _markerHeadSmall;
        LabeledLine _markerHeadLeft;
        LabeledLine _markerHeadRight;
        LabeledLine _markerFaceCenter;

        public void Awake() {
            LineColor = Color.blue;

            _markerEyeMidHeight = CreateLineMarker("Eye Height", LineColor, Vector3.left);
            _markerEyeRightOuter = CreateLineMarker("Eye Right Outer", LineColor, Vector3.up);
            _markerEyeLeftOuter = CreateLineMarker("Eye Left Outer", LineColor, Vector3.up);
            _markerNoseBottomHeight = CreateLineMarker("Nose Bottom Height", LineColor, Vector3.left);
            _markerMouthMidHeight = CreateLineMarker("Mouth Height", LineColor, Vector3.left);
            _markerMouthLeft = CreateLineMarker("Mouth Left", LineColor, Vector3.up);
            _markerMouthRight = CreateLineMarker("Mouth Right", LineColor, Vector3.up);
            _markerChinSmall = CreateLineMarker("Chin Small", LineColor, Vector3.left);
            _markerHeadSmall = CreateLineMarker("Head Small", LineColor, Vector3.left);
            _markerHeadRight = CreateLineMarker("Head Right", LineColor, Vector3.up);
            _markerHeadLeft = CreateLineMarker("Head Left", LineColor, Vector3.up);
            _markerFaceCenter = CreateLineMarker("Face Center", LineColor, Vector3.up);
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

            var parentPos = transform.parent.transform.position;
            var parentRot = transform.parent.transform.rotation;
            var parentRotEuler = Quaternion.Euler(parentRot.eulerAngles);

            var halfHeadHeight = (Measurements.HeadHeight ?? 0) / 2;
            var halfHeadWidth = (Measurements.HeadWidth?? 0) / 2;

            var halfHeadWidthVector = new Vector3(halfHeadWidth, 0, 0);
            var halfHeadHeightVector = new Vector3(0, halfHeadHeight, 0);
            var headCenterVector = new Vector3(0, (Measurements.Height ?? 0) - halfHeadHeight, 0);
             
            // eye midline
            SetMainMarkerProperties(_markerEyeMidHeight, Measurements.EyesHeight);
            _markerEyeMidHeight.Length = Measurements.HeadWidth ?? 0;
            _markerEyeMidHeight.transform.position = parentRotEuler * (Offset + new Vector3(0, Measurements.EyesHeight ?? 0, 0) + halfHeadWidthVector) + parentPos;

            // eye right outer
            SetMainMarkerProperties(_markerEyeRightOuter, Measurements.EyesWidth);
            _markerEyeRightOuter.Length = halfHeadWidth / 2;
            _markerEyeRightOuter.transform.position = parentRotEuler * (Offset + new Vector3((Measurements.EyesWidth ?? 0) / 2, (Measurements.EyesHeight ?? 0) - (_markerEyeRightOuter.Length / 2), 0)) + parentPos;

            // eye left outer
            SetMainMarkerProperties(_markerEyeLeftOuter, Measurements.EyesWidth);
            _markerEyeLeftOuter.Length = halfHeadWidth / 2;
            _markerEyeLeftOuter.transform.position = parentRotEuler * (Offset + new Vector3(-1 * (Measurements.EyesWidth ?? 0) / 2, (Measurements.EyesHeight ?? 0) - (_markerEyeLeftOuter.Length / 2), 0)) + parentPos;

            // nose bottom
            SetMainMarkerProperties(_markerNoseBottomHeight, Measurements.NoseHeight);
            _markerNoseBottomHeight.Length = Measurements.HeadWidth ?? 0;
            _markerNoseBottomHeight.transform.position = parentRotEuler * (Offset + new Vector3(0, Measurements.NoseHeight ?? 0, 0) + halfHeadWidthVector) + parentPos;

            // mouth middle
            SetMainMarkerProperties(_markerMouthMidHeight, Measurements.MouthHeight);
            _markerMouthMidHeight.Length = Measurements.HeadWidth ?? 0;
            _markerMouthMidHeight.transform.position = parentRotEuler * (Offset + new Vector3(0, Measurements.MouthHeight ?? 0, 0) + halfHeadWidthVector) + parentPos;

            // mouth left
            SetMainMarkerProperties(_markerMouthLeft, Measurements.MouthWidth);
            _markerMouthLeft.Length = halfHeadWidth / 4;
            _markerMouthLeft.transform.position = parentRotEuler * (Offset + new Vector3((Measurements.MouthWidth?? 0) / 2, (Measurements.MouthHeight ?? 0) - (_markerMouthLeft.Length / 2), 0)) + parentPos;

            // mouth right
            SetMainMarkerProperties(_markerMouthRight, Measurements.MouthWidth);
            _markerMouthRight.Length = halfHeadWidth / 4;
            _markerMouthRight.transform.position = parentRotEuler * (Offset + new Vector3(-1 * (Measurements.MouthWidth?? 0) / 2, (Measurements.MouthHeight ?? 0) - (_markerMouthRight.Length / 2), 0)) + parentPos;

            // chin
            SetMainMarkerProperties(_markerChinSmall, Measurements.ChinHeight);
            _markerChinSmall.Length = Measurements.HeadWidth ?? 0;
            _markerChinSmall.transform.position = parentRotEuler * (Offset + new Vector3(0, Measurements.ChinHeight ?? 0, 0) + halfHeadWidthVector) + parentPos;

            // head 
            SetMainMarkerProperties(_markerHeadSmall, Measurements.Height);
            _markerHeadSmall.Length = Measurements.HeadWidth ?? 0;
            _markerHeadSmall.transform.position = parentRotEuler * (Offset + new Vector3(0, Measurements.Height ?? 0, 0) + halfHeadWidthVector) + parentPos;

            // head left
            SetMainMarkerProperties(_markerHeadLeft, Measurements.HeadWidth);
            _markerHeadLeft.Length = Measurements.HeadHeight ?? 0;
            _markerHeadLeft.transform.position = parentRotEuler * (Offset - new Vector3(halfHeadWidth, 0, 0) - halfHeadHeightVector + headCenterVector) + parentPos;

            // head right
            SetMainMarkerProperties(_markerHeadRight, Measurements.HeadWidth);
            _markerHeadRight.Length = Measurements.HeadHeight ?? 0;
            _markerHeadRight.transform.position = parentRotEuler * (Offset + new Vector3(halfHeadWidth, 0, 0) - halfHeadHeightVector + headCenterVector) + parentPos;

            // face center
            SetMainMarkerProperties(_markerFaceCenter, Measurements.HeadWidth);
            _markerFaceCenter.Length = Measurements.HeadHeight ?? 0;
            _markerFaceCenter.transform.position = parentRotEuler * (Offset - halfHeadHeightVector + headCenterVector) + parentPos;
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