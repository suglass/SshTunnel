using SshTunnel;
using MySql.Data.MySqlClient;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace SshTunnel
{
    public class DbMgr
    {
        protected readonly string _database;
        protected readonly string _server;
        protected readonly int _port;
        protected readonly string _uid;
        protected readonly string _password;
        protected readonly bool _ssh;
        protected readonly string _ssh_server;
        protected readonly int _ssh_port;
        protected readonly string _ssh_uid;
        protected readonly string _ssh_password;
        protected readonly string _ssh_keyfile;

        protected ForwardedPortLocal portFwld = null;
        protected SshClient _ssh_client = null;


        public DbMgr(string database, string server, int port, string uid, string password, bool ssh = false, string ssh_server = "", int ssh_port = 22, string ssh_uid = "", string ssh_password = "", string ssh_keyfile = "")
        {
            _database = database;
            _server = server;
            _port = port;
            _uid = uid;
            _password = password;
            _ssh = ssh;
            _ssh_server = ssh_server;
            _ssh_port = ssh_port;
            _ssh_uid = ssh_uid;
            _ssh_password = ssh_password;
            _ssh_keyfile = ssh_keyfile;
        }
        protected bool connect_ssh()
        {
            if (!_ssh)
                return true;

            List<AuthenticationMethod> methods = new List<AuthenticationMethod>();

            if (_ssh_password != "")
                methods.Add(new PasswordAuthenticationMethod(_ssh_uid, _ssh_password));

            if (_ssh_keyfile != "")
            {
                Stream w_keyStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SshTunnel.arpan.ppk");
                var keyFile = new PrivateKeyFile(w_keyStream);
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
            //x portFwld = new ForwardedPortLocal(IPAddress.Loopback.ToString(), "localhost", 3306);

            portFwld = new ForwardedPortLocal(IPAddress.Loopback.ToString(), 3306, _server, 3306);
            _ssh_client.AddForwardedPort(portFwld);
            portFwld.Start();
            if (!portFwld.IsStarted)
                return false;

            return true;
        }
        public bool connectable()
        {
            bool ret = false;
            using (MySqlConnection connection = connect())
            {
                try
                {
                    connection.Open();
                    ret = (connection.State == ConnectionState.Open);
                }
                catch (Exception exception)
                {
                    //x MyLogger.Error($"Exception Error ({System.Reflection.MethodBase.GetCurrentMethod().Name}): {exception.Message + "\n" + exception.StackTrace}");
                    Console.WriteLine($"Exception Error ({System.Reflection.MethodBase.GetCurrentMethod().Name}): {exception.Message + "\n" + exception.StackTrace}");
                    return false;
                }
            }
            return ret;
        }
        public MySqlConnection connect()
        {
            if (_ssh)
            {
                if (portFwld == null || !portFwld.IsStarted)
                    connect_ssh();
            }

            string connection_string;
            if (!_ssh)
                connection_string = String.Format("server={0};port={1};database={2};uid={3};password={4}", _server, _port, _database, _uid, _password);
            else
                connection_string = String.Format("server={0};database={1};uid={2};password={3};port={4}", portFwld.BoundHost, _database, _uid, _password, portFwld.BoundPort);

            //x MessageBox.Show(connection_string);
            return new MySqlConnection(connection_string);
        }
        public void close()
        {
            if ( portFwld != null && portFwld.IsStarted )
            {
                portFwld.Stop();
                portFwld = null;
            }

            return;
        }
        public void execute_sql(string sql, Dictionary<string, object> cmd_params)
        {
            //MyLogger.Info($"... DB execute_sql : {sql}");
            using (MySqlConnection connection = connect())
            {
                connection.Open();
                using (MySqlTransaction tr = connection.BeginTransaction())
                {
                    try
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = connection;
                            cmd.CommandText = sql;
                            cmd.CommandTimeout = 600;
                            if (cmd_params != null)
                            {
                                foreach (KeyValuePair<string, object> p in cmd_params)
                                    cmd.Parameters.AddWithValue(p.Key, p.Value);
                            }
                            cmd.ExecuteNonQuery();

                            tr.Commit();
                        }
                    }
                    catch (MySqlException exception)
                    {
                        Console.WriteLine($"Exception Error ({System.Reflection.MethodBase.GetCurrentMethod().Name})\n    Message : {exception.Message}\n    Number : {exception.Number}\n    {exception.StackTrace}");

                        try
                        {
                            tr.Rollback();
                        }
                        catch (MySqlException exception1)
                        {
                            Console.WriteLine($"Rollback Exception Error ({System.Reflection.MethodBase.GetCurrentMethod().Name})\n    Message : {exception1.Message}\n    Number : {exception1.Number}\n    {exception1.StackTrace}");
                        }

                        throw exception;
                    }
                }
            }
        }
        public int insert_sql(string sql, Dictionary<string, object> cmd_params)
        {
            int last_inserted_id = -1;

            //MyLogger.Info($"... DB insert_sql : {sql}");
            using (MySqlConnection connection = connect())
            {
                connection.Open();
                using (MySqlTransaction tr = connection.BeginTransaction())
                {
                    try
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = connection;
                            cmd.CommandText = sql;
                            cmd.CommandTimeout = 600;
                            if (cmd_params != null)
                            {
                                foreach (KeyValuePair<string, object> p in cmd_params)
                                    cmd.Parameters.AddWithValue(p.Key, p.Value);
                            }
                            cmd.ExecuteNonQuery();

                            last_inserted_id = (int)cmd.LastInsertedId;

                            tr.Commit();
                        }
                    }
                    catch (MySqlException exception)
                    {
                        Console.WriteLine($"Exception Error ({System.Reflection.MethodBase.GetCurrentMethod().Name})\n    Message : {exception.Message}\n    Number : {exception.Number}\n    {exception.StackTrace}");

                        try
                        {
                            tr.Rollback();
                        }
                        catch (MySqlException exception1)
                        {
                            Console.WriteLine($"Rollback Exception Error ({System.Reflection.MethodBase.GetCurrentMethod().Name})\n    Message : {exception1.Message}\n    Number : {exception1.Number}\n    {exception1.StackTrace}");
                        }
                        last_inserted_id = -1;

                        throw exception;
                    }
                }
            }
            return last_inserted_id;
        }
        public DataTable select(string sql, Dictionary<string, object> cmd_params)
        {
            //MyLogger.Info($"... DB SELECT : {sql}");
            var dt = new DataTable();
            using (MySqlConnection connection = connect())
            {
                using (var da = new MySqlDataAdapter(sql, connection))
                {
                    da.SelectCommand.CommandTimeout = 600;
                    if (cmd_params != null)
                    {
                        foreach (KeyValuePair<string, object> p in cmd_params)
                            da.SelectCommand.Parameters.AddWithValue(p.Key, p.Value);
                    }
                    try
                    {
                        connection.Open(); // not necessarily needed in this case because DataAdapter.Fill does it otherwise 
                        da.Fill(dt);
                    }
                    catch (MySqlException exception)
                    {
                        Console.WriteLine($"Exception Error ({System.Reflection.MethodBase.GetCurrentMethod().Name})\n    Message : {exception.Message}\n    Number : {exception.Number}\n    {exception.StackTrace}");

                        throw exception;
                    }
                }
            }
            return dt;
        }


    }
}
