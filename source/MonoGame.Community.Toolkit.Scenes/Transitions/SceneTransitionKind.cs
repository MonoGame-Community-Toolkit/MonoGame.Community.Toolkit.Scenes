// Copyright (c) Christopher Whitley. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame.Community.Toolkit.Scenes.Transitions;

/// <summary>
/// Enum defining the type of transition for scenes in a game.
/// </summary>
public enum SceneTransitionKind
{
    /// <summary>
    /// Represents a transition into a scene.
    /// </summary>
    In,

    /// <summary>
    /// Represents a transition out of a scene.
    /// </summary>
    Out
}
