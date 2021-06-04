using UnityEngine;
using UnityEngine.UI;

namespace LFE {

    public class LabeledLine : MonoBehaviour {
        public string Name { get; set; } = "Name";
        public string Label { get; set; }
        public Color Color { get; set; } = Color.green;
        public float Length { get; set; } = 0.5f;
        public float Thickness { get; set; } = 0.003f;
        public Vector3 LineDirection { get; set; } = Vector3.left;
        public bool Enabled { get; set; } = false;
        public bool LabelEnabled { get; set; } = false;

        LineRenderer _lineRenderer;
        RectTransform _rt;
        Canvas _canvas;

        public void Awake() {
            var go = new GameObject();
            go.transform.SetParent(transform);
            _lineRenderer = go.AddComponent<LineRenderer>();
            _lineRenderer.useWorldSpace = false;
            _lineRenderer.gameObject.SetActive(Enabled);
            _lineRenderer.startWidth = Thickness;
            _lineRenderer.endWidth = Thickness;
            _lineRenderer.material.color = Color;
            _lineRenderer.SetPositions(new Vector3[] {
                Vector3.zero,
                CalculateEndpoint(Vector3.zero)
            });
            InitTextLabel();
        }

        public void Update() {
            bool isHubShowing = SuperController.singleton?.hubBrowser?.UITransform?.gameObject?.activeSelf ?? false;
            bool isTopMenuShowing = SuperController.singleton?.worldUI?.gameObject?.activeSelf ?? false;
            if(isHubShowing || isTopMenuShowing) {
                _canvas.enabled = false;
            }
            else {
                _canvas.enabled = true;
            }

            _lineRenderer.material.color = Color;
            _lineRenderer.startColor = Color;
            _lineRenderer.endColor = Color;
            _lineRenderer.startWidth = Thickness;
            _lineRenderer.endWidth = Thickness;
            _lineRenderer.SetPositions(new Vector3[] {
                Vector3.zero,
                CalculateEndpoint(Vector3.zero)
            });
            _lineRenderer.gameObject.SetActive(Enabled);
            _canvas.gameObject.SetActive(_lineRenderer.gameObject.activeInHierarchy);
            if(Enabled) {
                var text = _canvas.GetComponentInChildren<Text>();
                if(text) {
                    _canvas.gameObject.SetActive(LabelEnabled);
                    if(LineDirection == Vector3.left) {
                        text.alignment = TextAnchor.MiddleLeft;
                        text.text = Label;
                        text.color = Color;
                        if(_rt != null) {
                            _rt.anchoredPosition = new Vector2((Length * 500f) + 5, 0);
                            _rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
                            _rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

                        }
                    }
                    if(LineDirection == Vector3.right) {
                        text.alignment = TextAnchor.MiddleRight;
                        text.text = Label;
                        text.color = Color;
                        if(_rt != null){
                            _rt.anchoredPosition = new Vector2((Length * -500f) - 30, 0);
                            _rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 50);
                            _rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
                        }
                    }
                }
            }
        }

        public void OnDestroy() {
            Destroy(_lineRenderer);
            _lineRenderer = null;
            if (SuperController.singleton != null)
            {
                SuperController.singleton.RemoveCanvas(_canvas);
            }
            if(_canvas != null) {
                _canvas.transform.SetParent(null, false);
                if (_canvas.gameObject != null)
                {
                    Destroy(_canvas.gameObject);
                }
            }
        }

        private Vector3 CalculateEndpoint(Vector3 start) {
            var direction = LineDirection;
            direction.x *= Length;
            direction.y *= Length;
            direction.z *= Length;
            return start + direction;
        }

        private void InitTextLabel() {
            GameObject canvasObject = new GameObject();
            canvasObject.transform.SetParent(transform, false);

            _canvas = canvasObject.AddComponent<Canvas>();
            _canvas.sortingOrder = -100;
            _canvas.renderMode = RenderMode.WorldSpace;
            _canvas.pixelPerfect = false;
            _canvas.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);
            _canvas.transform.Rotate(new Vector3(0, 180, 0));
            SuperController.singleton.AddCanvas(_canvas);

            CanvasScaler cs = _canvas.gameObject.AddComponent<CanvasScaler>();
            cs.scaleFactor = 80.0f;
            cs.dynamicPixelsPerUnit = 1f;

            GameObject container = new GameObject();
            container.name = "Placeholder";
            container.transform.SetParent(_canvas.transform, false);

            Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

            Text placeholderText = container.AddComponent<Text>();
            placeholderText.color = new Color(1, 1, 1);
            placeholderText.font = ArialFont;
            placeholderText.fontSize = 20;
            placeholderText.fontStyle = FontStyle.Normal;
            placeholderText.supportRichText = false;
            placeholderText.horizontalOverflow = HorizontalWrapMode.Overflow;
            placeholderText.verticalOverflow = VerticalWrapMode.Overflow;
            placeholderText.text = "";

            _rt = container.GetComponent<RectTransform>();
            // var textOffsetX = (Length * 500) + 5;
            // if(LineDirection == Vector3.left) {
            //     textOffsetX *= -1;
            // }
            // float horizontalOffset = (LineDirection == Vector3.right) ? (Length * 500f) + 50 : -1 * ((Length * 500f) + 5);
            _rt.anchoredPosition = new Vector2(0, 0);
            _rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            _rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        }
    }
}