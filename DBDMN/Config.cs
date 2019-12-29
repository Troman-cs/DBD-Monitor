using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDMN
{
    public static class Config
    {
        public static string filePath = Utils.getAppPath() + "\\config.txt";

        private static Dictionary<string, object> settingValues = new Dictionary<string, object>();

        public const string keyAndValueSeparatorForSaving = " = ";
        public const string keyAndSubkeySeparatorForSaving = ".";
        public const string keyPrefixGeneral = "GeneralBool";

        //public const string actionOnLobbyFound = "OnLobbyFound";
        //public const string actionOnAllReady = "OnAllReady";
        //public const string actionOnStartigGame = "OnStartigGame";

        public const string keyActionBringGameToFront = "BringToFront";
        public const string keyActionMustPlaySound = "PlaySound";
        public const string keyActionSoundName = "SoundName";
        public const string keyActionPlayLooped = "PlayLooped";

        public const string keyGeneralPlaySoundOnSavingCurGameStats = "PlaySoundOnSavingCurGameStats";
        public const string keyGeneralMuteAllSounds = "MuteAllSounds";
        public const string keyGeneralGamePreviewOn = "GamePreviewOn";
        public const string keyGeneralActionsEnabled = "ActionsEnabled";
        public const string keyGeneralNewGameResultsEnabled = "AddNewGameresultsOn";
        public const string keyGeneralAlwaysOnTopOn = "AlwaysOnTopOn";
        public const string keyGeneralSoundOnNewStats = "SoundOnNewStats";

        public static void init()
        {
            setConfigValue( keyGeneralPlaySoundOnSavingCurGameStats, true );
        }

        //public static object getActionConfigValue( string sEventName, string sFieldName )
        //{
        //    return getConfigValue( sEventName + sFieldName );
        //}

        //public static void setActionConfigValue( string sEventName, string sFieldName,
        //    string sValue)
        //{
        //    setConfigValue( sEventName + sFieldName, sValue );
        //}

        //public static void setActionConfigValue( string sEventName, string sFieldName,
        //    bool bValue )
        //{
        //    setConfigValue( sEventName + sFieldName, bValue );
        //}

        public static void setConfigValue( string sFieldName, string sValue )
        {
            settingValues[ sFieldName.Trim() ] = sValue.Trim();
        }

        public static void setConfigValue( string sFieldName, bool sValue )
        {
            settingValues[ sFieldName.Trim() ] = sValue;
        }

        public static object getConfigValue( string sFieldName )
        {
            sFieldName = sFieldName.Trim();

            if ( !settingValues.ContainsKey( sFieldName ) )
            {
                Dbg.onDebugError( "No such config key: " + sFieldName );
                return null;
            }

            return settingValues[ sFieldName ];
        }

        public static bool getConfigValueAsBool( string sFieldName, bool bDefaultValue = false )
        {
            // If have value
            if( hasConfigKey(sFieldName))
                return (bool)getConfigValue( sFieldName );

            return bDefaultValue;
        }

        private static bool hasConfigKey(string key)
        {
            return settingValues.ContainsKey( key.Trim() );
        }
             


        public static void load()
        {
            // Exit if no stats file
            if ( !File.Exists( filePath ) )
                return;

            using ( StreamReader file = new StreamReader( filePath ) )
            {
                while ( !file.EndOfStream )
                {
                    var line = file.ReadLine();

                    line = line.Trim();

                    var v = line.Split( new string[] { Config.keyAndValueSeparatorForSaving },
                        StringSplitOptions.None );

                    // Make sure we have both: field name and value
                    if ( v.Length != 2 )
                        continue;

                    string fieldName = v[ 0 ].Trim();
                    string value = v[ 1 ].Trim();

                    // Make sure this is not a duplicate entry
                    Dbg.assert( !settingValues.ContainsKey( fieldName ) );

                    //settingValues[ fieldName ] = value;

                    // Get key and subkey
                    var keyAndSubkey = fieldName.Split( new string[] { Config.keyAndSubkeySeparatorForSaving },
                        StringSplitOptions.None );

                    Dbg.assert( keyAndSubkey.Length == 2 );

                    string mainKey = keyAndSubkey[ 0 ].Trim();
                    string subKey = keyAndSubkey[ 1 ].Trim();

                    if(mainKey == Config.keyPrefixGeneral)
                    {
                        parseGeneralSettingLine( subKey, value );
                    }
                    else
                    {
                        // This is a setting for an action
                        parseSettingLineForAction( mainKey, subKey, value );
                    }
                }
            }

            void parseGeneralSettingLine(string subKey, string setting)
            {
                subKey = subKey.Trim();

                Config.setConfigValue( subKey, bool.Parse( setting ) );
            }

            void parseSettingLineForAction( string mainKey, string subKey, string value )
            {
                string stateString = mainKey;
                var state = StateManager.getStateEnumFromStateEnumDescription( stateString );

                Dbg.assert( state != null );

                var action = Actions.getActions()[ ( StateManager.State )state ];
                action.loadSettingValueFromConfig( subKey, value );
            }
        }


        public static void save()
        {
            using ( StreamWriter file = new StreamWriter( filePath ) )
            {
                
                foreach(var action in Actions.getActions() )
                {
                    string actionSettings = action.Value.getValuesForConfig();
                    file.Write( actionSettings );
                    file.Write( "\n"  );
                }

                string value = null;
                string key = null;
                foreach ( var kvp in settingValues )
                {
                    value = kvp.Value.ToString();
                    key = Config.keyPrefixGeneral + Config.keyAndSubkeySeparatorForSaving + kvp.Key;

                    file.Write( key + Config.keyAndValueSeparatorForSaving + value );
                    file.Write( "\n" );
                }
            }
        }

    }
}
