using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsGroupReader.Model
{
    public class NntpReader : StreamReader
    {

        /// <summary>
        /// reads multiple lines from the networkstream
        /// </summary>
        /// <returns>
        /// returns an IEnumerable with all the lines as string 
        /// </returns>
        public IEnumerable<string> ReadAllLines()
        {
            string line;

            while ((line = this.ReadLine()) != null)
            {


                if (line == ".") break;

                if (line.StartsWith(".."))
                    line = line.Substring(1);



                yield return line;
            }
        }


        //helper class to convert readalllines to a list 
        public List<string> ListAllLines()
        {
            List<string> lines = ReadAllLines().ToList();
            return lines;
        }






        public NntpReader(Stream stream) : base(stream)
        {
        }

        public NntpReader(string path) : base(path)
        {
        }

        public NntpReader(Stream stream, bool detectEncodingFromByteOrderMarks) : base(stream, detectEncodingFromByteOrderMarks)
        {
        }

        public NntpReader(Stream stream, Encoding encoding) : base(stream, encoding)
        {
        }

        public NntpReader(string path, FileStreamOptions options) : base(path, options)
        {
        }

        public NntpReader(string path, bool detectEncodingFromByteOrderMarks) : base(path, detectEncodingFromByteOrderMarks)
        {
        }

        public NntpReader(string path, Encoding encoding) : base(path, encoding)
        {
        }

        public NntpReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks) : base(stream, encoding, detectEncodingFromByteOrderMarks)
        {
        }

        public NntpReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks) : base(path, encoding, detectEncodingFromByteOrderMarks)
        {
        }

        public NntpReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize)
        {
        }

        public NntpReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : base(path, encoding, detectEncodingFromByteOrderMarks, bufferSize)
        {
        }

        public NntpReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, FileStreamOptions options) : base(path, encoding, detectEncodingFromByteOrderMarks, options)
        {
        }

        public NntpReader(Stream stream, Encoding? encoding = null, bool detectEncodingFromByteOrderMarks = true, int bufferSize = -1, bool leaveOpen = false) : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen)
        {
        }
    }
}
