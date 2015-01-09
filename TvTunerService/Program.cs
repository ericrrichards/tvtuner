using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TvTunerService {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args) {
            if (args.Length > 0) {
                var svc = new Service1();
                svc.Start();

                Console.WriteLine("Press enter to terminate...");
                Console.ReadLine();
                svc.End();

            }
            else {
                var servicesToRun = new ServiceBase[] {
                    new Service1()
                };
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
