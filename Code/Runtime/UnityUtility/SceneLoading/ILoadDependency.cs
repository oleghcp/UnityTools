using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityUtility.SceneLoading
{
    public interface ILoadDependency
    {
        bool Done { get; }
    }
}
