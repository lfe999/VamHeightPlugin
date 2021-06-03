using UnityEngine;

namespace LFE {
    public class BaseVisualGuides : MonoBehaviour {
        public string UnitDisplay { get; set; } = UnitUtils.Centimeters;
        public bool Enabled { get; set; } = true;
        public bool LabelsEnabled { get; set; } = true;
        public float LineThickness { get; set; } = 3;
        public Color LineColor { get; set; } = Color.green;
        public Vector3 Offset { get; set; } = Vector3.zero;
        public float Spread { get; set; } = 0;
        public CharacterMeasurements Measurements { get; set; } = null;
    }
}