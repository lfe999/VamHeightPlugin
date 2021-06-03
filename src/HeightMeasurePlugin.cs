using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LFE
{
    public class HeightMeasurePlugin : MVRScript
    {
        private UI _ui;
        CharacterMeasurements _measurements;

        List<BaseVisualGuides> _visualGuides = new List<BaseVisualGuides>();
        MainVisualGuides _mainGuides;
        HeadVisualGuides _headGuides;
        FaceVisualGuides _faceGuides;
        ArcVisualGuides _bustGuides;
        ArcVisualGuides _underbustGuides;
        ArcVisualGuides _waistGuides;
        ArcVisualGuides _hipGuides;

        public override void Init()
        {
            _ui = new UI(this);

            _mainGuides = gameObject.AddComponent<MainVisualGuides>();
            _mainGuides.transform.SetParent(containingAtom.mainController.transform);
            _mainGuides.Measurements = _measurements;
            _visualGuides.Add(_mainGuides);

            _headGuides = gameObject.AddComponent<HeadVisualGuides>();
            _headGuides.transform.SetParent(containingAtom.mainController.transform);
            _headGuides.Measurements = _measurements;
            _visualGuides.Add(_headGuides);

            _faceGuides = gameObject.AddComponent<FaceVisualGuides>();
            _faceGuides.transform.SetParent(containingAtom.mainController.transform);
            _faceGuides.Measurements = _measurements;
            _visualGuides.Add(_faceGuides);

            _bustGuides = gameObject.AddComponent<ArcVisualGuides>();
            _bustGuides.transform.SetParent(containingAtom.mainController.transform);
            _bustGuides.Measurements = _measurements;
            _visualGuides.Add(_bustGuides);

            _underbustGuides = gameObject.AddComponent<ArcVisualGuides>();
            _underbustGuides.transform.SetParent(containingAtom.mainController.transform);
            _underbustGuides.Measurements = _measurements;
            _visualGuides.Add(_underbustGuides);

            _waistGuides = gameObject.AddComponent<ArcVisualGuides>();
            _waistGuides.transform.SetParent(containingAtom.mainController.transform);
            _waistGuides.Measurements = _measurements;
            _visualGuides.Add(_waistGuides);

            _hipGuides = gameObject.AddComponent<ArcVisualGuides>();
            _hipGuides.transform.SetParent(containingAtom.mainController.transform);
            _hipGuides.Measurements = _measurements;
            _visualGuides.Add(_hipGuides);
        }

        public void OnEnable() {
            _mainGuides.Enabled = _ui.showFeatureMarkersStorable.val;
            _headGuides.Enabled = _ui.showHeadHeightMarkersStorable.val; 
            _faceGuides.Enabled = _ui.showFaceMarkersStorable.val;
            _bustGuides.Enabled = _ui.showCircumferenceMarkersStorable.val;
            _underbustGuides.Enabled = _ui.showCircumferenceMarkersStorable.val;
            _waistGuides.Enabled = _ui.showCircumferenceMarkersStorable.val;
            _hipGuides.Enabled = _ui.showCircumferenceMarkersStorable.val;
        }

        public void OnDisable() {
            _mainGuides.Enabled = false;
            _headGuides.Enabled = false;
            _headGuides.Enabled = false;
            _bustGuides.Enabled = false;
            _underbustGuides.Enabled = false;
            _waistGuides.Enabled = false;
            _hipGuides.Enabled = false;
        }

        public void OnDestroy() {
            foreach(var g in _visualGuides) {
                Destroy(g);
            }
            _visualGuides = new List<BaseVisualGuides>(); 
            _mainGuides = null;
            _headGuides = null;
            _faceGuides = null;
            _bustGuides = null;
            _underbustGuides = null;
            _waistGuides = null;
            _hipGuides = null;
            _measurements = null;

            // destroy the markers
            foreach(var m in gameObject.GetComponentsInChildren<LabeledLine>()) {
                Destroy(m);
            }

            foreach(var h in _penisMarkersFromMorph) {
                Destroy(h);
            }
        }

        public void Update() {
            if(SuperController.singleton.freezeAnimation) {
                return;
            }

            if(_ui == null) {
                return;
            }

            try {
                _measurements = AutoMeasurements(_ui, containingAtom);

                _ui.headSizeHeightStorable.val = _measurements.HeadHeight ?? 0;
                _ui.headSizeWidthStorable.val = _measurements.HeadWidth ?? 0;
                _ui.fullHeightStorable.val = _measurements.Height ?? 0;
                _ui.heightInHeadsStorable.val = _ui.headSizeHeightStorable.val == 0 ? 0 : _ui.fullHeightStorable.val / _ui.headSizeHeightStorable.val;
                _ui.chinHeightStorable.val = _measurements.ChinHeight ?? 0;
                _ui.shoulderHeightStorable.val = _measurements.ShoulderHeight ?? 0;
                _ui.nippleHeightStorable.val = _measurements.NippleHeight ?? 0;
                _ui.underbustHeightStorable.val = _measurements.UnderbustHeight ?? 0;
                _ui.navelHeightStorable.val = _measurements.NavelHeight ?? 0;
                _ui.crotchHeightStorable.val = _measurements.CrotchHeight ?? 0;
                _ui.kneeBottomHeightStorable.val = _measurements.KneeHeight ?? 0;

                _ui.waistSizeStorable.val = _measurements.WaistSize ?? 0;
                _ui.hipSizeStorable.val = _measurements.HipSize ?? 0;

                _ui.breastBustStorable.val = _measurements.BustSize ?? 0;
                _ui.breastUnderbustStorable.val = _measurements.UnderbustSize ?? 0;
                _ui.breastBandStorable.val = _measurements.BandSize ?? 0;
                _ui.breastCupStorable.val = _measurements.CupSize ?? "";

                _ui.penisLength.val = _measurements.PenisLength ?? 0;
                _ui.penisWidth.val = _measurements.PenisWidth ?? 0;
                _ui.penisGirth.val = _measurements.PenisGirth ?? 0;

                UpdateVisuals();
                UpdatePenisMarkers();

            }
            catch(Exception e) {
                SuperController.LogError(e.ToString());
            }
        }


        private void UpdateVisuals() {
            if(_measurements == null || _ui == null) {
                return;
            }

            // tell all the display elements about the measurements
            foreach(var g in _visualGuides) {
                g.Measurements = _measurements;
            }

            var euler = Quaternion.Euler(containingAtom.mainController.transform.rotation.eulerAngles);

            var spreadVector = new Vector3(_ui.markerSpreadStorable.val, 0, 0);
            var frontBackVector = new Vector3(0, 0, _ui.markerFrontBackStorable.val);
            var leftRightVector = new Vector3(_ui.markerLeftRightStorable.val, 0, 0);

            // raise markers based on foot height
            var pos = frontBackVector - leftRightVector + _measurements.HeelToFloorOffset;

            var isMale = _measurements.POI?.IsMale ?? false;

            // feature guide
            _mainGuides.Enabled = _ui.showFeatureMarkersStorable.val;
            _mainGuides.LabelsEnabled = _ui.showFeatureMarkerLabelsStorable.val;
            _mainGuides.LineColor = HSVToColor(_ui.featureMarkerColor.val);
            _mainGuides.LineThickness = _ui.lineThicknessFigureStorable.val;
            _mainGuides.UnitDisplay = _ui.unitsStorable.val;
            _mainGuides.Offset = pos - spreadVector;

            // head height guide
            _headGuides.Enabled = _ui.showHeadHeightMarkersStorable.val;
            _headGuides.LabelsEnabled = true;
            _headGuides.LineColor = HSVToColor(_ui.headMarkerColor.val);
            _headGuides.LineThickness = _ui.lineThicknessHeadStorable.val;
            _headGuides.UnitDisplay = _ui.unitsStorable.val;
            _headGuides.Offset = pos + spreadVector;

            // face guide
            _faceGuides.Enabled = _ui.showFaceMarkersStorable.val;
            _faceGuides.LabelsEnabled = true;
            _faceGuides.LineColor = HSVToColor(_ui.faceMarkerColor.val);
            _faceGuides.LineThickness = _ui.lineThicknessFaceStorable.val;
            _faceGuides.UnitDisplay = _ui.unitsStorable.val;
            _faceGuides.Offset = pos + new Vector3(0, 0, -0.03f);

            // bust guide
            _bustGuides.Enabled = _ui.showCircumferenceMarkersStorable.val && !isMale;
            _bustGuides.LabelsEnabled = false;
            _bustGuides.LineColor = HSVToColor(_ui.circumferenceMarkerColor.val);
            _bustGuides.LineThickness = _ui.lineThicknessCircumferenceStorable.val;
            _bustGuides.UnitDisplay = _ui.unitsStorable.val;
            _bustGuides.Offset = pos;
            _bustGuides.Points = _measurements.POI?.BustPoints ?? new Vector3[0];

            // underbust guide
            _underbustGuides.Enabled = _ui.showCircumferenceMarkersStorable.val && !isMale;
            _underbustGuides.LabelsEnabled = false;
            _underbustGuides.LineColor = HSVToColor(_ui.circumferenceMarkerColor.val);
            _underbustGuides.LineThickness = _ui.lineThicknessCircumferenceStorable.val;
            _underbustGuides.UnitDisplay = _ui.unitsStorable.val;
            _underbustGuides.Offset = pos;
            _underbustGuides.Points = _measurements.POI?.UnderbustPoints ?? new Vector3[0];

            // waist guide
            _waistGuides.Enabled = _ui.showCircumferenceMarkersStorable.val && !isMale;
            _waistGuides.LabelsEnabled = false;
            _waistGuides.LineColor = HSVToColor(_ui.circumferenceMarkerColor.val);
            _waistGuides.LineThickness = _ui.lineThicknessCircumferenceStorable.val;
            _waistGuides.UnitDisplay = _ui.unitsStorable.val;
            _waistGuides.Offset = pos;
            _waistGuides.Points = _measurements.POI?.WaistPoints ?? new Vector3[0];

            // hip guide
            _hipGuides.Enabled = _ui.showCircumferenceMarkersStorable.val && !isMale;
            _hipGuides.LabelsEnabled = false;
            _hipGuides.LineColor = HSVToColor(_ui.circumferenceMarkerColor.val);
            _hipGuides.LineThickness = _ui.lineThicknessCircumferenceStorable.val;
            _hipGuides.UnitDisplay = _ui.unitsStorable.val;
            _hipGuides.Offset = pos;
            _hipGuides.Points = _measurements.POI?.HipPoints ?? new Vector3[0];

        }

        private float LineLength(Vector3[] vertices) {
            float total = 0;
            for(var i = 1; i < vertices.Length; i++) {
                var distance = Mathf.Abs(Vector3.Distance(vertices[i-1], vertices[i]));
                total += distance;
            }
            return total;
        }

        readonly List<GameObject> _penisMarkersFromMorph = new List<GameObject>();
        float _penisLength = 0;
        float _penisGirth = 0;
        float _penisWidth = 0;
        private void UpdatePenisMarkers() {
            if(_measurements == null) {
                return;
            }

            if(!(_measurements.POI?.IsMale ?? false)) {
                foreach(var m in _penisMarkersFromMorph) {
                    Destroy(m);
                }
                _penisMarkersFromMorph.Clear();
                return;
            }

            var penisTipPos = _measurements.POI?.PenisTip ?? Vector3.zero;
            var penisBasePos = _measurements.POI?.PenisBase ?? Vector3.zero;
            var penisShaftLeftPos = _measurements?.POI.PenisShaftLeft ?? Vector3.zero;
            var penisShaftRightPos = _measurements?.POI.PenisShaftRight ?? Vector3.zero;

            if(_ui.showCircumferenceMarkersStorable.val){
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

        private CupSize GetCupInfo(float bust, float underbust, ICupCalculator cupCalculator) {
            return cupCalculator?.Calculate(bust, underbust);
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

        public HSVColor ColorToHSV(Color color) {
            float H, S, V;
            Color.RGBToHSV(color, out H, out S, out V);
            return new HSVColor { H = H, S = S, V = V };
        }

        public Color HSVToColor(HSVColor hsv) {
            return Color.HSVToRGB(hsv.H, hsv.S, hsv.V);
        }

        public CharacterMeasurements AutoMeasurements(UI ui, Atom person) {
            var measurements = new CharacterMeasurements();
            var poi = new CharacterPointsOfInterest(person);

            var headPos = poi.CraniumHeight;
            var headPosEulered = Quaternion.Euler(person.mainController.transform.rotation.eulerAngles) * headPos;

            var footPos = poi.HeelHeight;
            // TODO: figire out how to get the X/Z centering-on-the head stuff to work with rotation
            footPos.x = headPos.x;
            footPos.z = headPos.z;

            var chinPos = poi.ChinHeight;
            chinPos.x = headPos.x;
            chinPos.z = headPos.z;

            var shoulderPos = poi.ShoulderHeight;
            shoulderPos.x = headPos.x;
            shoulderPos.z = headPos.z;

            var nipplePos = poi.BustHeight;
            nipplePos.x = headPos.x;
            nipplePos.z = headPos.z;

            var underbustPos = poi.UnderbustHeight;
            underbustPos.x = headPos.x;
            underbustPos.z = headPos.z;

            var navelPos = poi.NavelHeight;
            navelPos.x = headPos.x;
            navelPos.z = headPos.z;

            var crotchPos = poi.CrotchHeight;
            crotchPos.x = headPos.x;
            crotchPos.z = headPos.z;

            var kneePos = poi.KneeUnderHeight;
            kneePos.x = headPos.x;
            kneePos.z = headPos.z;

            var eyeHeightPos = poi.EyeLeftCenter;
            eyeHeightPos.x = headPos.x;
            eyeHeightPos.z = headPos.z;

            var mouthHeightPos = poi.MouthCenterHeight;
            mouthHeightPos.x = headPos.x;
            mouthHeightPos.z = headPos.z;

            // set measurements
            measurements.Height = Vector3.Distance(headPos, footPos);
            measurements.ChinHeight = Vector3.Distance(chinPos, footPos);
            measurements.HeadWidth = Vector3.Distance(poi.CraniumLeftSide, poi.CraniumRightSide);
            measurements.ShoulderHeight = Vector3.Distance(shoulderPos, footPos);
            measurements.NippleHeight = poi.IsMale ? (float?)null : Vector3.Distance(nipplePos, footPos);
            measurements.UnderbustHeight = poi.IsMale ? (float?)null : Vector3.Distance(underbustPos, footPos);
            measurements.NavelHeight = Vector3.Distance(navelPos, footPos);
            measurements.CrotchHeight = Vector3.Distance(crotchPos, footPos);
            measurements.KneeHeight = Vector3.Distance(kneePos, footPos);
            measurements.HeelHeight = 0;
            measurements.HeelToFloorOffset = footPos - person.mainController.transform.position;

            measurements.WaistSize = LineLength(poi.WaistPoints);
            measurements.HipSize = LineLength(poi.HipPoints);

            if(poi.IsMale) {
                measurements.BustSize = null;
                measurements.UnderbustSize = null;
                measurements.BandSize = null;
                measurements.CupSize = null;
            }
            else {
                var cupInfo = GetCupInfo(LineLength(poi.BustPoints), LineLength(poi.UnderbustPoints), ui.CupCalculator);
                if(cupInfo != null) {
                    measurements.BustSize = cupInfo.Bust;
                    measurements.UnderbustSize = cupInfo.Underbust;
                    measurements.BandSize = cupInfo.Band;
                    measurements.CupSize = cupInfo.Cup;
                }
                else {
                    measurements.BustSize = null;
                    measurements.UnderbustSize = null;
                    measurements.BandSize = null;
                    measurements.CupSize = null;
                }
            }

            measurements.EyesHeight = Vector3.Distance(eyeHeightPos, footPos);
            measurements.EyesWidth = Vector3.Distance(poi.EyeLeftOuterSide, poi.EyeRightOuterSide);
            measurements.EyesOffsetLeftRight = 0;
            measurements.NoseHeight = poi.IsMale ? (float?)null : Vector3.Distance(poi.NoseBottomHeight, footPos);
            measurements.MouthHeight = poi.IsMale ? (float?)null : Vector3.Distance(mouthHeightPos, footPos);
            measurements.MouthWidth = poi.IsMale ? (float?)null : Vector3.Distance(poi.MouthLeftSide, poi.MouthRightSide);
            measurements.MouthOffsetLeftRight = 0;

            if(poi.IsMale) {
                measurements.PenisLength = _penisLength;
                measurements.PenisGirth = _penisGirth;
                measurements.PenisWidth = _penisWidth;
            }
            else {
                measurements.PenisLength = null;
                measurements.PenisGirth = null;
                measurements.PenisWidth = null;
            }

            measurements.POI = poi;

            return measurements;
        }

        // public static CharacterMeasurements StaticMeasurements(UI ui) {

        // }

    }
}
