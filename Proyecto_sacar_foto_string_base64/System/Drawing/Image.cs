using System.IO;

namespace System.Drawing
{
    internal class Image
    {
        internal readonly object RawFormat;

        internal class FromFile : Image
        {
            private string imagePath;

            public FromFile(string imagePath)
            {
                this.imagePath = imagePath;
            }
        }
    }
}