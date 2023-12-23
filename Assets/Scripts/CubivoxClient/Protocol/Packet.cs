using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubivoxClient.Protocol
{
    public interface Packet
    {
        byte GetType();
    }
}
