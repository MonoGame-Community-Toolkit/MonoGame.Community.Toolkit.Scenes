// Copyright (c) Christopher Whitley. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Community.Toolkit.Scenes.Transitions;

public sealed class FadeTransition : SceneTransition
{
    public FadeTransition(Game game, TimeSpan transitionTime, SceneTransitionKind kind)
        : base(game, transitionTime, kind) { }

    protected override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture: SourceTexture,
                         destinationRectangle: SourceTexture.Bounds,
                         sourceRectangle: SourceTexture.Bounds,
                         color: Color.White * GetAlpha());
    }

    private float GetAlpha()
    {
        double timeLeft = TransitionTimeRemaining.TotalSeconds;

        if (Kind == SceneTransitionKind.Out)
        {
            return (float)(timeLeft / TransitionTime.TotalSeconds);
        }
        else
        {
            return (float)(1.0 - (timeLeft / TransitionTime.TotalSeconds));
        }
    }
}
