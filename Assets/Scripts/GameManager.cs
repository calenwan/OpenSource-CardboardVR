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

    // for line renderer
    public GameObject linePrefab;

    private GameObject[] currentLine;
    private List<GameObject>[] lines;
    private List<Vector3>[] drawPositions;
    
    public bool leftHoldKey;
    private bool rightHoldKey;
    
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

        // init line renderer
        currentLine = new GameObject[numHands];
        lines = new List<GameObject>[numHands];
        drawPositions = new List<Vector3>[numHands];

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

            lines[i] = new List<GameObject>();
            drawPositions[i] = new List<Vector3>();
        }
        
        leftHoldKey = false;
        rightHoldKey = false;
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
            } else if (imgNumOnHand == 2) {
                // 6th image
                offset = new Vector3(0f, (float)cubeWidth/100, (float)cubeHeight/200);
            }
            
            /**
            float handPosition = (float)cubeWidth/100;
            if (imgNumOnHand == 1 || imgNumOnHand == 3) {
                // if 2nd and 4th images, use cubeLength
                handPosition = (float)cubeLength/100;
            } else if (imgNumOnHand == 4 || imgNumOnHand == 5) {
                // if 5th and 6th images, use cubeHeight
                handPosition = (float)cubeHeight/100;
            }
            **/
            
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
    // and also check input buttons; if input signal, draw lines
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
        
        We use A and B for left controller, and X and Y for right controller in our demo.
        **/
        
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

        // left controller buttons
        if (Input.GetKeyDown(KeyCode.L))
        {
            // press button A, draw lines with left controller
            GameObject prefab = spawnedPrefabs["0"];
            if (prefab.activeSelf)
            {
                leftHoldKey = true;
                CreateLine(prefab, 0);
            }
        }
        if (Input.GetKey(KeyCode.L))
        {
            // hold button A, continue drawing with left controller
            GameObject prefab = spawnedPrefabs["0"];
            if (prefab.activeSelf && leftHoldKey)
            { 
                UpdateLine(prefab, 0);
            }
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            // release button A. This will happen earlier than GetKeyUp(KeyCode.L)
            leftHoldKey = false;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            // button B, remove last line drew by left controller
            RemoveLine(0);
        }

        // right controller buttons
        if (Input.GetKeyDown(KeyCode.Y))
        {
            // button X, draw lines with right controller
            GameObject prefab = spawnedPrefabs[numOfImagesEachHand.ToString()];
            if (prefab.activeSelf)
            {
                rightHoldKey = true;
                CreateLine(prefab, 1);
            }
        }
        if (Input.GetKey(KeyCode.Y))
        {
            // holding button X, continue drawing with right controller
            GameObject prefab = spawnedPrefabs[numOfImagesEachHand.ToString()];
            if (prefab.activeSelf && rightHoldKey)
            {
                UpdateLine(prefab, 1);
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            // release button X. This will happen earlier than GetKeyUp(KeyCode.Y)
            rightHoldKey = false;
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            // button Y, remove last line drew by right controller
            RemoveLine(1);
        }
    }


    void CreateLine(GameObject prefab, int num)
    { 
        currentLine[num] = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lines[num].Add(currentLine[num]);
        LineRenderer lineRenderer = currentLine[num].GetComponent<LineRenderer>();
        lineRenderer.material.color = Color.blue;
        if (num == 1)
        {
            lineRenderer.material.color = Color.red;
        }
        drawPositions[num].Clear();
        drawPositions[num].Add(prefab.transform.GetChild(0).position);
        drawPositions[num].Add(prefab.transform.GetChild(0).position);
        lineRenderer.SetPosition(0, drawPositions[num][0]);
        lineRenderer.SetPosition(1, drawPositions[num][1]);
    }

    void UpdateLine(GameObject prefab, int num)
    {
        Vector3 newPosition = prefab.transform.GetChild(0).position;
        drawPositions[num].Add(newPosition);
        LineRenderer lineRenderer = currentLine[num].GetComponent<LineRenderer>();
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount-1, newPosition);
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
