using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace DBDMN
{
    public static class Sound
    {
        public enum SoundsEnum { None, Notify1, Notify2, Check, Horn, Shutter, Starting, Tada };

        private static SoundPlayer Player = new SoundPlayer();

        private static string soundPath = Utils.getAppPath() + "\\sounds\\";

        public static Dictionary<SoundsEnum, string> soundsToString = new Dictionary<SoundsEnum, string>
        {
            [ SoundsEnum.Notify1 ] = "Notify 1",
            [ SoundsEnum.Notify2 ] = "Notify 2",
            [ SoundsEnum.Tada ] = "Tada",
            [ SoundsEnum.Horn ] = "Horn",
            [ SoundsEnum.Starting ] = "Starting",
            [ SoundsEnum.Check ] = "Check"
        };

        public static void stopSound()
        {
            Player.Stop();
        }

        public static void playSound( Sound.SoundsEnum sound, bool bLooped = false )
        {
            // All sound muted? - don't play
            if ( Form1.getInstance().isSoundMuted() )
                return;

            switch ( sound )
            {
                case Sound.SoundsEnum.Shutter:
                    Player = new SoundPlayer( soundPath + "stored.wav" );
                    if ( bLooped )
                        Player.PlayLooping();
                    else
                        Player.Play();
                    break;
                case Sound.SoundsEnum.Notify1:
                    Player = new SoundPlayer( soundPath + "notify1.wav" );
                    if ( bLooped )
                        Player.PlayLooping();
                    else
                        Player.Play();
                    //System.Media.SystemSounds.Exclamation.Play();
                    break;
                case Sound.SoundsEnum.Notify2:
                    Player = new SoundPlayer( soundPath + "notify2.wav" );
                    if ( bLooped )
                        Player.PlayLooping();
                    else
                        Player.Play();
                    break;
                case Sound.SoundsEnum.Tada:
                    Player = new SoundPlayer( soundPath + "tada.wav" );
                    if ( bLooped )
                        Player.PlayLooping();
                    else
                        Player.Play();
                    break;
                case Sound.SoundsEnum.Horn:
                    Player = new SoundPlayer( soundPath + "horn.wav" );
                    if ( bLooped )
                        Player.PlayLooping();
                    else
                        Player.Play();
                    break;
                case Sound.SoundsEnum.Check:
                    Player = new SoundPlayer( soundPath + "check.wav" );
                    if ( bLooped )
                        Player.PlayLooping();
                    else
                        Player.Play();
                    break;
                case Sound.SoundsEnum.Starting:
                    Player = new SoundPlayer( soundPath + "Starting.wav" );
                    if ( bLooped )
                        Player.PlayLooping();
                    else
                        Player.Play();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sounds.Notify2 => "Notify 2"
        /// </summary>
        public static string getSoundNameFromSound( SoundsEnum sound )
        {
            Dbg.assert( sound != SoundsEnum.None, "Wrong sound: " + sound.ToString() );
            Dbg.assert( soundsToString.ContainsKey( sound ), "Wrong sound: " + sound.ToString() );

            return soundsToString[ sound ];
        }

        /// <summary>
        /// "Notify 2" => Sounds.Notify2
        /// </summary>
        public static SoundsEnum getSoundFromSoundName( string sSoundName )
        {
            sSoundName = sSoundName.Trim();
            Dbg.assert( soundsToString.ContainsValue( sSoundName ) );

            foreach ( var kvp in soundsToString )
            {
                if ( kvp.Value == sSoundName )
                    return kvp.Key;
            }

            Dbg.onDebugError( "Wrong sound name: " + sSoundName );

            return SoundsEnum.None;
        }

        /// <summary>
        /// "Notify2" => Sounds.Notify2
        /// </summary>
        public static SoundsEnum? getSoundsEnumFromSoundsEnumString( string sSoundsEnumName )
        {
            sSoundsEnumName = sSoundsEnumName.Trim();

            var soundsEnumValues = Enum.GetValues( typeof( SoundsEnum ) ).Cast<SoundsEnum>();
            foreach ( var soundEnum in soundsEnumValues )
            {
                if ( soundEnum.ToString() == sSoundsEnumName )
                    return soundEnum;
            }

            return null;
        }
    }
}
