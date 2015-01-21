using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;

namespace transcode {
    class Program {
        [STAThread]
        static void Main(string[] args) {

            var opener = new OpenFileDialog();
            var saver = new SaveFileDialog();

            opener.ShowDialog();
            var original = opener.FileName;

            saver.ShowDialog();
            var converted = saver.FileName;

            //MediaToolkit(original, converted);
            Handbrake(original, converted);
            Console.ReadLine();

        }

        private static void Handbrake(string original, string converted) {
            var proc = new ProcessStartInfo("HandBrakeCLI.exe") {
                Arguments = string.Format("-i \"{0}\" -o \"{1}\" --preset=\"Normal\"", original, converted)
            };
            var p = Process.Start(proc);


            p.WaitForExit();
        }

        private static void ConvertToMp4(string original, string converted) {
            var input = new MediaFile(original);
            var output = new MediaFile(converted);
            using (var engine = new Engine()) {
                engine.ConvertProgressEvent += (sender, e) => {
                    var prc = e.ProcessedDuration.TotalSeconds/e.TotalDuration.TotalSeconds*100;
                    Console.WriteLine(prc);
                };
                engine.Convert(input, output);
            }
        }
    }
}
