using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;

public class InputController : MonoBehaviour
{
    public float raycastDistance = 100f;
    private Rubik _rubik;
    private Camera _mainCamera;
    private UIDocument _pauseMenu;
    private UIDocument _victoryScreen;


    public bool playerInputEnabled = true;
    private bool isRotating;
    private float startMousePosition;

    public float Speed = 10f;

    public Faces faceClicked { get; private set; }
    public Faces faceToTurn { get; private set; }

    private void Start()
    {
        _mainCamera = Camera.main;
        _rubik = FindObjectOfType<Rubik>();
        _pauseMenu = GameObject.Find("PauseMenu").GetComponent<UIDocument>();
        _pauseMenu.rootVisualElement.style.display = DisplayStyle.None;
        _victoryScreen = GameObject.Find("VictoryScreen").GetComponent<UIDocument>();
        _victoryScreen.rootVisualElement.style.display = DisplayStyle.None;

    }

    void Update()
    {
        if (playerInputEnabled)
        {
            //Click
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, raycastDistance) &&
                    hit.collider.gameObject.name != "Piece(Clone)") {

                    bool turnClockwise = Input.GetMouseButtonDown(0);
                    _rubik.Turn(Rubik.IdentifyFace(hit.collider.gameObject.transform.up), turnClockwise);
                    //Rubik.PrintCubeMatrix(_rubik.CubeMatrix);
                    if (_rubik.CheckSolved()) {
                        playerInputEnabled = false;
                        _victoryScreen.rootVisualElement.style.display = DisplayStyle.Flex;
                    }
                }
            }

            //Click and drag
            /*if (Input.GetMouseButtonDown(0)) {
                isRotating = true;
                startMousePosition = Input.mousePosition.x;
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, raycastDistance) &&
                    hit.collider.gameObject.name != "Piece(Clone)") {

                    faceClicked = Rubik.IdentifyFace(hit.collider.gameObject.transform.up);
                }
            }
            else if (Input.GetMouseButtonUp(0)) {
                isRotating = false;
                _rubik.Turn(faceToTurn, true);
            }
            if (isRotating) {
                float currentMousePosition = Input.mousePosition.x;
                Vector3 dir = new Vector3(startMousePosition, currentMousePosition);
                Vector3 turnNormal = Vector3.Cross(dir, Rubik.FaceNormals[faceClicked]);
                Debug.DrawLine(turnNormal, turnNormal);

                float minDistance = float.MaxValue;
                faceToTurn = Faces.White;
                foreach (var face in Rubik.FaceNormals.Values) {
                    if (Vector3.Distance(turnNormal, face) < minDistance) {
                        minDistance = Vector3.Distance(turnNormal, face);
                        faceToTurn = Rubik.IdentifyFace(face);
                    }
                }
            }*/

            //Scramble cube
            if (Input.GetKeyDown(KeyCode.R))
            {
                _rubik.Randomize();
            }
        }
        //Open-Close pause menu
        if (Input.GetKeyDown(KeyCode.M))
        {
            playerInputEnabled = !playerInputEnabled;
            _pauseMenu.rootVisualElement.style.display = _pauseMenu.rootVisualElement.style.display == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
