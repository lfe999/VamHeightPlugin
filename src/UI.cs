using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace LFE {
    public class UI {

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

        // TOOD: shoulder width

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

        public JSONStorableBool showTapeMarkersStorable;

        public JSONStorableStringChooser cupAlgorithmStorable;
        public JSONStorableFloat markerSpreadStorable;
        public JSONStorableFloat markerLeftRightStorable;
        public JSONStorableFloat markerFrontBackStorable;


        public UI(HeightMeasurePlugin plugin) {
            _plugin = plugin;
            InitStorables();
        }

        private bool _choosingFeatureColor = false;
        private bool _choosingHeadColor = false;
        private bool _choosingFaceColor = false;
        private void InitStorables() {

            //////////////////////////////////////
            // UI related
            // Cup algorithm choice

            showFeatureMarkersStorable = new JSONStorableBool("Feature Guides", true, (bool value) => {
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

            showHeadHeightMarkersStorable = new JSONStorableBool("Head Heights Guides", true, (bool value) => {
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

            showFaceMarkersStorable = new JSONStorableBool("Face Guides", true, (bool value) => {
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

            showTapeMarkersStorable = new JSONStorableBool("Circumference Guides", false, (bool value) => {
                showTapeMarkersStorable.valNoCallback = value;
                Draw();
            });
            _plugin.RegisterBool(showTapeMarkersStorable);

            //////////////////

            // Choose
            cupAlgorithmStorable = new JSONStorableStringChooser(
                "Cup Size Method",
                _plugin.cupCalculators.Select(cc => cc.Name).ToList(),
                _plugin.cupCalculators[0].Name,
                "Cup Size Method"
            );
            _plugin.RegisterStringChooser(cupAlgorithmStorable);

            // Float: How far apart markers are spread apart
            markerSpreadStorable = new JSONStorableFloat("Spread Guides", 0.02f, -1, 1);
            _plugin.RegisterFloat(markerSpreadStorable);

            // Float: Move markers forward or backward relative to center depth of the head
            markerFrontBackStorable = new JSONStorableFloat("Move Guides Forward/Backward", 0.15f, -5, 5);
            _plugin.RegisterFloat(markerFrontBackStorable);

            // Float: Move markers left or right 
            markerLeftRightStorable = new JSONStorableFloat("Move Guides Left/Right", 0, -5, 5);
            _plugin.RegisterFloat(markerLeftRightStorable);


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

        private UIDynamicPopup _cupAlgorithm;
        private UIDynamicSlider _markerSpread;
        private UIDynamicSlider _markerFrontBack;
        private UIDynamicSlider _markerLeftRight;

        public void Draw() {
            Clear();

            int defaultButtonHeight = 50;
            int defaultSliderHeight = 120;
            int defaultSectionSpacerHeight = 0;

            // Feature Guides
            CreateStandardDivider();
            _featureMarkerToggle = _plugin.CreateToggle(showFeatureMarkersStorable);
            _featureMarkerToggle.backgroundColor = HEADER_COLOR;
            if(showFeatureMarkersStorable.val) {
                _featureMarkerColorButton = _plugin.CreateButton("Set Line Color");
                _featureMarkerColorButton.buttonColor = _plugin.HSVToColor(featureMarkerColor.val);
                _featureMarkerColorButton.button.onClick.AddListener(() => {
                    _choosingFeatureColor = !_choosingFeatureColor;
                    Draw();
                });
                if(_choosingFeatureColor) {
                    _featureMarkerColorPicker = _plugin.CreateColorPicker(featureMarkerColor);
                }
                _featureMarkerLineThickness = _plugin.CreateSlider(lineThicknessFigureStorable);
                _featureMarkerToggleLabels = _plugin.CreateToggle(showFeatureMarkerLabelsStorable);
            }
            else {
                CreateStandardSpacer(defaultButtonHeight);
                CreateStandardSpacer(defaultSliderHeight);
                CreateStandardSpacer(defaultButtonHeight);
            }
            CreateStandardSpacer(defaultSectionSpacerHeight);

            // Head Height Guides
            CreateStandardDivider(rightSide: true);
            _headMarkerToggle = _plugin.CreateToggle(showHeadHeightMarkersStorable, rightSide: true);
            _headMarkerToggle.backgroundColor = HEADER_COLOR;
            if(showHeadHeightMarkersStorable.val) {
                _headMarkerColorButton = _plugin.CreateButton("Set Line Color", rightSide: true);
                _headMarkerColorButton.buttonColor = _plugin.HSVToColor(headMarkerColor.val);
                _headMarkerColorButton.button.onClick.AddListener(() => {
                    _choosingHeadColor = !_choosingHeadColor;
                    Draw();
                });
                if(_choosingHeadColor) {
                    _headMarkerColorPicker = _plugin.CreateColorPicker(headMarkerColor, rightSide: true);
                }
                _headMarkerLineThickness = _plugin.CreateSlider(lineThicknessHeadStorable, rightSide: true);
                CreateStandardSpacer(defaultButtonHeight, rightSide: true);
            }
            else {
                CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                CreateStandardSpacer(defaultButtonHeight, rightSide: true);
            }
            CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: true);

            // Face Guides
            CreateStandardDivider();
            _faceMarkerToggle = _plugin.CreateToggle(showFaceMarkersStorable);
            _faceMarkerToggle.backgroundColor = HEADER_COLOR;
            if(showFaceMarkersStorable.val) {
                _faceMarkerColorButton = _plugin.CreateButton("Set Line Color");
                _faceMarkerColorButton.buttonColor = _plugin.HSVToColor(faceMarkerColor.val);
                _faceMarkerColorButton.button.onClick.AddListener(() => {
                    _choosingFaceColor = !_choosingFaceColor;
                    Draw();
                });
                if(_choosingFaceColor) {
                    _faceMarkerColorPicker = _plugin.CreateColorPicker(faceMarkerColor);
                }
                _faceMarkerLineThickness = _plugin.CreateSlider(lineThicknessFaceStorable);
                CreateStandardSpacer(defaultButtonHeight);
            }
            else {
                CreateStandardSpacer(defaultButtonHeight);
                CreateStandardSpacer(defaultSliderHeight);
                CreateStandardSpacer(defaultButtonHeight);
            }
            CreateStandardSpacer(defaultSectionSpacerHeight);

            CreateStandardDivider();

            // Circumference Guides
            CreateStandardDivider(rightSide: true);
            _circumferenceMarkerToggle = _plugin.CreateToggle(showTapeMarkersStorable, rightSide: true);
            _circumferenceMarkerToggle.backgroundColor = HEADER_COLOR;
            if(showTapeMarkersStorable.val) {
                CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                CreateStandardSpacer(defaultButtonHeight, rightSide: true);
            }
            else {
                CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                CreateStandardSpacer(defaultButtonHeight, rightSide: true);
            }
            CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: true);

            // right side
            CreateStandardDivider(rightSide: true);
            _cupAlgorithm = _plugin.CreateScrollablePopup(cupAlgorithmStorable);
            _markerSpread = _plugin.CreateSlider(markerSpreadStorable, rightSide: true);
            _markerFrontBack = _plugin.CreateSlider(markerFrontBackStorable, rightSide: true);
            _markerLeftRight = _plugin.CreateSlider(markerLeftRightStorable, rightSide: true);

        }

        public void Clear() {
            foreach(var spacer in _spacerList) {
                _plugin.RemoveSpacer(spacer);
            }
            foreach(var spacer in _spacerLinesList) {
                _plugin.RemoveTextField(spacer);
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
            if(_cupAlgorithm) {
                _plugin.RemovePopup(_cupAlgorithm);
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