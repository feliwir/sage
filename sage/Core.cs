using System.IO;
using sage.big;
using sage.vp6;
using System;

namespace sage
{
    public class Core
    {
        private string m_rootDir;

        public Core() : this(Directory.GetCurrentDirectory())
        {
        }

        public Core(string root)
        {
            m_rootDir = root;

            Console.WriteLine("Root directory is:"+ m_rootDir);
            Console.WriteLine("Initializing SAGE...");

            //List all big archives in this folder
            Console.WriteLine("Loading all archives...");
            foreach (string file in Directory.GetFiles(m_rootDir, "*.big", SearchOption.AllDirectories))
            {
                BigArchive arch = new BigArchive(File.Open(file, FileMode.Open));
            }

            //Play all vp6 videos
            Console.WriteLine("Loading all videos...");            
            Demuxer vid = new Demuxer(File.Open("F:/Development/Repositories/sage/test/test.vp6", FileMode.Open,FileAccess.Read,FileShare.Read));


        }
    }
}
