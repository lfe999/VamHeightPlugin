using System;
using System.Collections.Generic;
using UnityEngine;

namespace LFE
{
    public class HeightMeasurePlugin : MVRScript
    {
        private UI _ui;
        private bool _enabled = false;
        CharacterMeasurements _autoMeasurements;
        CharacterMeasurements _manualMeasurements;

        List<GameObject> _visualGuides = new List<GameObject>();
        MainVisualGuides _mainGuides;
        HeadVisualGuides _headGuides;
        FaceVisualGuides _faceGuides;
        ArcVisualGuides _bustGuides;
        ArcVisualGuides _underbustGuides;
        ArcVisualGuides _waistGuides;
        ArcVisualGuides _hipGuides;

        MainVisualGuides _mainGuidesManual;
        HeadVisualGuides _headGuidesManual;

        private bool _initRun = false;
        public override void Init()
        {
            _ui = new UI(this);

            _mainGuides = CreateVisualGuide<MainVisualGuides>();
            _mainGuidesManual = CreateVisualGuide<MainVisualGuides>();
            _headGuides = CreateVisualGuide<HeadVisualGuides>();
            _headGuidesManual = CreateVisualGuide<HeadVisualGuides>();
            _faceGuides = CreateVisualGuide<FaceVisualGuides>();
            _bustGuides = CreateVisualGuide<ArcVisualGuides>();
            _underbustGuides = CreateVisualGuide<ArcVisualGuides>();
            _waistGuides = CreateVisualGuide<ArcVisualGuides>();
            _hipGuides = CreateVisualGuide<ArcVisualGuides>();

            _initRun = true;
        }

        public void OnEnable() {
            _enabled = true;
            if(_initRun) {
                _mainGuides.Enabled = _ui.showFeatureMarkersStorable.val;
                _mainGuidesManual.Enabled = false;
                _headGuides.Enabled = _ui.showHeadHeightMarkersStorable.val; 
                _headGuidesManual.Enabled = false;
                _faceGuides.Enabled = _ui.showFaceMarkersStorable.val;
                _bustGuides.Enabled = _ui.showCircumferenceMarkersStorable.val;
                _underbustGuides.Enabled = _ui.showCircumferenceMarkersStorable.val;
                _waistGuides.Enabled = _ui.showCircumferenceMarkersStorable.val;
                _hipGuides.Enabled = _ui.showCircumferenceMarkersStorable.val;
            }
        }

        public void OnDisable() {
            _enabled = false;
            _mainGuides.Enabled = false;
            _mainGuidesManual.Enabled = false;
            _headGuides.Enabled = false;
            _headGuidesManual.Enabled = false;
            _faceGuides.Enabled = false;
            _bustGuides.Enabled = false;
            _underbustGuides.Enabled = false;
            _waistGuides.Enabled = false;
            _hipGuides.Enabled = false;
        }

        public void OnDestroy() {
            _enabled = false;
            foreach(var g in _visualGuides) {
                Destroy(g);
            }
            _visualGuides = new List<GameObject>(); 

            _mainGuides = null;
            _mainGuidesManual = null;
            _headGuides = null;
            _headGuidesManual = null;
            _faceGuides = null;
            _bustGuides = null;
            _underbustGuides = null;
            _waistGuides = null;
            _hipGuides = null;
            _autoMeasurements = null;

            foreach(var h in _penisMarkersFromMorph) {
                Destroy(h);
            }

            if(_autoMeasurements != null) {
                if(_autoMeasurements.POI != null) {
                    _autoMeasurements.POI.Person = null;
                    _autoMeasurements.POI = null;
                }
                _autoMeasurements = null;
            }
        }

        bool _enabledPrev = false; // allows for performant disabling
        public void Update() {
            if(SuperController.singleton.freezeAnimation) {
                return;
            }

            // allows for performant disabling with OnEnable/Disable
            // if disabling, the Update should still try and shut off
            // components one last time
            if(_enabledPrev == false && !_enabled) {
                return;
            }
            _enabledPrev = _enabled;

            if(_ui == null) {
                return;
            }

            try {
                _autoMeasurements = AutoMeasurements(_ui, containingAtom);

                _manualMeasurements = StaticMeasurements(_ui);
                _manualMeasurements.HeelToFloorOffset = _autoMeasurements.HeelToFloorOffset;

                if(containingAtom.type == "Person" && _ui.manualMarkersCopy.val) {
                    _ui.manualHeightStorable.val = (_autoMeasurements.Height ?? 0) * 100;
                    _ui.manualChinHeightStorable.val = (_autoMeasurements.ChinHeight ?? 0) * 100;
                    _ui.manualShoulderHeightStorable.val = (_autoMeasurements.ShoulderHeight ?? 0) * 100;
                    _ui.manualNippleHeightStorable.val = (_autoMeasurements.NippleHeight ?? 0) * 100;
                    _ui.manualUnderbustHeightStorable.val = (_autoMeasurements.UnderbustHeight ?? 0) * 100;
                    _ui.manualNavelHeightStorable.val = (_autoMeasurements.NavelHeight ?? 0) * 100;
                    _ui.manualCrotchHeightStorable.val = (_autoMeasurements.CrotchHeight ?? 0) * 100;
                    _ui.manualKneeBottomHeightStorable.val = (_autoMeasurements.KneeHeight ?? 0) * 100;
                }

                _ui.headSizeHeightStorable.val = _autoMeasurements.HeadHeight ?? 0;
                _ui.headSizeWidthStorable.val = _autoMeasurements.HeadWidth ?? 0;
                _ui.fullHeightStorable.val = _autoMeasurements.Height ?? 0;
                _ui.heightInHeadsStorable.val = _ui.headSizeHeightStorable.val == 0 ? 0 : _ui.fullHeightStorable.val / _ui.headSizeHeightStorable.val;
                _ui.chinHeightStorable.val = _autoMeasurements.ChinHeight ?? 0;
                _ui.shoulderHeightStorable.val = _autoMeasurements.ShoulderHeight ?? 0;
                _ui.nippleHeightStorable.val = _autoMeasurements.NippleHeight ?? 0;
                _ui.underbustHeightStorable.val = _autoMeasurements.UnderbustHeight ?? 0;
                _ui.navelHeightStorable.val = _autoMeasurements.NavelHeight ?? 0;
                _ui.crotchHeightStorable.val = _autoMeasurements.CrotchHeight ?? 0;
                _ui.kneeBottomHeightStorable.val = _autoMeasurements.KneeHeight ?? 0;

                _ui.waistSizeStorable.val = _autoMeasurements.WaistSize ?? 0;
                _ui.hipSizeStorable.val = _autoMeasurements.HipSize ?? 0;

                _ui.breastBustStorable.val = _autoMeasurements.BustSize ?? 0;
                _ui.breastUnderbustStorable.val = _autoMeasurements.UnderbustSize ?? 0;
                _ui.breastBandStorable.val = _autoMeasurements.BandSize ?? 0;
                _ui.breastCupStorable.val = _autoMeasurements.CupSize ?? "";

                _ui.penisLength.val = _autoMeasurements.PenisLength ?? 0;
                _ui.penisWidth.val = _autoMeasurements.PenisWidth ?? 0;
                _ui.penisGirth.val = _autoMeasurements.PenisGirth ?? 0;

                UpdateVisuals();
                UpdatePenisMarkers();

            }
            catch(Exception e) {
                SuperController.LogError(e.ToString());
            }
        }

        private T CreateVisualGuide<T>() where T : MonoBehaviour {
            var go = new GameObject();
            go.transform.SetParent(transform);
            var guide = go.AddComponent<T>();
            guide.transform.SetParent(containingAtom.mainController.transform);
            _visualGuides.Add(go);
            return guide;
        }

        private void UpdateVisuals() {
            if(_autoMeasurements == null || _ui == null || _manualMeasurements == null) {
                return;
            }

            // tell all the display elements about the measurements
            foreach(var go in _visualGuides) {
                var g = go.GetComponent<BaseVisualGuides>();
                if(g) {
                    g.Measurements = _autoMeasurements;
                }
            }
            _mainGuidesManual.Measurements = _manualMeasurements;
            _headGuidesManual.Measurements = _manualMeasurements;


            var euler = Quaternion.Euler(containingAtom.mainController.transform.rotation.eulerAngles);

            var spreadVector = new Vector3(_ui.markerSpreadStorable.val, 0, 0);
            var frontBackVector = new Vector3(0, 0, _ui.markerFrontBackStorable.val);
            var leftRightVector = new Vector3(_ui.markerLeftRightStorable.val, 0, 0);
            var upDownVector = new Vector3(0, _ui.markerUpDownStorable.val, 0);

            // raise markers based on foot height
            var pos = frontBackVector - leftRightVector + upDownVector + _autoMeasurements.HeelToFloorOffset;

            if(containingAtom.type == "Person") {
                var isMale = _autoMeasurements.POI?.IsMale ?? false;

                // auto feature guide
                _mainGuides.Enabled = _ui.showFeatureMarkersStorable.val;
                _mainGuides.LabelsEnabled = _ui.showFeatureMarkerLabelsStorable.val;
                _mainGuides.LineColor = HSVToColor(_ui.featureMarkerColor.val);
                _mainGuides.LineThickness = _ui.lineThicknessFigureStorable.val;
                _mainGuides.UnitDisplay = _ui.unitsStorable.val;
                _mainGuides.Offset = pos - spreadVector;

                // auto head height guide
                _headGuides.Enabled = _ui.showHeadHeightMarkersStorable.val;
                _headGuides.LabelsEnabled = true;
                _headGuides.LineColor = HSVToColor(_ui.headMarkerColor.val);
                _headGuides.LineThickness = _ui.lineThicknessHeadStorable.val;
                _headGuides.UnitDisplay = _ui.unitsStorable.val;
                _headGuides.Offset = pos + spreadVector;

                // auto face guide
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
                _bustGuides.Points = _autoMeasurements.POI?.BustPoints ?? new Vector3[0];

                // underbust guide
                _underbustGuides.Enabled = _ui.showCircumferenceMarkersStorable.val && !isMale;
                _underbustGuides.LabelsEnabled = false;
                _underbustGuides.LineColor = HSVToColor(_ui.circumferenceMarkerColor.val);
                _underbustGuides.LineThickness = _ui.lineThicknessCircumferenceStorable.val;
                _underbustGuides.UnitDisplay = _ui.unitsStorable.val;
                _underbustGuides.Offset = pos;
                _underbustGuides.Points = _autoMeasurements.POI?.UnderbustPoints ?? new Vector3[0];

                // waist guide
                _waistGuides.Enabled = _ui.showCircumferenceMarkersStorable.val && !isMale;
                _waistGuides.LabelsEnabled = false;
                _waistGuides.LineColor = HSVToColor(_ui.circumferenceMarkerColor.val);
                _waistGuides.LineThickness = _ui.lineThicknessCircumferenceStorable.val;
                _waistGuides.UnitDisplay = _ui.unitsStorable.val;
                _waistGuides.Offset = pos;
                _waistGuides.Points = _autoMeasurements.POI?.WaistPoints ?? new Vector3[0];

                // hip guide
                _hipGuides.Enabled = _ui.showCircumferenceMarkersStorable.val && !isMale;
                _hipGuides.LabelsEnabled = false;
                _hipGuides.LineColor = HSVToColor(_ui.circumferenceMarkerColor.val);
                _hipGuides.LineThickness = _ui.lineThicknessCircumferenceStorable.val;
                _hipGuides.UnitDisplay = _ui.unitsStorable.val;
                _hipGuides.Offset = pos;
                _hipGuides.Points = _autoMeasurements.POI?.HipPoints ?? new Vector3[0];
            }

            // manual feature guide
            _mainGuidesManual.Enabled = _ui.showManualMarkersStorable.val;
            _mainGuidesManual.LabelsEnabled = _ui.showManualMarkersStorable.val && _mainGuidesManual.Measurements.Height > 0;
            _mainGuidesManual.LineColor = HSVToColor(_ui.manualMarkerColor.val);
            _mainGuidesManual.LineThickness = _ui.lineThicknessManualStorable.val;
            _mainGuidesManual.UnitDisplay = _ui.unitsStorable.val;
            _mainGuidesManual.Offset = pos - spreadVector - new Vector3(0, 0, 0.002f); // put these just a bit behind the auto guides

            // manual head height guide
            _headGuidesManual.Enabled = _ui.showManualMarkersStorable.val && _headGuidesManual.Measurements.Height > 0;
            _headGuidesManual.LabelsEnabled = true;
            _headGuidesManual.LineColor = HSVToColor(_ui.manualMarkerColor.val);
            _headGuidesManual.LineThickness = _ui.lineThicknessManualStorable.val;
            _headGuidesManual.UnitDisplay = _ui.unitsStorable.val;
            _headGuidesManual.Offset = pos + spreadVector - new Vector3(0, 0, 0.002f); // put these just a bit behind the auto guides
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
            if(_autoMeasurements == null) {
                return;
            }

            if(!(_autoMeasurements.POI?.IsMale ?? false)) {
                foreach(var m in _penisMarkersFromMorph) {
                    Destroy(m);
                }
                _penisMarkersFromMorph.Clear();
                return;
            }

            var penisTipPos = _autoMeasurements.POI?.PenisTip ?? Vector3.zero;
            var penisBasePos = _autoMeasurements.POI?.PenisBase ?? Vector3.zero;
            var penisShaftLeftPos = _autoMeasurements?.POI.PenisShaftLeft ?? Vector3.zero;
            var penisShaftRightPos = _autoMeasurements?.POI.PenisShaftRight ?? Vector3.zero;

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


        // LineRenderer _debugLine;
        public CharacterMeasurements AutoMeasurements(UI ui, Atom atom) {
            var measurements = new CharacterMeasurements();
            if(atom.type != "Person") {
                return measurements;
            }
            var poi = new CharacterPointsOfInterest(atom);

            // switching skins sometimes leaves things in a bad state for a few seconds - watch for that
            if(!poi.HasSkin) {
                return measurements;
            }

            var rootTransform = atom.mainController.transform;
            var rootPos = rootTransform.position;

            var floor = Vector3.ProjectOnPlane(rootPos, rootTransform.up);
            var floorDistanceOffset = Vector3.Distance(rootPos, floor) * (Vector3.Dot(rootPos - floor, rootTransform.up) < 0 ? -1 : 1);

            var headPos = poi.CraniumHeight;
            var footPos = poi.HeelHeight;
            var chinPos = poi.ChinHeight;
            var shoulderPos = poi.ShoulderHeight;
            var nipplePos = poi.BustHeight;
            var underbustPos = poi.UnderbustHeight;
            var navelPos = poi.NavelHeight;
            var crotchPos = poi.CrotchHeight;
            var kneePos = poi.KneeUnderHeight;
            var eyeHeightPos = poi.EyeLeftCenter;
            var mouthHeightPos = poi.MouthCenterHeight;

            // if(_debugLine == null) {
            //     var go = new GameObject();
            //     go.transform.SetParent(transform);
            //     var lr = go.AddComponent<LineRenderer>();
            //     lr.transform.SetParent(go.transform);
            //     lr.gameObject.SetActive(true);
            //     lr.startWidth = 0.01f;
            //     lr.endWidth = 0.01f;
            //     _debugLine = lr;
            // }
            // _debugLine.SetPositions(new Vector3[] {
            //     // footPos,
            //     Vector3.ProjectOnPlane(footPos, rootTransform.up),
            //     Vector3.ProjectOnPlane(rootPos, rootTransform.up)
            // });

            var footOffset = Vector3.Distance(footPos, Vector3.ProjectOnPlane(footPos, rootTransform.up)) - floorDistanceOffset;

            // set measurements
            measurements.HeelToFloorOffset = Vector3.up * footOffset;
            measurements.Height = Vector3.Distance(headPos, Vector3.ProjectOnPlane(headPos, rootTransform.up)) - floorDistanceOffset - footOffset;
            measurements.ChinHeight = Vector3.Distance(chinPos, Vector3.ProjectOnPlane(chinPos, rootTransform.up)) - floorDistanceOffset - footOffset;
            measurements.HeadWidth = Vector3.Distance(poi.CraniumLeftSide, poi.CraniumRightSide);
            measurements.ShoulderHeight = Vector3.Distance(shoulderPos, Vector3.ProjectOnPlane(shoulderPos, rootTransform.up)) - floorDistanceOffset - footOffset;
            measurements.NippleHeight = poi.IsMale ? (float?)null : Vector3.Distance(nipplePos, Vector3.ProjectOnPlane(nipplePos, rootTransform.up)) - floorDistanceOffset - footOffset;
            measurements.UnderbustHeight = poi.IsMale ? (float?)null : Vector3.Distance(underbustPos, Vector3.ProjectOnPlane(underbustPos, rootTransform.up)) - floorDistanceOffset - footOffset;
            measurements.NavelHeight = Vector3.Distance(navelPos, Vector3.ProjectOnPlane(navelPos, rootTransform.up)) - floorDistanceOffset - footOffset;
            measurements.CrotchHeight = Vector3.Distance(crotchPos, Vector3.ProjectOnPlane(crotchPos, rootTransform.up)) - floorDistanceOffset - footOffset;
            measurements.KneeHeight = Vector3.Distance(kneePos, Vector3.ProjectOnPlane(kneePos, rootTransform.up)) - floorDistanceOffset - footOffset;
            measurements.HeelHeight = 0;

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

            measurements.EyesHeight = Vector3.Distance(eyeHeightPos, Vector3.ProjectOnPlane(eyeHeightPos, rootTransform.up)) - floorDistanceOffset - footOffset;
            measurements.EyesWidth = Vector3.Distance(poi.EyeLeftOuterSide, poi.EyeRightOuterSide);
            measurements.EyesOffsetLeftRight = 0;
            measurements.NoseHeight = poi.IsMale ? (float?)null : Vector3.Distance(poi.NoseBottomHeight, Vector3.ProjectOnPlane(poi.NoseBottomHeight, rootTransform.up)) - floorDistanceOffset - footOffset;
            measurements.MouthHeight = poi.IsMale ? (float?)null : Vector3.Distance(mouthHeightPos, Vector3.ProjectOnPlane(mouthHeightPos, rootTransform.up)) - floorDistanceOffset - footOffset;
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

        public static CharacterMeasurements StaticMeasurements(UI ui) {
            var measurements = new CharacterMeasurements {
                Height = ui.manualHeightStorable.val / 100,
                ChinHeight = ui.manualChinHeightStorable.val / 100,
                ShoulderHeight = ui.manualShoulderHeightStorable.val / 100,
                NippleHeight = ui.manualNippleHeightStorable.val / 100,
                UnderbustHeight = ui.manualUnderbustHeightStorable.val / 100,
                NavelHeight = ui.manualNavelHeightStorable.val / 100,
                CrotchHeight = ui.manualCrotchHeightStorable.val / 100,
                KneeHeight = ui.manualKneeBottomHeightStorable.val / 100,
                HeelHeight = 0,
                HeelToFloorOffset = Vector3.zero
            };
            return measurements;
        }

    }
}
