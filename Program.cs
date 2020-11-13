using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Samples.Debugging.MdbgEngine;
using Microsoft.Samples.Debugging.CorDebug;

/*

NoJit - written by mr_me

# about:

Creates config files for all loaded assemblies found in a running process to disable JIT so that dnspy will work correctly!

# dependencies:

1. Install-Package Microsoft.Samples.Debugging.MdbgEngine -Version 1.4.0

*/
namespace NoJit
{
    class Program
    {

        static void Main(string[] args)
        {
            if (args.Length != 1) {
                Console.WriteLine(string.Format("(+) Usage: {0} <pid>", System.AppDomain.CurrentDomain.FriendlyName));
                return;
            }

            string nojit = @"[.NET Framework Debugging Control]
GenerateTrackingInfo=1
AllowOptimize=0";

            int pid = int.Parse(args[0]);
            MDbgEngine _engine = new MDbgEngine();
            MDbgProcess proc = null;
            
            // attach
            try {
                proc = _engine.Attach(pid, RuntimeEnvironment.GetSystemVersion());
            }
            catch (Exception) {
                Console.WriteLine("(-) check your pid!");
                return;
            }

            proc.Go().WaitOne();

            // find all AppDomains
            foreach (MDbgAppDomain appDomain in proc.AppDomains){

                // get Assemblies for each AppDomain
                foreach (CorAssembly assembly in appDomain.CorAppDomain.Assemblies){
                    string filename = null;
                    if (assembly.Name.EndsWith(".dll")) {
                        filename = assembly.Name.Replace(".dll", ".ini");
                    } else if (assembly.Name.EndsWith(".DLL")) {
                        filename = assembly.Name.Replace(".DLL", ".ini");
                    }

                    // patch config so that jit is disabled
                    if (!String.IsNullOrEmpty(filename))
                        File.WriteAllText(filename, nojit);

                }
            }
            proc.Detach();
        }
    }
}
