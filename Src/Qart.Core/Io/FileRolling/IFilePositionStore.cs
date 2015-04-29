using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Core.Io.FileRolling
{
    public interface IFilePositionStore
    {
        FilePosition GetPosition(string baseFileName);
        void SetPosition(FileId fileId, long pos);
    }
}
