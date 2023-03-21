using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace SshTunnel
{
    public partial class MainForm : Form
    {
        //. SSH 
        int _ssh_listen_port = 7015;
        string _ssh_user_id = "arpan";
        string _ssh_host_ip = "103.121.236.50";
        int _ssh_host_port = 22;
        string _ssh_keyfile = "arpan_openssh.ppk";

        //. DB
        string _db_server_ip = "192.168.10.51";
        int _db_server_port = 3306;
        string _db_name = "customers";
        string _db_user_id = "tunnel";
        string _db_user_pw = "th#3Q@@8$M3rg";

        private SshMgr _sshMgr = null;

        public MainForm()
        {
            InitializeComponent();

            InitControls();
        }

        private void InitControls()
        {            
#if DEBUG
            _txtUserName.Text = "diva";
            _txtUserPswd.Text = "pace";
#endif
            SetConnectionState(0);
        }

        private void SetConnectionState(int state)
        {
            switch(state)
            {
                case -1:
                    _btnConnect.Enabled = false;
                    _btnDisconnect.Enabled = false;
                    break;
                case 0:
                    _btnConnect.Enabled = true;
                    _btnDisconnect.Enabled = false;
                    break;
                case 1:
                    _btnConnect.Enabled = false;
                    _btnDisconnect.Enabled = true;
                    break;
            }
        }

        private void _btnDisconnect_Click(object sender, EventArgs e)
        {
            //--------------------------------------------
            // Check connection state
            //--------------------------------------------
            if (_sshMgr != null && _sshMgr.ssh_isconnected())
            {
                SetConnectionState(-1);
                InvokeDumpLog("Disconnecting...");
                _sshMgr.disconnect_ssh();
                InvokeDumpLog("Disconnected!");
                SetConnectionState(0);

                return;
            }
        }

        private void _btnConnect_Click(object sender, EventArgs e)
        {
            //--------------------------------------------
            // Check connection state
            //--------------------------------------------
            if (_sshMgr != null && _sshMgr.ssh_isconnected())
            {
                InvokeDumpLog("The SSH tunnel is connected already!");
                return;
            }

            //--------------------------------------------
            // Check the input data.
            //--------------------------------------------
            if (string.IsNullOrEmpty(_txtUserName.Text))
            {
                InvokeDumpLog("Please Input the user name.");
                return;
            }

            if (string.IsNullOrEmpty(_txtUserName.Text))
            {
                InvokeDumpLog("Please Input the user password.");
                return;
            }

            //--------------------------------------------
            //
            //--------------------------------------------
            Cursor.Current = Cursors.WaitCursor;
            SetConnectionState(-1);
            do
            {
                //--------------------------------------------
                // Connect to Database
                //--------------------------------------------
                InvokeDumpLog("Connecting to DB...");
                DbMgr w_dbMgr = new DbMgr(_db_name, _db_server_ip, _db_server_port, _db_user_id, _db_user_pw, true, _ssh_host_ip, _ssh_host_port, _ssh_user_id, "", _ssh_keyfile);
                if (!w_dbMgr.connectable())
                {
                    InvokeDumpLog("Failed to connect db.");
                    break;
                }
                else
                {
                    InvokeDumpLog("Success to connect db!");
                }

                //--------------------------------------------
                // Query account info in DB
                //--------------------------------------------
                DataTable w_dt = null;
                DataRow w_selRow = null;
                try
                {
                    string w_query = @"SELECT * FROM server";
                    w_dt = w_dbMgr.select(w_query, null);
                    if (w_dt == null || w_dt.Rows == null)
                    {
                        InvokeDumpLog("Account Table is empty.");
                        w_dbMgr.close();
                        break;
                    }

                    //--------------------------------------------
                    // Find the user account information for input user.
                    //--------------------------------------------
                    bool w_incorrect_pswd = false;
                    foreach (DataRow w_row in w_dt.Rows)
                    {
                        string w_user_name = w_row["user_name"].ToString();
                        string w_user_pswd = w_row["user_pass"].ToString();

                        if (w_user_name == _txtUserName.Text)
                        {
                            if ( w_user_pswd != _txtUserPswd.Text )
                            {
                                InvokeDumpLog("the password is not correct.");
                                w_incorrect_pswd = true;
                                MessageBox.Show("the password is not correct.");
                                break;
                            }

                            w_selRow = w_row;
                            break;
                        }
                    }
                    w_dbMgr.close();

                    if ( w_selRow == null )
                    {
                        if (!w_incorrect_pswd)
                        {
                            InvokeDumpLog("There is not the account for input user.");
                            MessageBox.Show("There is not the account for input user.");
                        }
                        break;
                    }

                    /*
                    string w_dump = JsonConvert.SerializeObject(w_dt, Formatting.Indented);
                    Console.WriteLine(w_dump);
                    InvokeDumpLog(w_dump); //*/
                }
                catch (Exception ex)
                {
                    InvokeDumpLog("Failed to execute to Select Query." + ex.Message);
                    w_dbMgr.close();
                    break;
                }

                //--------------------------------------------
                // Query account info in DB
                //--------------------------------------------
                bool w_connect = ConnectSshTunnel(w_selRow);

                //--------------------------------------------
                // Launch the app.
                //--------------------------------------------
                if (w_connect == true && w_selRow != null)
                {
                    string w_exeCmd = w_selRow["file_path_to_run"].ToString().Trim();

                    if ( !string.IsNullOrEmpty(w_exeCmd) )
                    {
                        executeCommand(w_exeCmd);
                    }
                }

            }
            while (false);

            if ( _sshMgr != null && _sshMgr.ssh_isconnected() )
            {
                SetConnectionState(1);
            }
            else
            {
                SetConnectionState(0);
            }

            Cursor.Current = Cursors.Default;

            InvokeDumpLog("---- END ----");

            return;
        }

        private bool ConnectSshTunnel(DataRow accountInfo)
        {
            bool w_ret = false;

            if (accountInfo == null)
                return false;

            do
            {
                try
                {
                    //--------------------------------------------
                    // Get connection info
                    //--------------------------------------------
                    string w_proxy_server = accountInfo["proxy_ip"].ToString();
                    int w_proxy_port = int.Parse(accountInfo["proxy_port"].ToString());
                    string w_proxy_user = accountInfo["proxy_user"].ToString();
                    string w_ssh_keyfile = accountInfo["ssh_file_path"].ToString();
                    string w_bound_ip = IPAddress.Loopback.ToString();
                    int w_bound_port = int.Parse(accountInfo["local_port"].ToString());
                    string w_host_ip = accountInfo["remote_ip"].ToString();
                    int w_host_port = int.Parse(accountInfo["remote_port"].ToString());

                    //--------------------------------------------
                    // Connect to SSH Tunnel
                    //--------------------------------------------
                    InvokeDumpLog("Connecting to server...");

                    _sshMgr = new SshMgr(w_proxy_server, w_proxy_port, false, w_proxy_user, "", w_ssh_keyfile, w_bound_port, w_host_ip, w_host_port);
                    if ( !_sshMgr.connect_ssh() )
                    {
                        InvokeDumpLog("The Server Connection is Failed.");
                        break;
                    }

                    InvokeDumpLog("The Server Connection is Successed!!!");

                }
                catch (Exception ex)
                {
                    InvokeDumpLog("Your PC is not authorized.");
                    break;
                }

                w_ret = true;
            } while (false);

            return w_ret;
        }

        private void _btnClearLog_Click(object sender, EventArgs e)
        {
            _lstLogs.Items.Clear();
        }

        private bool executeCommand(string command)
        {
            bool w_ret = false;
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = $"cmd.exe";
                startInfo.Arguments = "/C " + command;
                process.StartInfo = startInfo;
                process.Start();

                w_ret = true;

                //x InvokeDumpLog("The Startup Application is Launched!");
            }
            catch (Exception ex)
            {
                InvokeDumpLog("Failed to Launch Application : " + ex.Message);
            }

            return w_ret;
        }

        private delegate void SafeCallDelegate(string text, bool append);
        public void InvokeDumpLog(string text, bool append = false)
        {
            if (_lstLogs.InvokeRequired)
            {
                Invoke(new SafeCallDelegate(InvokeDumpLog), new object[] { text, append });
            }
            else
            {
                //x ListBoxItem w_item = new ListBoxItem(Color.Red, text);
                string w_appendText = text;
                if (append)
                {
                    int w_lastIdx = _lstLogs.Items.Count - 1;
                    if (w_lastIdx >= 0)
                    {
                        string w_prevText = (string)_lstLogs.Items[w_lastIdx];
                        w_appendText = string.Format("{0,-5:#####} {1}", w_prevText, text);
                        _lstLogs.Items[w_lastIdx] = w_appendText;
                    }
                    else
                    {
                        _lstLogs.Items.Add(text);
                    }
                }
                else
                {
                    _lstLogs.Items.Add(text);
                }

                _lstLogs.TopIndex = _lstLogs.Items.Count - 1;
            }
        }

    }
}
