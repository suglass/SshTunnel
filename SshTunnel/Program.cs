using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SshTunnel
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //: Load the DLL from Embedded Resource into Memory. Use EmbeddedAssembly.Load to load it into memory.
            // EmbeddedAssembly.Load("SshTunnel.Lib.BouncyCastle.Crypto.dll", "BouncyCastle.Crypto.dll");
            // EmbeddedAssembly.Load("SshTunnel.Lib.MySql.Data.dll", "MySql.Data.dll");
            // EmbeddedAssembly.Load("SshTunnel.Lib.Newtonsoft.Json.dll", "Newtonsoft.Json.dll");
            // EmbeddedAssembly.Load("SshTunnel.Lib.Renci.SshNet.dll", "Renci.SshNet.dll");
            // EmbeddedAssembly.Load("SshTunnel.Lib.Ubiety.Dns.Core.dll", "Ubiety.Dns.Core.dll");
            // EmbeddedAssembly.Load("SshTunnel.Lib.Google.Protobuf.dll", "Google.Protobuf.dll");

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            //: Original.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }
    }
}
