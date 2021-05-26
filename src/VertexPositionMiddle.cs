using UnityEngine;

namespace LFE {
    public class VertexPositionMiddle : IVertexPosition {
        readonly int _indexA;
        readonly int _indexB;
        readonly float _ratio;

        public VertexPositionMiddle(int indexA, int indexB, float ratio = 0.5f)
        {
            _indexA = indexA;
            _indexB = indexB;
            _ratio = Mathf.Clamp01(ratio);
        }

        public Vector3 Position(HeightMeasurePlugin plugin) {
            if(plugin.Skin == null) {
                return Vector3.zero;
            }
            if(_indexA < 0 || _indexA >= plugin.Skin.rawSkinnedVerts.Length) {
                return Vector3.zero;
            }
            if(_indexB < 0 || _indexB >= plugin.Skin.rawSkinnedVerts.Length) {
                return Vector3.zero;
            }

            var vertexA = plugin.Skin.rawSkinnedVerts[_indexA];
            var vertexB = plugin.Skin.rawSkinnedVerts[_indexB];

            return Vector3.Lerp(vertexA, vertexB, _ratio);
        }
    }
}