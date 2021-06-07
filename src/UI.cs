using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace LFE {
    public class UI {

        private readonly ICupCalculator[] _cupCalculators = new ICupCalculator[] {
            new EvasIntimatesCupCalculator(),
            new KnixComCupCalculator(),
            new SizeChartCupCalculator(),
            new ChateLaineCupCalculator(),
            new KSK9404CupCalculator()
        };


        private static Color HEADER_COLOR = new Color(0, 0, 0, 0.25f);
        private static Color SPACER_COLOR = new Color(0, 0, 0, 0.5f);

        private readonly HeightMeasurePlugin _plugin;

        // measurement storables
        public JSONStorableFloat fullHeightStorable;
        public JSONStorableFloat headSizeHeightStorable;
        public JSONStorableFloat headSizeWidthStorable;
        public JSONStorableFloat chinHeightStorable;
        public JSONStorableFloat shoulderHeightStorable;
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

        // TOOD: shoulder width

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

        public JSONStorableBool showHeadHeightMarkersStorable;
        public JSONStorableColor headMarkerColor;
        public JSONStorableFloat lineThicknessHeadStorable;

        public JSONStorableBool showFaceMarkersStorable;
        public JSONStorableColor faceMarkerColor;
        public JSONStorableFloat lineThicknessFaceStorable;

        public JSONStorableBool showCircumferenceMarkersStorable;
        public JSONStorableColor circumferenceMarkerColor;
        public JSONStorableFloat lineThicknessCircumferenceStorable;

        public JSONStorableStringChooser cupAlgorithmStorable;
        public JSONStorableStringChooser unitsStorable;
        public JSONStorableFloat markerSpreadStorable;
        public JSONStorableFloat markerLeftRightStorable;
        public JSONStorableFloat markerFrontBackStorable;
        public JSONStorableFloat markerUpDownStorable;

        public ICupCalculator CupCalculator => _cupCalculators.FirstOrDefault(c => c.Name.Equals(cupAlgorithmStorable.val));

        public UI(HeightMeasurePlugin plugin) {
            _plugin = plugin;
            InitStorables();
            Draw();
        }

        private bool _choosingFeatureColor = false;
        private bool _choosingHeadColor = false;
        private bool _choosingFaceColor = false;
        private bool _choosingCircumferenceColor = false;
        private bool _choosingManualColor = false;
        private void InitStorables() {

            //////////////////////////////////////
            // UI related
            // Cup algorithm choice

            showFeatureMarkersStorable = new JSONStorableBool("Auto Feature Guides", true, (bool value) => {
                showFeatureMarkersStorable.valNoCallback = value;
                Draw();
            });
            _plugin.RegisterBool(showFeatureMarkersStorable);

            lineThicknessFigureStorable = new JSONStorableFloat("Feature Guide Line Width", 2, 1, 10, constrain: true);
            _plugin.RegisterFloat(lineThicknessFigureStorable);

            featureMarkerColor = new JSONStorableColor("Feature Guide Color", _plugin.ColorToHSV(Color.green), (float h, float s, float v) => {
                featureMarkerColor.valNoCallback = new HSVColor { H = h, S = s, V = v };
                _featureMarkerColorButton.buttonColor = _plugin.HSVToColor(featureMarkerColor.valNoCallback);
            });

            showFeatureMarkerLabelsStorable = new JSONStorableBool("Feature Guide Labels", true, (bool value) => {
                Draw();
            });
            _plugin.RegisterBool(showFeatureMarkerLabelsStorable);

            //////////////////

            showHeadHeightMarkersStorable = new JSONStorableBool("Auto Head Heights Guides", true, (bool value) => {
                showHeadHeightMarkersStorable.valNoCallback = value;
                Draw();
            });
            _plugin.RegisterBool(showHeadHeightMarkersStorable);

            lineThicknessHeadStorable = new JSONStorableFloat("Head Heights Line Width", 2, 1, 10, constrain: true);
            _plugin.RegisterFloat(lineThicknessHeadStorable);

            headMarkerColor = new JSONStorableColor("Feature Guide Color", _plugin.ColorToHSV(Color.white), (float h, float s, float v) => {
                headMarkerColor.valNoCallback = new HSVColor { H = h, S = s, V = v };
                _headMarkerColorButton.buttonColor = _plugin.HSVToColor(headMarkerColor.valNoCallback);
            });
            _plugin.RegisterColor(headMarkerColor);

            //////////////////

            showFaceMarkersStorable = new JSONStorableBool("Auto Face Guides", true, (bool value) => {
                Draw();
            });
            _plugin.RegisterBool(showFaceMarkersStorable);

            lineThicknessFaceStorable = new JSONStorableFloat("Face Line Width", 2, 1, 10, constrain: true);
            _plugin.RegisterFloat(lineThicknessFaceStorable);

            faceMarkerColor = new JSONStorableColor("Face Guide Color", _plugin.ColorToHSV(Color.blue), (float h, float s, float v) => {
                faceMarkerColor.valNoCallback = new HSVColor { H = h, S = s, V = v };
                _faceMarkerColorButton.buttonColor = _plugin.HSVToColor(faceMarkerColor.valNoCallback);
            });
            _plugin.RegisterColor(faceMarkerColor);

            //////////////////

            showCircumferenceMarkersStorable = new JSONStorableBool("Auto Circumference Guides", false, (bool value) => {
                showCircumferenceMarkersStorable.valNoCallback = value;
                Draw();
            });
            _plugin.RegisterBool(showCircumferenceMarkersStorable);

            lineThicknessCircumferenceStorable = new JSONStorableFloat("Circumference Guide Line Width", 2, 1, 10, constrain: true);
            _plugin.RegisterFloat(lineThicknessCircumferenceStorable);

            circumferenceMarkerColor = new JSONStorableColor("Circumference Guide Color", _plugin.ColorToHSV(Color.green), (float h, float s, float v) => {
                circumferenceMarkerColor.valNoCallback = new HSVColor { H = h, S = s, V = v };
                _circumferenceMarkerColorButton.buttonColor = _plugin.HSVToColor(circumferenceMarkerColor.valNoCallback);
            });
            _plugin.RegisterColor(circumferenceMarkerColor);

            //////////////////

            // Choose
            cupAlgorithmStorable = new JSONStorableStringChooser(
                "Cup Size Method",
                _cupCalculators.Select(cc => cc.Name).ToList(),
                _cupCalculators[0].Name,
                "Cup Size Method"
            );
            _plugin.RegisterStringChooser(cupAlgorithmStorable);

            unitsStorable = new JSONStorableStringChooser(
                "Units",
                new List<string> { "us", "metric", "m", "cm", "in", "ft" },
                new List<string> { "US", "Metric", "Meters", "Centimeters", "Inches", "Feet"},
                "cm",
                "Units"
            );
            _plugin.RegisterStringChooser(unitsStorable);

            // Float: How far apart markers are spread apart
            markerSpreadStorable = new JSONStorableFloat("Spread Guides", 0.02f, -1, 1);
            _plugin.RegisterFloat(markerSpreadStorable);

            // Float: Move markers forward or backward relative to center depth of the head
            markerFrontBackStorable = new JSONStorableFloat("Move Guides Forward/Backward", 0.15f, -5, 5);
            _plugin.RegisterFloat(markerFrontBackStorable);

            // Float: Move markers left or right 
            markerLeftRightStorable = new JSONStorableFloat("Move Guides Left/Right", 0, -5, 5);
            _plugin.RegisterFloat(markerLeftRightStorable);

            // Float: Move markers forward or backward relative to center depth of the head
            markerUpDownStorable = new JSONStorableFloat("Move Guides Up/Down", 0, -5, 5);
            _plugin.RegisterFloat(markerUpDownStorable);


            //////////////////////////////////////

            // calculated positions and distances for other _plugins to use if wanted
            fullHeightStorable = new JSONStorableFloat("figureHeight", 0, 0, 100);
            _plugin.RegisterFloat(fullHeightStorable);

            heightInHeadsStorable = new JSONStorableFloat("figureHeightHeads", 0, 0, 100);
            _plugin.RegisterFloat(heightInHeadsStorable);

            headSizeHeightStorable = new JSONStorableFloat("headSizeHeight", 0, 0, 100);
            _plugin.RegisterFloat(headSizeHeightStorable);

            chinHeightStorable = new JSONStorableFloat("chinHeight", 0, 0, 100);
            _plugin.RegisterFloat(chinHeightStorable);

            shoulderHeightStorable = new JSONStorableFloat("shoulderHeight", 0, 0, 100);
            _plugin.RegisterFloat(shoulderHeightStorable);

            nippleHeightStorable = new JSONStorableFloat("nippleHeight", 0, 0, 100);
            _plugin.RegisterFloat(nippleHeightStorable);

            underbustHeightStorable = new JSONStorableFloat("underbustHeight", 0, 0, 100);
            _plugin.RegisterFloat(underbustHeightStorable);

            navelHeightStorable = new JSONStorableFloat("navelHeight", 0, 0, 100);
            _plugin.RegisterFloat(navelHeightStorable);

            crotchHeightStorable = new JSONStorableFloat("crotchHeight", 0, 0, 100);
            _plugin.RegisterFloat(crotchHeightStorable);

            kneeBottomHeightStorable = new JSONStorableFloat("kneeHeight", 0, 0, 100);
            _plugin.RegisterFloat(kneeBottomHeightStorable);

            headSizeWidthStorable = new JSONStorableFloat("headSizeWidth", 0, 0, 100);
            _plugin.RegisterFloat(headSizeWidthStorable);

            breastBustStorable = new JSONStorableFloat("breastBustSize", 0, 0, 100);
            _plugin.RegisterFloat(breastBustStorable);

            breastUnderbustStorable = new JSONStorableFloat("breastUnderbustSize", 0, 0, 100);
            _plugin.RegisterFloat(breastUnderbustStorable);

            breastBandStorable = new JSONStorableFloat("breastBandSize", 0, 0, 100);
            _plugin.RegisterFloat(breastBandStorable);

            breastCupStorable = new JSONStorableString("breastCupSize", "");
            _plugin.RegisterString(breastCupStorable);

            waistSizeStorable = new JSONStorableFloat("waistSize", 0, 0, 100);
            _plugin.RegisterFloat(waistSizeStorable);

            hipSizeStorable = new JSONStorableFloat("hipSize", 0, 0, 100);
            _plugin.RegisterFloat(hipSizeStorable);

            penisLength = new JSONStorableFloat("penisLength", 0, 0, 100);
            _plugin.RegisterFloat(penisLength);

            penisWidth = new JSONStorableFloat("penisWidth", 0, 0, 100);
            _plugin.RegisterFloat(penisWidth);

            penisGirth = new JSONStorableFloat("penisGirth", 0, 0, 100);
            _plugin.RegisterFloat(penisGirth);

            // manual heights
            showManualMarkersStorable = new JSONStorableBool("Manual Markers", false, (bool value) => {
                showManualMarkersStorable.valNoCallback = value;
                Draw();
            });
            _plugin.RegisterBool(showManualMarkersStorable);

            manualMarkersCopy = new JSONStorableBool("Copy Markers From Person", false);
            _plugin.RegisterBool(manualMarkersCopy);

            lineThicknessManualStorable = new JSONStorableFloat("Manual Guide Line Width", 2, 1, 10, constrain: true);
            _plugin.RegisterFloat(lineThicknessManualStorable);

            manualMarkerColor = new JSONStorableColor("Manual Guide Color", _plugin.ColorToHSV(Color.yellow), (float h, float s, float v) => {
                manualMarkerColor.valNoCallback = new HSVColor { H = h, S = s, V = v };
                _manualMarkerColorButton.buttonColor = _plugin.HSVToColor(manualMarkerColor.valNoCallback);
            });

            manualHeightStorable = new JSONStorableFloat("Height", 0, 0, 300);
            _plugin.RegisterFloat(manualHeightStorable);

            manualChinHeightStorable = new JSONStorableFloat("Chin Height", 0, 0, 300);
            _plugin.RegisterFloat(manualChinHeightStorable);

            manualShoulderHeightStorable = new JSONStorableFloat("Shoulder Height", 0, 0, 300);
            _plugin.RegisterFloat(manualShoulderHeightStorable);

            manualNippleHeightStorable = new JSONStorableFloat("Bust Height", 0, 0, 300);
            _plugin.RegisterFloat(manualNippleHeightStorable);

            manualUnderbustHeightStorable = new JSONStorableFloat("Underbust Height", 0, 0, 300);
            _plugin.RegisterFloat(manualUnderbustHeightStorable);

            manualNavelHeightStorable = new JSONStorableFloat("Navel Height", 0, 0, 300);
            _plugin.RegisterFloat(manualNavelHeightStorable);

            manualCrotchHeightStorable = new JSONStorableFloat("Crotch Height", 0, 0, 300);
            _plugin.RegisterFloat(manualCrotchHeightStorable);

            manualKneeBottomHeightStorable = new JSONStorableFloat("Knee Height", 0, 0, 300);
            _plugin.RegisterFloat(manualKneeBottomHeightStorable);
        }

        private UIDynamicToggle _featureMarkerToggle;
        private UIDynamicButton _featureMarkerColorButton;
        private UIDynamicColorPicker _featureMarkerColorPicker;
        private UIDynamicSlider _featureMarkerLineThickness;
        private UIDynamicToggle _featureMarkerToggleLabels;

        private UIDynamicToggle _headMarkerToggle;
        private UIDynamicButton _headMarkerColorButton;
        private UIDynamicColorPicker _headMarkerColorPicker;
        private UIDynamicSlider _headMarkerLineThickness;

        private UIDynamicToggle _faceMarkerToggle;
        private UIDynamicButton _faceMarkerColorButton;
        private UIDynamicColorPicker _faceMarkerColorPicker;
        private UIDynamicSlider _faceMarkerLineThickness;

        private UIDynamicToggle _circumferenceMarkerToggle;
        private UIDynamicButton _circumferenceMarkerColorButton;
        private UIDynamicColorPicker _circumferenceMarkerColorPicker;
        private UIDynamicSlider _circumferenceMarkerLineThickness;

        private UIDynamicPopup _cupAlgorithm;
        private UIDynamicPopup _units;
        private UIDynamicSlider _markerSpread;
        private UIDynamicSlider _markerFrontBack;
        private UIDynamicSlider _markerLeftRight;
        private UIDynamicSlider _markerUpDown;

        private UIDynamicToggle _manualMarkerToggle;
        private UIDynamicToggle _copyManualMarkers;
        private UIDynamicButton _manualMarkerColorButton;
        private UIDynamicColorPicker _manualMarkerColorPicker;
        private UIDynamicSlider _manualMarkerLineThickness;
        private List<UIDynamicSlider> _manualSliders = new List<UIDynamicSlider>();

        public void Draw() {
            Clear();

            int defaultButtonHeight = 50;
            int defaultSliderHeight = 120;
            int defaultSectionSpacerHeight = 0;

            if(_plugin.containingAtom.type == "Person") {
                // Head Height Guides
                CreateStandardDivider(rightSide: false);
                _headMarkerToggle = _plugin.CreateToggle(showHeadHeightMarkersStorable, rightSide: false);
                _headMarkerToggle.backgroundColor = HEADER_COLOR;
                if(showHeadHeightMarkersStorable.val) {
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
                    CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                }
                else {
                    // if the right side is expanded, add some height padding
                    if(showFeatureMarkersStorable.val) {
                        CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                        CreateStandardSpacer(defaultSliderHeight, rightSide: false);
                        CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                    }
                }
                CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: false);

                // Feature Guides
                CreateStandardDivider(rightSide: true);
                _featureMarkerToggle = _plugin.CreateToggle(showFeatureMarkersStorable, rightSide: true);
                _featureMarkerToggle.backgroundColor = HEADER_COLOR;
                if(showFeatureMarkersStorable.val) {
                    _featureMarkerColorButton = _plugin.CreateButton("Set Line Color", rightSide: true);
                    _featureMarkerColorButton.buttonColor = _plugin.HSVToColor(featureMarkerColor.val);
                    _featureMarkerColorButton.button.onClick.AddListener(() => {
                        _choosingFeatureColor = !_choosingFeatureColor;
                        Draw();
                    });
                    if(_choosingFeatureColor) {
                        _featureMarkerColorPicker = _plugin.CreateColorPicker(featureMarkerColor, rightSide: true);
                    }
                    _featureMarkerLineThickness = _plugin.CreateSlider(lineThicknessFigureStorable, rightSide: true);
                    _featureMarkerToggleLabels = _plugin.CreateToggle(showFeatureMarkerLabelsStorable, rightSide: true);
                }
                else {
                    // if the left side is expanded, add some height padding
                    if(showHeadHeightMarkersStorable.val) {
                        CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                        CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                        CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                    }
                }
                CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: true);

                // Face Guides
                CreateStandardDivider(rightSide: false);
                _faceMarkerToggle = _plugin.CreateToggle(showFaceMarkersStorable, rightSide: false);
                _faceMarkerToggle.backgroundColor = HEADER_COLOR;
                if(showFaceMarkersStorable.val) {
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
                }
                else {
                    // if right side is expanded
                    if(showCircumferenceMarkersStorable.val) {
                        CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                        CreateStandardSpacer(defaultSliderHeight, rightSide: false);
                        CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                    }
                }
                CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: false);

                // Circumference Guides
                CreateStandardDivider(rightSide: true);
                _circumferenceMarkerToggle = _plugin.CreateToggle(showCircumferenceMarkersStorable, rightSide: true);
                _circumferenceMarkerToggle.backgroundColor = HEADER_COLOR;
                if(showCircumferenceMarkersStorable.val) {
                    _circumferenceMarkerColorButton = _plugin.CreateButton("Set Line Color", rightSide: true);
                    _circumferenceMarkerColorButton.buttonColor = _plugin.HSVToColor(circumferenceMarkerColor.val);
                    _circumferenceMarkerColorButton.button.onClick.AddListener(() => {
                        _choosingCircumferenceColor = !_choosingCircumferenceColor;
                        Draw();
                    });
                    if(_choosingCircumferenceColor) {
                        _circumferenceMarkerColorPicker = _plugin.CreateColorPicker(circumferenceMarkerColor, rightSide: true);
                    }
                    _circumferenceMarkerLineThickness = _plugin.CreateSlider(lineThicknessCircumferenceStorable, rightSide: true);
                    CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                }
                else {
                    // if the left side is expanded, add some height padding
                    if(showFaceMarkersStorable.val) {
                        CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                        CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                        CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                    }
                }
                CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: true);
            }

            CreateStandardDivider(rightSide: false);
            _cupAlgorithm = _plugin.CreateScrollablePopup(cupAlgorithmStorable, rightSide: false);
            _units = _plugin.CreateScrollablePopup(unitsStorable, rightSide: false);
            _markerSpread = _plugin.CreateSlider(markerSpreadStorable, rightSide: false);
            _markerFrontBack = _plugin.CreateSlider(markerFrontBackStorable, rightSide: false);
            _markerLeftRight = _plugin.CreateSlider(markerLeftRightStorable, rightSide: false);
            _markerUpDown = _plugin.CreateSlider(markerUpDownStorable, rightSide: false);

            CreateStandardDivider(rightSide: true);
            _manualMarkerToggle = _plugin.CreateToggle(showManualMarkersStorable, rightSide: true);
            if(showManualMarkersStorable.val) {
                if(_plugin.containingAtom.type == "Person") {
                    _copyManualMarkers = _plugin.CreateToggle(manualMarkersCopy, rightSide: true);
                }
                _manualMarkerColorButton = _plugin.CreateButton("Set Line Color", rightSide: true);
                _manualMarkerColorButton.buttonColor = _plugin.HSVToColor(manualMarkerColor.val);
                _manualMarkerColorButton.button.onClick.AddListener(() => {
                    _choosingManualColor = !_choosingManualColor;
                    Draw();
                });
                if(_choosingManualColor) {
                    _manualMarkerColorPicker = _plugin.CreateColorPicker(manualMarkerColor, rightSide: true);
                }
                _manualMarkerLineThickness = _plugin.CreateSlider(lineThicknessManualStorable, rightSide: true);
                _manualSliders.Add(_plugin.CreateSlider(manualHeightStorable, rightSide: true));
                _manualSliders.Add(_plugin.CreateSlider(manualChinHeightStorable, rightSide: true));
                _manualSliders.Add(_plugin.CreateSlider(manualShoulderHeightStorable, rightSide: true));
                _manualSliders.Add(_plugin.CreateSlider(manualNippleHeightStorable, rightSide: true));
                _manualSliders.Add(_plugin.CreateSlider(manualUnderbustHeightStorable, rightSide: true));
                _manualSliders.Add(_plugin.CreateSlider(manualNavelHeightStorable, rightSide: true));
                _manualSliders.Add(_plugin.CreateSlider(manualCrotchHeightStorable, rightSide: true));
                _manualSliders.Add(_plugin.CreateSlider(manualKneeBottomHeightStorable, rightSide: true));
            }
        }

        public void Clear() {
            foreach(var spacer in _spacerList) {
                _plugin.RemoveSpacer(spacer);
            }
            foreach(var spacer in _spacerLinesList) {
                _plugin.RemoveTextField(spacer);
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

            if(_headMarkerToggle) {
                _plugin.RemoveToggle(_headMarkerToggle);
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
    }
}