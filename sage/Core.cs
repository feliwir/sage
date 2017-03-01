using System;
using System.IO;
using sage.big;

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
            foreach(string file in Directory.GetFiles(m_rootDir,"*.big",SearchOption.AllDirectories))
            {
                BigArchive arch = new BigArchive(File.Open(file, FileMode.Open));
                foreach(var e in arch.Entries)
                {
                    Console.WriteLine(e.FullName);
                }
            }
        }
    }
}
