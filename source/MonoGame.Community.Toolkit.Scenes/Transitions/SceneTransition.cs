// Copyright (c) Christopher Whitley. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Community.Toolkit.Scenes.Transitions;

/// <summary>
/// Represents a transition effect between scenes in a game.
/// </summary>
public abstract class SceneTransition : IDisposable
{
    private readonly Game _game;
    private Rectangle _bounds;

    /// <summary>
    /// Gets a value indicating whether this transition has been disposed.
    /// </summary>
    public bool IsDisposed { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether this transition is currently in progress.
    /// </summary>
    public bool IsTransitioning { get; private set; }

    /// <summary>
    /// Gets the kind of transition effect.
    /// </summary>
    public SceneTransitionKind Kind { get; }

    /// <summary>
    /// Gets the total duration of the transition.
    /// </summary>
    public TimeSpan TransitionTime { get; }

    /// <summary>
    /// Gets the remaining duration of the transition.
    /// </summary>
    public TimeSpan TransitionTimeRemaining { get; private set; }

    /// <summary>
    /// Gets the source texture for the transition.
    /// </summary>
    public RenderTarget2D SourceTexture { get; private set; }

    /// <summary>
    /// Gets the render target for the transition.
    /// </summary>
    public RenderTarget2D RenderTarget { get; private set; }

    /// <summary>
    /// Occurs when the transition has completed.
    /// </summary>
    public event EventHandler<EventArgs> TransitionCompleted;

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneTransition"/> class.
    /// </summary>
    /// <param name="game">The game instance.</param>
    /// <param name="transitionTime">The duration of the transition.</param>
    /// <param name="kind">The kind of transition effect.</param>
    protected SceneTransition(Game game, TimeSpan transitionTime, SceneTransitionKind kind)
    {
        _game = game;
        TransitionTime = TransitionTimeRemaining = transitionTime;
        Kind = kind;
    }

    /// <summary/>
    ~SceneTransition() => Dispose(false);

    /// <summary>
    /// Starts the transition effect.
    /// </summary>
    /// <param name="sourceTexture">The source texture to transition from.</param>
    public virtual void Start(RenderTarget2D sourceTexture)
    {
        SourceTexture = sourceTexture;
        IsTransitioning = true;
    }

    /// <summary>
    /// Updates the transition effect.
    /// </summary>
    /// <param name="gameTime">A snapshot of timing values.</param>
    public virtual void Update(GameTime gameTime)
    {
        Debug.Assert(gameTime is not null);
        TransitionTimeRemaining -= gameTime.ElapsedGameTime;
        if (TransitionTimeRemaining <= TimeSpan.Zero)
        {
            IsTransitioning = false;
            TransitionCompleted?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Draws the transition effect.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch to draw sprites.</param>
    /// <param name="clearColor">The color to use to clear the screen.</param>
    public void Draw(SpriteBatch spriteBatch, Color clearColor)
    {
        Debug.Assert(spriteBatch is not null);
        BeginDraw(spriteBatch, clearColor);
        Draw(spriteBatch);
        EndDraw(spriteBatch);
    }

    private void BeginDraw(SpriteBatch spriteBatch, Color clearColor)
    {
        if (_bounds.Equals(Rectangle.Empty))
        {
            if (RenderTarget is not null)
            {
                _bounds = RenderTarget.Bounds;
            }
            else
            {
                int width = _game.GraphicsDevice.PresentationParameters.BackBufferWidth;
                int height = _game.GraphicsDevice.PresentationParameters.BackBufferHeight;
                _bounds = new Rectangle(0, 0, width, height);
            }
        }

        _game.GraphicsDevice.SetRenderTarget(RenderTarget);
        _game.GraphicsDevice.Viewport = new Viewport(_bounds);
        _game.GraphicsDevice.Clear(clearColor);

        spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    }

    /// <summary>
    /// Draws the scene transition
    /// </summary>
    /// <param name="spriteBatch">The sprite batch to draw sprites.</param>
    protected virtual void Draw(SpriteBatch spriteBatch) { }

    private void EndDraw(SpriteBatch spriteBatch)
    {
        spriteBatch.End();
        _game.GraphicsDevice.SetRenderTarget(null);
    }

    /// <summary>
    /// Handles the graphics device being created.
    /// </summary>
    protected virtual void HandleGraphicsDeviceCreated() => GenerateRenderTarget();

    /// <summary>
    /// Handles the graphics device being reset.
    /// </summary>
    protected virtual void HandleGraphicsDeviceReset() => GenerateRenderTarget();

    /// <summary>
    /// Handles the client size being changed.
    /// </summary>
    protected virtual void OnClientSizeChanged() => GenerateRenderTarget();

    /// <summary>
    /// Generates the render target for the transition effect.
    /// </summary>
    protected virtual void GenerateRenderTarget()
    {
        int width = _game.GraphicsDevice.PresentationParameters.BackBufferWidth;
        int height = _game.GraphicsDevice.PresentationParameters.BackBufferHeight;
        if (RenderTarget is not null && !RenderTarget.IsDisposed)
        {
            RenderTarget.Dispose();
        }

        RenderTarget = new RenderTarget2D(_game.GraphicsDevice, width, height);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="SceneTransition"/> and optionally releases the managed
    /// resources.
    /// </summary>
    /// <param name="disposing">
    /// <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only
    /// unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed) { return; }

        if (disposing)
        {
            if (RenderTarget is not null)
            {
                RenderTarget.Dispose();
                RenderTarget = null;
            }
        }

        IsDisposed = true;
    }



}
