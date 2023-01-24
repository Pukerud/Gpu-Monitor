# Gpu-Monitor
![image](https://user-images.githubusercontent.com/35293441/214382220-1883577c-f717-4c53-a33f-889619828749.png)
![image](https://user-images.githubusercontent.com/35293441/213907017-2e80262b-d385-41a9-9bf4-5f4398326cb9.png)


REQUIREMENTS:
.Net Desktop Runtime - Will ask for this when you start the Program.
It uses nvidia-smi so it is only for NVIDIA cards and Drivers must be pre installed.

Settings is stored in the registry
Computer\HKEY_CURRENT_USER\Software\GPUMonitor

Start Check  
Starts the GPU check. Uses the settings you provide in the Settings Dialog. 
This checks number of GPU's. 
If there is 3 or less GPU with high GPU load and the rest are lowload (80% or less). It triggers a Double-Check. If it is still in this state, it sends a mail.

Stop Check  
What do you think? its Stops the check..

Test Mail

Test mail sending with the settings you provide in the Settings dialog.

AutoStart

If you check this, Start Check will be triggered automatically when the Program starts.

Rndr Log

Not Implemented yet- Creates a Google Spreadsheet (Rndr-Stats), with a Sheet name that uses current computer name.
Append data to it from the rndr_log based on settings. Then you can create graphs or whatever you want in another sheet base on that data.
A lot to do here you need to provide your own credentials file so it can talk to YOUR google cloud. Easy instruction will be made when done.

![image](https://user-images.githubusercontent.com/35293441/213871142-44414a3d-c821-436e-a00a-ee799e46bbf3.png)


PS. I am a noob , do not look at the code to hard or your brain will explode...
