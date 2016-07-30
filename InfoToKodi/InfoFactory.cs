using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace InfoToKodi
{
    public class InfoFactory
    {
        public static Info Get(String Mediafile)
        {
            MediathekInfo mi = new MediathekInfo(Mediafile);
            if (File.Exists(mi.InfoFile))
                return mi;

            DvbViewerInfo di = new DvbViewerInfo(Mediafile);
            if (File.Exists(di.InfoFile))
                return di;

            EmtyInfo ei = new EmtyInfo(Mediafile);
            return ei;
        }
    }

    public abstract class Info
    {
        public Info(String MediaFile)
        {
            this.MediaFile = MediaFile;
        }

        public virtual void genInfos(bool overwrite)
        {
            
        }
        
        public virtual String InfoFile
        {
            get
            {
                return "";
            }
            private set { }
        }

        public virtual String OutputFile
        {
            get
            {
                FileInfo fi = new FileInfo(MediaFile);
                String woExt = fi.FullName.Replace(fi.Extension, "");
                return woExt + ".nfo";
            }
        }

        protected void ReadInfos()
        {
            SourceContent = File.ReadAllText(InfoFile,Encoding.Default);
        }

        protected void WriteNfoFile(tvshow show, bool overwrite)
        {
            if (overwrite)
                Target = new FileStream(OutputFile, FileMode.Create);
            else
                Target = new FileStream(OutputFile, FileMode.CreateNew);

            XmlSerializer xs = new XmlSerializer(typeof(tvshow));
            xs.Serialize(Target, show);

            Target.Close();
        }
                
        protected String MediaFile;
        protected String SourceContent;        
        protected FileStream Target;
    }

    public class EmtyInfo : Info
    {
        public EmtyInfo(string MediaFile) : base(MediaFile)
        {
        }
    }

    public class MediathekInfo : Info
    {
        public MediathekInfo(string MediaFile) : base(MediaFile)
        {
            
        }

        public override void genInfos(bool overwrite)
        {
            //Read old Infos in
            ReadInfos();

            tvshow tv = new tvshow();

            List<String> infos = SourceContent.Split('\n').ToList();
            infos.RemoveAll(delegate (String s)
            {
                return String.IsNullOrWhiteSpace(s);
            });

            tv.studio = infos[0].Replace("Sender:","").Trim();
            tv.title = infos[2].Replace("Titel:", "").Trim();
            tv.aired = infos[3].Split(' ').Last();

            int pos=infos.FindLastIndex(delegate (String s)
            {
                return s.StartsWith("URL");
            });
            tv.plot = "";
            for (int i = pos + 2; i < infos.Count; i++)
            {
                tv.plot += infos[i] + '\n';
            }

            //Write New File
            WriteNfoFile(tv,overwrite);
        }

        public override string InfoFile
        {
            get
            {
                return MediaFile + ".txt";
            }
        }
    }

    public class DvbViewerInfo : Info
    {
        public DvbViewerInfo(string MediaFile) : base(MediaFile)
        {
            
        }

        public override string InfoFile
        {
            get
            {
                FileInfo fi = new FileInfo(MediaFile);
                String woExt = fi.FullName.Replace(fi.Extension, "");
                return woExt + ".txt";
            }
        }
    }    

    [Serializable]
    public class tvshow
    {
        public String title { get; set; }

        public String plot { get; set; }

        public String aired { get; set; }

        public String studio { get; set; }
    }
  
}
