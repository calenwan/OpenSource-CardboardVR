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

