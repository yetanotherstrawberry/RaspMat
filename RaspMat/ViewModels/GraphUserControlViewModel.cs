using RaspMat.Models;
using RaspMat.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace RaspMat.ViewModels
{
    internal class GraphUserControlViewModel : ViewModelBase, IDisposable
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
        private DirectedGraph _graph;

        /// <summary>
        /// The <see cref="DirectedGraph"/> that is currently being edited.
        /// </summary>
        public DirectedGraph Graph
        {
            get => _graph;
            private set => SetProperty(ref _graph, value);
        }

        /// <summary>
        /// Nodes of the <see cref="Graph"/> that are currently being edited.
        /// </summary>
        public IEnumerable<Vertex> Vertices => Graph.Vertices;

        public GraphUserControlViewModel(ICommandingService commandingService, IViewService viewService)
        {
            _viewService = viewService;
            _commandingService = commandingService;
            Graph = new DirectedGraph();
            Graph.VertexAdded += VerticesChanged;
            Graph.VertexRemoved += VerticesChanged;
        }

        /// <summary>
        /// Handler for the change of <see cref="DirectedGraph.Vertices"/>.
        /// </summary>
        /// <param name="vertex">The instance that was either added or removed.</param>
        private void VerticesChanged(Vertex vertex) => OnPropertyChanged(nameof(Vertices));

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
                        _viewService.Execute(() => Graph.AddVertex(vertexName));
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
                    _removeVertexCommand = _commandingService.CreateFromAction<Vertex>(action: vertex =>
                    {
                        _viewService.Execute(() => Graph.RemoveVertex(vertex ?? throw new ArgumentNullException(nameof(vertex))));
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
                        _viewService.Execute(() => Graph.AddEdge(LeftVertex, RightVertex));
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
                        _viewService.Execute(() => Graph.RemoveEdge(LeftVertex, RightVertex));
                    });
                }
                return _removeEdgeCommand;
            }
        }

        /// <summary>
        /// Field for <see cref="LeftVertex"/>.
        /// </summary>
        private Vertex _leftVertex;

        /// <summary>
        /// The <see cref="DirectedEdge.Source"/> <see cref="Vertex"/> currently selected by the user.
        /// </summary>
        public Vertex LeftVertex
        {
            get => _leftVertex;
            set => SetProperty(ref _leftVertex, value);
        }

        /// <summary>
        /// Field for <see cref="RightVertex"/>.
        /// </summary>
        private Vertex _rightVertex;

        /// <summary>
        /// The <see cref="DirectedEdge.Target"/> <see cref="Vertex"/> currently selected by the user.
        /// </summary>
        public Vertex RightVertex
        {
            get => _rightVertex;
            set => SetProperty(ref _rightVertex, value);
        }

        public void Dispose()
        {
            Graph.VertexAdded -= VerticesChanged;
            Graph.VertexRemoved -= VerticesChanged;
        }

    }
}
