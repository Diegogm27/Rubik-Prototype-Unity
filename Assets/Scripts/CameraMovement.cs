using UnityEngine;
using Cinemachine;

public class CameraMovement : MonoBehaviour
{
    private CinemachineFreeLook _freeLookCamera;
    private CinemachineFreeLook.Orbit[] _originalOrbits;
    private Rubik _rubik;
    private InputController _inputController;

    [SerializeField] [Range(1f, 5f)] private float maxZoom;
    [SerializeField] [Range(1f, 5f)] private float minZoom;
    [SerializeField] [Range(100f, 1000f)] private float horizontalSpeed = 500f;
    [SerializeField] [Range(1f, 5f)] private float verticalSpeed = 2f;
    
    [AxisStateProperty]
    public AxisState zAxis = new AxisState(0, 1, false, true, 50f, 0.1f, 0.1f, "Mouse ScrollWheel", false);



    private void Awake()
    {
        _inputController = GameObject.Find("InputController").GetComponent<InputController>();
        _rubik = FindObjectOfType<Rubik>();
        _freeLookCamera = GetComponent<CinemachineFreeLook>();


        if (_freeLookCamera != null)
        {
            _originalOrbits = new CinemachineFreeLook.Orbit[_freeLookCamera.m_Orbits.Length];


            for (int i = 0; i < _freeLookCamera.m_Orbits.Length; i++)
            {
                _originalOrbits[i].m_Height = _freeLookCamera.m_Orbits[i].m_Height;
                _originalOrbits[i].m_Radius = _freeLookCamera.m_Orbits[i].m_Radius;
            }

            _freeLookCamera.LookAt = _rubik.transform;
        }
    }

    private void FixedUpdate()
    {
        if (_inputController.playerInputEnabled)
        {
            if (Input.GetMouseButton(2))
            {
                _freeLookCamera.LookAt = _rubik.transform;
                _freeLookCamera.m_XAxis.m_MaxSpeed = horizontalSpeed;
                _freeLookCamera.m_YAxis.m_MaxSpeed = verticalSpeed;
            }
            else
            {
                _freeLookCamera.m_XAxis.m_MaxSpeed = 0;
                _freeLookCamera.m_YAxis.m_MaxSpeed = 0;
            }

            if (_originalOrbits != null)
            {
                // float normalizedScroll = Mathf.InverseLerp(-1, 1, Input.mouseScrollDelta.y);
                // float scale = Mathf.Lerp(maxZoom, minZoom, normalizedScroll);
                zAxis.Update(Time.deltaTime);
                float scale = Mathf.Lerp(maxZoom, minZoom, zAxis.Value);
                for (int i = 0; i < _originalOrbits.Length; i++)
                {
                    _freeLookCamera.m_Orbits[i].m_Height = _originalOrbits[i].m_Height * scale;
                    _freeLookCamera.m_Orbits[i].m_Radius = _originalOrbits[i].m_Radius * scale;
                }
            }
        }
    }
}