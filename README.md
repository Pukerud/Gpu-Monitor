# Gpu-Monitor
![image](https://user-images.githubusercontent.com/35293441/213880927-3b6ae267-28f1-4ba4-b079-83fbcb4eefa0.png)
![image](https://user-images.githubusercontent.com/35293441/213870025-bcdd3836-7439-47a1-b9e9-f9efdf3c83ee.png)

My first project is a GPU-Monitor.

REQUIREMENTS:
.Net Desktop Runtime - Will ask for this when you start the Program.
It uses nvidia-smi so it is only for NVIDIA cards and Drivers must have been installed.

Settings is stored in the registry
Computer\HKEY_CURRENT_USER\Software\GPUMonitor

Start Check - Starts the GPU check. 
Uses the settings you provide in the Settings Dialog.


Stop Check - What do you think? its Stops the check..
Test Mail - Test mail sending with the settings you provide in the Settings dialog.


Rndr Log - Not Implemented yet 
Going to create a Google Spreadsheet, with a Sheet that has the computer name where it is running.
Append data to it from the rndr_log based on settings. Then you can create graphs or whatever you want in another sheet base on that data.
![image](https://user-images.githubusercontent.com/35293441/213871142-44414a3d-c821-436e-a00a-ee799e46bbf3.png)



