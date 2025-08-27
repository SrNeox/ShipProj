using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

[RequireComponent(typeof(InputPlayer))]
public class DrawLine : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private LayerMask _planeLayerMask;
    private InputPlayer _inputPlayer;

    private bool _isDrawing = false;
    private bool _isOverDrawZone = false; // ‘лаг нахождени€ над зоной рисовани€

    public List<Vector3> MousePositionList { get; private set; }

    private void Awake()
    {
        _inputPlayer = GetComponent<InputPlayer>();
        _lineRenderer = GetComponent<LineRenderer>();

        _lineRenderer.widthCurve = AnimationCurve.Constant(0, 1, 0.3f);
        _lineRenderer.positionCount = 0;

        MousePositionList = new List<Vector3>();
    }

    private void Start()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
    }

    private void Update()
    {
        CheckDrawLine();
        CheckDrawZone(); // ѕровер€ем нахождение над зоной рисовани€
    }

    public void Init(LayerMask layerMask)
    {
        _planeLayerMask = layerMask;
    }

    // ѕроверка нахождени€ над зоной рисовани€
    private void CheckDrawZone()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        _isOverDrawZone = Physics.Raycast(ray, Mathf.Infinity, _planeLayerMask);
    }

    private void DrawVector()
    {
        if (!_isOverDrawZone) return; // –исуем только над специальной зоной

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _planeLayerMask))
        {
            Vector3 hitPoint = hit.point;

            if (MousePositionList.Count == 0 ||
                (MousePositionList[MousePositionList.Count - 1] - hitPoint).sqrMagnitude > 0.01f)
            {
                MousePositionList.Add(hitPoint);
                _lineRenderer.positionCount = MousePositionList.Count;
                _lineRenderer.SetPosition(MousePositionList.Count - 1, hitPoint);
            }
        }
    }

    private void Clear()
    {
        MousePositionList.Clear();
        _lineRenderer.positionCount = 0;
    }

    private void CheckDrawLine()
    {
        if (_inputPlayer.Draw())
        {
            // Ќачинаем рисование только если находимс€ над зоной
            if (!_isDrawing && _isOverDrawZone)
            {
                Clear();
                _isDrawing = true;
            }

            if (_isDrawing)
            {
                DrawVector();
            }
        }
        else
        {
            _isDrawing = false;
        }
    }
}