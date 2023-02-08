using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T9Project
{
    public interface IDatabaseService
    {
        public Task SearchText(string value);
    }
}
