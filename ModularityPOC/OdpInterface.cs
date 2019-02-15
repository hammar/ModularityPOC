using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModularityPOC
{
    class OdpInterface
    {
        private string label;

        public OdpInterface(string label)
        {
            this.Label = label ?? throw new ArgumentNullException(nameof(label));
        }

        public string Label { get => label; set => label = value; }

        public override string ToString()
        {
            return this.Label;
        }
    }
}
