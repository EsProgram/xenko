// Copyright (c) 2014-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.
using System;
using System.Collections.Generic;
using SiliconStudio.Core.Annotations;
using SiliconStudio.Presentation.Quantum.Presenters;
using SiliconStudio.Presentation.Quantum.ViewModels;
using SiliconStudio.Quantum;

namespace SiliconStudio.Presentation.Quantum
{
    /// <summary>
    /// A class that provides various services to <see cref="GraphViewModel"/> objects
    /// </summary>
    public class GraphViewModelService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphViewModelService"/> class.
        /// </summary>
        public GraphViewModelService([NotNull] NodeContainer nodeContainer)
        {
            if (nodeContainer == null) throw new ArgumentNullException(nameof(nodeContainer));
            NodePresenterFactory = new NodePresenterFactory(nodeContainer.NodeBuilder, AvailableCommands, AvailableUpdaters);
            NodeViewModelFactory = new NodeViewModelFactory();
        }

        public INodePresenterFactory NodePresenterFactory { get; set; }

        public NodeViewModelFactory NodeViewModelFactory { get; set; }

        public List<INodePresenterCommand> AvailableCommands { get; } = new List<INodePresenterCommand>();

        public List<INodePresenterUpdater> AvailableUpdaters { get; } = new List<INodePresenterUpdater>();
    }
}