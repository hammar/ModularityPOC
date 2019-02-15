using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModularityPOC
{
    class Odp
    {
        private string label;
        private List<OdpInterface> implementedInterfaces;
        private List<OdpInterface> requiredInterfaces;

        public Odp(string label)
        {
            this.Label = label ?? throw new ArgumentNullException(nameof(label));
        }

        public string Label { get => label; set => label = value; }
        internal List<OdpInterface> ImplementedInterfaces { get => implementedInterfaces; set => implementedInterfaces = value; }
        internal List<OdpInterface> RequiredInterfaces { get => requiredInterfaces; set => requiredInterfaces = value; }

        public override string ToString() {
            return this.Label;
        }
    }
}
