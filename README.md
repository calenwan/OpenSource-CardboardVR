# Opensource Cardboard VR system for iPhone devices

This is an opensource cardboard VR system project in Unity, implemented from [mobfish Cardboard Unity SDK](https://github.com/mobfishgmbh/Cardboard-VR-Unity-SDK). It is built on Unity 2019.4.9f1, with the ARFoudnation 2.1.10, AR Subsystems 2.1.3, ARCore XR Plugin 2.1.12 and ARKit XR Plugin 2.1.10. This project works on iPhone devices which support of AR technology only.

# Preparation

In order to use our system, you need to have a compatible iPhone device (iPhone 6s or later), a development platform (MacBook with XCode), two Universal Wireless VR Remote & Gamepad (like [Fortune Tech](https://www.amazon.com/gp/product/B06XKPTMWZ/ref=ppx_yo_dt_b_asin_title_o01_s01?ie=UTF8&psc=1) or [Alupper](https://www.amazon.com/gp/product/B08FSRRNF9/ref=ppx_yo_dt_b_asin_title_o01_s00?ie=UTF8&psc=1)), and a Cardboard Viewer(like [Google Cardboard Viewer](https://www.amazon.com/Google-87002823-01-Official-Cardboard-Brown/dp/B01MQ5J5J4/ref=sxts_sxwds-bia-wc-nc-drs1_0?cv_ct_cx=google+cardboard&dchild=1&keywords=google+cardboard&pd_rd_i=B01MQ5J5J4&pd_rd_r=3c496796-07bc-4407-8ccd-48c62b42f4a9&pd_rd_w=3InPs&pd_rd_wg=nwW5X&pf_rd_p=84ce0865-d9ca-42e3-87ed-168be8f93162&pf_rd_r=NVXC8GF2PH89CGWZQVCE&psc=1&qid=1607305552&sr=1-1-88388c6d-14b8-4f70-90f6-05ac39e80cc0)). 

The Cardboard Viewer can be any brand, but it should at least work with your iPhone's screen size and contain a QR code viewer profile at the bottom.

<p align="center"><img src="docs/qr.jpg" width="300"></p>

Moreover, you need to make sure to cut a small hole on the front of your viewer to avoid occulding the camera at the back of our iPhone.

<p align="center"><img src="docs/cut.jpg" width="300"></p>

The VR controller, however, can be any brand but need to have the **exact same type** (shape and buttons) like this.

<p align="center"><img src="docs/controller.jpg" width="300"></p>

Since different type of controller has different button mapping to the mobile devices, our system need this exact type to work without changing the button mapping in our source codes.

In addition, you need to make two hand made cubes and attach the bluetooth controllers perpendicular to one face near the bottom.

<p align="center"><img src="docs/cube.jpg" width="300"></p>

The recommend dimensions of such cube is 10cm/10cm/20cm for length/width/height. The controller is attached to one of the face has length and height.

Next, you need to download our images under Assets/OpenSource_Cardboard_SDK/HandController/Tracked Images. Please print those 12 images with the dimension of 10cm*10cm, and attach them to the faces of both cubes at the same height. 

**The order of the images are important.** Image 0 should be attached to the face with the left controller, and then follow the **clockwise order** to attach Image 1, 2, 3 on the other 3 faces. Image 4 should be attached to the top face, while image 5 to the bottom face. Similarly, you need to attach image 6-11 on the corresponding faces of the right cube. **The rotation of those images are also essential.** For example, please make sure that you don't attach the images upside down.

Here is what your left cube supposed to be:

<img src="docs/l1.jpg" width="300"><img src="docs/l2.jpg" width="300"><img src="docs/l3.jpg" width="300">
<img src="docs/l4.jpg" width="300"><img src="docs/l5.jpg" width="300"><img src="docs/l6.jpg" width="300">

Here is what your right cube supposed to be:

<img src="docs/r1.jpg" width="300"><img src="docs/r2.jpg" width="300"><img src="docs/r3.jpg" width="300">
<img src="docs/r4.jpg" width="300"><img src="docs/r5.jpg" width="300"><img src="docs/r6.jpg" width="300">

Please make sure that both of your hand controllers are connected to your device successfully and the controllers are power on. You can put the switch on the side of the controllers to "KEY", and press any buttons such as A and B to see if the controllers can control your devices (increasing or reducing the volumn, for example). If success, please switch the side button to "GAME".

After you have make those preparations mentioned above, you can begin to try our system! In this repo, we provide all of our files under several folders such as Assets and Packages. You can download and copy all folders into your own Unity project. For convenience, we also provide two .unitypackage to import the necessary files into the project without copying the folders. 

Demo.unitypackage is for users who want to [try our painting demo in a cubes room](https://youtu.be/2WLhy9QTawE). 

SDK.unitypackage, however, is for other developers who want to design their own projects and deploy them onto the iPhone devices with VR effects and 6 DoF using our system. We provide multiple APIs for others design their own functionalities, instead of our paiting app, to control the app with two hand cube-attached controllers. 

We will provide the instructions on how to setup the project for both user groups below.

# Instructions

 1. Download and Install Unity Hub. Install Unity 2019.4.9f1. 
 
 2. Under "Installs" tab, click the three dots of this version, and open "Add Modules". Please make sure if you have at least installed the "iOS Build Support".
 
 <p align="center"><img src="docs/modules.jpg" width="800"></p>
 
 3. For users, press the "NEW" button under projects table, and create a new **3D** project with any name you like. For developers, open the project you want to use our system with this Unity version. Please make sure that your existing project can work successfully in this version before adopting our system.
 
 <p align="center"><img src="docs/newproject.jpg" width="800"></p>
 
 4. Navigate to **Window > Package Manager**, find and install ARFoundation (version 2.1.10), AR Subsystems (version 2.1.3, installed automatically with ARFoundation), ARCore XR Plugin (version 2.1.12, not necessary on iPhone devices, but useful to expand our system to Android devices in the future) and ARKit XR Plugin (version 2.1.10).
 
 <p align="center"><img src="docs/arfoundation.jpg" width="800"></p>
 
## Configuring iOS project settings

 Navigate to **File > Build Settings**, 
  Select **iOS** and choose **Switch Platform**.
  
### Player Settings
   1. change company name under "player" tab to some unique name you want.
   
   <p align="center"><img src="docs/company.jpg" width="800"></p>
   
   2. Navigate to **Project Settings > Player > Resolution and Presentation**. Set the **Default Orientation** to **Landscape Left**.
   
   <p align="center"><img src="docs/landscape.jpg" width="800"></p>
 
   3. Navigate to **Project Settings > Player > Other Settings**. In **Camera Usage Description**, write "Cardboard SDK requires camera permission to read the QR code (required to get the encoded device parameters).". In **Target minimum iOS Version**, write 12.0. Check **Requires ARKit Support**. In **Architecture**, choose ARM64.
   
   <p align="center"><img src="docs/other.jpg" width="800"></p>
   
   4. Navigate to **Project Settings > Player > XR Settings**. Make sure **Virtual Reality Supported"** is **not** checked.
   
   <p align="center"><img src="docs/xr.jpg" width="800"></p>

Next, import the unitypackage into the Unity project. The **following section** is for regular users **who wants to use our painting app**. For **developers who wants to use our system in their own projects**, please **skip the next section**.

# Instructions to install painting demo (regular users, not for developers)


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
  


