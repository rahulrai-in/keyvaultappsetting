using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyVaultAppSetting
{
    using System.Configuration;

    class Program
    {
        static void Main(string[] args)
        {
            var value = ConfigurationManager.AppSettings["Key"];
        }
    }
}
