using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Core.Io.FileRolling
{
    public class RollingFileTextReader : TextReader
    {
        private RollingFileReader _reader;

        public RollingFileTextReader(RollingFileReader reader)
        {
            _reader = reader;
        }

        //public virtual void Close();
        //public void Dispose();

        //protected virtual void Dispose(bool disposing)
        //public virtual int Peek();
        //public virtual int Read();
        //public virtual int Read(char[] buffer, int index, int count);
        //public virtual Task<int> ReadAsync(char[] buffer, int index, int count);
        //public virtual int ReadBlock(char[] buffer, int index, int count);
        //public virtual Task<int> ReadBlockAsync(char[] buffer, int index, int count);
        
        //public virtual Task<string> ReadLineAsync();
        //public virtual string ReadToEnd();
        //public virtual Task<string> ReadToEndAsync();
        //public static TextReader Synchronized(TextReader reader);

        const int BUFFER_LENGTH = 1000;
        byte[] _byteBuffer = new byte[BUFFER_LENGTH];
        int currentPos;
        int currentLen;
        StringBuilder _sb = new StringBuilder();

        public override string ReadLine()
        {
            bool bContinue = true;
            
            while(bContinue)
            {
                bContinue = false;
                if (currentPos == currentLen)
                {
                    currentLen = _reader.Read(_byteBuffer, 0, BUFFER_LENGTH);
                    currentPos = 0;
                }

                if (currentPos == currentLen)
                    break;

                int pos = -1;
                for (int i = currentPos; i != currentLen; ++i)
                {
                    if (_byteBuffer[i] == '\n') //TODO Unix/Mac style
                    {
                        pos = i;
                        break;
                    }
                }

                char[] chars;
                if (pos == -1)
                {
                    bContinue = true;
                    chars = ASCIIEncoding.UTF8.GetChars(_byteBuffer, currentPos, currentLen - currentPos);
                    _sb.Append(chars);
                    currentPos = currentLen;
                }
                else
                {                    
                    chars = ASCIIEncoding.UTF8.GetChars(_byteBuffer, currentPos, pos - currentPos);
                    _sb.Append(chars);
                    var line = _sb.ToString();
                    _sb.Clear();
                    currentPos = pos + 1;//TODO \n\r
                    return line;
                }
            }
            return null;
        }

        public void Ack()
        {
            _reader.Ack();//TODO ack only full line
        }

        public void RollBack()
        {
            _reader.RollBack();
        }
    }
}
