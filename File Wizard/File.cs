using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_Wizard
{
    public class File
    {
        public string FileName { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; }
        public string DirectoryName { get; set; }
        public bool IsReadOnly { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime ModifiedTime { get; set; }
        public DateTime AccessedTime { get; set; }
        public Int64 FileSize { get; set; }
    }
}
