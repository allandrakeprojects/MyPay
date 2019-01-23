using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyPay
{
    public partial class Main_Form : Form
    {
        private JObject __jo;
        private bool __isLogin = false;
        private bool __isClose;
        private bool __isBreak = false;
        private bool __autoReject = false;
        private int __secho;
        private int __display_length = 5000;
        private int __total_page;
        private int __result_count_json;
        private int __send = 0;
        private string __brand_code = "MyPay";
        private string __app = "FD Grab";
        private string __app_type = "3";
        private string __player_last_bill_no = "";
        private string __player_last_bill_no_pending = "";
        private string __player_id = "";
        private string __playerlist_cn = "";
        private string __playerlist_cn_pending = "";
        private string __last_username = "";
        private string __bill_no = "";
        Form __mainFormHandler;

        // Drag Header to Move
        private bool m_aeroEnabled;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        // ----- Drag Header to Move

        // Form Shadow
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );
        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ACTIVATEAPP = 0x001C;
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int CS_DBLCLKS = 0x8;
        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }
        protected override CreateParams CreateParams
        {
            get
            {
                m_aeroEnabled = CheckAeroEnabled();

                CreateParams cp = base.CreateParams;
                if (!m_aeroEnabled)
                    cp.ClassStyle |= CS_DROPSHADOW;

                cp.Style |= WS_MINIMIZEBOX;
                cp.ClassStyle |= CS_DBLCLKS;
                return cp;
            }
        }
        private bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0;
                DwmIsCompositionEnabled(ref enabled);
                return (enabled == 1) ? true : false;
            }
            return false;
        }
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCPAINT:
                    if (m_aeroEnabled)
                    {
                        var v = 2;
                        DwmSetWindowAttribute(Handle, 2, ref v, 4);
                        MARGINS margins = new MARGINS()
                        {
                            bottomHeight = 1,
                            leftWidth = 0,
                            rightWidth = 0,
                            topHeight = 0
                        };
                        DwmExtendFrameIntoClientArea(Handle, ref margins);

                    }
                    break;
                default:
                    break;
            }
            base.WndProc(ref m);

            if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT)
                m.Result = (IntPtr)HTCAPTION;
        }
        // ----- Form Shadow

        public Main_Form()
        {
            InitializeComponent();

            timer_landing.Start();
        }

        // Drag to Move
        private void panel_header_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void label_title_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }

            //Properties.Settings.Default.______last_bill_no = "";
            //Properties.Settings.Default.Save();
        }
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void pictureBox_loader_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void label_brand_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void panel_landing_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void pictureBox_landing_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        // ----- Drag to Move

        // Click Close
        private void pictureBox_close_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Exit the program?", "MyPay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                __isClose = true;
                Environment.Exit(0);
            }
        }

        // Click Minimize
        private void pictureBox_minimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        // Form Closing
        private void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!__isClose)
            {
                DialogResult dr = MessageBox.Show("Exit the program?", "MyPay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    Environment.Exit(0);
                }
            }

            Environment.Exit(0);
        }

        // Form Load
        private void Main_Form_Load(object sender, EventArgs e)
        {
            // asdasd
            //Properties.Settings.Default.______pending_bill_no = "";
            //Properties.Settings.Default.Save();

            webBrowser.Navigate("http://secure.skyking88.com/kf83wf/admin/login.jsp");
        }

        static int LineNumber([System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            return lineNumber;
        }

        // WebBrowser
        private async void WebBrowser_DocumentCompletedAsync(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser.ReadyState == WebBrowserReadyState.Complete)
            {
                if (e.Url == webBrowser.Url)
                {
                    try
                    {
                        if (webBrowser.Url.ToString().Equals("http://secure.skyking88.com/kf83wf/admin/login.jsp"))
                        {
                            if (__isLogin)
                            {
                                label_brand.Visible = false;
                                pictureBox_loader.Visible = false;
                                label_player_last_bill_no.Visible = false;
                                label_page_count.Visible = false;
                                label_currentrecord.Visible = false;
                                __mainFormHandler = Application.OpenForms[0];
                                __mainFormHandler.Size = new Size(466, 468);

                                string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                                SendPaymentTeam("The application have been logout, please re-login again.");
                                SendMyBot("The application have been logout, please re-login again.");
                                __send = 0;
                            }

                            __isLogin = false;
                            timer.Stop();
                            webBrowser.Document.GetElementById("name").SetAttribute("value", "MypayRain");
                            webBrowser.Document.GetElementById("pwd").SetAttribute("value", "Rain@1234");
                            webBrowser.Visible = true;
                            webBrowser.WebBrowserShortcutsEnabled = true;
                        }
                        
                        if (webBrowser.Url.ToString().Equals("http://secure.skyking88.com/kf83wf/admin/index.jsp"))
                        {
                            label_brand.Visible = true;
                            pictureBox_loader.Visible = true;
                            label_player_last_bill_no.Visible = true;
                            label_page_count.Visible = true;
                            label_currentrecord.Visible = true;
                            __mainFormHandler = Application.OpenForms[0];
                            __mainFormHandler.Size = new Size(466, 168);

                            if (!__isLogin)
                            {
                                __isLogin = true;
                                webBrowser.Visible = false;
                                label_brand.Visible = true;
                                pictureBox_loader.Visible = true;
                                label_player_last_bill_no.Visible = true;
                                webBrowser.WebBrowserShortcutsEnabled = false;
                                ___LastBillNoAsync();
                                await ___GetListsRequest();
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                        SendPaymentTeam("There's a problem to the server, please re-open the application.");
                        SendMyBot(err.ToString());
                        __send = 0;

                        __isClose = false;
                        Environment.Exit(0);
                    }
                }
            }
        }

        private void timer_landing_Tick(object sender, EventArgs e)
        {
            panel_landing.Visible = false;
            timer_landing.Stop();
        }

        private async void ___LastBillNoAsync()
        {
            if (Properties.Settings.Default.______last_bill_no == "")
            {
                await ___GetLastBillNoAsync();
            }

            label_player_last_bill_no.Text = "Last Bill No.: " + Properties.Settings.Default.______last_bill_no;
        }

        private async Task ___GetLastBillNoAsync()
        {
            try
            {
                using (var wb = new WebClient())
                {
                    var result = await wb.DownloadDataTaskAsync("http://zeus.ssimakati.com:8080/API/lastMyPay");
                    string responsebody = Encoding.UTF8.GetString(result);
                    var deserializeObject = JsonConvert.DeserializeObject(responsebody);
                    JObject jo = JObject.Parse(deserializeObject.ToString());
                    JToken lbn = jo.SelectToken("$.msg");
                    
                    Properties.Settings.Default.______last_bill_no = lbn.ToString();
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    __send++;
                    if (__send == 5)
                    {
                        string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                        SendPaymentTeam("There's a problem to the server, please re-open the application.");
                        SendMyBot(err.ToString());
                        __send = 0;

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        await ___GetLastBillNoAsync2();
                    }
                }
            }
        }

        private async Task ___GetLastBillNoAsync2()
        {
            try
            {
                using (var wb = new WebClient())
                {
                    var result = await wb.DownloadDataTaskAsync("http://zeus2.ssimakati.com:8080/API/lastMyPay");
                    string responsebody = Encoding.UTF8.GetString(result);
                    var deserializeObject = JsonConvert.DeserializeObject(responsebody);
                    JObject jo = JObject.Parse(deserializeObject.ToString());
                    JToken lbn = jo.SelectToken("$.msg");
                    
                    Properties.Settings.Default.______last_bill_no = lbn.ToString();
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    __send++;
                    if (__send == 5)
                    {
                        string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                        SendPaymentTeam("There's a problem to the server, please re-open the application.");
                        SendMyBot(err.ToString());
                        __send = 0;

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        await ___GetLastBillNoAsync();
                    }
                }
            }
        }

        private void ___SaveLastBillNo(string username)
        {
            Properties.Settings.Default.______last_bill_no = username;
            Properties.Settings.Default.Save();
        }

        // ------ Functions
        private async Task ___GetListsRequest()
        {
            __isBreak = false;

            try
            {
                var cookie = Cookie.GetCookieInternal(webBrowser.Url, false);
                WebClient wc = new WebClient();

                wc.Headers.Add("Cookie", cookie);
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                wc.Headers["X-Requested-With"] = "XMLHttpRequest";

                var reqparm = new NameValueCollection
                {
                    {"_search", "false"},
                    {"nd", "1547789368393"},
                    {"showCount", "5000"},
                    {"pageNumber", ""},
                    {"order", "createDate"},
                    {"orderSeq", "desc"}
                };

                byte[] result = await wc.UploadValuesTaskAsync("http://secure.skyking88.com/kf83wf/admin/onlinePaymentMgmt/findOPOrderGrid4AtoFlash.zv?queryStatus=-1", "POST", reqparm);
                string responsebody = Encoding.UTF8.GetString(result);
                var deserializeObject = JsonConvert.DeserializeObject(responsebody);
                __jo = JObject.Parse(deserializeObject.ToString());
                JToken count = __jo.SelectToken("$.result.resultList");
                __result_count_json = count.Count();
                ___ListAsync();
                __send = 0;
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    __send++;
                    if (__send == 5)
                    {
                        string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                        SendPaymentTeam("There's a problem to the server, please re-open the application.");
                        SendMyBot(err.ToString());
                        __send = 0;

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        await ___GetListsRequest();
                    }
                }
            }
        }

        private void ___ListAsync()
        {
            List<string> player_info = new List<string>();

            for (int i = 0; i < 1; i++)
            {
                if (__isBreak)
                {
                    break;
                }

                for (int ii = 0; ii < __result_count_json; ii++)
                {
                    Application.DoEvents();
                    JToken bill_no = __jo.SelectToken("$.result.resultList.[" + ii + "].orderNo").ToString();

                    if (bill_no.ToString().Trim() != Properties.Settings.Default.______last_bill_no)
                    {
                        JToken source_order_no = __jo.SelectToken("$.result.resultList.[" + ii + "].sourceOrderNo").ToString();
                        JToken merchant_name = __jo.SelectToken("$.result.resultList.[" + ii + "].merchantName").ToString();
                        JToken payment_method_name = __jo.SelectToken("$.result.resultList.[" + ii + "].paymentMethodName").ToString();

                        if (ii == 0)
                        {
                            __player_last_bill_no = bill_no.ToString().Trim();
                        }
                        
                        player_info.Add(bill_no + "*|*" + source_order_no + "*|*" + merchant_name + "*|*" + payment_method_name);
                    }
                    else
                    {
                        // send to api
                        if (player_info.Count != 0)
                        {
                            player_info.Reverse();
                            string player_info_get = String.Join(",", player_info);
                            char[] split = ",".ToCharArray();
                            string[] values = player_info_get.Split(split);
                            foreach (string value in values)
                            {
                                Application.DoEvents();
                                string[] values_inner = value.Split(new string[] { "*|*" }, StringSplitOptions.None);
                                int count = 0;
                                string _bill_no = "";
                                string _source_order_no = "";
                                string _merchant_name = "";
                                string _payment_method_name = "";

                                foreach (string value_inner in values_inner)
                                {
                                    count++;

                                    // Username
                                    if (count == 1)
                                    {
                                        _bill_no = value_inner;
                                    }
                                    // Name
                                    else if (count == 2)
                                    {
                                        _source_order_no = value_inner;
                                    }
                                    // Deposit Date
                                    else if (count == 3)
                                    {
                                        _merchant_name = value_inner;
                                    }
                                    // VIP
                                    else if (count == 4)
                                    {
                                        _payment_method_name = value_inner;
                                    }
                                }

                                ___InsertData(_bill_no, _source_order_no, _merchant_name, _payment_method_name);

                                __send = 0;
                            }
                        }

                        if (!String.IsNullOrEmpty(__player_last_bill_no.Trim()))
                        {
                            ___SaveLastBillNo(__player_last_bill_no);

                            Invoke(new Action(() =>
                            {
                                label_player_last_bill_no.Text = "Last Bill No.: " + Properties.Settings.Default.______last_bill_no;
                            }));
                        }

                        player_info.Clear();
                        timer.Start();
                        __isBreak = true;
                        break;
                    }
                }
            }
        }

        private void ___InsertData(string bill_no, string source_order_no, string merchant_name, string payment_method_name)
        {
            try
            {
                string password = merchant_name + source_order_no + "youdieidie";
                byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                string token = BitConverter.ToString(hash)
                   .Replace("-", string.Empty)
                   .ToLower();

                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection
                    {
                        ["pg"] = merchant_name,
                        ["method"] = payment_method_name,
                        ["trans_id"] = source_order_no,
                        ["pg_trans_id"] = bill_no,
                        ["token"] = token
                    };

                    var response = wb.UploadValues("http://zeus.ssimakati.com:8080/API/fixMyPay", "POST", data);
                    string responseInString = Encoding.UTF8.GetString(response);

                    using (StreamWriter file = new StreamWriter(Path.GetTempPath() + @"\fdgrab_mypay.txt", true, Encoding.UTF8))
                    {
                        file.WriteLine(bill_no + "*|*" + source_order_no + "*|*" + merchant_name + "*|*" + payment_method_name);
                        file.Close();
                    }
                }
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    __send++;
                    if (__send == 5)
                    {
                        string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                        SendPaymentTeam("There's a problem to the server, please re-open the application.");
                        SendMyBot(err.ToString());
                        __send = 0;

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ____InsertData2(bill_no, source_order_no, merchant_name, payment_method_name);
                    }
                }
            }
        }

        private void ____InsertData2(string bill_no, string source_order_no, string merchant_name, string payment_method_name)
        {
            try
            {
                string password = merchant_name + source_order_no + "youdieidie";
                byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                string token = BitConverter.ToString(hash)
                   .Replace("-", string.Empty)
                   .ToLower();

                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection
                    {
                        ["pg"] = merchant_name,
                        ["method"] = payment_method_name,
                        ["trans_id"] = source_order_no,
                        ["pg_trans_id"] = bill_no,
                        ["token"] = token
                    };

                    var response = wb.UploadValues("http://zeus2.ssimakati.com:8080/API/fixMyPay", "POST", data);
                    string responseInString = Encoding.UTF8.GetString(response);

                    using (StreamWriter file = new StreamWriter(Path.GetTempPath() + @"\fdgrab_mypay.txt", true, Encoding.UTF8))
                    {
                        file.WriteLine(bill_no + "*|*" + source_order_no + "*|*" + merchant_name + "*|*" + payment_method_name);
                        file.Close();
                    }
                }
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    __send++;
                    if (__send == 5)
                    {
                        string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                        SendPaymentTeam("There's a problem to the server, please re-open the application.");
                        SendMyBot(err.ToString());
                        __send = 0;

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ___InsertData(bill_no, source_order_no, merchant_name, payment_method_name);
                    }
                }
            }
        }

        private async void timer_TickAsync(object sender, EventArgs e)
        {
            timer.Stop();
            await ___GetListsRequest();
        }

        private void SendMyBot(string message)
        {
            try
            {
                string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                string urlString = "https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}";
                string apiToken = "772918363:AAHn2ufmP3ocLEilQ1V-IHcqYMcSuFJHx5g";
                string chatId = "@allandrake";
                string text = "-----" + __brand_code + " " + __app + "-----%0A%0ADate%20and%20Time:%20[" + datetime + "]%0AMessage:%20" + message + "";
                urlString = String.Format(urlString, apiToken, chatId, text);
                WebRequest request = WebRequest.Create(urlString);
                Stream rs = request.GetResponse().GetResponseStream();
                StreamReader reader = new StreamReader(rs);
                string line = "";
                StringBuilder sb = new StringBuilder();
                while (line != null)
                {
                    line = reader.ReadLine();
                    if (line != null)
                        sb.Append(line);
                }
            }
            catch (Exception err)
            {
                __send++;
                if (__send == 5)
                {
                    MessageBox.Show(err.ToString());

                    __isClose = false;
                    Environment.Exit(0);
                }
                else
                {
                    SendMyBot(message);
                }
            }
        }

        private void SendPaymentTeam(string message)
        {
            try
            {
                string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                string urlString = "https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}";
                string apiToken = "612187347:AAE9doWWcStpWrDrfpOod89qGSxCJ5JwQO4";
                string chatId = "@mypay_payment_team";
                string text = "Date%20and%20Time:%20[" + datetime + "]%0AMessage:%20" + message + "";
                urlString = String.Format(urlString, apiToken, chatId, text);
                WebRequest request = WebRequest.Create(urlString);
                Stream rs = request.GetResponse().GetResponseStream();
                StreamReader reader = new StreamReader(rs);
                string line = "";
                StringBuilder sb = new StringBuilder();
                while (line != null)
                {
                    line = reader.ReadLine();
                    if (line != null)
                    {
                        sb.Append(line);
                    }
                }
            }
            catch (Exception err)
            {
                __send++;
                if (__send == 5)
                {
                    MessageBox.Show(err.ToString());

                    __isClose = false;
                    Environment.Exit(0);
                }
                else
                {
                    SendPaymentTeam(message);
                }
            }
        }

        private void label_player_last_bill_no_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void label_player_last_bill_no_MouseClick(object sender, MouseEventArgs e)
        {
            Clipboard.SetText(label_player_last_bill_no.Text.Replace("Last Bill No.: ", "").Trim());
        }

        private void timer_flush_memory_Tick(object sender, EventArgs e)
        {
            FlushMemory();
        }

        public static void FlushMemory()
        {
            Process prs = Process.GetCurrentProcess();
            try
            {
                prs.MinWorkingSet = (IntPtr)(300000);
            }
            catch (Exception err)
            {
                // leave blank
            }
        }

        private void timer_detect_running_Tick(object sender, EventArgs e)
        {
            ___DetectRunning();
        }

        private void ___DetectRunning()
        {
            try
            {
                string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string password = "TF" + datetime + "youdieidie";
                byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                string token = BitConverter.ToString(hash)
                   .Replace("-", string.Empty)
                   .ToLower();

                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection
                    {
                        ["brand_code"] = "TF",
                        ["app_type"] = __app_type,
                        ["last_update"] = datetime,
                        ["token"] = token
                    };

                    var response = wb.UploadValues("http://zeus.ssimakati.com:8080/API/updateAppStatus", "POST", data);
                    string responseInString = Encoding.UTF8.GetString(response);
                }
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    __send++;
                    if (__send == 5)
                    {
                        string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                        SendPaymentTeam("There's a problem to the server, please re-open the application.");
                        SendMyBot(err.ToString());
                        __send = 0;

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ___DetectRunning2();
                    }
                }
            }
        }

        private void ___DetectRunning2()
        {
            try
            {
                string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string password = "TF" + datetime + "youdieidie";
                byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                string token = BitConverter.ToString(hash)
                   .Replace("-", string.Empty)
                   .ToLower();

                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection
                    {
                        ["brand_code"] = "TF",
                        ["app_type"] = __app_type,
                        ["last_update"] = datetime,
                        ["token"] = token
                    };

                    var response = wb.UploadValues("http://zeus2.ssimakati.com:8080/API/updateAppStatus", "POST", data);
                    string responseInString = Encoding.UTF8.GetString(response);
                }
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    __send++;
                    if (__send == 5)
                    {
                        string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                        SendPaymentTeam("There's a problem to the server, please re-open the application.");
                        SendMyBot(err.ToString());
                        __send = 0;

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ___DetectRunning();
                    }
                }
            }
        }
    }
}
