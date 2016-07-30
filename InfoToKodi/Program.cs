using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoToKodi
{
    class Program
    {
        static void Main(string[] args)
        {
            worker1.genList(Properties.Settings.Default.SearchDir.Split(';').ToList(),
                Properties.Settings.Default.Extensions.Split(';').ToList());

            worker1.genInfos(Properties.Settings.Default.Overwrite);

        }

        static Worker worker1 = new Worker();
    }

    public class Worker
    {
        public void genList(List<String> Dir,List<String> Ext)
        {
            files = new List<string>();

            foreach (String dir in Dir)
            {
                searchForFiles(dir,Ext);
            }

            MediaFiles = new List<Info>();
            foreach (String s in files)
                MediaFiles.Add(InfoFactory.Get(s));
        }

        public void genInfos(bool overwrite)
        {
            foreach (Info i in MediaFiles)
                i.genInfos(overwrite);
        }


        private List<String> files;
        private List<Info> MediaFiles;

        private void searchForFiles(String dir,List<String> ext)
        {
            foreach (String extension in ext)
                files.AddRange(Directory.GetFiles(dir, extension));

            foreach (string pdir in Directory.GetDirectories(dir))
                searchForFiles(pdir,ext);
        }
    }
}
