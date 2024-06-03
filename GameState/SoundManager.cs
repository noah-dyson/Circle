using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Circle{
    public class SoundManager
    {
        private SoundEffect _jumpSound;

        public SoundManager()
        {

        }

        public void LoadContent(ContentManager content)
        {
            _jumpSound = content.Load<SoundEffect>("jumpsound");
        }

        public void PlayJumpSound()
        {
            _jumpSound.Play();
        }
    }
}