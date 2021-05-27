using UnityEngine;
using UnityEngine.UI;

namespace LFE {

    public class HorizontalMarker : MonoBehaviour {
        public string Name { get; set; } = "Name";
        public string Label { get; set; }
        public Color Color { get; set; } = Color.green;
        public float Length { get; set; } = 0.5f;
        public float Thickness { get; set; } = 0.003f;
        public Vector3 Origin { get; set; } = Vector3.zero;
        public Vector3 LineDirection { get; set; } = Vector3.left;
        public bool Enabled { get; set; } = true;
        public bool LabelEnabled { get; set; } = true;

        LineRenderer _lineRenderer;
        Canvas _canvas;

        public void Awake() {
            var gameObject = new GameObject();
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.startWidth = Thickness;
            _lineRenderer.endWidth = Thickness;
            _lineRenderer.material.color = Color;
            _lineRenderer.SetPositions(new Vector3[] {
                Origin,
                CalculateEndpoint(Origin)
            });
            InitTextLabel();
        }

        public void Update() {
            _canvas.enabled = SuperController.singleton.mainHUD.gameObject.activeSelf;
            transform.position = Origin;
            _lineRenderer.material.color = Color;
            _lineRenderer.startColor = Color;
            _lineRenderer.endColor = Color;
            _lineRenderer.startWidth = Thickness;
            _lineRenderer.endWidth = Thickness;
            _lineRenderer.SetPositions(new Vector3[] {
                Origin,
                CalculateEndpoint(Origin)
            });
            _lineRenderer.gameObject.SetActive(Enabled);
            // SuperController.LogMessage($"{_lineRenderer.gameObject.layer} - {_canvas.gameObject.layer}");
            _canvas.gameObject.SetActive(_lineRenderer.gameObject.activeInHierarchy);
            if(Enabled) {
                var text = _canvas.GetComponentInChildren<Text>();
                if(text) {
                    _canvas.gameObject.SetActive(LabelEnabled);
                    if(LineDirection == Vector3.left) {
                        text.text = Label;
                        text.transform.parent.transform.position = CalculateEndpoint(Origin);
                        text.color = Color;
                    }
                    if(LineDirection == Vector3.right) {
                        text.text = Label;
                        text.transform.parent.transform.position = CalculateEndpoint(Origin) + new Vector3(0.07f, 0, 0);
                        text.color = Color;
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
                        // test text input
            GameObject canvasObject = new GameObject();
            _canvas = canvasObject.AddComponent<Canvas>();
            _canvas.sortingOrder = -100;
            _canvas.renderMode = RenderMode.WorldSpace;
            _canvas.pixelPerfect = false;
            SuperController.singleton.AddCanvas(_canvas);

            CanvasScaler cs = canvasObject.AddComponent<CanvasScaler>();
            cs.scaleFactor = 80.0f;
            cs.dynamicPixelsPerUnit = 1f;

            // GraphicRaycaster gr = canvasObject.AddComponent<GraphicRaycaster>();
            _canvas.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);
            //canvas.transform.localPosition = new Vector3(-0.7f, 0, 0);
            _canvas.transform.localPosition = new Vector3(0.26f, -0.14f, 0.0f);
            _canvas.transform.Rotate(new Vector3(0, 180, 0));

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
            placeholderText.text = "";
            placeholderText.alignment = TextAnchor.MiddleLeft;


            container.transform.position = new Vector3(0, 0, 0);

            RectTransform rt = container.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(260, 0);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 500);
        }

        private bool ShouldShowCanvas() {
            return SuperController.singleton.mainHUD.gameObject.activeSelf;
        }
    }
}