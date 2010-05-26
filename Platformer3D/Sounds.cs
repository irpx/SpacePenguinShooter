using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;


namespace Platformer3D
{
    public class Sounds
    {

        public static Sounds Instance;

        public static Dictionary<string, SoundEffectInstance> soundInstances;
        public static Dictionary<string, SoundEffect> sounds;

        public static Dictionary<string, Song> musicSongs;

        public Sounds()
        {
            if (Instance != null) throw new Exception("Sound engine instance doubled!");
            Sounds.Instance = this;

            soundInstances = new Dictionary<string,SoundEffectInstance>();
            sounds = new Dictionary<string,SoundEffect>();
            musicSongs = new Dictionary<string, Song>();
        }
        public static void LoadSounds()
        {
            LoadSound("blip");
            LoadSound("enemyExplode");
            LoadSound("explosionShort");
            LoadSound("footStep");
            LoadSound("hitDamage");
            LoadSound("jetpack");
            LoadSound("jetpack_start");
            LoadSound("jetpack_stop");
            LoadSound("jump");
            LoadSound("pickup");
            LoadSound("powerup");
            LoadSound("rocket");
            LoadSound("shootBullet");
            LoadSound("shootLaser");
            LoadSound("snap");
            LoadSound("spawn");
            LoadSound("swap");
            LoadMusic("music");
            LoadMusic("music2");
            LoadMusic("music8bit");
        }

        private static void LoadMusic(string soundName)
        {
            Song sng = Game1.Instance.Content.Load<Song>("sounds\\" + soundName);
            musicSongs.Add(soundName, sng);
        }

        private static void LoadSound(String soundName)
        {
            SoundEffect snd = Game1.Instance.Content.Load<SoundEffect>("sounds\\" + soundName);
            SoundEffectInstance sndI = snd.CreateInstance();

            sounds.Add(soundName, snd);
            soundInstances.Add(soundName, sndI);
        }
        public static void PlaySound(String soundName)
        {

            //SoundEffectInstance sndI;
            SoundEffect snd;
            Sounds.sounds.TryGetValue(soundName, out snd);
            if(snd!=null) snd.Play();
            /*
            SoundEffectInstance sndI;
            soundInstances.TryGetValue(soundName, out sndI);
            if (sndI.State == SoundState.Playing) sndI.Stop(true);
            sndI.Play();
             * */
        }
        public static void PlayMusic(String soundName)
        {
            Song sng;
            musicSongs.TryGetValue(soundName, out sng);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(sng);
        }

        internal static void LoopSound(string p,string s)
        {
            SoundEffectInstance sndI;
            if (soundInstances.TryGetValue(p, out sndI))
            {
                if (sndI.State == SoundState.Playing && sndI.IsLooped) return;
                sndI.Stop(true);
                soundInstances.Remove(p);
            }

            SoundEffect snd;
            Sounds.sounds.TryGetValue(p, out snd);
            sndI = snd.CreateInstance();
            sndI.IsLooped = true;
            sndI.Play();
            soundInstances.Add(p,sndI);

            PlaySound(s);
        }

        internal static void StopSound(string p,string s)
        {
            SoundEffectInstance sndI;
            if(!soundInstances.TryGetValue(p, out sndI)) return;

            if (sndI.State == SoundState.Playing)
            {
                sndI.Stop(true);
                soundInstances.Remove(p);
                PlaySound(s);
            }
        }
    }
}
