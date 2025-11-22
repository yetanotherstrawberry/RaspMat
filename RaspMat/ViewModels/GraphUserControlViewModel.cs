
using Microsoft.Msagl.Drawing;
using RaspMat.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace RaspMat.ViewModels
{
    internal class GraphUserControlViewModel : ViewModelBase
    {

        /// <summary>
        /// 
        /// </summary>
        private readonly ICommandingService _commandingService;

        /// <summary>
        /// 
        /// </summary>
        private readonly IViewService _viewService;

        /// <summary>
        /// Field for <see cref="Graph"/>.
        /// </summary>
        private Graph _graph;

        /// <summary>
        /// The <see cref="DirectedGraph"/> that is currently being edited.
        /// </summary>
        public Graph Graph
        {
            get => _graph;
            private set => SetProperty(ref _graph, value);
        }

        public GraphUserControlViewModel(ICommandingService commandingService, IViewService viewService)
        {
            _viewService = viewService;
            _commandingService = commandingService;
            var graph = new Graph();
            graph.AddNode("a");
            graph.AddNode("b");
            graph.AddNode("c");
            graph.AddEdge("a", "TEST", "b");
            Graph = graph;
        }

        /// <summary>
        /// Field for <see cref="AddVertexCommand"/>.
        /// </summary>
        private ICommand _addVertexCommand;

        /// <summary>
        /// <see cref="ICommand"/> for adding a <see cref="Vertex"/> to the <see cref="Graph"/>.
        /// </summary>
        public ICommand AddVertexCommand
        {
            get
            {
                if (_addVertexCommand is null)
                {
                    _addVertexCommand = _commandingService.CreateFromAction<string>(action: vertexName =>
                    {
                        _viewService.Execute(() => Graph.AddEdge("c", "test2", "a"));
                    });
                }
                return _addVertexCommand;
            }
        }

        /// <summary>
        /// Field for <see cref="RemoveVertexCommand"/>.
        /// </summary>
        private ICommand _removeVertexCommand;

        /// <summary>
        /// <see cref="ICommand"/> for the removal of a <see cref="Vertex"/> from the <see cref="Graph"/>.
        /// </summary>
        public ICommand RemoveVertexCommand
        {
            get
            {
                if (_removeVertexCommand is null)
                {
                    _removeVertexCommand = _commandingService.CreateFromAction<object>(action: vertex =>
                    {
                        //_viewService.Execute(() => Graph.RemoveVertex(vertex ?? throw new ArgumentNullException(nameof(vertex))));
                    });
                }
                return _removeVertexCommand;
            }
        }

        /// <summary>
        /// Field for <see cref="AddEdgeCommand"/>.
        /// </summary>
        private ICommand _addEdgeCommand;

        /// <summary>
        /// <see cref="ICommand"/> for adding a <see langword="new"/> <see cref="DirectedEdge"/>.
        /// </summary>
        public ICommand AddEdgeCommand
        {
            get
            {
                if (_addEdgeCommand is null)
                {
                    _addEdgeCommand = _commandingService.CreateFromAction(action: () =>
                    {
                        //_viewService.Execute(() => Graph.AddEdge(LeftVertex, RightVertex));
                    });
                }
                return _addEdgeCommand;
            }
        }

        /// <summary>
        /// Field for <see cref="RemoveEdgeCommand"/>.
        /// </summary>
        private ICommand _removeEdgeCommand;

        /// <summary>
        /// <see cref="ICommand"/> for removing a <see cref="DirectedEdge"/>.
        /// </summary>
        public ICommand RemoveEdgeCommand
        {
            get
            {
                if (_removeEdgeCommand is null)
                {
                    _removeEdgeCommand = _commandingService.CreateFromAction(action: () =>
                    {
                        //_viewService.Execute(() => Graph.RemoveEdge(LeftVertex, RightVertex));
                    });
                }
                return _removeEdgeCommand;
            }
        }

    }
}
