using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using System.Threading;
using System.Threading.Tasks;

namespace RaspMat.Views
{
    /// <summary>
    /// Interaction logic for GraphUserControl.xaml
    /// </summary>
    internal partial class GraphUserControl : UserControlBase
    {

        /// <summary>
        /// Initializes XAML.
        /// </summary>
        public GraphUserControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            spanel.Children.Clear();
            var graph = new Graph();
            graph.AddNode("k");
            var msagl = new AutomaticGraphLayoutControl()
            {
                Graph = graph,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
            };
            spanel.Children.Add(msagl);
        }

    }
}
