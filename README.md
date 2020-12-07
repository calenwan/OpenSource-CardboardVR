# Opensource Cardboard VR system for iPhone devices

This is an opensource cardboard VR system project in Unity, implemented from [mobfish Cardboard Unity SDK](https://github.com/mobfishgmbh/Cardboard-VR-Unity-SDK). It is built on Unity 2019.4.9f1, with the ARFoudnation 2.1.10, AR Subsystems 2.1.3, ARCore XR Plugin 2.1.12 and ARKit XR Plugin 2.1.10. This project works on iPhone devices which support of AR technology only.

# Preparation

In order to use our system, you need to have a compatible iPhone device (iPhone 6s or later), a development platform (MacBook with XCode), two Universal Wireless VR Remote & Gamepad (like [Fortune Tech](https://www.amazon.com/gp/product/B06XKPTMWZ/ref=ppx_yo_dt_b_asin_title_o01_s01?ie=UTF8&psc=1) or [Alupper](https://www.amazon.com/gp/product/B08FSRRNF9/ref=ppx_yo_dt_b_asin_title_o01_s00?ie=UTF8&psc=1)), and a Cardboard Viewer(like [Google Cardboard Viewer](https://www.amazon.com/Google-87002823-01-Official-Cardboard-Brown/dp/B01MQ5J5J4/ref=sxts_sxwds-bia-wc-nc-drs1_0?cv_ct_cx=google+cardboard&dchild=1&keywords=google+cardboard&pd_rd_i=B01MQ5J5J4&pd_rd_r=3c496796-07bc-4407-8ccd-48c62b42f4a9&pd_rd_w=3InPs&pd_rd_wg=nwW5X&pf_rd_p=84ce0865-d9ca-42e3-87ed-168be8f93162&pf_rd_r=NVXC8GF2PH89CGWZQVCE&psc=1&qid=1607305552&sr=1-1-88388c6d-14b8-4f70-90f6-05ac39e80cc0)). 

The Cardboard Viewer can be any brand, but it should at least work with your iPhone's screen size and contain a QR code viewer profile at the bottom.<img src="docs/qr.jpg" width="150"> 


Moreover, you need to make sure to cut a small hole on the front of your viewer to avoid occulde the camera at the back of our iPhone.<img src="docs/cut.jpg" width="150">

The VR controller, however, can be any brand but need to have the exact same type (shape and buttons) like this.<img src="docs/controller.jpg" width="150">

Since different type of controller has different button mapping to the mobile devices, our system need this exact type to work without changing the button mapping in our source codes.


In addition, you need to make two hand made cubes and attach the bluetooth controllers perpendicular to one face near the bottom.<img src="docs/cube.jpg" width="150">

The recommend dimensions of such cube is 10cm/10cm/20cm for length/width/height. The controller is attached to one of the face has length and height.

Next, you need to download our images under Assets/OpenSource_Cardboard_SDK/HandController/Tracked Images. Please print those 12 images with the dimension of 10cm*10cm, and attach them to the faces of both cubes at the same height. 

The order of the images are important. Image 0 should be attached to the face with the left controller, and then follow the clockwise order to attach Image 1, 2, 3 on the other 3 faces. Image 4 should be attached to the top face, while image 5 to the bottom face. Similarly, you need to attach image 6-11 on the faces of the right cube. The rotation of those images are also essential. For example, please make sure that you don't attach the images upside down.

Here is what your left cube supposed to be:
<img src="docs/l1.jpg" width="100"><img src="docs/l2.jpg" width="100"><img src="docs/l3.jpg" width="100"><img src="docs/l4.jpg" width="100"><img src="docs/l5.jpg" width="100"><img src="docs/l6.jpg" width="100">

And Here is what your right cube supposed to be:
<img src="docs/r1.jpg" width="100"><img src="docs/r2.jpg" width="100"><img src="docs/r3.jpg" width="100"><img src="docs/r4.jpg" width="100"><img src="docs/r5.jpg" width="100"><img src="docs/r6.jpg" width="100">

Tracked Images: We use 12 images for our two-hand image tracking cubes. These images are located inside Assets/Tracked Images. You can use your own images, but please make sure they have enough features to track.

# Initial set up
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

Also remembered to press QR code button first, then scan your cardboard viewer to setup the stereo camera for your own viewer before using.


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
  


