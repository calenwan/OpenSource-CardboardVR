using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int cubeLength = 10; // the wand length on each hand
    [SerializeField]
    private int cubeWidth = 10; // the wand length on each hand
    [SerializeField]
    private int cubeHeight = 10; // the wand length on each hand
    private int numOfImagesEachHand = 6; // number of images for each hand, a 6-face cube
    private float rotateDegree; // for images on each hand, we will set different initial degrees
    [SerializeField]
    private int continueFrames = 60; // set number of frames to continue moving after image becomes invisble
    private int averageWindowSize = 15; // window size to average the velocity
    [SerializeField]
    private GameObject[] placeablePrefabs; // prefabs to place - in our case we use 2 (two hands)
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>(); // for each hand (prefab), store states
    private ARTrackedImageManager trackedImageManager;

    // store previous two images info for both hands
    private int numHands = 2;
    private int numButtons = 4;
    // number of frames left to render
    private int[] contFrames;
    // flag and index
    private bool[] isTracking;
    private bool[] validTracking;
    private int[] counter;
    // for position moving
    private Vector3[] lastPosition;
    private Vector3[] previousPVelocities;
    private Vector3[] avgPVelocity;
    // for rotation moving
    private Vector3[] lastRotation;
    private Vector3[] previousRVelocities;
    private Vector3[] avgRVelocity;

    // for buttons
    private string[] leftButtonInfo;
    private string[] rightButtonInfo;
    private bool[] leftButtonSwitch;
    private bool[] rightButtonSwitch;

    private void Awake()
    {
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        int j = 0;
        foreach(GameObject prefab in placeablePrefabs)
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newPrefab.name = prefab.name;
            for (int i = 0; i < numOfImagesEachHand; i++) {
                string name = (i+j*numOfImagesEachHand).ToString();
                spawnedPrefabs.Add(name, newPrefab);
            }
            j++;
        }
        rotateDegree = 360 / (numOfImagesEachHand-2); // rotate along with x-axis or z-axis

        contFrames = new int[numHands];
        isTracking = new bool[numHands];
        validTracking = new bool[numHands];
        counter = new int[numHands];
        
        lastPosition = new Vector3[numHands];
        avgPVelocity = new Vector3[numHands];
        lastRotation = new Vector3[numHands];
        avgRVelocity = new Vector3[numHands];
        previousPVelocities = new Vector3[numHands*averageWindowSize];
        previousRVelocities = new Vector3[numHands*averageWindowSize];

        for (int i = 0; i < numHands; ++i)
        {
            contFrames[i] = 0;
            isTracking[i] = false;
            validTracking[i] = false;
            counter[i] = 0;
            
            lastPosition[i] = Vector3.zero;
            avgPVelocity[i] = Vector3.zero;
            lastRotation[i] = Vector3.zero;
            avgRVelocity[i] = Vector3.zero;
        }

        // init button info for current controller
        leftButtonSwitch = new bool[numButtons];
        rightButtonSwitch = new bool[numButtons];
        leftButtonInfo = new string[numButtons];
        rightButtonInfo = new string[numButtons];
        for (int i = 0; i < numButtons; i++)
        {
            leftButtonSwitch[i] = false;
            rightButtonSwitch[i] = false;
            leftButtonInfo[i] = "F";
            rightButtonInfo[i] = "F";
        }
    }
    
    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += ImageChanged;
    }
    
    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= ImageChanged;
    }
    
    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }
        
        foreach(ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }
        
        foreach(ARTrackedImage trackedImage in eventArgs.removed)
        {
            // usually not called; currently, we use TrackingState == Limited to recognize that the image is not visble 
            spawnedPrefabs[trackedImage.name].SetActive(false);
        }
        
    }
    
    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name; // the name helps to detect the rotation of the game object
        int handNum = int.Parse(name, 0) / numOfImagesEachHand;

        if (trackedImage.trackingState == TrackingState.Tracking) // image is visible
        {
            // reset contframes
            contFrames[handNum] = 0;
            isTracking[handNum] = true;
            
            int imgNumOnHand = int.Parse(name, 0) % numOfImagesEachHand;
            float trot = imgNumOnHand * rotateDegree;
            
            if (imgNumOnHand == numOfImagesEachHand-2) {
                // if 5th image, rotate 3 rotateDegree
                trot = 3 * rotateDegree;
            } else if (imgNumOnHand == numOfImagesEachHand-1) {
                // if 6th image, rotate 1 rotateDegree
                trot = rotateDegree;
            }

            Vector3 offset = new Vector3(0f, 0f, 0f);
            if (imgNumOnHand == 1) {
                // 2nd image
                offset = new Vector3((float)cubeWidth/(-100), 0f, (float)cubeLength/100);
            } else if (imgNumOnHand == 2) {
                // 3rd image
                offset = new Vector3(0f, 0f, (float)cubeWidth/100);
            } else if (imgNumOnHand == 3) {
                // 4th image
                offset = new Vector3((float)cubeWidth/100, 0f, (float)cubeLength/100);
            } else if (imgNumOnHand == 4) {
                // 5th image
                offset = new Vector3(0f, (float)cubeWidth/(-100), (float)cubeHeight/200);
            } else if (imgNumOnHand == 5) {
                // 6th image
                offset = new Vector3(0f, (float)cubeWidth/100, (float)cubeHeight/200);
            }
            
            GameObject prefab = spawnedPrefabs[name];

            // get hand position
            prefab.transform.position = trackedImage.transform.position + offset;
            // get hand rotation
            prefab.transform.rotation = trackedImage.transform.rotation;
            // get hand rotation, along z-axis for first 4 images, and along x-axis for rest 2 images
            if (imgNumOnHand < numOfImagesEachHand-2) {
                prefab.transform.Rotate(0.0f, 0.0f, trot, Space.Self);
            } else {
                prefab.transform.Rotate(trot, 0.0f, 0.0f, Space.Self);
            }
            
            // store previous velocities
            previousPVelocities[handNum*averageWindowSize+counter[handNum]] = prefab.transform.rotation.eulerAngles - lastPosition[handNum];
            previousRVelocities[handNum*averageWindowSize+counter[handNum]] = prefab.transform.rotation.eulerAngles - lastRotation[handNum];
            counter[handNum]++;
            if (counter[handNum] >= averageWindowSize) {
                //valid velocity
                validTracking[handNum] = true;
                counter[handNum] = 0;
            }
            lastPosition[handNum] = prefab.transform.position;
            lastRotation[handNum] = prefab.transform.rotation.eulerAngles;
            
            prefab.SetActive(true);
        }
        else if (trackedImage.trackingState == TrackingState.Limited) // image become invisble, set contFrame value to continue doing motion for several frames
        {
            isTracking[handNum] = false;
            // calculate velocity based on the next image initial degree and current rotation degree
            if (validTracking[handNum]) {
                // get average velocity
                avgPVelocity[handNum] = previousPVelocities[handNum*averageWindowSize];
                avgRVelocity[handNum] = previousRVelocities[handNum*averageWindowSize];
                for (int i = 1; i < averageWindowSize; i++)
                {
                    avgPVelocity[handNum] += previousPVelocities[handNum*averageWindowSize+i];
                    avgRVelocity[handNum] += previousRVelocities[handNum*averageWindowSize+i];
                }
                
                float val = (float)1.0/averageWindowSize;
                avgPVelocity[handNum] = Vector3.Scale(avgPVelocity[handNum], new Vector3(val, val, val));
                // check to see the object rotate along with which axis
                if (Mathf.Abs(avgPVelocity[handNum].x) > Mathf.Abs(avgPVelocity[handNum].z)){
                    // along x axis
                    avgRVelocity[handNum] = Vector3.Scale(avgRVelocity[handNum], new Vector3(val, 0f, 0f));
                    // avgRVelocity[handNum].x = avgRVelocity[handNum].x % 5.0f;
                } else {
                    // along z axis
                    avgRVelocity[handNum] = Vector3.Scale(avgRVelocity[handNum], new Vector3(0f, 0f, val));
                    // avgRVelocity[handNum].z = avgRVelocity[handNum].z % 5.0f;
                }
                contFrames[handNum] = continueFrames;
            } else {
                // no velocity
                avgPVelocity[handNum] = Vector3.zero;
                avgRVelocity[handNum] = Vector3.zero;
                contFrames[handNum] = counter[handNum];
            }
            // clear validtracking flag
            validTracking[handNum] = false;
            counter[handNum] = 0;
        }
    }

    // check each contFrame; if > 0, render the rotations for several more frames
    void Update()
    {
        for (int i = 0; i < numHands; i++)
        {   
            if (contFrames[i] > 0)
            {
                GameObject prefab = spawnedPrefabs[(i * numOfImagesEachHand).ToString()];
                // if last frame, disable prefab objects
                if (contFrames[i] == 1)
                {
                    prefab.SetActive(false);
                }
                else
                {
                    // use the velocity to render objects
                    // prefab.transform.position += avgPVelocity[i];
                    Quaternion currRotation = prefab.transform.rotation;
                    currRotation.eulerAngles += avgRVelocity[i];
                    prefab.transform.rotation = currRotation;
                    prefab.SetActive(true);
                }
                contFrames[i]--;
            } else {
                // disable prefab
                if (isTracking[i] == false) {
                    GameObject prefab = spawnedPrefabs[(i * numOfImagesEachHand).ToString()];
                    prefab.SetActive(false);
                }
            }
        }

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
        **/
        // For the controller, we will store the information of button status as a string
        // for left hand, we allow A,B,UP,DOWN 
        // for right hand, we allow X,Y,LEFT,RIGHT
        // we will not use OK button for now
        // status T(press)H(hold)F(release)

        SetInputButtons();
    }

    void SetInputButtons()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            // press button A
            if (IsLeftActive())
            {
                leftButtonSwitch[0] = true;
                leftButtonInfo[0] = "T";
            } 
        }
        if (Input.GetKey(KeyCode.L) && leftButtonSwitch[0])
        {
            // hold button A
            if (IsLeftActive())
            {
                leftButtonInfo[0] = "H";
            }
        }
        if (Input.GetKeyDown(KeyCode.V) || Input.GetKeyUp(KeyCode.L))
        {
            // release button A
            if (IsLeftActive())
            {
                leftButtonSwitch[0] = false;
                leftButtonInfo[0] = "F";
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            // press button B
            if (IsLeftActive())
            {
                leftButtonSwitch[1] = true;
                leftButtonInfo[1] = "T";
            }
        }
        if (Input.GetKey(KeyCode.K) && leftButtonSwitch[1])
        {
            // hold button B
            if (IsLeftActive())
            {
                leftButtonInfo[1] = "H";
            }
        }
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyUp(KeyCode.K))
        {
            // release button B
            if (IsLeftActive())
            {
                leftButtonSwitch[1] = false;
                leftButtonInfo[1] = "F";
            }
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            // press button X
            if (IsRightActive())
            {
                rightButtonSwitch[0] = true;
                rightButtonInfo[0] = "T";
            }
        }
        if (Input.GetKey(KeyCode.Y) && rightButtonSwitch[0])
        {
            // hold button X
            if (IsRightActive())
            {
                rightButtonInfo[0] = "H";
            }
        }
        if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyUp(KeyCode.Y))
        {
            // release button X
            if (IsRightActive())
            {
                rightButtonSwitch[0] = false;
                rightButtonInfo[0] = "F";
            }
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            // press button Y
            if (IsRightActive())
            {
                rightButtonSwitch[1] = true;
                rightButtonInfo[1] = "T";
            }
        }
        if (Input.GetKey(KeyCode.U) && rightButtonSwitch[1])
        {
            // hold button Y
            if (IsRightActive())
            {
                rightButtonInfo[1] = "H";
            }
        }
        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyUp(KeyCode.U))
        {
            // release button Y
            if (IsRightActive())
            {
                rightButtonSwitch[1] = false;
                rightButtonInfo[1] = "F";
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            // press button UP
            if (IsLeftActive())
            {
                leftButtonSwitch[2] = true;
                leftButtonInfo[2] = "T";
            }
        }
        if (Input.GetKey(KeyCode.W) && leftButtonSwitch[2])
        {
            // hold button UP
            if (IsLeftActive())
            {
                leftButtonInfo[2] = "H";
            }
        }
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyUp(KeyCode.W))
        {
            // release button UP
            if (IsLeftActive())
            {
                leftButtonSwitch[2] = false;
                leftButtonInfo[2] = "F";
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            // press button DOWN
            if (IsLeftActive())
            {
                leftButtonSwitch[3] = true;
                leftButtonInfo[3] = "T";
            }
        }
        if (Input.GetKey(KeyCode.X) && leftButtonSwitch[3])
        {
            // hold button DOWN
            if (IsLeftActive())
            {
                leftButtonInfo[3] = "H";
            }
        }
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyUp(KeyCode.X))
        {
            // release button DOWN
            if (IsLeftActive())
            {
                leftButtonSwitch[3] = false;
                leftButtonInfo[3] = "F";
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            // press button LEFT
            if (IsRightActive())
            {
                rightButtonSwitch[2] = true;
                rightButtonInfo[2] = "T";
            }
        }
        if (Input.GetKey(KeyCode.A) && rightButtonSwitch[2])
        {
            // hold button LEFT
            if (IsRightActive())
            {
                rightButtonInfo[2] = "H";
            }
        }
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyUp(KeyCode.A))
        {
            // release button LEFT
            if (IsRightActive())
            {
                rightButtonSwitch[2] = false;
                rightButtonInfo[2] = "F";
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            // press button RIGHT
            if (IsRightActive())
            {
                rightButtonSwitch[3] = true;
                rightButtonInfo[3] = "T";
            }
        }
        if (Input.GetKey(KeyCode.D) && rightButtonSwitch[3])
        {
            // hold button RIGHT
            if (IsRightActive())
            {
                rightButtonInfo[3] = "H";
            }
        }
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyUp(KeyCode.D))
        {
            // release button RIGHT
            if (IsRightActive())
            {
                rightButtonSwitch[3] = false;
                rightButtonInfo[3] = "F";
            }
        }

        // if any prefab is not visble, release all button status
        if (!IsLeftActive())
        {
            for (int i = 0; i < numButtons; i++)
            {
                leftButtonSwitch[i] = false;
                leftButtonInfo[i] = "F";
            }
        }
        if (!IsRightActive())
        {
            for (int i = 0; i < numButtons; i++)
            {
                rightButtonSwitch[i] = false;
                rightButtonInfo[i] = "F";
            }
        }
    }

    // APIs to return info to other scripts
    // get button status on left hand
    public string GetButtonStatesLeft()
    {
        string buttonStates = leftButtonInfo[0];
        for (int i = 1; i < numButtons; i++)
        {
            buttonStates += leftButtonInfo[i];
        }
        return buttonStates;
    }

    // get button status on right hand
    public string GetButtonStatesRight()
    {
        string buttonStates = rightButtonInfo[0];
        for (int i = 1; i < numButtons; i++)
        {
            buttonStates += rightButtonInfo[i];
        }
        return buttonStates;
    }

    // return left prefab active status
    public bool IsLeftActive()
    {
        return spawnedPrefabs["0"].activeSelf;
    }

    // return right prefab active status
    public bool IsRightActive()
    {
        return spawnedPrefabs[numOfImagesEachHand.ToString()].activeSelf;
    }

    // return left prefab position
    public Vector3 GetPositionLeft()
    {
        return spawnedPrefabs["0"].transform.position;
    }

    // return right prefab position
    public Vector3 GetPositionRight()
    {
        return spawnedPrefabs[numOfImagesEachHand.ToString()].transform.position;
    }

    // return left prefab rotation in eulerAngles
    public Vector3 GetRotationLeft()
    {
        return spawnedPrefabs["0"].transform.rotation.eulerAngles;
    }

    // return right prefab rotation in eulerAngles
    public Vector3 GetRotationRight()
    {
        return spawnedPrefabs[numOfImagesEachHand.ToString()].transform.rotation.eulerAngles;
    }

    // return left hand position
    public Vector3 GetPositionLeftHand()
    {
        return spawnedPrefabs["0"].transform.GetChild(0).position;
    }

    // return right hand position
    public Vector3 GetPositionRightHand()
    {
        return spawnedPrefabs[numOfImagesEachHand.ToString()].transform.GetChild(0).position;
    }

    // return left hand rotation in eulerAngles
    public Vector3 GetRotationLeftHand()
    {
        return spawnedPrefabs["0"].transform.GetChild(0).rotation.eulerAngles;
    }

    // return right hand rotation in eulerAngles
    public Vector3 GetRotationRightHand()
    {
        return spawnedPrefabs[numOfImagesEachHand.ToString()].transform.GetChild(0).rotation.eulerAngles;
    }

    // get left hand color
    public Color GetColorLeftHand()
    {
        return spawnedPrefabs["0"].transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color;
    }

    // get right hand color
    public Color GetColorRightHand()
    {
        return spawnedPrefabs[numOfImagesEachHand.ToString()].transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color;
    }

    // get left hand scale
    public Vector3 GetScaleLeftHand()
    {
        return spawnedPrefabs["0"].transform.GetChild(0).localScale;
    }

    // get right hand scale
    public Vector3 GetScaleRightHand()
    {
        return spawnedPrefabs[numOfImagesEachHand.ToString()].transform.GetChild(0).localScale;
    }

    // set left hand color
    public void SetColorLeftHand(Color color)
    {
        spawnedPrefabs["0"].transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = color;
    }

    // set right hand color
    public void SetColorRightHand(Color color)
    {
        spawnedPrefabs[numOfImagesEachHand.ToString()].transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = color;
    }

    // set left hand scale
    public void SetScaleLeftHand(Vector3 scale)
    {
        spawnedPrefabs["0"].transform.GetChild(0).localScale = scale;
    }

    // set right hand scale
    public void SetScaleRightHand(Vector3 scale)
    {
        spawnedPrefabs[numOfImagesEachHand.ToString()].transform.GetChild(0).localScale = scale;
    }
}
