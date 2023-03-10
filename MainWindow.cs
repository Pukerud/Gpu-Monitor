using System;
using System.Collections.Generic;
using System.Resources;
//using NAudio.Wave;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using Microsoft.Win32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using Simple_Button.Properties;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.GData.Spreadsheets;
using Google.Apis.Auth.OAuth2;
using Microsoft.VisualBasic.ApplicationServices;
using Google;
using Google.Apis;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using GData.Spreadsheets;
using Spreadsheet = Google.Apis.Sheets.v4.Data.Spreadsheet;
using System.Text.RegularExpressions;
using System.Globalization;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Windows;
using Newtonsoft.Json;
using System.Text;

//using Google.GoogleApiException;

namespace Simple_Button
{
    public partial class MainWindow : Form
    {
        private bool _settingsValid = true;
        private bool _isRunning;
        private string? _path;
        private int _highLoadGpuCount;
        private int _lowLoadGpuCount;
        private int _gpuCount;
        private DateTime _lastSent;
        private bool _emailSent;
        private string _computer = Environment.MachineName;
        public string? _smtpServer { get; set; }
        private int _emailSmtpServerPort { get; set; }
        private string? _smtpUser { get; set; }
        private string? _smtpPassword { get; set; }
        private string? _mailTo { get; set; }
        private string? _mailFrom { get; set; }
        private string? _Body;
        private string? _Subject;
        private int _timeThreshold = 10;
        private int _sleepTime = 9000;
        private int _sleepTime2 = 60000;
        private string? GsheetShare { get; set; }
        private System.Threading.Timer timer;
        private bool isTimerCreated = false;
        private int GsheetTimer = 10;
        //private double Left = 0;
        //private double Top = 0;
        //private const string LeftKey = "Left";
        //private const string TopKey = "Top";

        public MainWindow()
        {
            InitializeComponent();
            // Check if the saved window position is valid
            //if (Properties.Settings.Default.WindowLocation != null)
            //{
            //    // Set the window location to the saved position
            //    this.Left = Properties.Settings.Default.WindowLocation.X;
            //    this.Top = Properties.Settings.Default.WindowLocation.Y;
            //}
            //else
            //{
            //    // Set the window location to the default position (centered of the screen)
            //    this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //}
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Pukerud's GPU Monitor Running On: " + Environment.MachineName; // + " (Build Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ")";
            Console.WriteLine("starthere");
            //if (DateTime.Now.Day == 20 && DateTime.Now.Month == 1)
            //{
            //    using (var audioFile = new AudioFileReader(Simple_Button.Properties.SoundFile))
            //    using (var outputDevice = new WaveOutEvent())
            //    {
            //        outputDevice.Init(audioFile);
            //        outputDevice.Play();
            //    }

            //}
            checkBox1.Checked = Properties.Settings.Default.Autostart;
            checkBox2.Checked = Properties.Settings.Default.AutoSheet;
            _lastSent = DateTime.Now.AddMinutes(-_timeThreshold);
            string startingFolder = @"C:\Windows\System32\DriverStore\FileRepository\";
            string fileName = "nvidia-smi.exe";
            string? path = null;
            _emailSent = false;

            Queue<string> folders = new Queue<string>();
            folders.Enqueue(startingFolder);

            while (folders.Count > 0)
            {
                string currentFolder = folders.Dequeue();
                string[] files = Directory.GetFiles(currentFolder, fileName);

                if (files.Length > 0)
                {
                    path = files[0];
                    break;
                }

                foreach (string subfolder in Directory.GetDirectories(currentFolder))
                {
                    folders.Enqueue(subfolder);
                }
            }

            if (path != null)
            {
                _path = path;
            }
            else
            {
                ErrorForm errorForm = new ErrorForm("nvidia-smi.exe not found");
                Application.Exit();
            }
            if (checkBox1.Checked) button1_Click(null, null);
            this.button1 = new System.Windows.Forms.Button();
            //if (checkBox2.Checked) autorunSheet(null, null);
            //this.button2 = new System.Windows.Forms.Button();

        }
        //private void Window_Closed(object sender, EventArgs e)
        //{
        //    // Save the window position when the window is closed
        //    Properties.Settings.Default.WindowLocation = new System.Drawing.Point((int)this.Left, (int)this.Top);
        //    Properties.Settings.Default.Save();
        //}
        //private void autorunSheet(object sender, EventArgs e)
        //{
        //    if (checkBox2.Checked)
        //    {
        //        timer = new System.Threading.Timer(TimerCallback, null, 0, 12 * 60 * 60 * 1000);
        //    }
        //    else
        //    {
        //        timer.Dispose();
        //    }
        //}

        private void TimerCallback(Object o)
        {
            if (checkBox2.Checked)                
            button7_Click(null, null);          
        }


        private async void button1_Click(object? sender, EventArgs? e)
        {
            //MessageBox.Show("GPULoad Check Started");            
            _isRunning = true;
            await Task.Run(() =>
            {
                while (_isRunning)
                {
                    RetrieveRegistryValues();
                    using (var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = _path,
                            Arguments = "--query-gpu=utilization.gpu --format=csv,noheader",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    })
                    {
                        process.Start();
                        var output = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();

                        var gpuUsage = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        Console.WriteLine(string.Join(",", gpuUsage));
                        _gpuCount = gpuUsage.Length;
                        var highLoadRegex = new Regex(@"^(8[0-9]|9[0-9]|100) %$");
                        _highLoadGpuCount = gpuUsage.Count(x => highLoadRegex.IsMatch(x));
                        var lowLoadRegex = new Regex(@"^[0-7][0-9] %$|^0 %$");
                        _lowLoadGpuCount = gpuUsage.Count(x => !highLoadRegex.IsMatch(x));
                        Console.WriteLine("highload= " + _highLoadGpuCount + " Lowload= " + _lowLoadGpuCount + "ok");
                    }
                    Console.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] GPU usage on " + _computer + ". Number of GPUs: " + _gpuCount + ", High load GPU count: " + _highLoadGpuCount + ", Low load GPU count: " + _lowLoadGpuCount);
                    textBox1.Invoke((MethodInvoker)delegate
                    {
                        textBox1.AppendText("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Number of GPUs: " + _gpuCount + ", High load GPU count: " + _highLoadGpuCount + ", Low load GPU count: " + _lowLoadGpuCount + Environment.NewLine);
                    });
                    if (_highLoadGpuCount <= 3 && _lowLoadGpuCount != _gpuCount)
                    {
                        Console.WriteLine("Need to Double-check GPU usage... waiting 60 seconds            ");
                        textBox1.Invoke((MethodInvoker)delegate
                        {
                            textBox1.AppendText("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Need to Double-check GPU usage... waiting " + _sleepTime2 + " seconds  \r\n");
                        });
                        System.Threading.Thread.Sleep(_sleepTime2 * 1000);
                        var currentTime = DateTime.Now;
                        var timeSinceLastEmail = currentTime - _lastSent;
                        if (timeSinceLastEmail.TotalMinutes >= _timeThreshold)
                        {
                            _emailSent = false;
                            textBox1.Invoke((MethodInvoker)delegate
                            {
                                textBox1.AppendText("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Sending Mail Enabled Again....\r\n");
                            });
                        }
                        Console.WriteLine("Double-checking GPU usage...");
                        textBox1.Invoke((MethodInvoker)delegate
                        {
                            textBox1.AppendText("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Double-checking GPU usage...\r\n");
                        });
                        using (var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = _path,
                                Arguments = "--query-gpu=utilization.gpu --format=csv,noheader",
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        })
                        {
                            process.Start();
                            var output = process.StandardOutput.ReadToEnd();
                            process.WaitForExit();

                            var gpuUsage = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            _gpuCount = gpuUsage.Length;
                            Console.WriteLine(string.Join(",", gpuUsage));
                            var highLoadRegex = new Regex(@"^(8[0-9]|9[0-9]|100) %$");
                            _highLoadGpuCount = gpuUsage.Count(x => highLoadRegex.IsMatch(x));
                            var lowLoadRegex = new Regex(@"^[0-7][0-9] %$|^0 %$");
                            _lowLoadGpuCount = gpuUsage.Count(x => !highLoadRegex.IsMatch(x));
                            Console.WriteLine("highload= " + _highLoadGpuCount + " Lowload= " + _lowLoadGpuCount + " ok");
                            //_highLoadGpuCount = gpuUsage.Count(x => x.Contains("100 %") || x.Contains("8[0-9] %") || x.Contains("9[0-9] %"));
                            //_lowLoadGpuCount = gpuUsage.Count(x => !x.Contains("100 %") && !x.Contains("8[0-9] %") && !x.Contains("9[0-9] %"));
                        }
                        Console.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] GPU usage on " + _computer + ". Number of GPUs: " + _gpuCount + ", High load GPU count: " + _highLoadGpuCount + ", Low load GPU count: " + _lowLoadGpuCount + Environment.NewLine);
                        textBox1.Invoke((MethodInvoker)delegate
                        {
                            textBox1.AppendText("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Number of GPUs: " + _gpuCount + ", High load GPU count: " + _highLoadGpuCount + ", Low load GPU count: " + _lowLoadGpuCount + Environment.NewLine);
                        });
                        if (_highLoadGpuCount <= 3 && _lowLoadGpuCount != _gpuCount && !_emailSent)
                        {
                            textBox1.Invoke((MethodInvoker)delegate
                            {
                                textBox1.AppendText("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Sending Mail \r\n");
                            });
                            _Subject = "Looks like thing are in a HUNG state at " + _computer;
                            _Body = "Looks like " + _highLoadGpuCount + " GPU(s) has 100% Load, the rest has 0% load ";
                            SendEmail(_mailTo, _Subject, _Body);
                            _lastSent = DateTime.Now;
                            _emailSent = true;
                        }
                        else
                        {
                            var timerSinceLastEmail = (DateTime.Now - _lastSent).TotalMinutes;
                            textBox1.Invoke((MethodInvoker)delegate
                            {
                                textBox1.AppendText("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] GPU's are fine again or Email is still timed out. " + timeSinceLastEmail.ToString(@"hh\:mm\:ss") + " since last Email \r\n");
                            });
                        }

                    }
                    System.Threading.Thread.Sleep(_sleepTime * 1000);
                }
            });

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            _isRunning = false;
        }
        private void SendEmail(string to, string subject, string body)
        {
            RetrieveRegistryValues();
            if (!_settingsValid)
            {
                return;
            }

            try
            {
                using (var client = new SmtpClient(_smtpServer, _emailSmtpServerPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                    client.Timeout = 5 * 1000;  // 5 seconds timeout
                    using (var message = new MailMessage(_mailFrom, to, subject, body))

                        try
                        {
                            client.Send(message);
                        }
                        catch (SmtpException ex)
                        {
                            // Handle the exception, for example by showing an error message
                            ErrorForm errorForm = new ErrorForm(ex.Message);
                            errorForm.ShowDialog();
                        }

                }
            }
            catch (SmtpException ex)
            {
                // Handle the exception, for example by displaying an error message
                ErrorForm errorForm = new ErrorForm(ex.Message);
                errorForm.ShowDialog();
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            RetrieveRegistryValues();
            ErrorForm errorForm = new ErrorForm("If the mail settings are wrong, this is going to be frozen for 5 Seconds");
            errorForm.ShowDialog();
            if (!_settingsValid)
            {
                return;
            }
            _Subject = "Looks like thing are in a HUNG state at " + _computer;
            _Body = "Looks like " + _highLoadGpuCount + " GPU(s) has 100% Load, the rest has 0% load ";
            SendEmail(_mailTo, _Subject, _Body);

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Autostart = checkBox1.Checked;
            Properties.Settings.Default.Save();
        }
        private void Settings_Click(object sender, EventArgs e)
        {

        }
        private void RetrieveRegistryValues()
        {
            _smtpServer = (string?)Registry.GetValue("HKEY_CURRENT_USER\\Software\\GPUMonitor", "SmtpServer", "");
            _emailSmtpServerPort = (int?)Registry.GetValue("HKEY_CURRENT_USER\\Software\\GPUMonitor", "SmtpServerPort", null) ?? 0;
            _smtpUser = (string?)Registry.GetValue("HKEY_CURRENT_USER\\Software\\GPUMonitor", "SmtpUser", "");
            _smtpPassword = (string?)Registry.GetValue("HKEY_CURRENT_USER\\Software\\GPUMonitor", "SmtpPassword", "");
            _mailTo = (string?)Registry.GetValue("HKEY_CURRENT_USER\\Software\\GPUMonitor", "MailTo", "");
            _mailFrom = (string?)Registry.GetValue("HKEY_CURRENT_USER\\Software\\GPUMonitor", "MailFrom", "");
            object timeThreshold = Registry.GetValue("HKEY_CURRENT_USER\\Software\\GPUMonitor", "EmailTimeout", null);
            _timeThreshold = (timeThreshold != null && timeThreshold is int) ? (int)timeThreshold : 0;
            _sleepTime = (int?)(Registry.GetValue("HKEY_CURRENT_USER\\Software\\GPUMonitor", "CheckTimer", null) as int?) ?? 0;
            object sleepTime2 = Registry.GetValue("HKEY_CURRENT_USER\\Software\\GPUMonitor", "DobbelCheck", null);
            _sleepTime2 = (sleepTime2 != null && sleepTime2 is int) ? (int)sleepTime2 : 0;
            GsheetShare = (string?)Registry.GetValue("HKEY_CURRENT_USER\\Software\\GPUMonitor", "GsheetShare", "");
            GsheetTimer = (int?)Registry.GetValue("HKEY_CURRENT_USER\\Software\\GPUMonitor", "GsheetTimer", null) ?? 0;
            if (_smtpServer == "" || _emailSmtpServerPort == 0 || _smtpUser == "" || _smtpPassword == "" || _mailTo == "" || _mailFrom == "" || _smtpServer == null || _smtpUser == null || _smtpPassword == null || _mailTo == null || _mailFrom == null)
            {
                _settingsValid = false;
                ErrorForm errorForm = new ErrorForm("Settings missing. Please open the settings form and save your settings");
                errorForm.ShowDialog();
                _isRunning = false;
                return;
            }
            _settingsValid = true;

        }


        private void Settings_Click_1(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            settingsForm.SmtpServer = _smtpServer;
            settingsForm.SmtpServerPort = _emailSmtpServerPort;
            settingsForm.SmtpUser = _smtpUser;
            settingsForm.SmtpPassword = _smtpPassword;
            settingsForm.MailTo = _mailTo;
            settingsForm.MailFrom = _mailFrom;
            settingsForm.ShowDialog();
        }

        private void RndrLog_Click(object sender, EventArgs e)
        {
            OpenRndrLog();

        }
        private void DeleteAllSpreadsheets(DriveService driveService)
        {
            var request = driveService.Files.List();
            request.Q = "mimeType='application/vnd.google-apps.spreadsheet'";
            var files = request.Execute();

            foreach (var file in files.Files)
            {
                driveService.Files.Delete(file.Id).Execute();
            }
        }
        private void DeleteSpreadsheetsOwnedByServiceAccount(DriveService driveService, string serviceAccountEmail)
        {
            var request = driveService.Files.List();
            request.Q = $"mimeType='application/vnd.google-apps.spreadsheet' and '{serviceAccountEmail}' in owners";
            var files = request.Execute();

            foreach (var file in files.Files)
            {
                driveService.Files.Delete(file.Id).Execute();
            }
        }

        private string GetSpreadsheetId(SheetsService service, string spreadsheetName)
        {
            try
            {
                // Path to the json credentials file
                //string credPath = "C:\\dev\\SimpleButton\\Simple Button\\bin\\Debug\\net7.0-windows\\credentials.json";
                var credPath = GetCredentialsPath();
                // Creat a new instance of the DriveService
                var driveService = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = GoogleCredential.FromFile(credPath).CreateScoped(DriveService.Scope.Drive),
                    ApplicationName = "GPUMonitor"
                });
                System.Diagnostics.Debug.WriteLine("spreadsheetName " + spreadsheetName);
                var request = driveService.Files.List();
                request.Q = "mimeType='application/vnd.google-apps.spreadsheet'";
                var files = request.Execute();
                System.Diagnostics.Debug.WriteLine("File List: " + string.Join(",", files.Files.Select(f => f.Name)));
                var targetFile = files.Files.FirstOrDefault(f => f.Name == spreadsheetName);
                if (targetFile != null)
                {
                    return targetFile.Id;
                }
                else
                {
                    return null;
                }
            }
            catch (Google.GoogleApiException e)
            {
                if (e.Error.Code == 404)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }
        private void CheckAndCreateSheet()
        {
            // Hard-coded spreadsheet name and sheet name
            string spreadsheetName = "Rndr-Stats";
            string sheetName = System.Environment.MachineName;

            // Create an instance of the SheetsService
            //string credPath = "C:\\dev\\SimpleButton\\Simple Button\\bin\\Debug\\net7.0-windows\\credentials.json";
            var credPath = GetCredentialsPath();
            var sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GoogleCredential.FromFile(credPath).CreateScoped(SheetsService.Scope.Spreadsheets),
                ApplicationName = "GPUMonitor"
            });

            // Check if the spreadsheet exists
            var spreadsheetId = GetSpreadsheetId(sheetsService, spreadsheetName);
            if (spreadsheetId == null)
            {
                // If the spreadsheet does not exist, create it
                spreadsheetId = CreateSpreadsheet(sheetsService, spreadsheetName);
                System.Diagnostics.Debug.WriteLine("CheckAndCreat: Created Spreadsheet" + spreadsheetId);
            }
            System.Diagnostics.Debug.WriteLine("CheckAndCreat: Spreadsheet exists " + spreadsheetId);

            // Check if the sheet with the current computer name exists
            var sheetId = GetSheetId(sheetsService, spreadsheetId, sheetName);
            if (sheetId == null)
            {
                // If the sheet does not exist, create it
                sheetId = CreateSheet(sheetsService, spreadsheetId, sheetName);
                System.Diagnostics.Debug.WriteLine("CheckAndCreat: Created sheet");
            }
            System.Diagnostics.Debug.WriteLine("CheckAndCreat: Sheet exists");
        }

        private string CreateSpreadsheet(SheetsService sheetsService, string spreadsheetName)
        {
            RetrieveRegistryValues();
            var spreadsheet = new Spreadsheet();
            spreadsheet.Properties = new SpreadsheetProperties();
            spreadsheet.Properties.Title = spreadsheetName;
            var request = sheetsService.Spreadsheets.Create(spreadsheet);
            var createdSpreadsheet = request.Execute();
            var spreadsheetId = createdSpreadsheet.SpreadsheetId;

            // Create a new instance of the DriveService
            //string credPath = "C:\\dev\\SimpleButton\\Simple Button\\bin\\Debug\\net7.0-windows\\credentials.json";
            var credPath = GetCredentialsPath();
            var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GoogleCredential.FromFile(credPath).CreateScoped(DriveService.Scope.Drive),
                ApplicationName = "GPUMonitor"
            });

            // Set permissions for the creator and an additional email address
            var emailAddress = GsheetShare;
            System.Diagnostics.Debug.WriteLine("CreateSpreadsheet emailshare: " + GsheetShare);
            var permission = new Permission
            {
                Type = "user",
                Role = "writer",
                EmailAddress = emailAddress
            };
            var request2 = driveService.Permissions.Create(permission, spreadsheetId);
            request2.SendNotificationEmail = true;
            request2.Execute();

            return spreadsheetId;
        }
        private static void CreateSheetIfNotExists(SheetsService sheetsService, string spreadsheetId)
        {
            var request = sheetsService.Spreadsheets.Get(spreadsheetId);
            var sheets = request.Execute().Sheets;
            var sheetName = "Stats";

            var sheetExists = sheets.Any(s => s.Properties.Title == sheetName);

            if (!sheetExists)
            {
                var addSheetRequest = new AddSheetRequest
                {
                    Properties = new SheetProperties
                    {
                        Title = sheetName
                    }
                };

                var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
                {
                    Requests = new List<Request>
            {
                new Request
                {
                    AddSheet = addSheetRequest
                }
            }
                };

                sheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, spreadsheetId).Execute();
            }
        }


        private string GetSheetId(SheetsService service, string spreadsheetId, string sheetName)
        {
            var request = service.Spreadsheets.Get(spreadsheetId);
            var spreadsheet = request.Execute();
            var sheetId = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == sheetName)?.Properties.SheetId.ToString();
            return sheetId;
        }

        private static string CreateSheet(SheetsService sheetsService, string spreadsheetId, string sheetName)
        {
            // Create a new sheet
            BatchUpdateSpreadsheetRequest requestBody = new BatchUpdateSpreadsheetRequest();
            requestBody.Requests = new List<Request>();
            requestBody.Requests.Add(new Request
            {
                AddSheet = new AddSheetRequest
                {
                    Properties = new SheetProperties
                    {
                        Title = sheetName
                    }
                }
            });

            // Execute the request
            var request = sheetsService.Spreadsheets.BatchUpdate(requestBody, spreadsheetId);
            var response = request.Execute();

            // Get the sheetId of the newly created sheet
            var newSheetId = response.Replies[0].AddSheet.Properties.SheetId;

            return newSheetId.ToString();
        }
        static void OpenRndrLog()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OtoyRndrNetwork", "rndr_log.txt");
            if (!System.IO.File.Exists(filePath))
            {
                Console.WriteLine("File does not exist.");
                return;
            }
            try
            {
                Process.Start("notepad.exe", filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public static void TDRDelay()
        {
            string registryPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\GraphicsDrivers";
            RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath, true);
            Debug.WriteLine("OpenSubKey returned: " + key);
            if (key == null)
            {
                key = Registry.LocalMachine.CreateSubKey(registryPath);
            }

            try
            {
                key.SetValue("TdrLevel", 3, RegistryValueKind.DWord);
                key.SetValue("TdrDelay", 60, RegistryValueKind.DWord);
                key.SetValue("TdrDdiDelay", 60, RegistryValueKind.DWord);
            }
            catch (Exception e)
            {
                Console.WriteLine("The registry key could not be created or modified:");
                Console.WriteLine(e.Message);
            }
            var processInfo = new ProcessStartInfo
            {
                Verb = "runas",
                FileName = System.Reflection.Assembly.GetExecutingAssembly().Location
            };
            try
            {
                var process = Process.Start(processInfo);
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("The process failed to start: {0}", ex.Message);
            }

        }



        static void OpenWIKI()
        {
            string url = "https://docs.google.com/document/d/1j3HdjX7V2ot4W95IofGP3q1hjeBxbRqcBcrSVmFzHb8/edit";
            System.Diagnostics.Process.Start("cmd", "/c start " + url);
        }        

        private void button4_Click(object sender, EventArgs e)
        {
            OpenWIKI();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //TDRDelay();
            var credPath = GetCredentialsPath();
            // Creat a new instance of the DriveService
            var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GoogleCredential.FromFile(credPath).CreateScoped(DriveService.Scope.Drive),
                ApplicationName = "GPUMonitor"
            });
            //DeleteAllSpreadsheets(driveService);
            DeleteSpreadsheetsOwnedByServiceAccount(driveService, "rndr-714@rndr-stats.iam.gserviceaccount.com");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "taskkill";
            startInfo.Arguments = "/IM TCPSVCS.exe /F";
            Process.Start(startInfo);

        }

        private void button7_Click(object sender, EventArgs e)
        {            
            CheckAndCreateSheet();
            textBox1.Invoke((MethodInvoker)delegate
            {
                textBox1.AppendText("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Checking Rndr_log.txt....\r\n");
            });
            string spreadsheetName = "Rndr-Stats";

            // sheetName
            string sheetName = System.Environment.MachineName;

            // Create an instance of the SheetsService            
            var credPath = GetCredentialsPath();
            var sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GoogleCredential.FromFile(credPath).CreateScoped(SheetsService.Scope.Spreadsheets),
                ApplicationName = "GPUMonitor"
            });

            // Get the spreadsheet ID
            var spreadsheetId = GetSpreadsheetId(sheetsService, spreadsheetName);

            // Create STATS SHEET
            CreateSheetIfNotExists(sheetsService, spreadsheetId);
            System.Diagnostics.Debug.WriteLine("CreatSheetIfNotExist done");
            // Add computer/sheetname to Stats page            
            bool computerNameExists = CheckComputerNameExists(sheetsService, spreadsheetId, sheetName, 5);
            System.Diagnostics.Debug.WriteLine("CheckComputerNameExists done");
            if (!computerNameExists)
            {
                // If the current computer name does not exist, add it to the first empty cell on row E, after row E4.
                AddComputerName(sheetsService, spreadsheetId, sheetName, 5);                
            }
            AddComputerScore(sheetsService, spreadsheetId, sheetName, 5);
            System.Diagnostics.Debug.WriteLine("Computername exist done");
            //Add Text to Sheet Stats
            AddValuesToCells(sheetsService, spreadsheetId);
            //Add Formulas to Sheet Stats
            AddFormulasToCells(sheetsService, spreadsheetId, sheetName , 5);
            // Call the lastrow of gheet          
            var lastRow = GetLastRowValue(sheetsService, spreadsheetId, sheetName);
            Tuple<List<string>, List<string>> extractedData;
            // Run fast or slow extraction base on how old lastRow is            
            if (lastRow == "2000-01-01 00:00:00")
            {
                extractedData = ExtractRenderTime(lastRow);
            }
            else
            {
                extractedData = ExtractRenderTimefast(lastRow);
            }
            //Add Time and RenderTimes to Sheet
            if (extractedData.Item1 != null && extractedData.Item1.Count > 0 && extractedData.Item2 != null && extractedData.Item2.Count > 0)
            {
                AddRenderTime(extractedData.Item1, extractedData.Item2, spreadsheetId, sheetName);
                textBox1.Invoke((MethodInvoker)delegate
                {
                    textBox1.AppendText("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Added RenderTime to " + sheetName + "\r\n");
                });
            }
        }
        private static bool CheckComputerNameExists(SheetsService sheetsService, string spreadsheetId, string computerName, int column)
        {
            try
            {
                var request = sheetsService.Spreadsheets.Values.Get(spreadsheetId, "Stats!E:E");
                //request.Timeout = TimeSpan.FromSeconds(5);
                ValueRange response = request.Execute();
                IList<IList<Object>> values = response.Values;
                if (values != null && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        if (row.Contains(computerName))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (TaskCanceledException ex)
            {
                // Handle the exception
                ErrorForm errorForm = new ErrorForm(ex.Message);
                errorForm.ShowDialog();
                Console.WriteLine("Request Timed Out: " + ex.Message);
                return false;
            }
        }       
        private static bool AddValuesToCells(SheetsService sheetsService, string spreadsheetId)
        {
            try
            {
                var request = sheetsService.Spreadsheets.Values.Get(spreadsheetId, "Stats!F3");
                ValueRange response = request.Execute();
                IList<IList<Object>> values = response.Values;

                if (values == null || values[0][0].ToString() == "")
                {
                    var valuesToInsert1 = new List<IList<Object>>
                    {
                      new List<Object> {"Today"}
                    };

                    var updateRequest1 = sheetsService.Spreadsheets.Values.Update(new ValueRange
                    {
                        MajorDimension = "ROWS",
                        Range = "Stats!F3",
                        Values = valuesToInsert1
                    }, spreadsheetId, "Stats!F3");
                    updateRequest1.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                    updateRequest1.Execute();

                    var valuesToInsert2 = new List<IList<Object>>
                    {
                      new List<Object> {"OB-Score", "ComputerName", "Seconds", "Minutes", "Hours", "Yesterday (h)", "Last Week(h)", "Last Render"}
                    };

                    var updateRequest2 = sheetsService.Spreadsheets.Values.Update(new ValueRange
                    {
                        MajorDimension = "ROWS",
                        Range = "Stats!D4:K4",
                        Values = valuesToInsert2
                    }, spreadsheetId, "Stats!D4:K4");
                    updateRequest2.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                    updateRequest2.Execute();

                }

                return true;
            }
            catch (TaskCanceledException ex)
            {
                ErrorForm errorForm = new ErrorForm(ex.Message);
                errorForm.ShowDialog();
                Console.WriteLine("Request Timed Out: " + ex.Message);
                return false;
            }
        }


        private static bool AddFormulasToCells(SheetsService sheetsService, string spreadsheetId, string computerName, int row)
        {
            try
            {
                var request = sheetsService.Spreadsheets.Values.Get(spreadsheetId, "Stats!E:E");
                ValueRange response = request.Execute();
                IList<IList<Object>> values = response.Values;
                System.Diagnostics.Debug.WriteLine("Response values: " + JsonConvert.SerializeObject(response.Values));
                if (values != null && values.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine("Value not null or count 0");
                    int currentRow = -1;
                    bool found = false;
                    foreach (var valueRow in values)
                    {
                        System.Diagnostics.Debug.WriteLine("Values in current row: " + JsonConvert.SerializeObject(valueRow));
                        currentRow++;
                        if (valueRow.Contains(computerName))
                        {
                            row = currentRow + 1;
                            found = true;
                            System.Diagnostics.Debug.WriteLine("Found computername. " + found);
                            break;
                        }
                    }

                    if (found)
                    {
                        var request2 = sheetsService.Spreadsheets.Values.Get(spreadsheetId, "Stats!F"+row);
                        ValueRange response2 = request2.Execute();
                        IList<IList<Object>> values2 = response2.Values;
                        System.Diagnostics.Debug.WriteLine("Values2: " + JsonConvert.SerializeObject(response2));
                        //System.Diagnostics.Debug.WriteLine("count2: " + JsonConvert.SerializeObject(values2.Count));

                        if (values2 == null || values2.Count < 0)
                        {
                            System.Diagnostics.Debug.WriteLine("trying to write DATATATAT");
                            var formulas = new List<Object> {
                                     "=ROUND(SUMIFS(INDIRECT(E5&\"!B:B\"),INDIRECT(E5&\"!A:A\"),\">=\"&TODAY(),INDIRECT(E5&\"!A:A\"),\"<=\"&TODAY()+1),2)",
                                     "=ROUND(SUMIFS(INDIRECT(E5&\"!B:B\"),INDIRECT(E5&\"!A:A\"),\">=\"&TODAY(),INDIRECT(E5&\"!A:A\"),\"<=\"&TODAY()+1)/60,2)",
                                     "=ROUND(SUMIFS(INDIRECT(E5&\"!B:B\"),INDIRECT(E5&\"!A:A\"),\">=\"&TODAY(),INDIRECT(E5&\"!A:A\"),\"<=\"&TODAY()+1)/60/60,2)",
                                     "=ROUND(SUMIFS(INDIRECT(E5&\"!B:B\"),INDIRECT(E5&\"!A:A\"),\">=\"&TODAY()-1,INDIRECT(E5&\"!A:A\"),\"<=\"&TODAY())/60/60,2)",
                                     "=ROUND(SUMIFS(INDIRECT(E5&\"!B:B\"),INDIRECT(E5&\"!A:A\"),\">=\"&TODAY() - WEEKDAY(TODAY(), 2) - 7,INDIRECT(E5&\"!A:A\"),\"<=\"&TODAY() - WEEKDAY(TODAY(), 2) - 1)/60/60,2)",
                                     "=INDEX(INDIRECT(E5&\"!A:A\"),MATCH(1,INDIRECT(E5&\"!A:A\"),-1))"
                         };

                            for (int i = 0; i < formulas.Count; i++)
                            {
                                formulas[i] = formulas[i].ToString().Replace("E5", $"E{row}");
                            }

                            var updateRequest = sheetsService.Spreadsheets.Values.Update(new ValueRange
                            {
                                MajorDimension = "ROWS",
                                Range = $"Stats!F{row}:K{row}",
                                Values = new List<IList<Object>> {
                              formulas
                             }

                            }, spreadsheetId, "Stats!F" + row + ":K" + row);
                            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                            updateRequest.Execute();
                            
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (TaskCanceledException ex)
            {
                // Handle the exception
                ErrorForm errorForm = new ErrorForm(ex.Message);
                errorForm.ShowDialog();
                Console.WriteLine("Request Timed Out: " + ex.Message);
                return false;
            }
        }
        private static void AddComputerScore(SheetsService sheetsService, string spreadsheetId, string computerName, int column)
        {
            int row = GetComputerNameRow(sheetsService, spreadsheetId, computerName, column);

            if (row == -1)
            {
                // Computer name not found, return
                return;
            }

            string score = (string)Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\Software\\OTOY", "SCORE", null);
            if (score == null)
            {
                score = "N/A";
            }

            var range = $"Stats!D{row}";
            var valueRange = new ValueRange
            {
                Values = new List<IList<object>> { new List<object> { score } }
            };
            var updateRequest = sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            updateRequest.Execute();
        }

        private static int GetComputerNameRow(SheetsService sheetsService, string spreadsheetId, string computerName, int column)
        {
            var range = $"Stats!E:E";
            var response = sheetsService.Spreadsheets.Values.Get(spreadsheetId, range).Execute();
            var values = response.Values;

            if (values == null || values.Count == 0)
            {
                return -1;
            }

            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].Count > 0 && values[i][0].ToString() == computerName)
                {
                    return i ;
                }
            }

            return -1;
        }


        private static void AddComputerName(SheetsService sheetsService, string spreadsheetId, string computerName, int column)
        {
            System.Diagnostics.Debug.WriteLine("Trying to add Computername to sheet.");
            int emptyRow = GetFirstEmptyRow(sheetsService, spreadsheetId, column);
            System.Diagnostics.Debug.WriteLine("the emptyrow is " +emptyRow);
            var range = $"Stats!E{emptyRow}";
            var valueRange = new ValueRange
            {
                Values = new List<IList<object>> { new List<object> { computerName } }
            };
            var updateRequest = sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            updateRequest.Execute();

        }
        private static int GetFirstEmptyRow(SheetsService sheetsService, string spreadsheetId, int column)
        {
            SpreadsheetsResource.ValuesResource.GetRequest request = sheetsService.Spreadsheets.Values.Get(spreadsheetId, "Stats!E4:E");
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                return values.Count + 4;
            }
            return 5;
        }


        private int GetLastRow(SheetsService sheetsService, string spreadsheetId, string sheetName)
        {
            var response = sheetsService.Spreadsheets.Values.Get(spreadsheetId, $"{sheetName}!A:A").Execute();
            if (response.Values != null)
            {
                return response.Values.Count + 1;
            }
            else
            {
                return 0;
            }
        }

        private string GetLastRowValue(SheetsService sheetsService, string spreadsheetId, string sheetName)
        {
            // Define the range of cells to check
            string range = sheetName + "!A:A";
            // Get the values of the cells in the range
            var request = sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
            var response = request.Execute();
            var values = response.Values;


            string lastRow = null;
            // Check if the range is empty
            if (values == null || values.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("The range is empty.");
                //lastRow = DateTime.ParseExact("2000-01-01 00:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToString();
                lastRow = "2000-01-01 00:00:00";
                System.Diagnostics.Debug.WriteLine("Last row value: " + lastRow);
                return lastRow;
            }
            else
            {
                // Get the last row value
                lastRow = values[values.Count - 1][0].ToString();
                System.Diagnostics.Debug.WriteLine("Last row value: " + lastRow);
                return lastRow;
            }
        }
            private void WriteHello(SheetsService sheetsService, string spreadsheetId, string sheetName)
        {
            // Define the range of cells to write to
            string range = sheetName + "!D1:D1";

            // Write "Hello" to cell D1
            var newValue = new List<IList<object>> { new List<object> { "Hello" } };
            var updateRequest = sheetsService.Spreadsheets.Values.Update(new ValueRange { Values = newValue }, spreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            updateRequest.Execute();
            System.Diagnostics.Debug.WriteLine("Hello written to " + sheetName + " D1");
        }
        private Tuple<List<string>, List<string>> ExtractRenderTime(string lastRow)
        {
            var renderLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OtoyRndrNetwork", "rndr_log.txt");
            string pattern = @"(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}).*(render time [\d.]+)";
            DateTime lastRenderTime;
            if (string.IsNullOrEmpty(lastRow))
            {
                lastRenderTime = DateTime.ParseExact("2000-01-01 00:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            else
            {
                string[] formats = { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd H:mm:ss" };
                if (!DateTime.TryParseExact(lastRow, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out lastRenderTime))
                {
                    // handle the error
                }
            }
            List<string> renderTimes = new List<string>();
            List<string> datetimeFromLog = new List<string>();  //initialize variable
            try
            {
                using (var stream = new FileStream(renderLogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Match match = Regex.Match(line, pattern);
                        if (match.Success)
                        {
                            System.Diagnostics.Debug.WriteLine("Matched line: " + line);
                            DateTime renderTime = DateTime.Parse(match.Groups[1].Value);

                            if (renderTime > lastRenderTime)
                            {
                                renderTimes.Add(match.Groups[2].Value);
                                renderTimes = renderTimes.Select(x => Regex.Replace(x, @"render time ", "")).ToList();
                                datetimeFromLog.Add(renderTime.ToString());
                                //System.Diagnostics.Debug.WriteLine("Added render time: " + match.Groups[2].Value);
                            }

                        }
                    }
                }
            }
            catch (IOException e)
            {
                System.Diagnostics.Debug.WriteLine("An error occurred: " + e.Message);
            }

            System.Diagnostics.Debug.WriteLine("datetimeFromLog: " + string.Join(",", datetimeFromLog));
            System.Diagnostics.Debug.WriteLine("renderTimes: " + string.Join(",", renderTimes));
            return Tuple.Create(datetimeFromLog, renderTimes);

        }
        private Tuple<List<string>, List<string>> ExtractRenderTimefast(string lastRow)
        {
            var renderLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OtoyRndrNetwork", "rndr_log.txt");
            string pattern = @"(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}).*(render time [\d.]+)";
            DateTime lastRenderTime;
            if (string.IsNullOrEmpty(lastRow))
            {
                lastRenderTime = DateTime.ParseExact("2000-01-01 00:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            else
            {
                string[] formats = { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd H:mm:ss" };
                if (!DateTime.TryParseExact(lastRow, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out lastRenderTime))
                {
                    // handle the error
                }
            }
            List<string> renderTimes = new List<string>();
            List<string> datetimeFromLog = new List<string>();  //initialize variable
            try
            {
                using (var stream = new FileStream(renderLogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(stream))
                              
                {
                    var lineCount = 0;                    
                    {
                        while (reader.ReadLine() != null)
                        {
                            lineCount++;
                        }
                    }
                    if (lineCount > 1000)
                    {                        
                        {
                            stream.Seek(-(1000 * (Environment.NewLine.Length + 100)), SeekOrigin.End);
                            reader.ReadLine();
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                Match match = Regex.Match(line, pattern);
                                if (match.Success)
                                {
                                    System.Diagnostics.Debug.WriteLine("Matched line: " + line);
                                    DateTime renderTime = DateTime.Parse(match.Groups[1].Value);

                                    if (renderTime > lastRenderTime)
                                    {
                                        renderTimes.Add(match.Groups[2].Value);
                                        renderTimes = renderTimes.Select(x => Regex.Replace(x, @"render time ", "")).ToList();
                                        datetimeFromLog.Add(renderTime.ToString());
                                        //System.Diagnostics

                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (IOException e)
            {
                System.Diagnostics.Debug.WriteLine("An error occurred: " + e.Message);
            }

            System.Diagnostics.Debug.WriteLine("FASTdatetimeFromLog: " + string.Join(",", datetimeFromLog));
            System.Diagnostics.Debug.WriteLine("FASTrenderTimes: " + string.Join(",", renderTimes));
            return Tuple.Create(datetimeFromLog, renderTimes);

        }
        private void AddRenderTime(List<string> datetimeFromLog, List<string> renderTimes, string spreadsheetId, string sheetName)
        {
            // Create a new instance of the Sheets API service
            var credPath = GetCredentialsPath();
            var sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GoogleCredential.FromFile(credPath).CreateScoped(SheetsService.Scope.Spreadsheets),
                ApplicationName = "GPUMonitor"
            });

            // Get the sheet ID and the last row in the sheet            
            var sheetId = GetSheetId(sheetsService, spreadsheetId, sheetName);
            int lastRow;
            var response = sheetsService.Spreadsheets.Values.Get(spreadsheetId, $"{sheetName}!A:A").Execute();
            if (response.Values != null)
            {
                lastRow = response.Values.Count + 1;
            }
            else
            {
                lastRow = 1;
            }
            // Add new rows to sheet
            int numRows = datetimeFromLog.Count;
            AddRows(sheetsService, spreadsheetId, sheetName, lastRow, numRows);

            // Create a new request body
            var requestBody = new BatchUpdateValuesRequest();
            requestBody.ValueInputOption = "USER_ENTERED";
            requestBody.Data = new List<ValueRange>();

            // Convert the datetimeFromLog values and render times values
            datetimeFromLog = datetimeFromLog.Select(x => DateTime.Parse(x).ToString("yyyy-MM-dd HH:mm:ss")).ToList();

            // Create the list of values to be added to the sheet
            List<IList<object>> values = new List<IList<object>>();
            for (int i = 0; i < datetimeFromLog.Count; i++)
            {
                values.Add(new List<object>() { datetimeFromLog[i], renderTimes[i] });
            }            

            // Add the values to the sheet
            requestBody.Data.Add(new ValueRange
            {
                Range = $"{sheetName}!A{lastRow}:B{lastRow + datetimeFromLog.Count - 1}",
                Values = values
            });

            // Execute the request
            var request = sheetsService.Spreadsheets.Values.BatchUpdate(requestBody, spreadsheetId);
            request.Execute();

            // Format the cells
            FormatCells(spreadsheetId, sheetName, lastRow + datetimeFromLog.Count);
        }
        private void AddRows(SheetsService sheetsService, string spreadsheetId, string sheetName, int startRow, int numRows)
        {
            var requests = new List<Request>();
            var insertDimensionRequest = new Request
            {
                InsertDimension = new InsertDimensionRequest
                {
                    Range = new DimensionRange
                    {
                        SheetId = int.Parse(GetSheetId(sheetsService, spreadsheetId, sheetName)),
                        Dimension = "ROWS",
                        StartIndex = startRow - 1,
                        EndIndex = startRow + numRows - 1
                    },
                    InheritFromBefore = startRow == 1 ? false : true
                }
            };
            requests.Add(insertDimensionRequest);

            var requestBody = new BatchUpdateSpreadsheetRequest
            {
                Requests = requests
            };

            sheetsService.Spreadsheets.BatchUpdate(requestBody, spreadsheetId).Execute();
        }


        private void FormatCells(string spreadsheetId, string sheetName, int numberOfRows)
        {
            // Create a new instance of the Sheets API service
            //string credPath = "C:\\dev\\SimpleButton\\Simple Button\\bin\\Debug\\net7.0-windows\\credentials.json";
            var credPath = GetCredentialsPath();
            var sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GoogleCredential.FromFile(credPath).CreateScoped(SheetsService.Scope.Spreadsheets),
                ApplicationName = "GPUMonitor"
            });

            var requestBody = new BatchUpdateSpreadsheetRequest();
            requestBody.Requests = new List<Request>();

            // Define the range of cells to format
            var cellRange = $"{sheetName}!A1:A{numberOfRows}";
            var sheetId = int.Parse(GetSheetId(sheetsService, spreadsheetId, sheetName));

            // Define the format of the cells
            var formatRequest = new Request
            {
                RepeatCell = new RepeatCellRequest
                {
                    Range = new GridRange
                    {
                        SheetId = sheetId,
                        StartColumnIndex = 0,
                        EndColumnIndex = 1,
                        StartRowIndex = 0,
                        EndRowIndex = numberOfRows
                    },
                    Cell = new CellData
                    {
                        UserEnteredFormat = new CellFormat
                        {
                            NumberFormat = new NumberFormat
                            {
                                Type = "DATE_TIME",
                                Pattern = "yyyy-MM-dd HH:mm:ss"
                            }
                        }
                    },
                    Fields = "userEnteredFormat.numberFormat"
                }
            };


            requestBody.Requests.Add(formatRequest);
            var updateRequest = sheetsService.Spreadsheets.BatchUpdate(requestBody, spreadsheetId);
            updateRequest.Execute();
        }
        private string GetCredentialsPath()
        {
            string exePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string credPath = Path.Combine(exePath, "credentials.json");
            if (!System.IO.File.Exists(credPath))
            {
                ErrorForm errorForm = new ErrorForm("credentials.json is missing in the root directory of GPU-Monitor.exe");
                errorForm.ShowDialog();
            }
            return credPath;
        }       

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            RetrieveRegistryValues();
            Properties.Settings.Default.AutoSheet = checkBox2.Checked;
            Properties.Settings.Default.Save();
            int minutes = GsheetTimer; // 310; // 12 hours is 720 min etc

            if (checkBox2.Checked && !isTimerCreated)
            {
                timer = new System.Threading.Timer(TimerCallback, null, 0, minutes * 60 * 1000);
                isTimerCreated = true;
            }
            else if (!checkBox2.Checked && isTimerCreated)
            {
                timer.Dispose();
                isTimerCreated = false;
            }
        }


    }

}



