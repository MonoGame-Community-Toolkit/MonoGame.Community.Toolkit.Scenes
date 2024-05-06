// Copyright (c) Christopher Whitley. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Community.Toolkit.Scenes.Transitions;

/// <summary>
/// Represents a scene transition that applies an even-odd tile effect.
/// </summary>
public sealed class EvenOddTileTransition : SceneTransition
{
    private readonly double _transitionHalfTime;
    private readonly int _tileSize;
    private int _columns;
    private int _rows;

    /// <summary>
    /// Initializes a new instance of the <see cref="EvenOddTileTransition"/> class.
    /// </summary>
    /// <param name="game">The game instance.</param>
    /// <param name="transitionTime">The duration of the transition.</param>
    /// <param name="kind">The kind of transition.</param>
    /// <param name="tileSize">The size of each tile in pixels.</param>
    public EvenOddTileTransition(Game game, TimeSpan transitionTime, SceneTransitionKind kind, int tileSize)
        : base(game, transitionTime, kind)
    {
        _transitionHalfTime = TransitionTime.TotalSeconds / 2;
        _tileSize = tileSize;
    }

    /// <inheritdoc/>
    public override void Start(RenderTarget2D sourceTexture)
    {
        base.Start(sourceTexture);

        _columns = (int)Math.Ceiling(sourceTexture.Width / (float)_tileSize);
        _rows = (int)Math.Ceiling(sourceTexture.Height / (float)_tileSize);
    }

    /// <inheritdoc/>
    protected override void Draw(SpriteBatch spriteBatch)
    {
        for (int row = 0; row < _rows; row++)
        {
            for (int column = 0; column < _columns; column++)
            {

                bool alternate = (IsEven(column) && IsEven(row))
                              || (IsOdd(column) && IsOdd(row));

                int size = GetSize(alternate);
                int xPos = ((column * _tileSize) + (_tileSize - size) / 2) + (size / 2);
                int yPos = ((row * _tileSize) + (_tileSize - size) / 2) + (size / 2);

                spriteBatch.Draw(texture: SourceTexture,
                                 destinationRectangle: new Rectangle(xPos, yPos, size, size),
                                 sourceRectangle: new Rectangle(column * _tileSize, row * _tileSize, _tileSize, _tileSize),
                                 color: Color.White,
                                 rotation: GetRotation(alternate),
                                 origin: new Vector2(_tileSize, _tileSize) * 0.5f,
                                 effects: SpriteEffects.None,
                                 layerDepth: 0.0f);
            }
        }
    }

    private float GetRotation(bool alternate)
    {
        double timeLeft = TransitionTimeRemaining.TotalSeconds;

        timeLeft = alternate
                 ? Math.Min(timeLeft, _transitionHalfTime)
                 : Math.Max(timeLeft - _transitionHalfTime, 0);

        return Kind == SceneTransitionKind.Out
                     ? 5.0f * (float)Math.Sin((timeLeft / _transitionHalfTime) - 1.0)
                     : 5.0f * (float)Math.Sin((timeLeft / _transitionHalfTime));
    }

    private int GetSize(bool alternate)
    {
        double timeLeft = TransitionTimeRemaining.TotalSeconds;

        timeLeft = alternate
                 ? Math.Min(timeLeft, _transitionHalfTime)
                 : Math.Max(timeLeft - _transitionHalfTime, 0);

        return Kind == SceneTransitionKind.Out
                     ? (int)((_tileSize) * (timeLeft / _transitionHalfTime))
                     : (int)((_tileSize) * (1 - (timeLeft / _transitionHalfTime)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsEven(int value) => value % 2 == 0;

    /// <summary>
    /// Determines whether the specified integer value is odd.
    /// </summary>
    /// <param name="value">The integer value to check.</param>
    /// <returns>
    /// <see langword="true"/> if the specified value is odd; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsOdd(int value) => !IsEven(value);
}
