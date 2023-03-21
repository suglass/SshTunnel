using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace SshTunnel
{
    class SshMgr
    {
        private readonly string _ssh_server = "";
        private readonly int _ssh_port = 22;
        private readonly bool _use_ssh_password = false;
        private readonly string _ssh_uid = "";
        private readonly string _ssh_password = "";
        private readonly string _ssh_keyfile = "";
        private readonly int _bound_port = 0;
        private readonly string _host_ip = "";
        private readonly int _host_port = 0;

        private SshClient _ssh_client = null;

        public SshMgr(string ssh_server, int ssh_port, bool use_pswd, string ssh_uid, string ssh_pswd, string ssh_keyfile, int bound_prot, string host_ip, int host_port)
        {
            _ssh_server = ssh_server;
            _ssh_port = ssh_port;
            _use_ssh_password = use_pswd;
            _ssh_uid = ssh_uid;
            _ssh_password = ssh_pswd;
            _ssh_keyfile = ssh_keyfile;
            _bound_port = bound_prot;
            _host_ip = host_ip;
            _host_port = host_port;           
        }

        public bool connect_ssh()
        {
            ForwardedPortLocal portFwld = null;

            List<AuthenticationMethod> methods = new List<AuthenticationMethod>();

            if (_ssh_password != "")
                methods.Add(new PasswordAuthenticationMethod(_ssh_uid, _ssh_password));

            if (_ssh_keyfile != "")
            {
                var keyFile = new PrivateKeyFile(_ssh_keyfile);
                var keyFiles = new[] { keyFile };
                methods.Add(new PrivateKeyAuthenticationMethod(_ssh_uid, keyFiles));
            }

            ConnectionInfo connectionInfo = new ConnectionInfo(_ssh_server, _ssh_port, _ssh_uid, methods.ToArray());
            connectionInfo.Timeout = TimeSpan.FromSeconds(1000);

            /*
            * It works fine for SSH user/password.
            * 
                    PasswordConnectionInfo connectionInfo = new PasswordConnectionInfo(_ssh_server, _ssh_uid, _ssh_password);
                    connectionInfo.Timeout = TimeSpan.FromSeconds(5);
            */
            _ssh_client = new SshClient(connectionInfo);
            _ssh_client.Connect();
            if (!_ssh_client.IsConnected)
                throw new Exception("SSH connection is inactive");
            //portFwld = new ForwardedPortLocal("127.0.0.1"/*your computer ip*/, _server /*server ip*/, 3306 /*server mysql port*/);
            portFwld = new ForwardedPortLocal(IPAddress.Loopback.ToString(), (uint)_bound_port, _host_ip, (uint)_host_port);
            _ssh_client.AddForwardedPort(portFwld);

            portFwld.RequestReceived += delegate (object sender, PortForwardEventArgs e)
            {
                Console.WriteLine(e.OriginatorHost + ":" + e.OriginatorPort);
            };

            portFwld.Start();
            if (!portFwld.IsStarted)
                return false;

            return true;
        }

        public bool ssh_isconnected()
        {
            if (_ssh_client == null)
                return false;

            return _ssh_client.IsConnected;
        }


        public bool disconnect_ssh()
        {
            if (_ssh_client == null)
                return true;

            if (_ssh_client.IsConnected)
            {
                _ssh_client.Disconnect();
            }

            _ssh_client = null;

            return true;
        }
    }
}
