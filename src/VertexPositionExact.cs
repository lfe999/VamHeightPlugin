using UnityEngine;

namespace LFE {
    public class VertexPositionExact : IVertexPosition {
        readonly int _indexA;

        public VertexPositionExact(int indexA)
        {
            _indexA = indexA;
        }

        public Vector3 Position(HeightMeasurePlugin plugin) {
            if(plugin.Skin == null) {
                return Vector3.zero;
            }
            if(_indexA < 0 || _indexA >= plugin.Skin.rawSkinnedVerts.Length) {
                return Vector3.zero;
            }
            return plugin.Skin.rawSkinnedVerts[_indexA];
        }
    }
}