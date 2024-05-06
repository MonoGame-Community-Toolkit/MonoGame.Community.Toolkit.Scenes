using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Community.Toolkit.Scenes;

/// <summary>
/// Defines a scene in the game.
/// </summary>
public abstract class Scene : IDisposable
{
    private readonly Game _game;

    /// <summary>
    /// Gets or sets a value indicating whether the scene is paused.
    /// </summary>
    public bool IsPaused { get; set; }

    /// <summary>
    /// Gets the render target of the scene.
    /// </summary>
    public RenderTarget2D RenderTarget { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether this scene has been disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Gets the local content manager for the scene.
    /// </summary>
    public ContentManager LocalContent { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Scene"/> class.
    /// </summary>
    /// <param name="game">The game instance.</param>
    protected Scene(Game game)
    {
        Debug.Assert(game is not null);
        _game = game;
    }

    /// <summary/>
    ~Scene() => Dispose(false);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="Scene"/> and optionally releases the managed resources.
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
            if (LocalContent is not null)
            {
                LocalContent.Dispose();
                LocalContent = null;
            }

            if (RenderTarget is not null && !RenderTarget.IsDisposed)
            {
                RenderTarget.Dispose();
                RenderTarget = null;
            }
        }

        IsDisposed = true;
    }

    /// <summary>
    /// Initializes the scene.
    /// </summary>
    /// <remarks>
    /// This will call <see cref="LoadContent"/> before returning back.
    /// </remarks>
    public virtual void Initialize()
    {
        LocalContent = new ContentManager(_game.Services);
        LocalContent.RootDirectory = _game.Content.RootDirectory;
        LoadContent();
    }

    /// <summary>
    /// Loads content for the scene.
    /// </summary>
    /// <remarks>
    /// This will call <see cref="GenerateRenderTarget"/> before returning back.
    /// </remarks>
    public virtual void LoadContent()
    {
        GenerateRenderTarget();
    }

    /// <summary>
    /// Unloads content from the scene, but does not dispose of the scene.
    /// </summary>
    public virtual void UnloadContent()
    {
        if (LocalContent is not null)
        {
            LocalContent.Dispose();
            LocalContent = null;
        }

        if (RenderTarget is not null && !RenderTarget.IsDisposed)
        {
            RenderTarget.Dispose();
            RenderTarget = null;
        }
    }

    /// <summary>
    /// Updates the scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of timing values.</param>
    public virtual void Update(GameTime gameTime) { }

    /// <summary>
    /// Prepares the scene for drawing.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch to draw sprites.</param>
    /// <param name="clearColor">The color used to clear the screen.</param>
    public virtual void BeforeDraw(SpriteBatch spriteBatch, Color clearColor)
    {
        Debug.Assert(spriteBatch is not null);
        _game.GraphicsDevice.SetRenderTarget(RenderTarget);
        _game.GraphicsDevice.Clear(clearColor);
        spriteBatch.Begin();
    }

    /// <summary>
    /// Draws the scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of timing values.</param>
    /// <param name="spriteBatch">The sprite batch to draw sprites.</param>
    public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) { }

    /// <summary>
    /// Finalizes the drawing of the scene.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch to draw sprites.</param>
    public virtual void AfterDraw(SpriteBatch spriteBatch)
    {
        Debug.Assert(spriteBatch is not null);
        spriteBatch.End();
        _game.GraphicsDevice.SetRenderTarget(null);
    }

    /// <summary>
    /// Generates the render target for the scene.
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

    /// <summary>
    /// Handles the graphics device being created.
    /// </summary>
    protected virtual void HandleGraphicsCreated() => GenerateRenderTarget();

    /// <summary>
    /// Handles the graphics device being reset.
    /// </summary>
    protected virtual void HandleGraphicsDeviceReset() => GenerateRenderTarget();

    /// <summary>
    /// Handles the client size being changed.
    /// </summary>
    protected virtual void HandleClientSizeChanged() => GenerateRenderTarget();

    /// <summary>
    /// Gets the game instance casted to a specified type.
    /// </summary>
    /// <typeparam name="T">The type of the game instance.</typeparam>
    /// <returns>The game instance casted to the specified type.</returns>
    public T GameAs<T>() where T : Game => (T)_game;
}
