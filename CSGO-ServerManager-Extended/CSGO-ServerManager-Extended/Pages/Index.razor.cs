using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGO_ServerManager_Extended.Pages
{
    partial class Index
    {
        public Servers Servers { get; set; }
        public List<ComponentBase> RenderFragments = new();

        void test()
        {
            RenderFragments.Add(Servers);
        }
    }
}
