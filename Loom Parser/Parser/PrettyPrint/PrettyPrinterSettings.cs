using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom_Parser.Parser.PrettyPrint
{
    internal class PrettyPrinterSettings
    {
        /// <summary>
        /// The indentation applied by the code 
        /// </summary>
        public string Indentation = "    ";

        /// <summary>
        /// Determines if the script should be minified
        /// </summary>
        public bool Minify = false;
    }
}
