using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Vestcom.PreProcSupport;


namespace Runner
{
   static class Program
    {
        static void Main()
        {
            try
            {
                int jobTaskId = 6;
                int jobDefId = 0;
                string packagePath = ConfigurationManager.AppSettings["FileWatcherPath"];
                List<string> files = Directory.GetFiles(packagePath).Select(x => Path.GetFileName(x)).ToList();
                int realm = (int)RealmTypes.Test;
                IRunnableExtended preproc = new Vestcom.PaperSign.ACME.ProcessInput.ProcessInput();
                //IRunnableExtended preproc = new Vestcom.PaperSign.ACME.ProcessDownback.ProcessDownback();
                //IRunnableExtended preproc = new Vestcom.PaperSign.ACME.Holiday.AcmeHoliday();

                RunSettings settings = GetTestSettings(jobTaskId, jobDefId, packagePath, files, realm, 0);
                Response result = preproc.Run(settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private static RunSettings GetTestSettings(int jobTaskId, int jobDefId, string packagePath, List<string> files, int realm, int configurationId)
        {
            RunSettings settings = new RunSettings(jobTaskId, jobDefId, packagePath, files, realm, configurationId);
            return settings;
        }
    }
}
