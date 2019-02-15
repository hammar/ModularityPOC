using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using VDS.RDF;
using VDS.RDF.Ontology;
using VDS.RDF.Parsing;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ModularityPOC
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        ObservableCollection<OdpInterface> requiredInterfaces = new ObservableCollection<OdpInterface>();
        ObservableCollection<OdpInterface> implementedInterfaces = new ObservableCollection<OdpInterface>();
        ObservableCollection<Odp> odps = new ObservableCollection<Odp>();

        internal ObservableCollection<Odp> Odps { get => odps; set => odps = value; }
        internal ObservableCollection<OdpInterface> ImplementedInterfaces { get => implementedInterfaces; set => implementedInterfaces = value; }
        internal ObservableCollection<OdpInterface> RequiredInterfaces { get => requiredInterfaces; set => requiredInterfaces = value; }



        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ReindexButton_Click(object sender, RoutedEventArgs e)
        {
            string interfaceNamespace = "https://ontologydesignpatterns.org/interfaces#";

            var executingAssembly = Assembly.GetExecutingAssembly();
            string folderName = string.Format("{0}.odps.patterns", executingAssembly.GetName().Name);
            Console.WriteLine(folderName);
            String[] patterns = executingAssembly.GetManifestResourceNames().Where(r => r.StartsWith(folderName) && r.EndsWith(".ttl")).ToArray();

            // Index patterns
            foreach (string pattern in patterns) {

                OntologyGraph odpGraph = new OntologyGraph();
                EmbeddedResourceLoader.Load(odpGraph, String.Format("{0}, {1}", pattern, executingAssembly.GetName().Name));// "listOntologyContents.rec_full.owl, listOntologyContents");
                string label = pattern.Remove(0, folderName.Length + 1);

                Odp newOdp = new Odp(label);

                HashSet<OdpInterface> implementedInterfaces = new HashSet<OdpInterface>();
                HashSet<OdpInterface> requiredInterfaces = new HashSet<OdpInterface>();

                foreach (OntologyClass c in odpGraph.OwlClasses)
                {
                    foreach (OntologyClass sc in c.DirectSuperClasses)
                    {
                        String scIri = sc.Resource.ToString();
                        if (scIri.Contains(interfaceNamespace))
                        {
                            OdpInterface implementedInterface = new OdpInterface(scIri.Remove(0, interfaceNamespace.Length));
                            implementedInterfaces.Add(implementedInterface);
                        }
                    }
                }
                foreach (OntologyProperty op in odpGraph.OwlObjectProperties)
                {
                    foreach (OntologyClass range in op.Ranges)
                    {
                        String rangeIri = range.Resource.ToString();
                        if (rangeIri.Contains(interfaceNamespace))
                        {
                            OdpInterface requiredInterface = new OdpInterface(rangeIri.Remove(0, interfaceNamespace.Length));
                            requiredInterfaces.Add(requiredInterface);
                        }
                    }
                }

                newOdp.ImplementedInterfaces = implementedInterfaces.ToList();
                newOdp.RequiredInterfaces = requiredInterfaces.ToList();
                Odps.Add(newOdp);
            }   
        }

        private static string GetLabel(OntologyResource ontologyResource, string language)
        {
            foreach (ILiteralNode label in ontologyResource.Label)
            {
                if (label.Language == language)
                {
                    return label.Value;
                }
            }
            return ontologyResource.Resource.ToString();
        }

        private void AvailablePatternsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Odp selectedOdp = (Odp)availablePatternsList.SelectedItem;
            ImplementedInterfaces.Clear();
            selectedOdp.ImplementedInterfaces.ForEach(ImplementedInterfaces.Add);

            RequiredInterfaces.Clear();
            selectedOdp.RequiredInterfaces.ForEach(RequiredInterfaces.Add);
        }
    }
}
