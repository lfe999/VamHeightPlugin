using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SimpleJSON;

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
        AgeStatsVisualGuides _ageGuides;
        ProportionTargetVisualGuides _proportionGuides;

        MainVisualGuides _mainGuidesManual;
        HeadVisualGuides _headGuidesManual;

        readonly CdcHeightCalculator _ageFromHeightCalculator = new CdcHeightCalculator();
        readonly Anatomy4SculptersCalculator _ageFromHeadRatioCalculator = new Anatomy4SculptersCalculator();

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
            _ageGuides = CreateVisualGuide<AgeStatsVisualGuides>();
            _proportionGuides = CreateVisualGuide<ProportionTargetVisualGuides>();

            _initRun = true;
        }

        public void OnEnable() {
            _enabled = true;
            if(_initRun) {
                _mainGuides.Enabled = _ui.showFeatureMarkersStorable.val;
                _mainGuidesManual.Enabled = _ui.showManualMarkersStorable.val;
                _headGuides.Enabled = _ui.showHeadHeightMarkersStorable.val; 
                _headGuidesManual.Enabled = _ui.showHeadHeightMarkersStorable.val;
                _faceGuides.Enabled = _ui.showFaceMarkersStorable.val;
                _bustGuides.Enabled = _ui.showCircumferenceMarkersStorable.val;
                _underbustGuides.Enabled = _ui.showCircumferenceMarkersStorable.val;
                _waistGuides.Enabled = _ui.showCircumferenceMarkersStorable.val;
                _hipGuides.Enabled = _ui.showCircumferenceMarkersStorable.val;
                _ageGuides.Enabled = _ui.showAgeMarkersStorable.val;
                _proportionGuides.Enabled = _ui.showProportionMarkersStorable.val;
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
            _ageGuides.Enabled = false;
            _proportionGuides.Enabled = false;
        }


        private string _jsonTemplatesKey = "ProportionTemplates";
        // loading scene
        public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true, JSONArray presetAtoms = null, bool setMissingToDefault = true)
        {
            base.RestoreFromJSON(jc, restorePhysical, restoreAppearance, presetAtoms, setMissingToDefault);

            try {
                if(jc.HasKey(_jsonTemplatesKey)) {
                    var proportionTemplates = new List<Proportions>();
                    foreach(SimpleJSON.JSONClass pj in jc[_jsonTemplatesKey].AsArray) {
                        var p = new Proportions() {
                            IsFemale = pj["isFemale"]?.AsBool ?? true,
                            ProportionName = pj["name"].Value,
                            FigureHeightInHeads = pj["figureHeightInHeads"]?.AsFloat ?? 0,
                            FigureChinToShoulder = pj["figureChinToShoulder"]?.AsFloat ?? 0,
                            FigureShoulderWidth = pj["figureShoulderWidth"]?.AsFloat ?? 0,
                            FigureShoulderToNipples = pj["figureShoulderToNipples"]?.AsFloat ?? 0,
                            FigureShoulderToNavel = pj["figureShoulderToNavel"]?.AsFloat ?? 0,
                            FigureShoulderToCrotch = pj["figureShoulderToCrotch"]?.AsFloat ?? 0,
                            FigureCrotchToBottomOfKnees = pj["figureCrotchToBottomOfKnees"]?.AsFloat ?? 0,
                            FigureLengthOfUpperLimb = pj["figureLengthOfUpperLimb"]?.AsFloat ?? 0,
                            FigureLengthOfLowerLimb = pj["figureLengthOfLowerLimb"]?.AsFloat ?? 0,
                            FigureBottomOfKneesToHeels = pj["figureBottomOfKneesToHeels"]?.AsFloat ?? 0,
                            EstimatedAgeRangeMin = pj["estimatedAgeRangeMin"]?.AsInt ?? 0,
                            EstimatedAgeRangeMax = pj["estimatedAgeRangeMax"]?.AsInt ?? 0 
                        };
                        proportionTemplates.Add(p);
                    }
                    _ui.ProportionTemplates = proportionTemplates;
                }
            }
            catch(Exception e) {
                SuperController.LogError($"{e}");
            }
        }

        // saving scene
        public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true, bool forceStore = false) {
            var json = base.GetJSON(includePhysical, includeAppearance, forceStore);

            var proportionTemplates = new SimpleJSON.JSONArray();
            foreach(var p in _ui.ProportionTemplates) {
                var pJson = new SimpleJSON.JSONClass();
                pJson["isFemale"].AsBool = p.IsFemale;
                pJson["name"] = p.ProportionName;
                pJson["figureHeightInHeads"].AsFloat = p.FigureHeightInHeads;
                pJson["figureChinToShoulder"].AsFloat = p.FigureChinToShoulder;
                pJson["figureShoulderWidth"].AsFloat = p.FigureShoulderWidth;
                pJson["figureShoulderToNipples"].AsFloat = p.FigureShoulderToNipples;
                pJson["figureShoulderToNavel"].AsFloat = p.FigureShoulderToNavel;
                pJson["figureShoulderToCrotch"].AsFloat = p.FigureShoulderToCrotch;
                pJson["figureCrotchToBottomOfKnees"].AsFloat = p.FigureCrotchToBottomOfKnees;
                pJson["figureLengthOfUpperLimb"].AsFloat = p.FigureLengthOfUpperLimb;
                pJson["figureLengthOfLowerLimb"].AsFloat = p.FigureLengthOfLowerLimb;
                pJson["figureBottomOfKneesToHeels"].AsFloat = p.FigureBottomOfKneesToHeels;
                pJson["estimatedAgeRangeMin"].AsInt = p.EstimatedAgeRangeMin;
                pJson["estimatedAgeRangeMax"].AsInt = p.EstimatedAgeRangeMax;

                proportionTemplates.Add(pJson);
            }
            json[_jsonTemplatesKey] = proportionTemplates;
            return json;
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
            _ageGuides = null;
            _proportionGuides = null;
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

        private DAZCharacterSelector _dcs;
        private DAZCharacterSelector GetDCS() {
            if(_dcs == null) {
                JSONStorable geometry = containingAtom.GetStorableByID("geometry");
                if (geometry == null)  {
                    return null;
                }

                DAZCharacterSelector dcs = geometry as DAZCharacterSelector;
                if (dcs == null) {
                    return null;
                }
                _dcs = dcs;
            }
            return _dcs;
        }

        private DAZMorph GetMorphByName(string name) {
            var dcs = GetDCS();
            if (dcs == null) {
                return null;
            }

            var mcui = dcs.morphsControlUI;

            return mcui.GetMorphByDisplayName(name);
        }

        bool _enabledPrev = false; // allows for performant disabling
        DAZMorph _headScaleMorph = null;
        int _lastSex = 0; // 0 female - 1 male
        float _prevScale = 0;
        public void Update() {
            try {

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

                bool globalSoftPhysicsEnabled = UserPreferences.singleton.softPhysics;
                if(!globalSoftPhysicsEnabled) {
                    SuperController.LogMessage($"forcing global soft physics on (main User Preferfences) for Height Measure plugin");
                    UserPreferences.singleton.softPhysics = true;
                }

                var breastPhysics  = containingAtom.GetStorableByID("BreastPhysicsMesh") as DAZPhysicsMesh;
                if(breastPhysics != null && !breastPhysics.on) {
                    SuperController.LogMessage($"forcing soft breast physics (F Breast Physics 2 screen) on for Height Measure plugin");
                    breastPhysics.on = true;

                }

                var glutePhysics = containingAtom.GetStorableByID("LowerPhysicsMesh") as DAZPhysicsMesh;
                if(glutePhysics != null && !glutePhysics.on) {
                    SuperController.LogMessage($"forcing soft glute physics (F Glute Physics screen) on for Height Measure plugin");
                    glutePhysics.on = true;
                }

                var physicsMeshesEnable = containingAtom.GetStorableByID("SoftBodyPhysicsEnabler") as DAZPhysicsMeshesEnable;
                if(physicsMeshesEnable != null && !physicsMeshesEnable.enabledJSON.val) {
                    SuperController.LogMessage($"forcing soft physics on (Control & Physics 1 screen) for Height Measure plugin");
                    physicsMeshesEnable.enabledJSON.val = true;
                }

                bool sexWasChanged = false;
                bool targetHeadMorphChanged = false;
                if(_autoMeasurements != null) {
                    var curSex = _autoMeasurements.POI?.IsMale ?? false ? 1 : 0;
                    sexWasChanged = curSex != _lastSex;
                    _lastSex = curSex;

                    targetHeadMorphChanged = _ui.targetHeadRatioMorphStorable.val != _headScaleMorph?.morphName;
                }

                var headRatioTarget = _ui.targetHeadRatioStorable.val;

                // adjust head ratio to match a forced target
                if(_ui.showTargetHeadRatioStorable.val && _autoMeasurements != null) {
                    var headRatioCurrent = _autoMeasurements.HeadHeight != null ? (_autoMeasurements.Height ?? 0)/(_autoMeasurements.HeadHeight ?? 0) : 0;
                    if(headRatioCurrent > 0) {
                        // morph preference "Head big" (reloaded lite) and then "Head Scale" (built in)
                        // head big:
                        // - increasing the morph makes head bigger, which reduces the RATIO 
                        // head scale:
                        // - increasing the morph makes head smaller, which increases the RATIO
                        if(sexWasChanged || targetHeadMorphChanged) {
                            _headScaleMorph = null;
                        }

                        if(_headScaleMorph == null) {
                            _headScaleMorph = GetMorphByName(_ui.targetHeadRatioMorphStorable.val);
                            /// hmm try all the morphs in the list
                            if(_headScaleMorph == null) {
                                foreach(var morphName in _ui.targetHeadRatioMorphStorable.choices) {
                                    _headScaleMorph = GetMorphByName(morphName);
                                    if(_headScaleMorph != null) {
                                        break;
                                    }
                                }

                            }
                        }
                        else {
                            int makeHeadBiggerMorphDirection = _headScaleMorph.displayName.Equals("Head big") ? 1 : -1;
                            int precision = 2;
                            if(Mathf.RoundToInt(headRatioCurrent*(precision+1)*100) != Mathf.RoundToInt(headRatioTarget*(precision+1)*100)) {
                                var offByPct = Mathf.Abs(headRatioTarget-headRatioCurrent)/headRatioCurrent * 100;
                                var morphValue = _headScaleMorph.morphValue;
                                var changeBy = 0.02f * offByPct;
                                if(changeBy != 0) {
                                    var newValue = (headRatioCurrent > headRatioTarget) 
                                        ? morphValue + (makeHeadBiggerMorphDirection * changeBy)
                                        : morphValue - (makeHeadBiggerMorphDirection * changeBy);
                                    if(newValue > _headScaleMorph.min && newValue < _headScaleMorph.max) {
                                        _headScaleMorph.morphValue = newValue;
                                    }
                                }
                            }
                        }
                    }
                }

                // adjust manual markers if scale of the containing atom has changed
                var currScale = GetScale();
                bool shouldScaleManualMarkers = _prevScale != 0 && _prevScale != currScale && _ui.manualMarkersCopyFrom.val == String.Empty;
                if(shouldScaleManualMarkers) {
                    ScaleStaticMeasurements(_ui, _prevScale, currScale);
                }
                _prevScale = currScale;

            }
            catch(Exception e) {
                SuperController.LogError($"{e}");
                return;
            }

            // remeasure model and display things
            try {
                _autoMeasurements = AutoMeasurements(_ui, containingAtom);

                _manualMeasurements = StaticMeasurements(_ui);
                _manualMeasurements.HeelToFloorOffset = _autoMeasurements.HeelToFloorOffset;

                ManualMarkersCopyFrom(_ui.manualMarkersCopyFrom.val);

                _ui.headSizeHeightStorable.val = _autoMeasurements.HeadHeight ?? 0;
                _ui.headSizeWidthStorable.val = _autoMeasurements.HeadWidth ?? 0;
                _ui.fullHeightStorable.val = _autoMeasurements.Height ?? 0;
                _ui.heightInHeadsStorable.val = _ui.headSizeHeightStorable.val == 0 ? 0 : _ui.fullHeightStorable.val / _ui.headSizeHeightStorable.val;
                _ui.chinHeightStorable.val = _autoMeasurements.ChinHeight ?? 0;
                _ui.shoulderHeightStorable.val = _autoMeasurements.ShoulderHeight ?? 0;
                _ui.shoulderWidthStorable.val = _autoMeasurements.ShoulderWidth ?? 0;
                _ui.armLengthStorable.val = _autoMeasurements.ArmLength ?? 0;
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

                _ui.ageGuessLowest.val = _ageGuides.AgeGuessPercentileLowest;
                _ui.ageGuessLowestLikely.val = _ageGuides.AgeGuessPercentileLowestLikely;
                _ui.ageGuessHighest.val = _ageGuides.AgeGuessPercentileHighest;
                _ui.ageGuessHighestLikely.val = _ageGuides.AgeGuessPercentileHighestLikely;

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
                _mainGuides.ShowDocumentation = !_ui.hideDocsStorable.val;
                _mainGuides.Enabled = _ui.showFeatureMarkersStorable.val;
                _mainGuides.LabelsEnabled = _ui.showFeatureMarkerLabelsStorable.val;
                _mainGuides.LinesEnabled = _ui.showFeatureMarkerLinesStorable.val;
                _mainGuides.LineColor = HSVToColor(_ui.featureMarkerColor.val);
                _mainGuides.LineThickness = _ui.lineThicknessFigureStorable.val;
                _mainGuides.UnitDisplay = _ui.unitsStorable.val;
                _mainGuides.Offset = pos - spreadVector;
                _mainGuides.OffsetSpread = spreadVector;
                _mainGuides.ShowHeight = _ui.showFeatureMarkerLineHeight.val;
                _mainGuides.ShowChin = _ui.showFeatureMarkerLineChin.val;
                _mainGuides.ShowShoulder = _ui.showFeatureMarkerLineShoulder.val;
                _mainGuides.ShowShoulderWidth = _ui.showFeatureMarkerLineShoulderWidth.val;
                _mainGuides.ShowArm = _ui.showFeatureMarkerLineArm.val;
                _mainGuides.ShowBust = _ui.showFeatureMarkerLineBust.val;
                _mainGuides.ShowUnderbust = _ui.showFeatureMarkerLineUnderbust.val;
                _mainGuides.ShowNavel = _ui.showFeatureMarkerLineNavel.val;
                _mainGuides.ShowCrotch = _ui.showFeatureMarkerLineCrotch.val;
                _mainGuides.ShowKnee = _ui.showFeatureMarkerLineKnee.val;
                _mainGuides.ShowHeel = _ui.showFeatureMarkerLineHeel.val;
                _mainGuides.FlipDirection = _ui.flipFeatureMarkerSide.val;

                // auto head height guide
                _headGuides.ShowDocumentation = !_ui.hideDocsStorable.val;
                _headGuides.Enabled = _ui.showHeadHeightMarkersStorable.val;
                _headGuides.LabelsEnabled = true;
                _headGuides.LinesEnabled = true;
                _headGuides.LineColor = HSVToColor(_ui.headMarkerColor.val);
                _headGuides.LineThickness = _ui.lineThicknessHeadStorable.val;
                _headGuides.UnitDisplay = _ui.unitsStorable.val;
                _headGuides.Offset = pos + spreadVector;
                _headGuides.OffsetSpread = spreadVector;
                _headGuides.FlipDirection = _ui.flipHeadMarkerSide.val;


                // auto face guide
                _faceGuides.ShowDocumentation = !_ui.hideDocsStorable.val;
                _faceGuides.Enabled = _ui.showFaceMarkersStorable.val;
                _faceGuides.LabelsEnabled = true;
                _faceGuides.LinesEnabled = true;
                _faceGuides.LineColor = HSVToColor(_ui.faceMarkerColor.val);
                _faceGuides.LineThickness = _ui.lineThicknessFaceStorable.val;
                _faceGuides.UnitDisplay = _ui.unitsStorable.val;
                _faceGuides.Offset = pos + new Vector3(0, 0, -0.03f);
                _faceGuides.OffsetSpread = spreadVector;

                // bust guide
                _bustGuides.ShowDocumentation = !_ui.hideDocsStorable.val;
                _bustGuides.Enabled = _ui.showCircumferenceMarkersStorable.val && !isMale;
                _bustGuides.LabelsEnabled = false;
                _bustGuides.LinesEnabled = true;
                _bustGuides.LineColor = HSVToColor(_ui.circumferenceMarkerColor.val);
                _bustGuides.LineThickness = _ui.lineThicknessCircumferenceStorable.val;
                _bustGuides.UnitDisplay = _ui.unitsStorable.val;
                _bustGuides.Offset = pos;
                _bustGuides.OffsetSpread = spreadVector;
                _bustGuides.Points = _autoMeasurements.POI?.BustPoints ?? new Vector3[0];

                // underbust guide
                _underbustGuides.ShowDocumentation = !_ui.hideDocsStorable.val;
                _underbustGuides.Enabled = _ui.showCircumferenceMarkersStorable.val && !isMale;
                _underbustGuides.LabelsEnabled = false;
                _underbustGuides.LinesEnabled = true;
                _underbustGuides.LineColor = HSVToColor(_ui.circumferenceMarkerColor.val);
                _underbustGuides.LineThickness = _ui.lineThicknessCircumferenceStorable.val;
                _underbustGuides.UnitDisplay = _ui.unitsStorable.val;
                _underbustGuides.Offset = pos;
                _underbustGuides.OffsetSpread = spreadVector;
                _underbustGuides.Points = _autoMeasurements.POI?.UnderbustPoints ?? new Vector3[0];

                // waist guide
                _waistGuides.ShowDocumentation = !_ui.hideDocsStorable.val;
                _waistGuides.Enabled = _ui.showCircumferenceMarkersStorable.val && !isMale;
                _waistGuides.LabelsEnabled = false;
                _waistGuides.LinesEnabled = true;
                _waistGuides.LineColor = HSVToColor(_ui.circumferenceMarkerColor.val);
                _waistGuides.LineThickness = _ui.lineThicknessCircumferenceStorable.val;
                _waistGuides.UnitDisplay = _ui.unitsStorable.val;
                _waistGuides.Offset = pos;
                _waistGuides.OffsetSpread = spreadVector;
                _waistGuides.Points = _autoMeasurements.POI?.WaistPoints ?? new Vector3[0];

                // hip guide
                _hipGuides.ShowDocumentation = !_ui.hideDocsStorable.val;
                _hipGuides.Enabled = _ui.showCircumferenceMarkersStorable.val && !isMale;
                _hipGuides.LabelsEnabled = false;
                _hipGuides.LinesEnabled = true;
                _hipGuides.LineColor = HSVToColor(_ui.circumferenceMarkerColor.val);
                _hipGuides.LineThickness = _ui.lineThicknessCircumferenceStorable.val;
                _hipGuides.UnitDisplay = _ui.unitsStorable.val;
                _hipGuides.Offset = pos;
                _hipGuides.OffsetSpread = spreadVector;
                _hipGuides.Points = _autoMeasurements.POI?.HipPoints ?? new Vector3[0];

                var proportionClosestMatch = _autoMeasurements.Proportions.ClostestMatch(_ui.ProportionTemplates);

                // age guide
                _ageGuides.ShowDocumentation = !_ui.hideDocsStorable.val;
                _ageGuides.Enabled = _ui.showAgeMarkersStorable.val;
                _ageGuides.LabelsEnabled = true;
                _ageGuides.LinesEnabled = true;
                _ageGuides.LineColor = Color.cyan;
                _ageGuides.LineThickness = 2.0f; // TODO
                _ageGuides.UnitDisplay = _ui.unitsStorable.val;
                _ageGuides.Offset = pos;
                _ageGuides.OffsetSpread = spreadVector;
                _ageGuides.TargetProportion = proportionClosestMatch;
                _ageGuides.HeadAgeVisuals = _ui.ageMarkerShowHeadVisual.val;
                _ageGuides.HeightAgeVisuals = _ui.ageMarkerShowHeightVisual.val;
                _ageGuides.ProportionAgeVisuals = _ui.ageMarkerShowProportionVisual.val;
                _ageGuides.WarnUnderage = _ui.ageWarnUnderage.val;

                // proportion guide
                _proportionGuides.ShowDocumentation = !_ui.hideDocsStorable.val;
                _proportionGuides.Enabled = _ui.showProportionMarkersStorable.val;
                _proportionGuides.LabelsEnabled = true;
                _proportionGuides.LinesEnabled = true;
                _proportionGuides.LineColor = HSVToColor(_ui.proportionMarkerColor.val);
                _proportionGuides.LineThickness = _ui.lineThicknessProportionStorable.val;
                _proportionGuides.ShowCalculate = _ui.showProportionMarkerCalc.val;
                _proportionGuides.FlipDirection = _ui.flipProportionMarkerSide.val;
                _proportionGuides.UnitDisplay = _ui.unitsStorable.val;
                _proportionGuides.Offset = pos - spreadVector - new Vector3(0, 0, 0.004f); // put these just a bit behind the auto guides
                _proportionGuides.OffsetSpread = spreadVector;
                _proportionGuides.TargetProportion = _ui.ProportionTemplates.FirstOrDefault(p => p.ProportionName.Equals(_ui.proportionSelectionStorable.val)) ?? proportionClosestMatch;
                _proportionGuides.ShowHeight = _ui.showProportionMarkerLineHeight.val;
                _proportionGuides.ShowChin = _ui.showProportionMarkerLineChin.val;
                _proportionGuides.ShowShoulder = _ui.showProportionMarkerLineShoulder.val;
                _proportionGuides.ShowShoulderWidth = _ui.showProportionMarkerLineShoulderWidth.val;
                _proportionGuides.ShowArm = _ui.showProportionMarkerLineArm.val;
                _proportionGuides.ShowNipple = _ui.showProportionMarkerLineBust.val;
                _proportionGuides.ShowNavel = _ui.showProportionMarkerLineNavel.val;
                _proportionGuides.ShowCrotch = _ui.showProportionMarkerLineCrotch.val;
                _proportionGuides.ShowKnee = _ui.showProportionMarkerLineKnee.val;
                _proportionGuides.ShowHeel = _ui.showProportionMarkerLineHeel.val;
            }

            // manual feature guide
            _mainGuidesManual.ShowDocumentation = !_ui.hideDocsStorable.val;
            _mainGuidesManual.Enabled = _ui.showManualMarkersStorable.val;
            _mainGuidesManual.LabelsEnabled = _ui.showManualMarkersStorable.val && _mainGuidesManual.Measurements.Height > 0 && _ui.showManualMarkerLabelsStorable.val;
            _mainGuidesManual.LinesEnabled = true;
            _mainGuidesManual.LineColor = HSVToColor(_ui.manualMarkerColor.val);
            _mainGuidesManual.LineThickness = _ui.lineThicknessManualStorable.val;
            _mainGuidesManual.FlipDirection = _ui.flipManualMarkerSide.val;
            _mainGuidesManual.UnitDisplay = _ui.unitsStorable.val;
            _mainGuidesManual.Offset = pos - spreadVector - new Vector3(0, 0, 0.002f); // put these just a bit behind the auto guides
            _mainGuidesManual.OffsetSpread = spreadVector;

            // manual head height guide
            _headGuidesManual.ShowDocumentation = !_ui.hideDocsStorable.val;
            _headGuidesManual.Enabled = _ui.showManualMarkersStorable.val && _headGuidesManual.Measurements.Height > 0 && _ui.showManualMarkerHeadsGuideStorable.val;
            _headGuidesManual.LabelsEnabled = true;
            _headGuidesManual.LinesEnabled = true;
            _headGuidesManual.LineColor = HSVToColor(_ui.manualMarkerColor.val);
            _headGuidesManual.LineThickness = _ui.lineThicknessManualStorable.val;
            _headGuidesManual.FlipDirection = _ui.flipManualMarkerSide.val;
            _headGuidesManual.UnitDisplay = _ui.unitsStorable.val;
            _headGuidesManual.Offset = pos + spreadVector - new Vector3(0, 0, 0.002f); // put these just a bit behind the auto guides
            _headGuidesManual.OffsetSpread = spreadVector;
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

        private string _copyFromUidPrev;

        private JSONStorableFloat _copyHeightStorable;
        private JSONStorableFloat _copyChinStorable;
        private JSONStorableFloat _copyShoulderStorable;
        private JSONStorableFloat _copyNippleStorable;
        private JSONStorableFloat _copyUnderbustStorable;
        private JSONStorableFloat _copyNavelStorable;
        private JSONStorableFloat _copyCrotchStorable;
        private JSONStorableFloat _copyKneeStorable;

        private Atom _copyFromAtom;
        public void ManualMarkersCopyFrom(string uid) {
            if(uid == String.Empty || uid == null) {
                _copyFromUidPrev = uid;
                return;
            }

            bool isPerson = _copyFromAtom?.type == "Person";
            if(_copyFromUidPrev == null || _copyFromUidPrev != uid) {
                _copyFromAtom = SuperController.singleton.GetAtomByUid(uid);
                isPerson = _copyFromAtom?.type == "Person";
                var plugin = GetHeightMeasurePluginForAtom(_copyFromAtom);
                _copyHeightStorable = plugin?.GetFloatJSONParam(isPerson ? "figureHeight" : "Height");
                _copyChinStorable = plugin?.GetFloatJSONParam(isPerson ? "chinHeight" : "Chin Height");
                _copyShoulderStorable = plugin?.GetFloatJSONParam(isPerson ? "shoulderHeight" : "Shoulder Height");
                _copyNippleStorable = plugin?.GetFloatJSONParam(isPerson ? "nippleHeight" : "Bust Height");
                _copyUnderbustStorable = plugin?.GetFloatJSONParam(isPerson ? "underbustHeight" : "Underbust Height");
                _copyNavelStorable = plugin?.GetFloatJSONParam(isPerson ? "navelHeight" : "Navel Height");
                _copyCrotchStorable = plugin?.GetFloatJSONParam(isPerson ? "crotchHeight" : "Crotch Height");
                _copyKneeStorable = plugin?.GetFloatJSONParam(isPerson ? "kneeHeight" : "Knee Height");
                _copyFromUidPrev = uid;
            }

            float multiplier = isPerson ? 100 : 1;
            if(_ui.manualMarkersCopyRelative.val) {
                float myHeight = _ui.fullHeightStorable.val;
                float theirHeight = (_copyHeightStorable?.val ?? 0) / (isPerson ? 1 : 100);
                if(myHeight > 0 && theirHeight > 0) {
                    multiplier *= myHeight/theirHeight;
                }
            }


            _ui.manualHeightStorable.val = (_copyHeightStorable?.val ?? 0) * multiplier;
            _ui.manualChinHeightStorable.val = (_copyChinStorable?.val ?? 0) * multiplier;
            _ui.manualShoulderHeightStorable.val = (_copyShoulderStorable?.val ?? 0) * multiplier;
            _ui.manualNippleHeightStorable.val = (_copyNippleStorable?.val ?? 0) * multiplier;
            _ui.manualUnderbustHeightStorable.val = (_copyUnderbustStorable?.val ?? 0) * multiplier;
            _ui.manualNavelHeightStorable.val = (_copyNavelStorable?.val ?? 0) * multiplier;
            _ui.manualCrotchHeightStorable.val = (_copyCrotchStorable?.val ?? 0) * multiplier;
            _ui.manualKneeBottomHeightStorable.val = (_copyKneeStorable?.val ?? 0) * multiplier;
        }

        private JSONStorableFloat _scaleStorable;
        private JSONStorableFloat GetScaleStorable() {
            if(_scaleStorable == null) {
                JSONStorable scaleStorable = containingAtom.GetStorableByID("rescaleObject") ?? containingAtom.GetStorableByID("scale");
                if(scaleStorable != null) {
                    JSONStorableFloat scale = scaleStorable.GetFloatJSONParam("scale");
                    if(scale != null) {
                        _scaleStorable = scale;
                    }
                }
            }
            return _scaleStorable;
        }

        private float GetScale() {
            var scale = GetScaleStorable();
            if(scale != null) {
                return scale.val;
            }
            return 0;
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
            var shoulderLeftPos = poi.ShoulderLeftSide;
            var shoulderRightPos = poi.ShoulderRightSide;
            var fingertipRightPos = poi.FingerTipRightSide;
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
            
            var footOffset = Vector3.Dot(footPos - rootPos, rootTransform.up);

            // set measurements
            measurements.HeelToFloorOffset = Vector3.up * footOffset;
            measurements.Height = Vector3.Dot(headPos - footPos, rootTransform.up);
            measurements.ChinHeight = Vector3.Dot(chinPos - footPos, rootTransform.up);
            measurements.HeadWidth = Vector3.Distance(poi.CraniumLeftSide, poi.CraniumRightSide);
            measurements.ShoulderHeight = Vector3.Dot(shoulderPos - footPos, rootTransform.up);
            measurements.ShoulderWidth = LineLength(new Vector3[] {shoulderLeftPos, shoulderRightPos}) + poi.ShoulderColliderRadius;
            measurements.ArmLength = LineLength(new Vector3[] {shoulderRightPos, fingertipRightPos});
            measurements.NippleHeight = poi.IsMale ? (float?)null : Vector3.Dot(nipplePos - footPos, rootTransform.up);
            measurements.UnderbustHeight = poi.IsMale ? (float?)null : Vector3.Dot(underbustPos - footPos, rootTransform.up);
            measurements.NavelHeight = Vector3.Dot(navelPos - footPos, rootTransform.up);
            measurements.CrotchHeight = Vector3.Dot(crotchPos - footPos, rootTransform.up);
            measurements.KneeHeight = Vector3.Dot(kneePos - footPos, rootTransform.up);
            measurements.HeelHeight = 0;

            // SuperController.singleton.ClearMessages();
            // SuperController.LogMessage($"height={measurements.Height}");
            // SuperController.LogMessage($"rootPos={rootPos}");
            // SuperController.LogMessage($"footOffset={footOffset}");
            // SuperController.LogMessage($"footPos={footPos}");
            // SuperController.LogMessage($"floor={floor}");
            // SuperController.LogMessage($"floorDistanceOffset={floorDistanceOffset}");


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

            if(measurements.Height != null) {
                measurements.AgeFromHeight = poi.IsMale
                    ? _ageFromHeightCalculator.GuessMaleAges(measurements.Height.Value)
                    : _ageFromHeightCalculator.GuessFemaleAges(measurements.Height.Value); 
            }

            if(measurements.HeadHeight != null) {
                var heightInHeads = (float)measurements.Height.Value/measurements.HeadHeight.Value;
                measurements.AgeFromHead = poi.IsMale
                    ? _ageFromHeadRatioCalculator.GuessMaleAges(heightInHeads)
                    : _ageFromHeadRatioCalculator.GuessFemaleAges(heightInHeads);
            }

            measurements.POI = poi;

            return measurements;
        }

        public void ScaleStaticMeasurements(UI ui, float fromScale, float toScale) {
            if(fromScale == toScale) {
                return;
            }
            if(ui.manualHeightStorable.val <= 0) {
                return;
            }
            var changePercentage = toScale/fromScale;
            ui.manualHeightStorable.val *= changePercentage;
            ui.manualChinHeightStorable.val *= changePercentage;
            ui.manualShoulderHeightStorable.val *= changePercentage;
            ui.manualNippleHeightStorable.val *= changePercentage;
            ui.manualUnderbustHeightStorable.val *= changePercentage;
            ui.manualNavelHeightStorable.val *= changePercentage;
            ui.manualCrotchHeightStorable.val *= changePercentage;
            ui.manualKneeBottomHeightStorable.val *= changePercentage;
            ui.markerUpDownStorable.val *= changePercentage;
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


        private JSONStorable _hmpCached;
        public JSONStorable GetHeightMeasurePluginForAtom(Atom atom) {
            if(atom == null) {
                return null;
            }
            if(_hmpCached == null || (_hmpCached != null && _hmpCached.containingAtom.uid != atom.uid)) {
                _hmpCached = null;
                foreach(var s in GetPluginStorables(atom)) {
                    if(s.name.Contains("LFE.HeightMeasurePlugin")) {
                        _hmpCached = s;
                    }
                }
            }
            return _hmpCached;
        }

        private Dictionary<string, MVRPluginManager> atomPluginManagers = new Dictionary<string, MVRPluginManager>();
        public IEnumerable<JSONStorable> GetPluginStorables(Atom atom)
        {
            if(atomPluginManagers.Count == 0) {
                foreach(var c in transform.root.GetComponentsInChildren<MVRPluginManager>(true)) {
                    atomPluginManagers[c.containingAtom.uid] = c;
                }
            }

            if(atomPluginManagers.ContainsKey(atom.uid)) {
                var manager = atomPluginManagers[atom.uid];
                var plugins = manager.GetJSON(true, true)["plugins"].AsObject;
                foreach(var pluginId in plugins.Keys)
                {
                    var receivers = atom
                        .GetStorableIDs()
                        .Where((sid) => sid.StartsWith(pluginId))
                        .Select((sid) => atom.GetStorableByID(sid));
                    foreach(var r in receivers)
                    {
                        yield return r;
                    }
                }
            }
        }

    }
}
