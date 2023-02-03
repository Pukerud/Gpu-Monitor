# Gpu-Monitor
![image](https://user-images.githubusercontent.com/35293441/216568410-b67eea87-6a5b-4717-974c-1bf205e0fac9.png)
![image](https://user-images.githubusercontent.com/35293441/216576629-9c4b8110-684e-4cde-b255-9ba97efe952c.png)



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

Rndr Log  
Opens the Rndr_log file.

Rndr Wiki  
Opens the Wiki

Kill RndrClient  
Kills the Render Client 

Gsheet
![image](https://user-images.githubusercontent.com/35293441/216578403-4ecb6f06-8c31-4075-ba4f-57b9075a1868.png)
Creates a Google spreadsheet and adds or updates a sheet with current computername. Also adds default formulas and headers.
You need a credential.json file in the GPU-Monitor.exe folder.  
  Not done with a guide for this yet, google how to setup a service account and get a credential.json :)  

![image](https://user-images.githubusercontent.com/35293441/216570766-f50104b9-4c9a-43e1-b38f-5e1f238004fb.png)  
Auto Gsheet  
Auto uploads to the sheet based on the Gsheet Timer you set (minutes)

AutoCheck  
If you check this, Start Check will be triggered automatically when the Program starts.


PS. I am a noob , do not look at the code to hard or your brain will explode...
