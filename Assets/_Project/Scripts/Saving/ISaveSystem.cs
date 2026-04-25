using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Project.Scripts.Saving
{
    public interface ISaveSystem
    {
        void Save<T>(T data, string fileName);
        T Load<T>(string fileName) where T : new();
        bool Exists(string fileName);
    }
}
