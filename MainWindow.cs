using System;
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

//using Google.GoogleApiException;

namespace Simple_Button
{
    public partial class MainWindow : Form
    {
        private bool _settingsValid = true;
        private bool _isRunning;
        private string ?_path;
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

        public MainWindow()
        {            
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Pukerud's GPU Monitor Running On: " + Environment.MachineName;
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
            _lastSent = DateTime.Now.AddMinutes(-_timeThreshold);
            string startingFolder = @"C:\Windows\System32\DriverStore\FileRepository\";
            string fileName = "nvidia-smi.exe";
            string ?path = null;
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

        }

        private async void button1_Click(object? sender, EventArgs? e)
        {                       
            //MessageBox.Show("GPULoad Check Started");            
            _isRunning = true;
            await Task.Run(() => {
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
                        Console.WriteLine("highload= "+ _highLoadGpuCount +" Lowload= "+ _lowLoadGpuCount + "ok");
                    }
                    Console.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] GPU usage on " + _computer + ". Number of GPUs: " + _gpuCount + ", High load GPU count: " + _highLoadGpuCount + ", Low load GPU count: " + _lowLoadGpuCount);
                    textBox1.Invoke((MethodInvoker)delegate {
                        textBox1.AppendText("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Number of GPUs: " + _gpuCount + ", High load GPU count: " + _highLoadGpuCount + ", Low load GPU count: " + _lowLoadGpuCount + Environment.NewLine);
                    });                    
                    if (_highLoadGpuCount <= 3 && _lowLoadGpuCount != _gpuCount)
                    {
                        Console.WriteLine("Need to Double-check GPU usage... waiting 60 seconds            ");
                        textBox1.Invoke((MethodInvoker)delegate {
                            textBox1.AppendText("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Need to Double-check GPU usage... waiting "+ _sleepTime2 + " seconds  \r\n");
                        });
                        System.Threading.Thread.Sleep(_sleepTime2 * 1000);
                        var currentTime = DateTime.Now;
                        var timeSinceLastEmail = currentTime - _lastSent;
                        if (timeSinceLastEmail.TotalMinutes >= _timeThreshold)
                        {
                            _emailSent = false;
                            textBox1.Invoke((MethodInvoker)delegate {
                                textBox1.AppendText("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Sending Mail Enabled Again....\r\n");
                            });
                        }
                        Console.WriteLine("Double-checking GPU usage...");
                        textBox1.Invoke((MethodInvoker)delegate {
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
                            Console.WriteLine("highload= " + _highLoadGpuCount + " Lowload= " + _lowLoadGpuCount + "ok");
                            //_highLoadGpuCount = gpuUsage.Count(x => x.Contains("100 %") || x.Contains("8[0-9] %") || x.Contains("9[0-9] %"));
                            //_lowLoadGpuCount = gpuUsage.Count(x => !x.Contains("100 %") && !x.Contains("8[0-9] %") && !x.Contains("9[0-9] %"));
                        }
                        Console.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] GPU usage on " + _computer + ". Number of GPUs: " + _gpuCount + ", High load GPU count: " + _highLoadGpuCount + ", Low load GPU count: " + _lowLoadGpuCount + Environment.NewLine);
                        textBox1.Invoke((MethodInvoker)delegate {
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
           // textBox1.Text = "";
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
            ErrorForm errorForm = new ErrorForm("The feature is not implemented yet.");
            errorForm.ShowDialog();
            //AddDataToSheet();

        }
        public void AddDataToSheet()
        {          
            try
            {
                // Get the current computer name            
                var computerName = Environment.MachineName;

            // Define the Spreadsheet and Sheet name
            var spreadsheetName = "Rndr-Stats";
            var sheetName = computerName;

            // Path to the json credentials file
            string credPath = "C:\\dev\\SimpleButton\\Simple Button\\bin\\Debug\\net7.0-windows\\credentials.json";

            // Create a new instance of the SheetsService
            var service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GoogleCredential.FromFile(credPath).CreateScoped(SheetsService.Scope.Spreadsheets),
                ApplicationName = "GPUMonitor"
            });       

                // Check if the spreadsheet exists
                var spreadsheetId = GetSpreadsheetId(service, spreadsheetName);
                // Print out the value of spreadsheetId before creating the spreadsheet
                System.Diagnostics.Debug.WriteLine("spreadsheetId before creating spreadsheet: " + spreadsheetId);

            // If the spreadsheet doesn't exist, create it
            if (spreadsheetId == null)
            {
                spreadsheetId = CreateSpreadsheet(service, spreadsheetName);
                    // Print out the value of spreadsheetId after creating the spreadsheet
                    System.Diagnostics.Debug.WriteLine("spreadsheetId after creating spreadsheet: " + spreadsheetId);
            }

            // Check if the sheet exists
            var sheetId = GetSheetId(service, spreadsheetId, sheetName);

            // If the sheet doesn't exist, create it
            if (sheetId == null)
            {
                sheetId = CreateSheet(service, spreadsheetId, sheetName);
                // Print out the value of spreadsheetName and sheetName
                System.Diagnostics.Debug.WriteLine("spreadsheetName: " + spreadsheetName);
                System.Diagnostics.Debug.WriteLine("sheetName: " + sheetName);
            }

                // Add data to the sheet
                
            var range = $"{sheetName}!A1";
            var valueRange = new ValueRange();
            valueRange.Values = new List<IList<object>> { new List<object> { "RenderTime", "DateTime" } };
            var appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
            appendRequest.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();
                var renderTimes = ReadRenderLogFile();
            }
            catch (GoogleApiException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

        }
        private string GetSpreadsheetId(SheetsService service, string spreadsheetName)
        {
            try
            {
                // Path to the json credentials file
                string credPath = "C:\\dev\\SimpleButton\\Simple Button\\bin\\Debug\\net7.0-windows\\credentials.json";
                // Creat a new instance of the DriveService
                var driveService = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = GoogleCredential.FromFile(credPath).CreateScoped(DriveService.Scope.Drive),
                    ApplicationName = "GPUMonitor"
                });
                var request = driveService.Files.List();
                request.Q = "mimeType='application/vnd.google-apps.spreadsheet'";
                var files = request.Execute();

                if (files.Files.Count > 0)
                {
                    return files.Files[0].Id;
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
            string credPath = "C:\\dev\\SimpleButton\\Simple Button\\bin\\Debug\\net7.0-windows\\credentials.json";
            var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GoogleCredential.FromFile(credPath).CreateScoped(DriveService.Scope.Drive),
                ApplicationName = "GPUMonitor"
            });

            // Set permissions for the creator and an additional email address
            var emailAddress = GsheetShare;
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
        private List<Tuple<string, string>> ReadRenderLogFile()
        {
            var renderLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OtoyRndrNetwork", "rndr_log.txt");
            System.Diagnostics.Debug.WriteLine("Renderlogpath: " + renderLogPath);
            var renderTimes = new List<Tuple<string, string>>();
            using (var stream = new FileStream(renderLogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                // Seek to the end of the file
                stream.Seek(0, SeekOrigin.End);
                var remainingBytes = stream.Length;
                var lineCount = 0;

                while (remainingBytes > 0 && lineCount < 10)
                {
                    // Go back one byte
                    stream.Seek(-1, SeekOrigin.Current);
                    remainingBytes--;

                    // Read the byte
                    var b = (char)stream.ReadByte();

                    // If the byte is a newline character, increment the line count
                    if (b == '\n')
                    {
                        lineCount++;
                    }
                }

                // Read the lines
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line.Contains("render time"))
                    {
                        var dateTime = line.Substring(0, 20);
                            var start = line.IndexOf("time") + 5;
                            var end = line.IndexOf("seconds") - 1;
                            var seconds = line.Substring(start, end - start + 1);
                            renderTimes.Add(Tuple.Create(dateTime, seconds));
                        }
                    }
                }
            if (renderTimes.Count == 0)
                System.Diagnostics.Debug.WriteLine("renderTimes list is empty");
            else
                foreach (var x in renderTimes)
                {
                    System.Diagnostics.Debug.WriteLine(x.Item1 + " " + x.Item2);
                }       
             return renderTimes;
        }
            
        



    }
}
