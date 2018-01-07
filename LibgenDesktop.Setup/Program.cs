using System;

namespace LibgenDesktop.Setup
{
    internal class Program
    {
        static int Main()
        {
            try
            {
                AppSetup.Build32BitSetupPackage();
                AppSetup.Build64BitSetupPackage();
                AppPortable.Build32BitPortablePackage();
                AppPortable.Build64BitPortablePackage();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                return -1;
            }
            return 0;
        }
    }
}