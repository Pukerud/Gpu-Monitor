# Gpu-Monitor
![image](https://user-images.githubusercontent.com/35293441/213880927-3b6ae267-28f1-4ba4-b079-83fbcb4eefa0.png)
![image](https://user-images.githubusercontent.com/35293441/213870025-bcdd3836-7439-47a1-b9e9-f9efdf3c83ee.png)

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

Not Implemented yet- Creates a Google Spreadsheet (Rndr-Stats), with a Sheet name that use current computername.
Append data to it from the rndr_log based on settings. Then you can create graphs or whatever you want in another sheet base on that data.
A lot to do here you need to provide your own crednetials file so it can talk to YOUR google cloud. Easy instruction will be made when done.

![image](https://user-images.githubusercontent.com/35293441/213871142-44414a3d-c821-436e-a00a-ee799e46bbf3.png)


PS. I am a noob , do not look at the code to hard or your brain will explode...