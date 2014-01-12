using System;
using System.IO;

namespace GateDiff
{
    class DiffFile : IDisposable
    {
        public String m_temporary;
        public FileInfo m_file;

        public DiffFile(FileInfo file)
        {
            m_file = file;
            if (!file.Exists)
            {
                m_temporary = System.IO.Path.GetTempFileName();
            }
        }

        public string Path
        {
            get { return m_temporary ?? m_file.FullName; }
        }

        public void Dispose()
        {
            if (m_temporary != null)
            {
                System.IO.File.Delete(m_temporary);
                m_temporary = null;
            }
        }
    }
}
