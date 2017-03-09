using System.IO;
using sage.big;
using sage.vp6;

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

            //List all big archives in this folder
            foreach (string file in Directory.GetFiles(m_rootDir, "*.big", SearchOption.AllDirectories))
            {
                BigArchive arch = new BigArchive(File.Open(file, FileMode.Open));
            }

            //Play all vp6 videos
            foreach (string file in Directory.GetFiles(m_rootDir, "*.vp6", SearchOption.AllDirectories))
            {
                Demuxer vid = new Demuxer(File.Open(file, FileMode.Open));
            }
        }
    }
}
