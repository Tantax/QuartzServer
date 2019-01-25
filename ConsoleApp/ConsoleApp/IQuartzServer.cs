using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public interface IQuartzServer
    {
        Task Initialize();

        void Start();

        void Stop();

        void Pause();

        void Resume();
    }
}
