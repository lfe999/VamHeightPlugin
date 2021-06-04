using UnityEngine;
using System.Collections.Generic;

namespace LFE {
    public class ArcVisualGuides : BaseVisualGuides {

        List<GameObject> _markers = new List<GameObject>();
        LineRenderer _line;

        public Vector3[] Points { get; set; }

        public void Awake() {
            LineColor = Color.green;

            _markers = new List<GameObject>();

            var go = new GameObject();
            go.transform.SetParent(transform);
            var lr = go.AddComponent<LineRenderer>();
            lr.transform.SetParent(go.transform);
            lr.gameObject.SetActive(false);
            // lr.useWorldSpace = false;
            _line = lr;
        }


        bool _enabledPrev = false; // allows for performant disabling
        public void Update()  {
            // allows for performant disabling with OnEnable/Disable
            // if disabling, the Update should still try and shut off
            // components one last time
            if(_enabledPrev == false && !Enabled) {
                return;
            }
            _enabledPrev = Enabled;

            // make sure markers buffer is at least as long as the points known about
            if(Points.Length > _markers.Count) {
                for(var i = _markers.Count; i < Points.Length; i++) {
                    // create 
                    _markers.Add(CreateMarker(LineColor));
                }
            }
            // update all markers with positions, colors, shown, and stuff
            for(var i = 0; i < _markers.Count; i++) {
                if(i < Points.Length) {
                    _markers[i].SetActive(Enabled);
                    _markers[i].transform.position = Points[i];
                    var r = _markers[i].GetComponent<Renderer>();
                    if(r) {
                        r.material.color = LineColor;
                    }
                }
                else {
                    // more markers than points, hide these
                    _markers[i].SetActive(false);
                }
            }

            // update the line
            _line.startWidth = LineThickness * 0.001f;
            _line.endWidth = LineThickness * 0.001f;
            _line.material.color = LineColor;
            _line.positionCount = Points.Length;
            _line.gameObject.SetActive(Enabled);
            for(var i = 0; i < Points.Length; i++) {
                _line.SetPosition(i, Points[i]);
            }

        }

        public void OnDestroy() {
            foreach(var m in _markers) {
                Destroy(m);
            }
            _markers = new List<GameObject>();

            if(_line != null) {
                Destroy(_line.transform.parent.gameObject);
            }
            _line = null;
        }

        private GameObject CreateMarker(Color color) {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.SetParent(transform);

            // random position to help avoid physics problems.
            go.transform.position = new Vector3 ((UnityEngine.Random.value*461)+10, (UnityEngine.Random.value*300)+10, 0F);

            go.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

            var r = go.GetComponent<Renderer>();
            if(r) {
                r.material.color = color;
            }

            // remove collisions
            foreach(var c in go.GetComponents<Collider>()) {
                Destroy(c);
            }

            go.SetActive(false);

            return go;
        }
    }
}