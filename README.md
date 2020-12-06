# OpenSource-CardboardVR
This is the repo for final project OpenSource CardboardVR using Unity and ARFoundation

Citation:

Use unofficial Google Cardboard SDK (mobfish GmbH) : https://github.com/mobfishgmbh/Cardboard-VR-Unity-SDK/releases/tag/1.0

Tracked Images: We use 12 images for our two-hand image tracking cubes. These images are located inside Assets/Tracked Images. You can use your own images, but please make sure they have enough features to track.


install AR Foundation    select window -> package manager -> AR Foundation 
install ARCore and ARkit XR plugin

open file and click build settings select Ios and switch platform.  
click player setting-> select player-> provide company name  
In resolution and presentation tab, select default orientation as Landscape left.  
Also, in other settings, give any string to camera usage description. Also change target minimum Ios version to 12.0. Check requires ARkit support. Change Architecture to ARM 64. 
In XR setting tab, uncheck virtual reality supported.
Import unity package.
In assets-> opensource_cardboard_sdk -> cardboard ->prefabs-> drag AR session, CardboardCamera, CardboardUIOverlay, EventSystem into your scene hierachy.

# Important parameters:
In CardboardCamera prefab -> game manager(component) -> cube length/width/height are the values of the length/width/height of handmade cube in cm.
In CardboardCamera prefab -> game manager(component) -> continue frames is number of frames after the cube image disappear.

# API methods:
 
    // get button status on left hand
    public string GetButtonStatesLeft()
    

    // get button status on right hand
    public string GetButtonStatesRight()
    
    // return left prefab active status
    public bool IsLeftActive()
   

    // return right prefab active status
    public bool IsRightActive()
   

    // return left prefab position
    public Vector3 GetPositionLeft()
    
    // return right prefab position
    public Vector3 GetPositionRight()
   

    // return left prefab rotation in eulerAngles
    public Vector3 GetRotationLeft()
   

    // return right prefab rotation in eulerAngles
    public Vector3 GetRotationRight()
    

    // return left hand position
    public Vector3 GetPositionLeftHand()
    

    // return right hand position
    public Vector3 GetPositionRightHand()
   

    // return left hand rotation in eulerAngles
    public Vector3 GetRotationLeftHand()
   

    // return right hand rotation in eulerAngles
    public Vector3 GetRotationRightHand()
    

    // get left hand color
    public Color GetColorLeftHand()
    

    // get right hand color
    public Color GetColorRightHand()
   

    // get left hand scale
    public Vector3 GetScaleLeftHand()
    

    // get right hand scale
    public Vector3 GetScaleRightHand()
    

    // set left hand color
    public void SetColorLeftHand(Color color)
   
    // set right hand color
    public void SetColorRightHand(Color color)
    

    // set left hand scale
    public void SetScaleLeftHand(Vector3 scale)
    

    // set right hand scale
    public void SetScaleRightHand(Vector3 scale)
  


