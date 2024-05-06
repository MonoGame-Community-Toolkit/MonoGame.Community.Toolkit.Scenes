// Copyright (c) Christopher Whitley. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Community.Toolkit.Scenes.Transitions;

namespace MonoGame.Community.Toolkit.Scenes;

/// <summary>
/// Manages the scenes and scene transitions in the game.
/// </summary>
public sealed class SceneManagerService
{
    private Scene _activeScene;
    private Scene _nextScene;
    private SceneTransition _transitionOut;
    private SceneTransition _transitionIn;
    private SceneTransition _currentTransition;

    /// <summary>
    /// Updates the scene manager service.
    /// </summary>
    /// <param name="gameTime">A snapshot of timing values.</param>
    public void Update(GameTime gameTime)
    {
        if (_currentTransition is not null && _currentTransition.IsTransitioning)
        {
            _currentTransition.Update(gameTime);
        }
        else if (_currentTransition is null && _nextScene is not null)
        {
            TransitionScene();
        }

        _activeScene?.Update(gameTime);
    }

    /// <summary>
    /// Draws the active scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of timing values.</param>
    /// <param name="spriteBatch">The sprite batch to draw sprites.</param>
    /// <param name="clearColor">The color to use to clear the screen.</param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color clearColor)
    {
        _activeScene?.BeforeDraw(spriteBatch, clearColor);
        _activeScene?.Draw(gameTime, spriteBatch);
        _activeScene?.AfterDraw(spriteBatch);

        _currentTransition?.Draw(spriteBatch, clearColor);


    }

    /// <summary>
    /// Changes the active scene to the specified scene.
    /// </summary>
    /// <param name="next">The scene to change to.</param>
    public void ChangeScene(Scene next)
    {
        Debug.Assert(!ReferenceEquals(_activeScene, next), "Attempted to change scene from itself to the same reference of itself");
        _nextScene = next;
    }

    /// <summary>
    /// Changes the active scene to the specified scene with the provided transitions.
    /// </summary>
    /// <param name="to">The scene to change to.</param>
    /// <param name="outTransition">The transition to use when transitioning out of the current scene.</param>
    /// <param name="inTransition">The transition to use when transitioning into the new scene.</param>
    public void ChangeScene(Scene to, SceneTransition outTransition, SceneTransition inTransition)
    {
        Debug.Assert(to is not null);

        if (_currentTransition is null || !_currentTransition.IsTransitioning)
        {
            Debug.Assert(!ReferenceEquals(_activeScene, to), "Attempted to change a scene from itself to the same reference of itself");
            _nextScene = to;
            _transitionOut = outTransition;
            _transitionIn = inTransition;

            _transitionOut.TransitionCompleted += OnTransitionOutCompleted;
            _transitionIn.TransitionCompleted += OnTransitionInCompleted;

            _currentTransition = _transitionOut;
            _currentTransition.Start(_activeScene?.RenderTarget!);
        }
    }

    private void OnTransitionOutCompleted(object sender, EventArgs e)
    {
        if (_transitionOut is not null)
        {
            _transitionOut.TransitionCompleted -= OnTransitionOutCompleted;
            _transitionOut.Dispose();
            _transitionOut = null;
        }

        TransitionScene();

        _currentTransition = _transitionIn;
        _currentTransition?.Start(_activeScene?.RenderTarget!);
    }

    private void OnTransitionInCompleted(object sender, EventArgs e)
    {
        if (_transitionIn is not null)
        {
            _transitionIn.TransitionCompleted -= OnTransitionInCompleted;
            _transitionIn.Dispose();
            _transitionIn = null;
        }
        _currentTransition?.Dispose();
        _currentTransition = null;
    }

    private void TransitionScene()
    {
        _activeScene?.UnloadContent();
        GC.Collect();
        _activeScene = _nextScene;
        _nextScene = null;
        _activeScene?.Initialize();
    }
}
