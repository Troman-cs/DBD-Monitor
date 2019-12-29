using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DBDMN.Actions;
using static DBDMN.Sound;

namespace DBDMN
{
    public class Action
    {

        public const string ACTION_ON_LOBBY_FOUND = "On lobby found";
        public const string ACTION_ON_ALL_PLAYERS_IN_LOBBY = "On all players are in lobby";
        public const string ACTION_ON_ALL_PLAYERS_CLICKED_READY = "On all players clicked ready";
        public const string ACTION_ON_FINISHED_LOADING_THE_GAME = "On finished loading the game";

        /// <summary>
        /// Action is for this state
        /// </summary>
        public StateManager.State forState = StateManager.State.Unknown;

        //private bool bBringGameToFront = false;
        //private bool bPlaySound = false;
        //private bool bLooped = false;

        /// <summary>
        /// Storing settings in a Dictionary for easier saving and loading
        /// </summary>
        private Dictionary<string, object> settings = new Dictionary<string, object>();

        /// <summary>
        /// Sound for an action
        /// </summary>
        private Sound.SoundsEnum actionSoundFile = Sound.SoundsEnum.None;

        /// <summary>
        /// Action name (description)
        /// </summary>
        private string name = null;

        public Action(string name)
        {
            this.name = name;

            // Init all settings
            this.setBringGameToFront( false );
            this.setMustPlaySound( false );
            this.setPlaySoundLooped( false );
            this.setSoundName( SoundsEnum.None );
        }



        public void setBringGameToFront(bool b)
        {
            this.settings[ Config.keyActionBringGameToFront ] = b;
        }



        public bool getBringGameToFront()
        {
            return (bool)settings[ Config.keyActionBringGameToFront ];
        }

        public void setMustPlaySound( bool bPlay )
        {
            this.settings[ Config.keyActionMustPlaySound ] = bPlay;
        }

        public void setPlaySoundLooped( bool bLooped )
        {
            this.settings[ Config.keyActionPlayLooped ] = bLooped;
        }

        public bool getMustPlaySound()
        {
            return (bool)this.settings[ Config.keyActionMustPlaySound ];
        }

        public bool getMustPlayLooped()
        {
            return ( bool )this.settings[ Config.keyActionPlayLooped ];
        }

        public void setSoundName( Sound.SoundsEnum sound )
        {
            this.settings[ Config.keyActionSoundName ] = sound;
            //this.actionSoundFile = sound;

            // Enable if have something to play
            setMustPlaySound( sound != Sound.SoundsEnum.None );
        }

        public SoundsEnum getSoundName()
        {
            return (SoundsEnum)this.settings[ Config.keyActionSoundName ];
        }

        /// <summary>
        /// Do actions
        /// </summary>
        public void activate()
        {
            // Actions disabled?
            if ( !Actions.Enabled )
                return;

            if(getBringGameToFront())
            {
                if ( !ScreenCapture.isDBDWindowFocused() )    // Don't activate if active already
                    ScreenCapture.activateGame();
            }

            if(getMustPlaySound())
            {
                if ( getSoundName() != SoundsEnum.None )
                {
                    Actions.playSound( getSoundName(), getMustPlayLooped() );
                }
            }
        }

        public override string ToString()
        {
            return this.name;
        }



        public string getValuesForConfig()
        {
            string result = "";

            string s = Config.keyAndSubkeySeparatorForSaving;
            string separator = Config.keyAndValueSeparatorForSaving;

            // Instead of enum.toString(), get its description, that
            // oesn't get changed when an enum value is renamed
            string myEnumStateName = this.forState.getEnumValueName();

            foreach ( var kvp in this.settings)
            {
                string key = myEnumStateName + s + kvp.Key;
                string value = kvp.Value.ToString();
                result = result + key + separator + value + "\n";
            }

            //// Bring to front
            //string key = this.forState.ToString() + s + Config.keyBringGameToFront;
            //string value = this.getBringGameToFront().ToString();
            //result = key + separator + value + "\n";

            //// Play sound
            //key = this.forState.ToString() + s + Config.keyPlaySound;
            //value = this.getMustPlaySound().ToString();
            //result = result + key + separator + value + "\n";

            //// Play looped
            //key = this.forState.ToString() + s + Config.keyPlayLooped;
            //value = this.getMustPlayLooped().ToString();
            //result = result + key + separator + value + "\n";

            return result;
        }

        public void loadSettingValueFromConfig(string key, string value)
        {
            switch ( key )
            {
                case Config.keyActionBringGameToFront:
                    setBringGameToFront( bool.Parse( value ) );
                    break;
                case Config.keyActionMustPlaySound:
                    setMustPlaySound( bool.Parse( value ) );
                    break;
                case Config.keyActionSoundName:
                    var soundEnum = Sound.getSoundsEnumFromSoundsEnumString( value );

                    Dbg.assert( soundEnum != null );
                    
                    setSoundName( (SoundsEnum)soundEnum );
                    break;
                case Config.keyActionPlayLooped:
                    setPlaySoundLooped( bool.Parse( value ) );
                    break;
                default:
                    Dbg.onDebugError( "Unknown action setting: " + key );
                    break;
            }
        }
    }
}
