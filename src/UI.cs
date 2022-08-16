// #define LFE_EXPERIMENTAL
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace LFE {
    public class UI {

        private readonly ICupCalculator[] _cupCalculators = new ICupCalculator[] {
            new EvasIntimatesCupCalculator(),
            new EvasIntimatesFRCupCalculator(),
            new KnixComCupCalculator(),
            new SizeChartAUSCupCalculator(),
            new SizeChartCupCalculator(),
            new SizeChartEUCupCalculator(),
            new SizeChartFRCupCalculator(),
            new ChateLaineCupCalculator(),
            new KSK9404CupCalculator()
        };

        private List<Proportions> _proportionTemplates = new List<Proportions>();

        public List<Proportions> ProportionTemplates {
            get { return _proportionTemplates; }
            set { _proportionTemplates = value ?? new List<Proportions>(); Draw(); }
        }

        private static Color HEADER_COLOR = new Color(0.5f, 0f, 0.5f, 0.65f);
        private static Color HEADER_TEXT_COLOR = Color.white;
        private static Color OPTIONS_COLOR = HEADER_COLOR;
        private static Color OPTIONS_TEXT_COLOR = HEADER_TEXT_COLOR;
        private static Color SPACER_COLOR = new Color(0, 0, 0, 0.5f);

        private readonly HeightMeasurePlugin _plugin;

        public JSONStorableBool experimentalModeStorable;

        // measurement storables
        public JSONStorableFloat fullHeightStorable;
        public JSONStorableFloat headSizeHeightStorable;
        public JSONStorableFloat headSizeWidthStorable;
        public JSONStorableFloat chinHeightStorable;
        public JSONStorableFloat shoulderHeightStorable;
        public JSONStorableFloat shoulderWidthStorable;
        public JSONStorableFloat armLengthStorable;
        public JSONStorableFloat nippleHeightStorable;
        public JSONStorableFloat underbustHeightStorable;
        public JSONStorableFloat navelHeightStorable;
        public JSONStorableFloat crotchHeightStorable;
        public JSONStorableFloat kneeBottomHeightStorable;
        public JSONStorableFloat heightInHeadsStorable;
        public JSONStorableFloat breastBustStorable;
        public JSONStorableFloat breastUnderbustStorable;
        public JSONStorableFloat breastBandStorable;
        public JSONStorableString breastCupStorable;
        public JSONStorableFloat waistSizeStorable;
        public JSONStorableFloat hipSizeStorable;
        public JSONStorableFloat penisLength;
        public JSONStorableFloat penisWidth;
        public JSONStorableFloat penisGirth;


        public JSONStorableBool showAgeMarkersStorable;
        public JSONStorableBool ageMarkerShowHeadVisual;
        public JSONStorableBool ageMarkerShowHeightVisual;
        public JSONStorableBool ageMarkerShowProportionVisual;
        public JSONStorableBool ageWarnUnderage;

        public JSONStorableFloat ageGuessLowest;
        public JSONStorableFloat ageGuessLowestLikely;
        public JSONStorableFloat ageGuessHighestLikely;
        public JSONStorableFloat ageGuessHighest;

        public JSONStorableString proportionClosestMatch;

        // manual heights
        public JSONStorableBool showManualMarkersStorable;
        public JSONStorableBool manualMarkersCopy;
        public JSONStorableColor manualMarkerColor;
        public JSONStorableFloat lineThicknessManualStorable;

        public JSONStorableFloat manualHeightStorable;
        public JSONStorableFloat manualChinHeightStorable;
        public JSONStorableFloat manualShoulderHeightStorable;
        public JSONStorableFloat manualNippleHeightStorable;
        public JSONStorableFloat manualUnderbustHeightStorable;
        public JSONStorableFloat manualNavelHeightStorable;
        public JSONStorableFloat manualCrotchHeightStorable;
        public JSONStorableFloat manualKneeBottomHeightStorable;


        // other storables
        public JSONStorableBool showFeatureMarkersStorable;
        public JSONStorableColor featureMarkerColor;
        public JSONStorableFloat lineThicknessFigureStorable;
        public JSONStorableBool showFeatureMarkerLabelsStorable;
        public JSONStorableBool showFeatureMarkerLinesStorable;
        public JSONStorableBool showFeatureMarkerLineHeight;
        public JSONStorableBool showFeatureMarkerLineChin;
        public JSONStorableBool showFeatureMarkerLineBust;
        public JSONStorableBool showFeatureMarkerLineUnderbust;
        public JSONStorableBool showFeatureMarkerLineNavel;
        public JSONStorableBool showFeatureMarkerLineCrotch;
        public JSONStorableBool showFeatureMarkerLineKnee;
        public JSONStorableBool showFeatureMarkerLineHeel;
        public JSONStorableBool showFeatureMarkerLineShoulder;
        public JSONStorableBool showFeatureMarkerLineShoulderWidth;
        public JSONStorableBool showFeatureMarkerLineArm;
        public JSONStorableBool flipFeatureMarkerSide;



        public JSONStorableBool showHeadHeightMarkersStorable;
        public JSONStorableColor headMarkerColor;
        public JSONStorableFloat lineThicknessHeadStorable;
        public JSONStorableBool flipHeadMarkerSide;

        public JSONStorableBool showFaceMarkersStorable;
        public JSONStorableColor faceMarkerColor;
        public JSONStorableFloat lineThicknessFaceStorable;

        public JSONStorableBool showCircumferenceMarkersStorable;
        public JSONStorableColor circumferenceMarkerColor;
        public JSONStorableFloat lineThicknessCircumferenceStorable;

        public JSONStorableBool showProportionMarkersStorable;
        public JSONStorableColor proportionMarkerColor;
        public JSONStorableFloat lineThicknessProportionStorable;
        public JSONStorableStringChooser proportionSelectionStorable;
        public JSONStorableBool showProportionMarkerLineHeight;
        public JSONStorableBool showProportionMarkerLineChin;
        public JSONStorableBool showProportionMarkerLineBust;
        public JSONStorableBool showProportionMarkerLineNavel;
        public JSONStorableBool showProportionMarkerLineCrotch;
        public JSONStorableBool showProportionMarkerLineKnee;
        public JSONStorableBool showProportionMarkerLineHeel;
        public JSONStorableBool showProportionMarkerLineShoulder;
        public JSONStorableBool showProportionMarkerLineShoulderWidth;
        public JSONStorableBool showProportionMarkerLineArm;
        public JSONStorableBool flipProportionMarkerSide;

        public JSONStorableBool showTargetHeadRatioStorable;
        public JSONStorableFloat targetHeadRatioStorable;
        public JSONStorableStringChooser targetHeadRatioMorphStorable;

        public JSONStorableStringChooser cupAlgorithmStorable;
        public JSONStorableStringChooser unitsStorable;
        public JSONStorableFloat markerSpreadStorable;
        public JSONStorableFloat markerLeftRightStorable;
        public JSONStorableFloat markerFrontBackStorable;
        public JSONStorableFloat markerUpDownStorable;
        public JSONStorableBool hideDocsStorable;

        public ICupCalculator CupCalculator => _cupCalculators.FirstOrDefault(c => c.Name.Equals(cupAlgorithmStorable.val));

        public UI(HeightMeasurePlugin plugin) {
            _plugin = plugin;
            _proportionTemplates = Proportions.CommonProportions;
            InitStorables();
            Draw();
        }

        private bool _choosingFeatureColor = false;
        private bool _choosingHeadColor = false;
        private bool _choosingFaceColor = false;
        private bool _choosingCircumferenceColor = false;
        private bool _choosingManualColor = false;
        private bool _choosingProportionColor = false;

        private bool _editingProportion = false;
        private bool _creatingProportion = false;
        private Proportions _preEditProportion;
        private void InitStorables() {

#if LFE_EXPERIMENTAL
            experimentalModeStorable = new JSONStorableBool("Enable Experiments", true);
#else
            experimentalModeStorable = new JSONStorableBool("Enable Experiments", false);
#endif
            experimentalModeStorable.setCallbackFunction = (bool value) => {
                experimentalModeStorable.valNoCallback = value;
                Draw();
            };
            _plugin.RegisterBool(experimentalModeStorable);

            //////////////////////////////////////
            // UI related
            // Cup algorithm choice

            showFeatureMarkersStorable = new JSONStorableBool("Auto Feature Guides", true, (bool value) => {
                showFeatureMarkersStorable.valNoCallback = value;
                Draw();
            });
            showFeatureMarkersStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showFeatureMarkersStorable);

            lineThicknessFigureStorable = new JSONStorableFloat("Feature Guide Line Width", 2, 1, 10, constrain: true);
            lineThicknessFigureStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(lineThicknessFigureStorable);

            featureMarkerColor = new JSONStorableColor("Feature Guide Color", _plugin.ColorToHSV(Color.green), (float h, float s, float v) => {
                var hsv = new HSVColor { H = h, S = s, V = v };
                featureMarkerColor.valNoCallback = hsv;
                if(_featureMarkerColorButton != null) {
                    _featureMarkerColorButton.buttonColor = _plugin.HSVToColor(hsv);
                }
            });
            featureMarkerColor.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterColor(featureMarkerColor);

            showFeatureMarkerLabelsStorable = new JSONStorableBool("Feature Guide Labels", true, (bool value) => {
                Draw();
            });
            showFeatureMarkerLabelsStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showFeatureMarkerLabelsStorable);

            showFeatureMarkerLinesStorable = new JSONStorableBool("Feature Guide Lines", true, (bool value) => {
                Draw();
            });
            showFeatureMarkerLinesStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showFeatureMarkerLinesStorable);


            flipFeatureMarkerSide = new JSONStorableBool("Feature Guide Flip Side", false, (bool value) => {
                Draw();
            });
            flipFeatureMarkerSide.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(flipFeatureMarkerSide);

            showFeatureMarkerLineHeight = new JSONStorableBool("Show Feature - Height", true);
            showFeatureMarkerLineHeight.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showFeatureMarkerLineHeight);

            showFeatureMarkerLineChin = new JSONStorableBool("Show Feature - Chin", true);
            showFeatureMarkerLineChin.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showFeatureMarkerLineChin);

            showFeatureMarkerLineBust = new JSONStorableBool("Show Feature - Bust", true);
            showFeatureMarkerLineBust.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showFeatureMarkerLineBust);

            showFeatureMarkerLineUnderbust = new JSONStorableBool("Show Feature - Underbust", true);
            showFeatureMarkerLineUnderbust.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showFeatureMarkerLineUnderbust);

            showFeatureMarkerLineNavel = new JSONStorableBool("Show Feature - Navel", true);
            showFeatureMarkerLineNavel.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showFeatureMarkerLineNavel);

            showFeatureMarkerLineCrotch = new JSONStorableBool("Show Feature - Crotch", true);
            showFeatureMarkerLineCrotch.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showFeatureMarkerLineCrotch);

            showFeatureMarkerLineKnee = new JSONStorableBool("Show Feature - Knee", true);
            showFeatureMarkerLineKnee.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showFeatureMarkerLineKnee);

            showFeatureMarkerLineHeel = new JSONStorableBool("Show Feature - Heel", true);
            showFeatureMarkerLineHeel.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showFeatureMarkerLineHeel);

            showFeatureMarkerLineShoulder = new JSONStorableBool("Show Feature - Shoulder", true);
            showFeatureMarkerLineShoulder.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showFeatureMarkerLineShoulder);

            showFeatureMarkerLineShoulderWidth = new JSONStorableBool("Show Feature - Shoulder Width", true);
            showFeatureMarkerLineShoulderWidth.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showFeatureMarkerLineShoulderWidth);

            showFeatureMarkerLineArm = new JSONStorableBool("Show Feature - Arm", true);
            showFeatureMarkerLineArm.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showFeatureMarkerLineArm);

            //////////////////

            showHeadHeightMarkersStorable = new JSONStorableBool("Auto Head Heights Guides", true, (bool value) => {
                showHeadHeightMarkersStorable.valNoCallback = value;
                Draw();
            });
            showHeadHeightMarkersStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showHeadHeightMarkersStorable);

            lineThicknessHeadStorable = new JSONStorableFloat("Head Heights Line Width", 2, 1, 10, constrain: true);
            lineThicknessHeadStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(lineThicknessHeadStorable);

            headMarkerColor = new JSONStorableColor("Head Guide Color", _plugin.ColorToHSV(Color.white), (float h, float s, float v) => {
                var hsv = new HSVColor { H = h, S = s, V = v };
                headMarkerColor.valNoCallback = hsv;
                if(_headMarkerColorButton != null) {
                    _headMarkerColorButton.buttonColor = _plugin.HSVToColor(hsv);
                }
            });
            headMarkerColor.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterColor(headMarkerColor);

            flipHeadMarkerSide = new JSONStorableBool("Head Guide Flip Side", false, (bool value) => {
                Draw();
            });
            flipHeadMarkerSide.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(flipHeadMarkerSide);

            //////////////////

            showFaceMarkersStorable = new JSONStorableBool("Auto Face Guides", false, (bool value) => {
                Draw();
            });
            showFaceMarkersStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showFaceMarkersStorable);

            lineThicknessFaceStorable = new JSONStorableFloat("Face Line Width", 2, 1, 10, constrain: true);
            lineThicknessFaceStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(lineThicknessFaceStorable);

            faceMarkerColor = new JSONStorableColor("Face Guide Color", _plugin.ColorToHSV(Color.blue), (float h, float s, float v) => {
                var hsv = new HSVColor { H = h, S = s, V = v };
                faceMarkerColor.valNoCallback = hsv;
                if(_faceMarkerColorButton != null) {
                    _faceMarkerColorButton.buttonColor = _plugin.HSVToColor(hsv);
                }
            });
            faceMarkerColor.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterColor(faceMarkerColor);

            //////////////////

            showCircumferenceMarkersStorable = new JSONStorableBool("Auto Circumference Guides", false, (bool value) => {
                showCircumferenceMarkersStorable.valNoCallback = value;
                Draw();
            });
            showCircumferenceMarkersStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showCircumferenceMarkersStorable);

            lineThicknessCircumferenceStorable = new JSONStorableFloat("Circumference Guide Line Width", 2, 1, 10, constrain: true);
            lineThicknessCircumferenceStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(lineThicknessCircumferenceStorable);

            circumferenceMarkerColor = new JSONStorableColor("Circumference Guide Color", _plugin.ColorToHSV(Color.green), (float h, float s, float v) => {
                var hsv = new HSVColor { H = h, S = s, V = v };
                circumferenceMarkerColor.valNoCallback = hsv;
                if(_circumferenceMarkerColorButton != null) {
                    _circumferenceMarkerColorButton.buttonColor = _plugin.HSVToColor(hsv);
                }
            });
            circumferenceMarkerColor.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterColor(circumferenceMarkerColor);

            //////////////////
            showProportionMarkersStorable = new JSONStorableBool("Proportion Guides", false, (bool value) => {
                Draw();
            });
            showProportionMarkersStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showProportionMarkersStorable);

            lineThicknessProportionStorable = new JSONStorableFloat("Proportion Line Width", 2, 1, 10, constrain: true);
            lineThicknessProportionStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(lineThicknessProportionStorable);

            proportionMarkerColor = new JSONStorableColor("Proportion Guide Color", _plugin.ColorToHSV(Color.yellow), (float h, float s, float v) => {
                var hsv = new HSVColor { H = h, S = s, V = v };
                proportionMarkerColor.valNoCallback = hsv;
                if(_proportionMarkerColorButton != null) {
                    _proportionMarkerColorButton.buttonColor = _plugin.HSVToColor(hsv);
                }
            });
            proportionMarkerColor.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterColor(proportionMarkerColor);

            var proportionNames = ProportionTemplateNames();
            proportionSelectionStorable = new JSONStorableStringChooser(
                "Selected Proportions",
                proportionNames,
                proportionNames[0],
                "Selected Proportions"
            );
            proportionSelectionStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterStringChooser(proportionSelectionStorable);

            showProportionMarkerLineHeight = new JSONStorableBool("Show Proportion - Height", true);
            showProportionMarkerLineHeight.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showProportionMarkerLineHeight);

            showProportionMarkerLineChin = new JSONStorableBool("Show Proportion - Chin", true);
            showProportionMarkerLineChin.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showProportionMarkerLineChin);

            showProportionMarkerLineBust = new JSONStorableBool("Show Proportion - Nipple", true);
            showProportionMarkerLineBust.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showProportionMarkerLineBust);

            showProportionMarkerLineNavel = new JSONStorableBool("Show Proportion - Navel", true);
            showProportionMarkerLineNavel.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showProportionMarkerLineNavel);

            showProportionMarkerLineCrotch = new JSONStorableBool("Show Proportion - Crotch", true);
            showProportionMarkerLineCrotch.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showProportionMarkerLineCrotch);

            showProportionMarkerLineKnee = new JSONStorableBool("Show Proportion - Knee", true);
            showProportionMarkerLineKnee.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showProportionMarkerLineKnee);

            showProportionMarkerLineHeel = new JSONStorableBool("Show Proportion - Heel", true);
            showProportionMarkerLineHeel.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showProportionMarkerLineHeel);

            showProportionMarkerLineShoulder = new JSONStorableBool("Show Proportion - Shoulder", true);
            showProportionMarkerLineShoulder.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showProportionMarkerLineShoulder);

            showProportionMarkerLineShoulderWidth = new JSONStorableBool("Show Proportion - Shoulder Width", true);
            showProportionMarkerLineShoulderWidth.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showProportionMarkerLineShoulderWidth);

            showProportionMarkerLineArm = new JSONStorableBool("Show Proportion - Arm", true);
            showProportionMarkerLineArm.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showProportionMarkerLineArm);

            flipProportionMarkerSide = new JSONStorableBool("Proportion Guide Flip Side", false, (bool value) => {
                Draw();
            });
            flipProportionMarkerSide.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(flipProportionMarkerSide);


            //////////////////

            showTargetHeadRatioStorable = new JSONStorableBool("Enable Auto Head Ratio Targeting", false, (bool value) => {
                showTargetHeadRatioStorable.valNoCallback = value;
                Draw();

            });
            showTargetHeadRatioStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showTargetHeadRatioStorable);

            targetHeadRatioStorable = new JSONStorableFloat("Head Ratio Target", 7.6f, 0, 10);
            targetHeadRatioStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(targetHeadRatioStorable);

            targetHeadRatioMorphStorable = new JSONStorableStringChooser(
                "Head Ratio Morph",
                new List<string> { "Head big", "Head Scale" },
                "Head big",
                "Prefer Morph"
            );
            targetHeadRatioMorphStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterStringChooser(targetHeadRatioMorphStorable);

            // Choose
            cupAlgorithmStorable = new JSONStorableStringChooser(
                "Cup Size Method",
                _cupCalculators.Select(cc => cc.Name).ToList(),
                _cupCalculators[0].Name,
                "Cup Size Method"
            );
            cupAlgorithmStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterStringChooser(cupAlgorithmStorable);

            unitsStorable = new JSONStorableStringChooser(
                "Units",
                new List<string> { UnitUtils.US, UnitUtils.Metric, UnitUtils.Meters, UnitUtils.Centimeters, UnitUtils.Inches, UnitUtils.Feet, UnitUtils.Heads },
                new List<string> { "US (in / ft)", "Metric (cm / m)", "Meters (m)", "Centimeters (cm)", "Inches (in)", "Feet (ft)", "Heads (head.unit.)"},
                UnitUtils.Centimeters, 
                "Units"
            );
            unitsStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterStringChooser(unitsStorable);

            // Float: How far apart markers are spread apart
            markerSpreadStorable = new JSONStorableFloat("Spread Guides", 0.25f, -1, 1, constrain: false);
            markerSpreadStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(markerSpreadStorable);

            // Float: Move markers forward or backward relative to center depth of the head
            markerFrontBackStorable = new JSONStorableFloat("Move Guides Forward/Backward", 0, -5, 5, constrain: false);
            markerFrontBackStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(markerFrontBackStorable);

            // Float: Move markers left or right 
            markerLeftRightStorable = new JSONStorableFloat("Move Guides Left/Right", 0, -5, 5, constrain: false);
            markerLeftRightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(markerLeftRightStorable);

            // Float: Move markers forward or backward relative to center depth of the head
            markerUpDownStorable = new JSONStorableFloat("Move Guides Up/Down", 0, -5, 5, constrain: false);
            markerUpDownStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(markerUpDownStorable);

            hideDocsStorable = new JSONStorableBool("Hide Documentation", false);
            hideDocsStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(hideDocsStorable);


            //////////////////////////////////////

            // calculated positions and distances for other _plugins to use if wanted
            fullHeightStorable = new JSONStorableFloat("figureHeight", 0, 0, 100);
            fullHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(fullHeightStorable);

            heightInHeadsStorable = new JSONStorableFloat("figureHeightHeads", 0, 0, 100);
            heightInHeadsStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(heightInHeadsStorable);

            headSizeHeightStorable = new JSONStorableFloat("headSizeHeight", 0, 0, 100);
            headSizeHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(headSizeHeightStorable);

            chinHeightStorable = new JSONStorableFloat("chinHeight", 0, 0, 100);
            chinHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(chinHeightStorable);

            shoulderHeightStorable = new JSONStorableFloat("shoulderHeight", 0, 0, 100);
            shoulderHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(shoulderHeightStorable);

            shoulderWidthStorable = new JSONStorableFloat("shoulderWidth", 0, 0, 100);
            shoulderWidthStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(shoulderWidthStorable);

            armLengthStorable = new JSONStorableFloat("armLength", 0, 0, 100);
            armLengthStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(armLengthStorable);

            nippleHeightStorable = new JSONStorableFloat("nippleHeight", 0, 0, 100);
            nippleHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(nippleHeightStorable);

            underbustHeightStorable = new JSONStorableFloat("underbustHeight", 0, 0, 100);
            underbustHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(underbustHeightStorable);

            navelHeightStorable = new JSONStorableFloat("navelHeight", 0, 0, 100);
            navelHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(navelHeightStorable);

            crotchHeightStorable = new JSONStorableFloat("crotchHeight", 0, 0, 100);
            crotchHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(crotchHeightStorable);

            kneeBottomHeightStorable = new JSONStorableFloat("kneeHeight", 0, 0, 100);
            kneeBottomHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(kneeBottomHeightStorable);

            headSizeWidthStorable = new JSONStorableFloat("headSizeWidth", 0, 0, 100);
            headSizeWidthStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(headSizeWidthStorable);

            breastBustStorable = new JSONStorableFloat("breastBustSize", 0, 0, 100);
            breastBustStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(breastBustStorable);

            breastUnderbustStorable = new JSONStorableFloat("breastUnderbustSize", 0, 0, 100);
            breastUnderbustStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(breastUnderbustStorable);

            breastBandStorable = new JSONStorableFloat("breastBandSize", 0, 0, 100);
            breastBandStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(breastBandStorable);

            breastCupStorable = new JSONStorableString("breastCupSize", "");
            breastCupStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterString(breastCupStorable);

            waistSizeStorable = new JSONStorableFloat("waistSize", 0, 0, 100);
            waistSizeStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(waistSizeStorable);

            hipSizeStorable = new JSONStorableFloat("hipSize", 0, 0, 100);
            hipSizeStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(hipSizeStorable);

            penisLength = new JSONStorableFloat("penisLength", 0, 0, 100);
            penisLength.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(penisLength);

            penisWidth = new JSONStorableFloat("penisWidth", 0, 0, 100);
            penisWidth.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(penisWidth);

            penisGirth = new JSONStorableFloat("penisGirth", 0, 0, 100);
            penisGirth.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(penisGirth);

            if(experimentalModeStorable.val) {
                showAgeMarkersStorable = new JSONStorableBool("Age Markers", false, (bool value) => {
                    showAgeMarkersStorable.valNoCallback = value;
                    Draw();
                });
                showAgeMarkersStorable.storeType = JSONStorableParam.StoreType.Full;
                _plugin.RegisterBool(showAgeMarkersStorable);
            }
            else {
                showAgeMarkersStorable = new JSONStorableBool("Age Markers", false, (bool value) => {
                    showAgeMarkersStorable.valNoCallback = value;
                    Draw();
                });
                showAgeMarkersStorable.storeType = JSONStorableParam.StoreType.Full;
                _plugin.RegisterBool(showAgeMarkersStorable);
            }

            ageMarkerShowHeadVisual = new JSONStorableBool("Show Head Age", true, (bool value) => {
                Draw();
            });
            ageMarkerShowHeadVisual.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(ageMarkerShowHeadVisual);

            ageMarkerShowHeightVisual = new JSONStorableBool("Show Height Age", true, (bool value) => {
                Draw();
            });
            ageMarkerShowHeightVisual.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(ageMarkerShowHeightVisual);

            ageMarkerShowProportionVisual = new JSONStorableBool("Show Proportion Age", true, (bool value) => {
                Draw();
            });
            ageMarkerShowProportionVisual.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(ageMarkerShowProportionVisual);

            ageWarnUnderage = new JSONStorableBool("Warn On Under 18 Guess", true, (bool value) => {
                Draw();
            });
            ageWarnUnderage.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(ageMarkerShowHeadVisual);

            ageGuessLowest = new JSONStorableFloat("ageGuessLowest", 0, 0, 100);
            ageGuessLowest.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(ageGuessLowest);
            
            ageGuessLowestLikely = new JSONStorableFloat("ageGuessLowestLikely", 0, 0, 100);
            ageGuessLowestLikely.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(ageGuessLowestLikely);
            
            ageGuessHighest = new JSONStorableFloat("ageGuessHighest", 0, 0, 100);
            ageGuessHighest.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(ageGuessHighest);
            
            ageGuessHighestLikely = new JSONStorableFloat("ageGuessHighestLikely", 0, 0, 100);
            ageGuessHighestLikely.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(ageGuessHighestLikely);

            proportionClosestMatch = new JSONStorableString("proportionMatch", "");
            proportionClosestMatch.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterString(proportionClosestMatch);

            // manual heights
            showManualMarkersStorable = new JSONStorableBool("Manual Markers", false, (bool value) => {
                showManualMarkersStorable.valNoCallback = value;
                Draw();
            });
            showManualMarkersStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showManualMarkersStorable);

            manualMarkersCopy = new JSONStorableBool("Copy Markers From Person", false);
            manualMarkersCopy.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(manualMarkersCopy);

            lineThicknessManualStorable = new JSONStorableFloat("Manual Guide Line Width", 2, 1, 10, constrain: true);
            lineThicknessManualStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(lineThicknessManualStorable);

            manualMarkerColor = new JSONStorableColor("Manual Guide Color", _plugin.ColorToHSV(Color.yellow), (float h, float s, float v) => {
                var hsv = new HSVColor { H = h, S = s, V = v };
                manualMarkerColor.valNoCallback = hsv;
                if(_manualMarkerColorButton != null) {
                    _manualMarkerColorButton.buttonColor = _plugin.HSVToColor(hsv);
                }
            });
            manualMarkerColor.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterColor(manualMarkerColor);

            manualHeightStorable = new JSONStorableFloat("Height", 0, 0, 300, constrain: false);
            manualHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(manualHeightStorable);

            manualChinHeightStorable = new JSONStorableFloat("Chin Height", 0, 0, 300, constrain: false);
            manualChinHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(manualChinHeightStorable);

            manualShoulderHeightStorable = new JSONStorableFloat("Shoulder Height", 0, 0, 300, constrain: false);
            manualShoulderHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(manualShoulderHeightStorable);

            manualNippleHeightStorable = new JSONStorableFloat("Bust Height", 0, 0, 300, constrain: false);
            manualNippleHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(manualNippleHeightStorable);

            manualUnderbustHeightStorable = new JSONStorableFloat("Underbust Height", 0, 0, 300, constrain: false);
            manualUnderbustHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(manualUnderbustHeightStorable);

            manualNavelHeightStorable = new JSONStorableFloat("Navel Height", 0, 0, 300, constrain: false);
            manualNavelHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(manualNavelHeightStorable);

            manualCrotchHeightStorable = new JSONStorableFloat("Crotch Height", 0, 0, 300, constrain: false);
            manualCrotchHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(manualCrotchHeightStorable);

            manualKneeBottomHeightStorable = new JSONStorableFloat("Knee Height", 0, 0, 300, constrain: false);
            manualKneeBottomHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(manualKneeBottomHeightStorable);
        }

        private UIDynamicToggle _featureMarkerToggle;
        private UIDynamicButton _featureMarkerColorButton;
        private UIDynamicColorPicker _featureMarkerColorPicker;
        private UIDynamicSlider _featureMarkerLineThickness;
        private UIDynamicToggle _featureMarkerToggleLabels;
        private UIDynamicToggle _featureMarkerToggleLines;
        private UIDynamicToggle _featureMarkerFlipSide;

        private UIDynamicToggle _headMarkerToggle;
        private UIDynamicButton _headMarkerColorButton;
        private UIDynamicColorPicker _headMarkerColorPicker;
        private UIDynamicSlider _headMarkerLineThickness;
        private UIDynamicToggle _headMarkerFlipSide;

        private UIDynamicToggle _faceMarkerToggle;
        private UIDynamicButton _faceMarkerColorButton;
        private UIDynamicColorPicker _faceMarkerColorPicker;
        private UIDynamicSlider _faceMarkerLineThickness;

        private UIDynamicToggle _circumferenceMarkerToggle;
        private UIDynamicButton _circumferenceMarkerColorButton;
        private UIDynamicColorPicker _circumferenceMarkerColorPicker;
        private UIDynamicSlider _circumferenceMarkerLineThickness;

        private UIDynamicToggle _proportionMarkerToggle;
        private UIDynamicButton _proportionMarkerColorButton;
        private UIDynamicColorPicker _proportionMarkerColorPicker;
        private UIDynamicSlider _proportionMarkerLineThickness;
        private UIDynamicPopup _proportionSelection;
        private UIDynamicToggle _proportionMarkerFlipSide;

        private UIDynamicTextField _proportionEditName;
        private UIDynamicToggle _proportionEditIsFemale;
        private UIDynamicSlider _proportionEditHeight;
        private UIDynamicSlider _proportionEditChin;
        private UIDynamicSlider _proportionEditShoulderWidth;
        private UIDynamicSlider _proportionEditArms;
        private UIDynamicSlider _proportionEditNipples;
        private UIDynamicSlider _proportionEditNavel;
        private UIDynamicSlider _proportionEditCrotch;
        private UIDynamicSlider _proportionEditLegs;
        private UIDynamicSlider _proportionEditKnee;
        private UIDynamicSlider _proportionEditHeel;
        private UIDynamicSlider _proportionEditAgeMin;
        private UIDynamicSlider _proportionEditAgeMax;

        private UIDynamicButton _proportionEditButton;
        private UIDynamicButton _proportionDeleteButton;
        private UIDynamicButton _proportionAddButton;
        private UIDynamicButton _proportionCancelButton;
        private UIDynamicButton _proportionSaveButton;

        private UIDynamicToggle _targetHeadRatioToggle;
        private UIDynamicSlider _targetHeadRatioSlider;
        private UIDynamicPopup _targetHeadRatioMorph;

        private UIDynamicPopup _cupAlgorithm;
        private UIDynamicPopup _units;
        private UIDynamicSlider _markerSpread;
        private UIDynamicSlider _markerFrontBack;
        private UIDynamicSlider _markerLeftRight;
        private UIDynamicSlider _markerUpDown;
        private UIDynamicToggle _hideDocs;

        private UIDynamicToggle _ageMarkerToggle;
        private UIDynamicTextField _ageDescription;
        private UIDynamicToggle _ageMarkerShowHeightToggle;
        private UIDynamicToggle _ageMarkerShowHeadToggle;
        private UIDynamicToggle _ageMarkerShowProportionToggle;
        private UIDynamicToggle _ageWarnUnderageToggle;

        private UIDynamicToggle _manualMarkerToggle;
        private UIDynamicToggle _copyManualMarkers;
        private UIDynamicButton _manualMarkerColorButton;
        private UIDynamicColorPicker _manualMarkerColorPicker;
        private UIDynamicSlider _manualMarkerLineThickness;
        private List<UIDynamicSlider> _manualSliders = new List<UIDynamicSlider>();

        private List<UIDynamicButton> _buttons = new List<UIDynamicButton>();
        private List<UIDynamicToggle> _featureLineToggles = new List<UIDynamicToggle>();

        private bool _showHeadMarkerOptions = false;
        private bool _showFeatureMarkerOptions = false;
        private bool _showFaceMarkerOptions = false;
        private bool _showCircumferenceMarkerOptions = false;
        private bool _showProportionMarkerOptions = false;
        private bool _showManualMarkerOptions = false;
        private bool _showAgeMarkerOptions = false;

        public void Draw() {
            Clear();

            int defaultButtonHeight = 50;
            int defaultSliderHeight = 120;
            int defaultSectionSpacerHeight = 0;

            if(_plugin.containingAtom.type == "Person") {
                // Head Height Guides
                _headMarkerToggle = _plugin.CreateToggle(showHeadHeightMarkersStorable, rightSide: false);
                _headMarkerToggle.backgroundColor = HEADER_COLOR;
                _headMarkerToggle.textColor = HEADER_TEXT_COLOR;
                if(showHeadHeightMarkersStorable.val) {
                    _buttons.Add(_plugin.CreateButton(_showHeadMarkerOptions ? "[-] close options" : "[+] show options", rightSide: true));
                    _buttons[_buttons.Count - 1].button.onClick.AddListener(() => {
                        _showHeadMarkerOptions = !_showHeadMarkerOptions;
                        Draw();
                    });
                    _buttons[_buttons.Count - 1].buttonColor = OPTIONS_COLOR;
                    _buttons[_buttons.Count - 1].textColor = OPTIONS_TEXT_COLOR;

                    if(_showHeadMarkerOptions) {
                        _headMarkerColorButton = _plugin.CreateButton("Set Line Color", rightSide: false);
                        _headMarkerColorButton.buttonColor = _plugin.HSVToColor(headMarkerColor.val);
                        _headMarkerColorButton.button.onClick.AddListener(() => {
                            _choosingHeadColor = !_choosingHeadColor;
                            Draw();
                        });
                        if(_choosingHeadColor) {
                            _headMarkerColorPicker = _plugin.CreateColorPicker(headMarkerColor, rightSide: false);
                        }
                        _headMarkerLineThickness = _plugin.CreateSlider(lineThicknessHeadStorable, rightSide: false);
                        _headMarkerFlipSide = _plugin.CreateToggle(flipHeadMarkerSide, rightSide: false);

                        CreateStandardSpacer(defaultButtonHeight, rightSide: true);

                        CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                        CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                        if(_choosingHeadColor) {
                            CreateStandardSpacer(_headMarkerColorPicker.height, rightSide: true);
                        }
                    }
                    CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: true);

                }
                else {
                    CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                    CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: true);
                }
                CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: false);

                // Feature Guides
                CreateStandardDivider(rightSide: false);
                CreateStandardDivider(rightSide: true);
                _featureMarkerToggle = _plugin.CreateToggle(showFeatureMarkersStorable, rightSide: false);
                _featureMarkerToggle.backgroundColor = HEADER_COLOR;
                _featureMarkerToggle.textColor = HEADER_TEXT_COLOR;
                if(showFeatureMarkersStorable.val) {
                    _buttons.Add(_plugin.CreateButton(_showFeatureMarkerOptions ? "[-] close options" : "[+] show options", rightSide: true));
                    _buttons[_buttons.Count - 1].button.onClick.AddListener(() => {
                        _showFeatureMarkerOptions = !_showFeatureMarkerOptions;
                        Draw();
                    });
                    _buttons[_buttons.Count - 1].buttonColor = OPTIONS_COLOR;
                    _buttons[_buttons.Count - 1].textColor = OPTIONS_TEXT_COLOR;
                    if(_showFeatureMarkerOptions) {
                        _featureMarkerColorButton = _plugin.CreateButton("Set Line Color", rightSide: false);
                        _featureMarkerColorButton.buttonColor = _plugin.HSVToColor(featureMarkerColor.val);
                        _featureMarkerColorButton.button.onClick.AddListener(() => {
                            _choosingFeatureColor = !_choosingFeatureColor;
                            Draw();
                        });
                        if(_choosingFeatureColor) {
                            _featureMarkerColorPicker = _plugin.CreateColorPicker(featureMarkerColor, rightSide: false);
                        }
                        _featureMarkerLineThickness = _plugin.CreateSlider(lineThicknessFigureStorable, rightSide: false);
                        _featureMarkerToggleLabels = _plugin.CreateToggle(showFeatureMarkerLabelsStorable, rightSide: false);
                        _featureMarkerToggleLines = _plugin.CreateToggle(showFeatureMarkerLinesStorable, rightSide: false);
                        _featureMarkerFlipSide = _plugin.CreateToggle(flipFeatureMarkerSide, rightSide: false);

                        _featureLineToggles.Add(_plugin.CreateToggle(showFeatureMarkerLineHeight, rightSide: true));
                        _featureLineToggles.Add(_plugin.CreateToggle(showFeatureMarkerLineChin, rightSide: true));
                        _featureLineToggles.Add(_plugin.CreateToggle(showFeatureMarkerLineShoulder, rightSide: true));
                        _featureLineToggles.Add(_plugin.CreateToggle(showFeatureMarkerLineShoulderWidth, rightSide: true));
                        _featureLineToggles.Add(_plugin.CreateToggle(showFeatureMarkerLineArm, rightSide: true));
                        _featureLineToggles.Add(_plugin.CreateToggle(showFeatureMarkerLineBust, rightSide: true));
                        _featureLineToggles.Add(_plugin.CreateToggle(showFeatureMarkerLineUnderbust, rightSide: true));
                        _featureLineToggles.Add(_plugin.CreateToggle(showFeatureMarkerLineNavel, rightSide: true));
                        _featureLineToggles.Add(_plugin.CreateToggle(showFeatureMarkerLineCrotch, rightSide: true));
                        _featureLineToggles.Add(_plugin.CreateToggle(showFeatureMarkerLineKnee, rightSide: true));
                        _featureLineToggles.Add(_plugin.CreateToggle(showFeatureMarkerLineHeel, rightSide: true));
                        CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                        CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                        CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                        CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                        CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                        CreateStandardSpacer(defaultButtonHeight, rightSide: false);

                        CreateStandardSpacer(defaultButtonHeight + 5, rightSide: true);
                        if(_choosingFeatureColor) {
                            CreateStandardSpacer(_featureMarkerColorPicker.height, rightSide: true);
                        }
                    }
                    CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: true);
                }
                else {
                    CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                    CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: true);
                }
                CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: false);

                // Face Guides
                CreateStandardDivider(rightSide: false);
                CreateStandardDivider(rightSide: true);
                _faceMarkerToggle = _plugin.CreateToggle(showFaceMarkersStorable, rightSide: false);
                _faceMarkerToggle.backgroundColor = HEADER_COLOR;
                _faceMarkerToggle.textColor = HEADER_TEXT_COLOR;
                if(showFaceMarkersStorable.val) {
                    _buttons.Add(_plugin.CreateButton(_showFaceMarkerOptions ? "[-] close options" : "[+] show options", rightSide: true));
                    _buttons[_buttons.Count - 1].button.onClick.AddListener(() => {
                        _showFaceMarkerOptions = !_showFaceMarkerOptions;
                        Draw();
                    });
                    _buttons[_buttons.Count - 1].buttonColor = OPTIONS_COLOR;
                    _buttons[_buttons.Count - 1].textColor = OPTIONS_TEXT_COLOR;
                    if(_showFaceMarkerOptions) {
                        _faceMarkerColorButton = _plugin.CreateButton("Set Line Color");
                        _faceMarkerColorButton.buttonColor = _plugin.HSVToColor(faceMarkerColor.val);
                        _faceMarkerColorButton.button.onClick.AddListener(() => {
                            _choosingFaceColor = !_choosingFaceColor;
                            Draw();
                        });
                        if(_choosingFaceColor) {
                            _faceMarkerColorPicker = _plugin.CreateColorPicker(faceMarkerColor, rightSide: false);
                        }
                        _faceMarkerLineThickness = _plugin.CreateSlider(lineThicknessFaceStorable, rightSide: false);
                        CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                        CreateStandardSpacer(defaultButtonHeight, rightSide: true);

                        CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                        CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                        if(_choosingFaceColor) {
                            CreateStandardSpacer(_faceMarkerColorPicker.height, rightSide: true);
                        }
                    }
                    CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: true);
                }
                else {
                    CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                    CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: true);
                }
                CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: false);

                // Circumference Guides
                CreateStandardDivider(rightSide: false);
                CreateStandardDivider(rightSide: true);
                _circumferenceMarkerToggle = _plugin.CreateToggle(showCircumferenceMarkersStorable, rightSide: false);
                _circumferenceMarkerToggle.backgroundColor = HEADER_COLOR;
                _circumferenceMarkerToggle.textColor = HEADER_TEXT_COLOR;
                if(showCircumferenceMarkersStorable.val) {
                    _buttons.Add(_plugin.CreateButton(_showCircumferenceMarkerOptions ? "[-] close options" : "[+] show options", rightSide: true));
                    _buttons[_buttons.Count - 1].button.onClick.AddListener(() => {
                        _showCircumferenceMarkerOptions = !_showCircumferenceMarkerOptions;
                        Draw();
                    });
                    _buttons[_buttons.Count - 1].buttonColor = OPTIONS_COLOR;
                    _buttons[_buttons.Count - 1].textColor = OPTIONS_TEXT_COLOR;
                    if(_showCircumferenceMarkerOptions) {
                        _circumferenceMarkerColorButton = _plugin.CreateButton("Set Line Color", rightSide: false);
                        _circumferenceMarkerColorButton.buttonColor = _plugin.HSVToColor(circumferenceMarkerColor.val);
                        _circumferenceMarkerColorButton.button.onClick.AddListener(() => {
                            _choosingCircumferenceColor = !_choosingCircumferenceColor;
                            Draw();
                        });
                        if(_choosingCircumferenceColor) {
                            _circumferenceMarkerColorPicker = _plugin.CreateColorPicker(circumferenceMarkerColor, rightSide: false);
                        }
                        _circumferenceMarkerLineThickness = _plugin.CreateSlider(lineThicknessCircumferenceStorable, rightSide: false);
                        CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                        CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                        if(_choosingCircumferenceColor) {
                            CreateStandardSpacer(_circumferenceMarkerColorPicker.height, rightSide: true);
                        }
                    }
                    CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: true);
                }
                else {
                    CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                    CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: true);
                }
                CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: false);

                // Age Guides
                CreateStandardDivider(rightSide: true);
                CreateStandardDivider(rightSide: false);
                _ageMarkerToggle = _plugin.CreateToggle(showAgeMarkersStorable, rightSide: false);
                _ageMarkerToggle.backgroundColor = HEADER_COLOR;
                _ageMarkerToggle.textColor = HEADER_TEXT_COLOR;
                if(showAgeMarkersStorable.val){
                    _buttons.Add(_plugin.CreateButton(_showAgeMarkerOptions ? "[-] close options" : "[+] show options", rightSide: true));
                    _buttons[_buttons.Count - 1].button.onClick.AddListener(() => {
                        _showAgeMarkerOptions = !_showAgeMarkerOptions;
                        Draw();
                    });
                    _buttons[_buttons.Count - 1].buttonColor = OPTIONS_COLOR;
                    _buttons[_buttons.Count - 1].textColor = OPTIONS_TEXT_COLOR;
                    if(_showAgeMarkerOptions) {
                        _ageDescription = _plugin.CreateTextField(new JSONStorableString("age description",
@"<b>Important:</b>

There is no great way to truly determine age based on simple math functions. Especially when there is some subjectivity involved. Your brain is powerful and this plugin can not replace its use. It is always up to you to make sure that your creations conform to VAM community standards.

<b>What are these charts?</b>

Each measurement is visualized as a 'box plots'.  The thick bar is the more likely range (25th to 75th percentile).  The thin lines are the less likely ranges (0-25, 75-100 percentiles).  Search the web for 'box plots' for more technical information.

<b>Main Age Guess</b>

This age calculation combines all of the calculations below.  If a lot of the below calculations agree strongly, then the bar will be thick.  If the below calculations agree weakly, then the bar will be thin.

<b>Head Age Guess</b>

Age from head size is based on proportion information for humans found in research papers and human proportions listed in references by anatomy4sculptors.com.

Changing 'Head big' and 'Head Scale' morphs will affect this heavily.

<b>Height Age Guess</b>

Age from height is based on human growth charts from the Centers for Disease Control (CDC).

Scaling your model from Control & Physics 1 tab will affect this heavily.

<b>Proportion Age Guess</b>

Age from proportions is based on a subset of human proportions found at https://hpc.anatomy4sculptors.com

Adding or editing your own proportions in the 'Proportions Guides' section of this plugin will affect this.

See https://hpc.anatomy4sculptors.com for more proportions or search the web for alternate proportions like 'anime proportions'."
                        ), rightSide: false);
                        _ageDescription.backgroundColor = new Color(0, 0, 0, 0);
                        _ageDescription.height = 700;

                        _ageMarkerShowHeadToggle = _plugin.CreateToggle(ageMarkerShowHeadVisual, rightSide: true);
                        _ageMarkerShowHeightToggle = _plugin.CreateToggle(ageMarkerShowHeightVisual, rightSide: true);
                        _ageMarkerShowProportionToggle = _plugin.CreateToggle(ageMarkerShowProportionVisual, rightSide: true);

                        CreateStandardSpacer(defaultButtonHeight, rightSide: true);

                        _ageWarnUnderageToggle = _plugin.CreateToggle(ageWarnUnderage, rightSide: true);

                        CreateStandardSpacer(_ageDescription.height - 325, rightSide: true);
                    }
                }
                else {
                    CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                }

                // Proportion Guides
                CreateStandardDivider(rightSide: false);
                CreateStandardDivider(rightSide: true);
                _proportionMarkerToggle = _plugin.CreateToggle(showProportionMarkersStorable, rightSide: false);
                _proportionMarkerToggle.backgroundColor = HEADER_COLOR;
                _proportionMarkerToggle.textColor = HEADER_TEXT_COLOR;
                if(showProportionMarkersStorable.val) {
                    _buttons.Add(_plugin.CreateButton(_showProportionMarkerOptions ? "[-] close options" : "[+] show options", rightSide: true));
                    _buttons[_buttons.Count - 1].button.onClick.AddListener(() => {
                        _showProportionMarkerOptions = !_showProportionMarkerOptions;
                        Draw();
                    });
                    _buttons[_buttons.Count - 1].buttonColor = OPTIONS_COLOR;
                    _buttons[_buttons.Count - 1].textColor = OPTIONS_TEXT_COLOR;
                    if(_showProportionMarkerOptions) {
                        var selectedProportion = ProportionTemplates.FirstOrDefault(p => p.ProportionName.Equals(proportionSelectionStorable.val));

                        proportionSelectionStorable.choices = ProportionTemplateNames();
                        proportionSelectionStorable.displayChoices = proportionSelectionStorable.choices;
                        proportionSelectionStorable.val = selectedProportion?.ProportionName ?? "Auto Detect";


                        if(_editingProportion || _creatingProportion) {
                            if(_preEditProportion == null) {
                                _preEditProportion = selectedProportion.Clone();
                            }
                            _proportionEditName =
                                CreateTextInput(new JSONStorableString("Name", selectedProportion.ProportionName));
                            CreateStandardSpacer(_proportionEditName.height, rightSide: true);
                            _proportionEditIsFemale =
                                _plugin.CreateToggle(new JSONStorableBool("Female?", selectedProportion.IsFemale, (value) => {
                                    selectedProportion.IsFemale = value;
                                }));
                            CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                            _proportionEditHeight = 
                                _plugin.CreateSlider(new JSONStorableFloat("Height in Heads", selectedProportion.FigureHeightInHeads, (value) => {
                                    selectedProportion.FigureHeightInHeads = value;
                                }, 0, 10));
                            CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                            _proportionEditChin = 
                                _plugin.CreateSlider(new JSONStorableFloat("Chin to Shoulder", selectedProportion.FigureChinToShoulder, (value) => {
                                    selectedProportion.FigureChinToShoulder = value;
                                }, 0, 10));
                            CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                            _proportionEditShoulderWidth = 
                                _plugin.CreateSlider(new JSONStorableFloat("Shoulder Width", selectedProportion.FigureShoulderWidth, (value) => {
                                    selectedProportion.FigureShoulderWidth = value;
                                }, 0, 10));
                            CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                            _proportionEditArms = 
                                _plugin.CreateSlider(new JSONStorableFloat("Full Arm Length", selectedProportion.FigureLengthOfUpperLimb, (value) => {
                                    selectedProportion.FigureLengthOfUpperLimb = value;
                                }, 0, 10));
                            CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                            _proportionEditNipples = 
                                _plugin.CreateSlider(new JSONStorableFloat("Shoulder to Nipples", selectedProportion.FigureShoulderToNipples, (value) => {
                                    selectedProportion.FigureShoulderToNipples = value;
                                }, 0, 10));
                            CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                            _proportionEditNavel = 
                                _plugin.CreateSlider(new JSONStorableFloat("Shoulder to Navel", selectedProportion.FigureShoulderToNavel, (value) => {
                                    selectedProportion.FigureShoulderToNavel = value;
                                }, 0, 10));
                            CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                            _proportionEditCrotch =
                                _plugin.CreateSlider(new JSONStorableFloat("Shoulder to Crotch", selectedProportion.FigureShoulderToCrotch, (value) => {
                                    selectedProportion.FigureShoulderToCrotch = value;
                                }, 0, 10));
                            CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                            _proportionEditLegs = 
                                _plugin.CreateSlider(new JSONStorableFloat("Full Leg Length", selectedProportion.FigureLengthOfLowerLimb, (value) => {
                                    selectedProportion.FigureLengthOfLowerLimb = value;
                                }, 0, 10));
                            CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                            _proportionEditKnee = 
                                _plugin.CreateSlider(new JSONStorableFloat("Crotch to Knee", selectedProportion.FigureCrotchToBottomOfKnees, (value) => {
                                    selectedProportion.FigureCrotchToBottomOfKnees = value;
                                }, 0, 10));
                            CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                            _proportionEditHeel = 
                                _plugin.CreateSlider(new JSONStorableFloat("Knee to Heel", selectedProportion.FigureBottomOfKneesToHeels, (value) => {
                                    selectedProportion.FigureBottomOfKneesToHeels = value;
                                }, 0, 10));
                            CreateStandardSpacer(defaultSliderHeight, rightSide: true);

                            if(experimentalModeStorable.val || showAgeMarkersStorable.val){
                                JSONStorableFloat minStorable = new JSONStorableFloat("Age Estimage Min", selectedProportion.EstimatedAgeRangeMin, 0, 100);
                                minStorable.setCallbackFunction = (value) => {
                                    selectedProportion.EstimatedAgeRangeMin = (int)Math.Floor(value);
                                    if(selectedProportion.EstimatedAgeRangeMin > selectedProportion.EstimatedAgeRangeMax) {
                                        selectedProportion.EstimatedAgeRangeMin = selectedProportion.EstimatedAgeRangeMax;
                                    }
                                    if(minStorable != null) {
                                        minStorable.valNoCallback = (float)selectedProportion.EstimatedAgeRangeMin;
                                    }
                                };
                                _proportionEditAgeMin = _plugin.CreateSlider(minStorable);
                                CreateStandardSpacer(defaultSliderHeight, rightSide: true);

                                JSONStorableFloat maxStorable = new JSONStorableFloat("Age Estimage Max", selectedProportion.EstimatedAgeRangeMax, 0, 100);
                                maxStorable.setCallbackFunction = (value) => {
                                    selectedProportion.EstimatedAgeRangeMax = (int)Math.Floor(value);
                                    if(selectedProportion.EstimatedAgeRangeMax < selectedProportion.EstimatedAgeRangeMin) {
                                        selectedProportion.EstimatedAgeRangeMax = selectedProportion.EstimatedAgeRangeMin;
                                    }
                                    if(maxStorable != null) {
                                        maxStorable.valNoCallback = (float)selectedProportion.EstimatedAgeRangeMax;
                                    }
                                };
                                _proportionEditAgeMax = _plugin.CreateSlider(maxStorable);
                                CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                            }

                            _proportionCancelButton = _plugin.CreateButton("Cancel");
                            _proportionCancelButton.buttonColor = Color.red;
                            _proportionCancelButton.button.onClick.AddListener(() => {
                                try {
                                    if(_editingProportion) {
                                        selectedProportion.ProportionName = _preEditProportion.ProportionName;
                                        selectedProportion.IsFemale = _preEditProportion.IsFemale;
                                        selectedProportion.FigureHeightInHeads = _preEditProportion.FigureHeightInHeads;
                                        selectedProportion.FigureChinToShoulder = _preEditProportion.FigureChinToShoulder;
                                        selectedProportion.FigureShoulderWidth = _preEditProportion.FigureShoulderWidth;
                                        selectedProportion.FigureShoulderToNipples = _preEditProportion.FigureShoulderToNipples;
                                        selectedProportion.FigureShoulderToNavel = _preEditProportion.FigureShoulderToNavel;
                                        selectedProportion.FigureShoulderToCrotch = _preEditProportion.FigureShoulderToCrotch;
                                        selectedProportion.FigureCrotchToBottomOfKnees = _preEditProportion.FigureCrotchToBottomOfKnees;
                                        selectedProportion.FigureLengthOfLowerLimb = _preEditProportion.FigureLengthOfLowerLimb;
                                        selectedProportion.FigureLengthOfUpperLimb = _preEditProportion.FigureLengthOfUpperLimb;
                                        selectedProportion.FigureBottomOfKneesToHeels = _preEditProportion.FigureBottomOfKneesToHeels;
                                        selectedProportion.EstimatedAgeRangeMin = _preEditProportion.EstimatedAgeRangeMin;
                                        selectedProportion.EstimatedAgeRangeMax = _preEditProportion.EstimatedAgeRangeMax;
                                    }

                                    if(_creatingProportion) {
                                        proportionSelectionStorable.val = "Auto Detect"; // select auto
                                        _proportionTemplates = ProportionTemplates.Where(p => !p.ProportionName.Equals("-- New --")).ToList(); // do not trigger a Draw using the public property
                                    }

                                    _editingProportion = false;
                                    _creatingProportion = false;
                                    _preEditProportion = (Proportions)null;
                                    Draw();
                                }
                                catch(Exception e) {
                                    SuperController.LogError($"{e}");
                                }
                            });
                            CreateStandardSpacer(defaultButtonHeight, rightSide: true);

                            _proportionSaveButton = _plugin.CreateButton(_creatingProportion ? "Create" : "Save");
                            _proportionSaveButton.buttonColor = Color.green;
                            _proportionSaveButton.button.onClick.AddListener(() => {
                                if(_preEditProportion != null) {
                                    selectedProportion.ProportionName = _proportionEditName.text;
                                    proportionSelectionStorable.val = selectedProportion.ProportionName;
                                }
                                _editingProportion = false;
                                _creatingProportion = false;
                                _preEditProportion = (Proportions)null;
                                Draw();
                            });
                            CreateStandardSpacer(defaultButtonHeight, rightSide: true);

                            CreateStandardSpacer(14, rightSide: true);

                        }
                        else {
                            _proportionMarkerColorButton = _plugin.CreateButton("Set Line Color");
                            _proportionMarkerColorButton.buttonColor = _plugin.HSVToColor(proportionMarkerColor.val);
                            _proportionMarkerColorButton.button.onClick.AddListener(() => {
                                _choosingProportionColor = !_choosingProportionColor;
                                Draw();
                            });

                            if(_choosingProportionColor) {
                                _proportionMarkerColorPicker = _plugin.CreateColorPicker(proportionMarkerColor, rightSide: false);
                            }
                            _proportionMarkerLineThickness = _plugin.CreateSlider(lineThicknessProportionStorable, rightSide: false);
                            _proportionMarkerFlipSide = _plugin.CreateToggle(flipProportionMarkerSide, rightSide: false);

                            _proportionSelection = _plugin.CreateScrollablePopup(proportionSelectionStorable, rightSide: false);
                            _proportionSelection.popup.onValueChangeHandlers += (e) => {
                                Draw();
                            };

                            // proportion action buttons
                            if(selectedProportion != null) {
                                _proportionEditButton = _plugin.CreateButton("Edit Selected Proportions", rightSide: false);
                                _proportionEditButton.button.onClick.AddListener(() => {
                                    _creatingProportion = false;
                                    _editingProportion = true;
                                    Draw();
                                });
                            }
                            if(selectedProportion != null) {
                                _proportionDeleteButton = _plugin.CreateButton("Delete Selected Proportions", rightSide: false);
                                _proportionDeleteButton.buttonColor = Color.red;
                                _proportionDeleteButton.button.onClick.AddListener(() => {
                                    proportionSelectionStorable.val = "Auto Detect"; // select auto
                                    ProportionTemplates = ProportionTemplates.Where(p => !p.ProportionName.Equals(selectedProportion.ProportionName)).ToList();
                                    Draw();
                                });
                            }
                            _proportionAddButton = _plugin.CreateButton("New Proportions", rightSide: false);
                            _proportionAddButton.button.onClick.AddListener(() => {
                                var newName = "-- New --";
                                var newProportion = selectedProportion ?? ProportionTemplates.First() ?? new Proportions(); // copy selected, or the first one, or start over
                                newProportion.ProportionName = newName;

                                ProportionTemplates = ProportionTemplates.Concat(new List<Proportions>{ newProportion }).ToList();
                                proportionSelectionStorable.val = newName;

                                _creatingProportion = true;
                                _editingProportion = false;
                                Draw();
                            });

                            _featureLineToggles.Add(_plugin.CreateToggle(showProportionMarkerLineHeight, rightSide: true));
                            _featureLineToggles.Add(_plugin.CreateToggle(showProportionMarkerLineChin, rightSide: true));
                            _featureLineToggles.Add(_plugin.CreateToggle(showProportionMarkerLineShoulder, rightSide: true));
                            _featureLineToggles.Add(_plugin.CreateToggle(showProportionMarkerLineShoulderWidth, rightSide: true));
                            _featureLineToggles.Add(_plugin.CreateToggle(showProportionMarkerLineArm, rightSide: true));
                            _featureLineToggles.Add(_plugin.CreateToggle(showProportionMarkerLineBust, rightSide: true));
                            _featureLineToggles.Add(_plugin.CreateToggle(showProportionMarkerLineNavel, rightSide: true));
                            _featureLineToggles.Add(_plugin.CreateToggle(showProportionMarkerLineCrotch, rightSide: true));
                            _featureLineToggles.Add(_plugin.CreateToggle(showProportionMarkerLineKnee, rightSide: true));
                            _featureLineToggles.Add(_plugin.CreateToggle(showProportionMarkerLineHeel, rightSide: true));
                            CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                            CreateStandardSpacer(defaultButtonHeight + 10, rightSide: false);

                            if(selectedProportion == null){
                                CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                                CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                            }

                            // CreateStandardSpacer(defaultButtonHeight + 5, rightSide: true);
                            CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                            if(_choosingProportionColor) {
                                CreateStandardSpacer(_proportionMarkerColorPicker.height, rightSide: true);
                            }
                        }
                    }
                }
                else {
                    CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                }
            }

            CreateStandardDivider(rightSide: true);
            CreateStandardDivider(rightSide: false);
            _manualMarkerToggle = _plugin.CreateToggle(showManualMarkersStorable, rightSide: false);
            _manualMarkerToggle.backgroundColor = HEADER_COLOR;
            _manualMarkerToggle.textColor = HEADER_TEXT_COLOR;
            if(showManualMarkersStorable.val) {
                _buttons.Add(_plugin.CreateButton(_showManualMarkerOptions ? "[-] close options" : "[+] show options", rightSide: true));
                _buttons[_buttons.Count - 1].button.onClick.AddListener(() => {
                    _showManualMarkerOptions = !_showManualMarkerOptions;
                    Draw();
                });
                _buttons[_buttons.Count - 1].buttonColor = OPTIONS_COLOR;
                _buttons[_buttons.Count - 1].textColor = OPTIONS_TEXT_COLOR;

                if(_showManualMarkerOptions) {
                    if(_plugin.containingAtom.type == "Person") {
                        _copyManualMarkers = _plugin.CreateToggle(manualMarkersCopy, rightSide: false);
                        CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                    }
                    _manualMarkerColorButton = _plugin.CreateButton("Set Line Color", rightSide: false);
                    _manualMarkerColorButton.buttonColor = _plugin.HSVToColor(manualMarkerColor.val);
                    _manualMarkerColorButton.button.onClick.AddListener(() => {
                        _choosingManualColor = !_choosingManualColor;
                        Draw();
                    });
                    CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                    if(_choosingManualColor) {
                        _manualMarkerColorPicker = _plugin.CreateColorPicker(manualMarkerColor, rightSide: false);
                        CreateStandardSpacer(_manualMarkerColorPicker.height, rightSide: true);
                    }
                    _manualMarkerLineThickness = _plugin.CreateSlider(lineThicknessManualStorable, rightSide: false);
                    CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                    _manualSliders.Add(_plugin.CreateSlider(manualHeightStorable, rightSide: false));
                    CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                    _manualSliders.Add(_plugin.CreateSlider(manualChinHeightStorable, rightSide: false));
                    CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                    _manualSliders.Add(_plugin.CreateSlider(manualShoulderHeightStorable, rightSide: false));
                    CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                    _manualSliders.Add(_plugin.CreateSlider(manualNippleHeightStorable, rightSide: false));
                    CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                    _manualSliders.Add(_plugin.CreateSlider(manualUnderbustHeightStorable, rightSide: false));
                    CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                    _manualSliders.Add(_plugin.CreateSlider(manualNavelHeightStorable, rightSide: false));
                    CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                    _manualSliders.Add(_plugin.CreateSlider(manualCrotchHeightStorable, rightSide: false));
                    CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                    _manualSliders.Add(_plugin.CreateSlider(manualKneeBottomHeightStorable, rightSide: false));
                    CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                }
            }
            else {
                CreateStandardSpacer(defaultButtonHeight, rightSide: true);
            }

            if(_plugin.containingAtom.type == "Person") {
                CreateStandardDivider(rightSide: false);
                CreateStandardDivider(rightSide: true);
                CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                _targetHeadRatioToggle = _plugin.CreateToggle(showTargetHeadRatioStorable, rightSide: false);
                _targetHeadRatioToggle.backgroundColor = HEADER_COLOR;
                _targetHeadRatioToggle.textColor = HEADER_TEXT_COLOR;
                if(showTargetHeadRatioStorable.val) {
                    _targetHeadRatioSlider = _plugin.CreateSlider(targetHeadRatioStorable, rightSide: false);
                    _targetHeadRatioMorph = _plugin.CreateScrollablePopup(targetHeadRatioMorphStorable, rightSide: true);
                    CreateStandardSpacer(5, rightSide: true);
                }
            }


            CreateStandardDivider(rightSide: false);
            CreateStandardDivider(rightSide: true);

            if(_plugin.containingAtom.type == "Person") {
                _cupAlgorithm = _plugin.CreateScrollablePopup(cupAlgorithmStorable, rightSide: false);
                _units = _plugin.CreateScrollablePopup(unitsStorable, rightSide: true);
            }
            else {
                _units = _plugin.CreateScrollablePopup(unitsStorable, rightSide: true);
                CreateStandardSpacer(_units.height, rightSide: false);
            }

            CreateStandardDivider(rightSide: false);
            CreateStandardDivider(rightSide: true);


            _markerSpread = _plugin.CreateSlider(markerSpreadStorable, rightSide: false);
            _markerFrontBack = _plugin.CreateSlider(markerFrontBackStorable, rightSide: true);
            _markerLeftRight = _plugin.CreateSlider(markerLeftRightStorable, rightSide: false);
            _markerUpDown = _plugin.CreateSlider(markerUpDownStorable, rightSide: true);

            CreateStandardDivider(rightSide: false);
            CreateStandardDivider(rightSide: true);

            _hideDocs = _plugin.CreateToggle(hideDocsStorable, rightSide: false);
            CreateStandardSpacer(defaultButtonHeight, rightSide: true);

        }

        public void Clear() {
            foreach(var b in _buttons) {
                _plugin.RemoveButton(b);
            }
            _buttons = new List<UIDynamicButton>();

            foreach(var t in _featureLineToggles) {
                _plugin.RemoveToggle(t);
            }
            _featureLineToggles = new List<UIDynamicToggle>();

            foreach(var spacer in _spacerList) {
                _plugin.RemoveSpacer(spacer);
            }
            foreach(var spacer in _spacerLinesList) {
                _plugin.RemoveTextField(spacer);
            }
            if(_ageMarkerToggle) {
                _plugin.RemoveToggle(_ageMarkerToggle);
            }
            if(_ageDescription) {
                _plugin.RemoveTextField(_ageDescription);
            }
            if(_ageMarkerShowHeadToggle){
                _plugin.RemoveToggle(_ageMarkerShowHeadToggle);
            }
            if(_ageMarkerShowHeightToggle){
                _plugin.RemoveToggle(_ageMarkerShowHeightToggle);
            }
            if(_ageMarkerShowProportionToggle){
                _plugin.RemoveToggle(_ageMarkerShowProportionToggle);
            }
            if(_ageWarnUnderageToggle){
                _plugin.RemoveToggle(_ageWarnUnderageToggle);
            }
            if(_manualMarkerToggle) {
                _plugin.RemoveToggle(_manualMarkerToggle);
            }
            if(_copyManualMarkers) {
                _plugin.RemoveToggle(_copyManualMarkers);
            }
            if(_manualMarkerColorButton) {
                _plugin.RemoveButton(_manualMarkerColorButton);
            }
            if(_manualMarkerColorPicker) {
                _plugin.RemoveColorPicker(_manualMarkerColorPicker);
            }
            if(_manualMarkerLineThickness) {
                _plugin.RemoveSlider(_manualMarkerLineThickness);
            }
            foreach(var s in _manualSliders) {
                _plugin.RemoveSlider(s);
            }
            _manualSliders = new List<UIDynamicSlider>();
            if(_featureMarkerFlipSide) {
                _plugin.RemoveToggle(_featureMarkerFlipSide);
            }
            if(_featureMarkerToggle) {
                _plugin.RemoveToggle(_featureMarkerToggle);
            }
            if(_featureMarkerColorButton) {
                _plugin.RemoveButton(_featureMarkerColorButton);
            }
            if(_featureMarkerColorPicker) {
                _plugin.RemoveColorPicker(_featureMarkerColorPicker);
            }
            if(_featureMarkerLineThickness) {
                _plugin.RemoveSlider(_featureMarkerLineThickness);
            }
            if(_featureMarkerToggleLabels) {
                _plugin.RemoveToggle(_featureMarkerToggleLabels);
            }
            if(_featureMarkerToggleLines) {
                _plugin.RemoveToggle(_featureMarkerToggleLines);
            }

            if(_headMarkerToggle) {
                _plugin.RemoveToggle(_headMarkerToggle);
            }
            if(_headMarkerFlipSide) {
                _plugin.RemoveToggle(_headMarkerFlipSide);
            }
            if(_headMarkerColorButton) {
                _plugin.RemoveButton(_headMarkerColorButton);
            }
            if(_headMarkerColorPicker) {
                _plugin.RemoveColorPicker(_headMarkerColorPicker);
            }
            if(_headMarkerLineThickness) {
                _plugin.RemoveSlider(_headMarkerLineThickness);
            }

            if(_faceMarkerToggle) {
                _plugin.RemoveToggle(_faceMarkerToggle);
            }
            if(_faceMarkerColorButton) {
                _plugin.RemoveButton(_faceMarkerColorButton);
            }
            if(_faceMarkerColorPicker) {
                _plugin.RemoveColorPicker(_faceMarkerColorPicker);
            }
            if(_faceMarkerLineThickness) {
                _plugin.RemoveSlider(_faceMarkerLineThickness);
            }

            if(_circumferenceMarkerToggle) {
                _plugin.RemoveToggle(_circumferenceMarkerToggle);
            }
            if(_circumferenceMarkerColorButton) {
                _plugin.RemoveButton(_circumferenceMarkerColorButton);
            }
            if(_circumferenceMarkerColorPicker) {
                _plugin.RemoveColorPicker(_circumferenceMarkerColorPicker);
            }
            if(_circumferenceMarkerLineThickness) {
                _plugin.RemoveSlider(_circumferenceMarkerLineThickness);
            }

            if(_proportionMarkerToggle) {
                _plugin.RemoveToggle(_proportionMarkerToggle);
            }
            if(_proportionMarkerFlipSide) {
                _plugin.RemoveToggle(_proportionMarkerFlipSide);
            }
            if(_proportionMarkerColorButton) {
                _plugin.RemoveButton(_proportionMarkerColorButton);
            }
            if(_proportionMarkerColorPicker) {
                _plugin.RemoveColorPicker(_proportionMarkerColorPicker);
            }
            if(_proportionMarkerLineThickness) {
                _plugin.RemoveSlider(_proportionMarkerLineThickness);
            }
            if(_proportionSelection) {
                _plugin.RemovePopup(_proportionSelection);
            }
            if(_proportionEditButton) {
                _plugin.RemoveButton(_proportionEditButton);
            }
            if(_proportionDeleteButton) {
                _plugin.RemoveButton(_proportionDeleteButton);
            }
            if(_proportionAddButton) {
                _plugin.RemoveButton(_proportionAddButton);
            }
            if(_proportionSaveButton) {
                _plugin.RemoveButton(_proportionSaveButton);
            }
            if(_proportionCancelButton) {
                _plugin.RemoveButton(_proportionCancelButton);
            }

            if(_proportionEditName) {
                _plugin.RemoveTextField(_proportionEditName);
            }
            if(_proportionEditIsFemale) {
                _plugin.RemoveToggle(_proportionEditIsFemale);
            }
            if(_proportionEditHeight) {
                _plugin.RemoveSlider(_proportionEditHeight);
            }
            if(_proportionEditChin) {
                _plugin.RemoveSlider(_proportionEditChin);
            }
            if(_proportionEditShoulderWidth) {
                _plugin.RemoveSlider(_proportionEditShoulderWidth);
            }
            if(_proportionEditArms) {
                _plugin.RemoveSlider(_proportionEditArms);
            }
            if(_proportionEditNipples) {
                _plugin.RemoveSlider(_proportionEditNipples);
            }
            if(_proportionEditNavel) {
                _plugin.RemoveSlider(_proportionEditNavel);
            }
            if(_proportionEditCrotch) {
                _plugin.RemoveSlider(_proportionEditCrotch);
            }
            if(_proportionEditLegs) {
                _plugin.RemoveSlider(_proportionEditLegs);
            }
            if(_proportionEditKnee) {
                _plugin.RemoveSlider(_proportionEditKnee);
            }
            if(_proportionEditHeel) {
                _plugin.RemoveSlider(_proportionEditHeel);
            }
            if(_proportionEditAgeMin) {
                _plugin.RemoveSlider(_proportionEditAgeMin);
            }
            if(_proportionEditAgeMax) {
                _plugin.RemoveSlider(_proportionEditAgeMax);
            }

            if(_targetHeadRatioToggle) {
                _plugin.RemoveToggle(_targetHeadRatioToggle);
            }
            if(_targetHeadRatioSlider) {
                _plugin.RemoveSlider(_targetHeadRatioSlider);
            }
            if(_targetHeadRatioMorph) {
                _plugin.RemovePopup(_targetHeadRatioMorph);
            }

            if(_cupAlgorithm) {
                _plugin.RemovePopup(_cupAlgorithm);
            }
            if(_units) {
                _plugin.RemovePopup(_units);
            }
            if(_markerSpread) {
                _plugin.RemoveSlider(_markerSpread);
            }
            if(_markerFrontBack) {
                _plugin.RemoveSlider(_markerFrontBack);
            }
            if(_markerLeftRight) {
                _plugin.RemoveSlider(_markerLeftRight);
            }
            if(_markerUpDown) {
                _plugin.RemoveSlider(_markerUpDown);
            }
            if(_hideDocs) {
                _plugin.RemoveToggle(_hideDocs);
            }
        }

        private readonly List<UIDynamic> _spacerList = new List<UIDynamic>();
        private void CreateStandardSpacer(float height, bool rightSide = false) {
            var spacer = _plugin.CreateSpacer(rightSide: rightSide);
            spacer.height = height;
            _spacerList.Add(spacer);
        }

        private readonly List<UIDynamicTextField> _spacerLinesList = new List<UIDynamicTextField>();
        private void CreateStandardDivider(bool rightSide = false) {
            var textField = _plugin.CreateTextField(new JSONStorableString("", ""), rightSide: rightSide);
            textField.backgroundColor = SPACER_COLOR;
            var layoutElement = textField.GetComponent<LayoutElement>();
            if(layoutElement != null) {
                layoutElement.minHeight = 0f;
                layoutElement.preferredHeight = 5;
            }
            // remove the scrollbar
            var scrollbar = textField.GetComponentInChildren<Scrollbar>();
            if(scrollbar) {
                UnityEngine.Object.Destroy(scrollbar);
            }
            _spacerLinesList.Add(textField);
        }

        private List<string> ProportionTemplateNames() {
            var proportionNames = ProportionTemplates.Select(p => p.ProportionName).ToList();
            proportionNames.Insert(0, "Auto Detect");
            return proportionNames;
        }

        private UIDynamicTextField CreateTextInput(JSONStorableString jss, bool rightSide = false, InputField.LineType inputFieldType = InputField.LineType.MultiLineNewline) {
            UIDynamicTextField textfield = _plugin.CreateTextField(jss, rightSide); 
            textfield.height = 1f; 
            textfield.backgroundColor = Color.white; 
            var input = textfield.gameObject.AddComponent<InputField>(); 
            input.textComponent = textfield.UItext; 
            input.lineType = inputFieldType; input.textComponent.resizeTextMaxSize = 30; input.textComponent.resizeTextForBestFit = true; jss.inputField = input; 
            return (textfield);
        } 

    }
}