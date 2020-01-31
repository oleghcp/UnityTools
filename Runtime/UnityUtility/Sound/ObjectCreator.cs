using UU.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UU.Sound
{
    public interface ObjectCreator<T> where T : class, Poolable
    {
        T Create();
    }
}
