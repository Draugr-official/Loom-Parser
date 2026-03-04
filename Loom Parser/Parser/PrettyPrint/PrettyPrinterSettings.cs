using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.PrettyPrint
{
    internal class PrettyPrinterSettings
    {
        /// <summary>
        /// The indentation applied by the code 
        /// </summary>
        public string Indentation { get; set; }

        /// <summary>
        /// The newline applied to the code
        /// </summary>
        public string NewLine { get; set; }

        public PrettyPrinterSettings(string indentation, string newLine)
        {
            Indentation = indentation;
            NewLine = newLine;
        }

        public static PrettyPrinterSettings Minify = new PrettyPrinterSettings("",
            " ");

        public static PrettyPrinterSettings Beautify = new PrettyPrinterSettings("\t",
            Environment.NewLine);
    }
}
