using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintManager : MonoBehaviour
{
    public GameObject linePrefab;
    private GameManager gm;
    private int numHands = 2;
    private int numButtons = 4;
    
    private GameObject[] currentLine;
    private List<GameObject>[] lines;
    private List<Vector3>[] drawPositions;

    private bool[] leftHoldKeys;
    private bool[] rightHoldKeys;

    public float[] lineWidths;
    private int leftWidthIdx;
    private int rightWidthIdx;
    private float scaleUnit;

    public Color[] colorPalette;
    private int leftColorIdx;
    private int rightColorIdx;

    // Start is called before the first frame update
    void Start()
    {
        gm = GetComponent<GameManager>();

        currentLine = new GameObject[numHands];
        lines = new List<GameObject>[numHands];
        drawPositions = new List<Vector3>[numHands];

        for (int i = 0; i < numHands; ++i)
        {
            lines[i] = new List<GameObject>();
            drawPositions[i] = new List<Vector3>();
        }

        leftHoldKeys = new bool[numButtons];
        rightHoldKeys = new bool[numButtons];

        for (int i = 0; i < numButtons; i++)
        {
            leftHoldKeys[i] = false;
            rightHoldKeys[i] = false;
        }

        leftWidthIdx = 0;
        rightWidthIdx = 0;
        leftColorIdx = 0;
        rightColorIdx = 0;
        scaleUnit = (float)1 / lineWidths.Length;
    }

    // Update is called once per frame
    void Update()
    {
        // debug
        /** you can use the below code block to test and get the key mapping
         for your own bluetooth controller
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                print("KeyCode down: " + kcode);
            }
        }
        **/

        /**
        Key mapping for blueTooth Controller used in our demo:
        joystick        keycode           keycode
        Button           Press            Release
          A                L                 V
          B                K                 P
          X                Y                 T
          Y                U                 F
          OK           Alpha1 & H        Alpha2 & R
          UP           W & UpArrow           E
         DOWN          X & DownArrow         Z
         LEFT          A & LeftArrow         Q
         RIGHT         D & RightArrow        C
        
        We use A and B and UP and DOWN for left controller, and X and Y and LEFT and RIGHT for right controller in our demo.
        **/
        string leftButtonInfo = gm.GetButtonStatesLeft();
        string rightButtonInfo = gm.GetButtonStatesRight();

        if (leftButtonInfo[0] == 'T')
        {
            // button A - draw line when button is pressed
            leftHoldKeys[0] = true;
            CreateLine(gm.GetPositionLeftHand(), gm.GetColorLeftHand(), 0);
        } else if (leftButtonInfo[0] == 'H')
        {
            if (!leftHoldKeys[0])
            {
                leftHoldKeys[0] = true;
                CreateLine(gm.GetPositionLeftHand(), gm.GetColorLeftHand(), 0);
            }
            else
            {
                UpdateLine(gm.GetPositionLeftHand(), 0);
            }
        } else
        {
            leftHoldKeys[0] = false;
        }

        if ((leftButtonInfo[1] == 'T' || leftButtonInfo[1] == 'H') && !leftHoldKeys[1])
        {
            // button B - undo last operation when pressed
            RemoveLine(0);
            leftHoldKeys[1] = true;
        } else if (leftButtonInfo[1] == 'F')
        {
            leftHoldKeys[1] = false;
        }

        if ((leftButtonInfo[2] == 'T' || leftButtonInfo[2] == 'H') && !leftHoldKeys[2])
        {
            // button UP - change line width
            leftWidthIdx++;
            if (leftWidthIdx >= lineWidths.Length)
            {
                // back to 0
                leftWidthIdx = 0;
            }
            float useScale = scaleUnit * leftWidthIdx + 1;
            gm.SetScaleLeftHand(new Vector3(useScale, useScale, useScale));
            leftHoldKeys[2] = true;
        } else if (leftButtonInfo[2] == 'F')
        {
            leftHoldKeys[2] = false;
        }

        if ((leftButtonInfo[3] == 'T' || leftButtonInfo[3] == 'H') && !leftHoldKeys[3])
        {
            // button DOWN - change line color (with hand prefab)
            leftColorIdx++;
            if (leftColorIdx >= colorPalette.Length)
            {
                // back to 0
                leftColorIdx = 0;
            }
            // change left hand color
            gm.SetColorLeftHand(colorPalette[leftColorIdx]);
            leftHoldKeys[3] = true;
        } else if (leftButtonInfo[3] == 'F')
        {
            leftHoldKeys[3] = false;
        }

        // right controller buttons
        if (rightButtonInfo[0] == 'T')
        {
            // button X - draw line when button is pressed
            rightHoldKeys[0] = true;
            CreateLine(gm.GetPositionRightHand(), gm.GetColorRightHand(), 1);
        } else if (rightButtonInfo[0] == 'H')
        {
            if (!rightHoldKeys[0])
            {
                rightHoldKeys[0] = true;
                CreateLine(gm.GetPositionRightHand(), gm.GetColorRightHand(), 1);
            }
            else
            {
                UpdateLine(gm.GetPositionRightHand(), 1);
            }
        } else
        {
            rightHoldKeys[0] = false;
        }

        if ((rightButtonInfo[1] == 'T' || rightButtonInfo[1] == 'H') && !rightHoldKeys[1])
        {
            // button Y - undo last operation when pressed
            RemoveLine(1);
            rightHoldKeys[1] = true;
        } else if (rightButtonInfo[1] == 'F')
        {
            rightHoldKeys[1] = false;
        }

        if ((rightButtonInfo[2] == 'T' || rightButtonInfo[2] == 'H') && !rightHoldKeys[2])
        {
            // button LEFT - change line width
            rightWidthIdx++;
            if (rightWidthIdx >= lineWidths.Length)
            {
                // back to 0
                rightWidthIdx = 0;
            }
            float useScale = scaleUnit * rightWidthIdx + 1;
            gm.SetScaleRightHand(new Vector3(useScale, useScale, useScale));
            rightHoldKeys[2] = true;
        } else if (rightButtonInfo[2] == 'F')
        {
            rightHoldKeys[2] = false;
        }

        if ((rightButtonInfo[3] == 'T' || rightButtonInfo[3] == 'H') && !rightHoldKeys[3])
        {
            // button RIGHT - change line color (with hand prefab)
            rightColorIdx++;
            if (rightColorIdx >= colorPalette.Length)
            {
                // back to 0
                rightColorIdx = 0;
            }
            // change left hand color
            gm.SetColorRightHand(colorPalette[rightColorIdx]);
            rightHoldKeys[3] = true;
        } else if (rightButtonInfo[3] == 'F')
        {
            rightHoldKeys[3] = false;
        }
    }

    void CreateLine(Vector3 position, Color color, int num)
    {
        currentLine[num] = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lines[num].Add(currentLine[num]);
        LineRenderer lineRenderer = currentLine[num].GetComponent<LineRenderer>();
        lineRenderer.material.color = color;
        if (num == 0)
        {
            //left hand
            lineRenderer.widthMultiplier = lineWidths[leftWidthIdx];
        } else {
            // right hand
            lineRenderer.widthMultiplier = lineWidths[rightWidthIdx];
        }
        drawPositions[num].Clear();
        drawPositions[num].Add(position);
        drawPositions[num].Add(position);
        lineRenderer.SetPosition(0, drawPositions[num][0]);
        lineRenderer.SetPosition(1, drawPositions[num][1]);
    }

    void UpdateLine(Vector3 position, int num)
    {
        drawPositions[num].Add(position);
        LineRenderer lineRenderer = currentLine[num].GetComponent<LineRenderer>();
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
    }

    void RemoveLine(int num)
    {
        if (lines[num].Count > 0)
        {
            GameObject last = lines[num][lines[num].Count - 1];
            Destroy(last);
            lines[num].RemoveAt(lines[num].Count - 1);
        }
    }
}
