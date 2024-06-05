using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circle
{
    class PlayerGhost
    {
        public Vector2 Position;
        private float _alpha;
        private float _alphaDelta;
        public float Scale;
        private float _scaleDelta;

        public PlayerGhost(Vector2 position)
        {
            Position = position;
            Scale = 0.14f;
            _scaleDelta = 0.0009f;

            _alpha = 0.8f;
            _alphaDelta = 0.02f;
        }

        public void Update(float speed)
        {
            Scale -= _scaleDelta;
            _alpha -= _alphaDelta;
            Position.X -= speed;
        }

        public void Render(SpriteBatch spriteBatch, Texture2D textureFront, Texture2D textureBack)
        {
            spriteBatch.Draw(textureBack, Position, null, Color.White*_alpha, 0, Vector2.Zero, Scale, SpriteEffects.None, 0.05f);
            spriteBatch.Draw(textureFront, new Vector2(Position.X + textureBack.Width * Scale, Position.Y), null, Color.White*_alpha, 0, Vector2.Zero, Scale, SpriteEffects.None, 0.2f);
        }
    }
}
