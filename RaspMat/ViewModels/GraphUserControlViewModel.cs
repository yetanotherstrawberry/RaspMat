using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using RaspMat.Helpers;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Threading;

namespace RaspMat.ViewModels
{

    internal class GraphUserControlViewModel : ViewModelBase
    {

        private Graph _graph = new Graph();
        public Graph Graph
        {
            get => _graph;
            set
            {
                SetProperty(ref _graph, value);
            }
        }

        public IEnumerable<Node> Nodes => Graph.Nodes;

        public GraphUserControlViewModel()
        {
            _graph.AddNode("C");
        }

        /// <summary>
        /// Adds a <see cref="Node"/>. 
        /// </summary>
        public ICommand AddNode
        {
            get
            {
                if (_addNode == null)
                {
                    _addNode = ICommandHelpers.CreateAsyncICommand(action: () =>
                    {
                        Dispatcher.CurrentDispatcher.Invoke(() =>
                        {
                            var temp = Graph;
                            Graph = null;
                            temp.AddNode("B");
                            Graph = temp;
                        });
                        //                        OnPropertyChanged("Graph");
                    });
                }

                return _addNode;
            }
        }

        /// <summary>
        /// Field for <see cref="AddNode" />.
        /// </summary>
        private ICommand _addNode;

    }
}
