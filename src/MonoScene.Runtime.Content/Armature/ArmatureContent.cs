﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoScene.Graphics.Content
{
    /// <summary>
    /// Represents a collection of hierarchically connected <see cref="NodeContent"/> elements.
    /// </summary>
    public class ArmatureContent
    {
        #region lifecycle

        /// <summary>
        /// Creates an armature from an array of <see cref="NodeContent"/>
        /// </summary>
        /// <param name="nodes">a flattened array of <see cref="NodeContent"/> objects.</param>
        /// <param name="atracks">animation tracks info</param>
        public ArmatureContent(NodeContent[] nodes, AnimationTrackInfo[] atracks)
        {
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));

            // check that child nodes always follow parent nodes

            for(int i=0; i < nodes.Length; ++i)
            {
                var n = nodes[i];

                if (n == null) throw new ArgumentNullException(nameof(nodes));                
                if (n.ParentIndex >= i) throw new ArgumentOutOfRangeException(nameof(nodes), $"[{i}].ParentIndex must be lower than {i}, but found {n.ParentIndex}");

                for(int j=0; j < n.ChildIndices.Count; ++j)
                {
                    var cidx = n.ChildIndices[j];
                    if (cidx >= nodes.Length) throw new ArgumentOutOfRangeException(nameof(nodes), $"[{i}].ChildIndices[{j}] must be lower than {nodes.Length}, but found {cidx}");
                    if (cidx <= i) throw new ArgumentOutOfRangeException(nameof(nodes), $"[{i}].ChildIndices[{j}] must be heigher than {i}, but found {cidx}");
                }                
            }

            _NodeTemplates = nodes;
            _AnimationTracks = atracks ?? (new AnimationTrackInfo[0]);
        }

        #endregion

        #region data

        private readonly NodeContent[] _NodeTemplates;
        private readonly AnimationTrackInfo[] _AnimationTracks;

        #endregion

        #region properties

        public int Count => _NodeTemplates.Length;

        public NodeContent this[int index] => _NodeTemplates[index];

        public IReadOnlyList<AnimationTrackInfo> Animations => _AnimationTracks;

        #endregion

        #region API

        public int IndexOfTrack(string name)
        {
            return Array.FindIndex(_AnimationTracks, item => item.Name == name);
        }

        public float GetTrackDuration(int trackLogicalIndex)
        {            
            if (trackLogicalIndex < 0) return 0;
            if (trackLogicalIndex >= _AnimationTracks.Length) return 0;
            return _AnimationTracks[trackLogicalIndex].Duration;
        }

        #endregion
    }
}
