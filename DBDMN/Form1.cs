using System;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DBDMN.ScreenCapture;
using static DBDMN.ScreenParser;
using System.Media;
using System.Diagnostics;
using System.IO;
using static DBDMN.EndscoreBpDigitGfx;
using static DBDMN.Sound;
using System.Threading;

namespace DBDMN
{
    public partial class Form1 : Form
    {
        public const string title = "DBD Monitor";

        private static Form1 me = null;

        private static Stopwatch stopWatch = new Stopwatch();

        Preview wndPreview = null;

        public Form1()
        {
            InitializeComponent();
            me = this;
        }


        public static Form1 getInstance()
        {
            return Form1.me;
        }

        public static void startStopwatch()
        {
            stopWatch.Reset();
            stopWatch.Start();
        }

        public static void stopStopwatch()
        {
            stopWatch.Stop();
        }

        /// <summary>
        /// Initial actions configuration
        /// </summary>
        private void initPlaySoundsConfig()
        {

            var actionLobbyFound = Actions.addAction( StateManager.State.Lobby_SurvivorOrKiller, 
                Action.ACTION_ON_LOBBY_FOUND );

            var actionAllPlayersInLobby = Actions.addAction( StateManager.State.Lobby_AllPlayersInLobby,
                Action.ACTION_ON_ALL_PLAYERS_IN_LOBBY );

            var actionAllPlayersReady = Actions.addAction( StateManager.State.Lobby_AllPlayersReady,
                Action.ACTION_ON_ALL_PLAYERS_CLICKED_READY );

            var actionLoadingAlmostDone = Actions.addAction( StateManager.State.LoadingMatch_AlmostDone,
                Action.ACTION_ON_FINISHED_LOADING_THE_GAME );

            actionLobbyFound.setSoundName( SoundsEnum.Tada );
            actionLobbyFound.setBringGameToFront( true );

            actionAllPlayersReady.setSoundName( SoundsEnum.Notify2 );

            actionLoadingAlmostDone.setSoundName( SoundsEnum.Notify2 );
            actionLoadingAlmostDone.setBringGameToFront( true );

            //chkBringToFront.Checked = true;
            //chkOnAllReadyBringToFront.Checked = true;
            //chkOnAllReadyPlaySound.Checked = true;
            //chkOnStartingTheGameBringToFront.Checked = true;


            //for (int i = 0; i < cmbSound.Items.Count-1; i++ )
            //{
            //    if( cmbSound.Items[i].ToString().ToLower() == "tada")
            //    {
            //        cmbSound.SelectedIndex = i;
            //        break;
            //    }
            //}

            //for ( int i = 0; i < cmbOnAllReadySound.Items.Count - 1; i++ )
            //{
            //    if ( cmbOnAllReadySound.Items[ i ].ToString().ToLower() == "notify 2" )
            //    {
            //        cmbOnAllReadySound.SelectedIndex = i;
            //        break;
            //    }
            //}

            //for ( int i = 0; i < cmbOnStartingGameSound.Items.Count - 1; i++ )
            //{
            //    if ( cmbOnStartingGameSound.Items[ i ].ToString().ToLower() == "notify 2" )
            //    {
            //        cmbOnStartingGameSound.SelectedIndex = i;
            //        break;
            //    }
            //}


        }

        private void initActions()
        {
            foreach(var a in Actions.getActions() )
            {
                lstEvents.Items.Add( a.Value );
            }

            // Fill sounds
            foreach(var kvp in Sound.soundsToString )
            {
                cmbSound.Items.Add( kvp.Value );
            }
        }

        private void setGuiStateFromConfigSettings()
        {
            chkMute.Checked = Config.getConfigValueAsBool( Config.keyGeneralMuteAllSounds, false );
            chkPreview.Checked = Config.getConfigValueAsBool( Config.keyGeneralGamePreviewOn, false );
            chkStatsEnabled.Checked = Config.getConfigValueAsBool( Config.keyGeneralNewGameResultsEnabled, true );
            chkOnTop.Checked = Config.getConfigValueAsBool( Config.keyGeneralAlwaysOnTopOn, false );
            chkPlaySoundOnStatsSave.Checked = Config.getConfigValueAsBool( Config.keyGeneralSoundOnNewStats, true );

            chkActionsEnabled.Checked = Config.getConfigValueAsBool( Config.keyGeneralActionsEnabled, true );
            onActionsEnabled( chkActionsEnabled.Checked );

            toolTip.InitialDelay = 100;
            toolTip.SetToolTip( lblStatsSwfEscapeRateValue, toolTip.GetToolTip( lblStatsSwfEscapeRate ) );
            toolTip.SetToolTip( lblOthersSwfEscapeRateValue, toolTip.GetToolTip( lblStatsSwfEscapeRate ) );

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Log.log("Startup");

            Dbg.saveErrorImageToFile();

            Config.init();

            ScreenParser.initialize();

            Dbg.initialDiagnostics();

            Gfx.initialize();

            initPlaySoundsConfig();

            // Load stats
            StatSaver.load();

            initActions();

            // Load only after creating actions
            Config.load();

            setGuiStateFromConfigSettings();

            recalcStats();

            if ( !Dbg.bDebug )
                tab1.TabPages.RemoveAt( 2 );


            //       Stats.addCurGameResult( new GameResult( PlayerIndex.Survivor1, EndgameSurvivorIcon.KilledSacrificed,
            //EndgameSurvivorIcon.KilledSacrificed, EndgameSurvivorIcon.KilledSacrificed,
            //EndgameSurvivorIcon.KilledSacrificed ) );



            wndPreview = new Preview(this);

            //var i = Image.FromFile(@"C:\temp\DBD\survivor_prelobby.jpg");
            //ScreenCapture.setScreenShot(new Bitmap(i));

            //picBox.Size = new Size(picBox.Width, picBox.Height);
            //picBox.SizeMode = PictureBoxSizeMode.Zoom;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        

        

        

        private void managePreviewWindow()
        {
            bool bHaveContentToDisplay = ScreenCapture.haveGameHwnd() || ScreenCapture.haveDebugPicture();

            // No game? - hide
            if (!bHaveContentToDisplay && wndPreview.Visible)
            {
                wndPreview.Hide();
                return;
            }

            if (chkPreview.Checked)
            {
                if ( bHaveContentToDisplay && !isDBDWindowFocused())
                {
                    if (!wndPreview.Visible)
                        wndPreview.Show();
                }
                else
                {
                    if (wndPreview.Visible)
                        wndPreview.Hide();
                }
            }
            else if (wndPreview.Visible)
            {
                wndPreview.Hide();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            

            // Stop spamming debug msgs on error
            if ( Dbg.bErrorMsg )
                return;




            //ScreenCapture.setDebugImageFile( @"C:\temp\DBD\problems\Screenshot_1.jpg" );
            //var bp11 = EndscoreBpDigitGfx.recognizePlayerBPNumber( PlayerIndex.Survivor1 );
            //var bp22 = EndscoreBpDigitGfx.recognizePlayerBPNumber( PlayerIndex.Survivor2 );
            //var bp33 = EndscoreBpDigitGfx.recognizePlayerBPNumber( PlayerIndex.Survivor3 );
            //var bp44 = EndscoreBpDigitGfx.recognizePlayerBPNumber( PlayerIndex.Survivor4 );
            //var bpKiller5 = EndscoreBpDigitGfx.recognizePlayerBPNumber( PlayerIndex.Killer );

            //var v0 =ScreenParser.hasKillerCancelButton();
            //var bHasCrossplayIcon = hasCrossplayIcon();
            //var v = ScreenParser.hasReadyGfx();
            //var v1 = ScreenParser.hasUnGfx();
            //var v2 = ScreenParser.hasShopIcon();
            //var v3 = ScreenParser.hasSurvivorLookingForMatchText();
            //var v4 = ScreenParser.isAnyPlayerInLobbyOrPrelobby();
            //var v5 = Gfx.unknownErrorMsg.recognize();
            //var v6 = Gfx.overlayDarkBlueMsg.recognize();


            //var v = hasAllPlayersClickedReady();

            //StateManager.beforeAnyStateUpdates();

            this.Text = title + " " + stopWatch.Elapsed.ToString(@"hh\:mm\:ss");

            bool bSuccess = ScreenParser.tick();


            //var b = ScreenParser.hasAllPlayersEnteredLobby();

            //var player2 = ScreenParser.getScoreboardSelectedPlayer(true);

            //var icon2 = getEndgameScoreboardSurvIcon( player2 );



            //var jpgFiles = Directory.GetFiles( @"C:\Users\temp\OneDrive\Pictures\Screenshots\Wraith",
            //    "*.png", SearchOption.TopDirectoryOnly);

            //int numScoreboardFiles = 0;
            //foreach ( var jpgFile in jpgFiles )
            //{
            //    ScreenCapture.setDebugImageFile( jpgFile );
            //    ScreenCapture.makeGameScreenshot();

            //    if ( hasEndgameScoreboard() )
            //    {
            //        numScoreboardFiles++;

            //        //if ( jpgFile == "C:\\temp\\DBD\\1080p\\End-Game\\Scoreboard\\Killer DC 1080p.jpg" )
            //        //{
            //        //    //Dbg.bTest = true;
            //        //    Debugger.Break();
            //        //}

            //        var player = ScreenParser.recognizeScoreboardSelectedPlayer( true );

            //        Dbg.assert( player != PlayerIndex.Error );

            //        //var icon1 = ScreenParser.recognizeEndgameScoreboardSurvIcon( PlayerIndex.Survivor1 );
            //        //var icon2 = ScreenParser.recognizeEndgameScoreboardSurvIcon( PlayerIndex.Survivor2 );
            //        //var icon3 = ScreenParser.recognizeEndgameScoreboardSurvIcon( PlayerIndex.Survivor3 );
            //        //var icon4 = ScreenParser.recognizeEndgameScoreboardSurvIcon( PlayerIndex.Survivor4 );

            //        var bp1 = EndscoreBpDigitGfx.recognizePlayerBPNumber( PlayerIndex.Survivor1 );
            //        var bp2 = EndscoreBpDigitGfx.recognizePlayerBPNumber( PlayerIndex.Survivor2 );
            //        var bp3 = EndscoreBpDigitGfx.recognizePlayerBPNumber( PlayerIndex.Survivor3 );
            //        var bp4 = EndscoreBpDigitGfx.recognizePlayerBPNumber( PlayerIndex.Survivor4 );
            //        var bpKiller = EndscoreBpDigitGfx.recognizePlayerBPNumber( PlayerIndex.Killer );

            //        Dbg.assert( bp1 != -1 && bp2 != -1 && bp3 != -1 && bp4 != -1 && bpKiller != -1 );

            //        Log.log( numScoreboardFiles + " (" + jpgFile + ")\nSelectedPlayer: " + player.ToString() );
            //        //Log.log( "Icons1: " + icon1.ToString() + ", icon2: " + icon2.ToString() +
            //        //    ", icon3: " + icon3.ToString() + ", icon4: " + icon4.ToString() + "\n" );

            //        Log.log( "Bp1: " + bp1.ToString() + ", Bp2: " + bp2.ToString() +
            //             ", bp3: " + bp3.ToString() + ", bp4: " + bp4.ToString() +
            //             ", killer: " + bpKiller.ToString()  + "\n" );

            //        //Debug.Assert(icon1 != EndgameSurvivorIcon.Error && icon2 != EndgameSurvivorIcon.Error &&
            //        //    icon3 != EndgameSurvivorIcon.Error && icon4 != EndgameSurvivorIcon.Error);

            //        //Log.log(numScoreboardFiles + " (" + pngFile + ")\n" + ": \nSurvivor1: " + icon1.ToString() +
            //        //    "\nSurvivor2: " + icon2.ToString() + "\nSurvivor3: " +
            //        //    icon3.ToString() + "\nSurvivor4: " + icon4.ToString() +
            //        //    "\nKiller: " + icon5.ToString() );
            //    }
            //    //else
            //    //{
            //    //    Dbg.onError( "" );
            //    //    Log.log( "Not scoreboard: " + jpgFile );
            //    //}

            //}

            managePreviewWindow();

            if ( !bSuccess || (!ScreenCapture.haveGameHwnd() && !ScreenCapture.haveDebugPicture()))
                return;


            //ScreenParser.parseScreenshot();
            var r = ScreenParser.bHasReadyButton;
            if (r)
                label1.Text = "READY";
            else
                label1.Text = "no ready icon";

            if (ScreenParser.bHasCrossplayIcon)
                label2.Text = "CROSSPLAY ICON";
            else
                label2.Text = "no crossplay icon";

            if (ScreenParser.bKillerCancelButton)
                label3.Text = "KILLER CANCEL";
            else
                label3.Text = "no killer cancel";

            if (ScreenParser.bSurvivorCancelButton)
                label4.Text = "SURVIVOR CANCEL";
            else
                label4.Text = "no survivor cancel";

            if (ScreenParser.bHasUnTextGfx)
                label5.Text = "HAS UN";
            else
                label5.Text = "no un";

            if (ScreenParser.bHasUnreadyButton)
                label6.Text = "HAS UNREADY";
            else
                label6.Text = "no unready";

            if (ScreenParser.bHasSurvivorLookingForMatchText)
            {
                label7.Text = "LOOKING FOR MATCH";
            }
            else
                label7.Text = "not looking for match";

            if (ScreenParser.bHasShopIcon)
                label8.Text = "SHOP ICON";
            else
                label8.Text = "no shop icon";

            if (isDBDWindowFocused())
                label10.Text = "DBD ACTIVE";
            else
                label10.Text = "dbd not active";

            lblDbgState.Text = StateManager.getState().ToString();
            lblgameType.Text = StateManager.getGameType().ToString();

            label9.Text = "Killer: " + getPlayerLobbyStatus( PlayerIndex.Killer) +
                ", Surv1: " + getPlayerLobbyStatus( PlayerIndex.Survivor1) +
                ", Surv2: " + getPlayerLobbyStatus( PlayerIndex.Survivor2) +
                ", Surv3: " + getPlayerLobbyStatus( PlayerIndex.Survivor3) +
                ", Surv4: " + getPlayerLobbyStatus( PlayerIndex.Survivor4);



            
        }

        
        private void btnSound_Click(object sender, EventArgs e)
        {
        }

        private void previewSound(string selectedSound, bool bLooped = false)
        {
            var sound = Sound.getSoundFromSoundName( selectedSound );

            if(sound != SoundsEnum.None)
            {
                Actions.playSound( sound, bLooped );
            }

            //switch ( selectedSound.Trim() )
            //{
            //    case "Notify 1":
            //        Actions.playSound( Actions.Sounds.Notify1, bLooped );
            //        break;
            //    case "Notify 2":
            //        Actions.playSound( Actions.Sounds.Notify2, bLooped );
            //        break;
            //    case "Tada":
            //        Actions.playSound( Actions.Sounds.Tada, bLooped );
            //        break;
            //    case "Horn":
            //        Actions.playSound( Actions.Sounds.Horn, bLooped );
            //        break;
            //    case "Custom...":

            //        Actions.playCustomSound( txtCustomSound.Text.Trim() );

            //        player = new System.Media.SoundPlayer( txtCustomSound.Text.Trim() );
            //        //player.Play();
            //        break;
            //    default:
            //        break;
            //}
        }

        
        

        private void cmbSound_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                DBDT.Visible = true;
            }
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            DBDT.Visible = false;
        }

        public void untickPreview()
        {
            if(chkPreview.Checked)
                chkPreview.Checked = false;
        }

        private void chkPreview_CheckedChanged(object sender, EventArgs e)
        {
            Config.setConfigValue( Config.keyGeneralGamePreviewOn, chkPreview.Checked );
        }

        private void Form1_FormClosing( object sender, FormClosingEventArgs e )
        {
            // Save stats
            StatSaver.save();
            Config.save();
        }

        private void btnStats_Click( object sender, EventArgs e )
        {
        }

        private Action getSelectedAction()
        {
            if ( lstEvents.SelectedIndex < 0 )
                return null;

            return ( Action )lstEvents.Items[ lstEvents.SelectedIndex ];
        }

        private void checkBox3_CheckedChanged( object sender, EventArgs e )
        {
            var selectedAction = getSelectedAction();
            if ( selectedAction == null )
                return;

            selectedAction.setBringGameToFront( chkBringToFront.Checked );
        }

        private void checkBox2_CheckedChanged( object sender, EventArgs e )
        {
        }

        private void checkBox6_CheckedChanged( object sender, EventArgs e )
        {
        }

        private void chkPlaySound_CheckedChanged( object sender, EventArgs e )
        {
            chkLooped.Enabled = chkPlaySound.Checked;

            var selectedAction = getSelectedAction();
            if ( selectedAction == null )
                return;

            selectedAction.setMustPlaySound( chkPlaySound.Checked );
            //selectedAction.setPlaySoundLooped( chkPlaySound.Checked, chkLooped.Checked );
        }

        private void chkSound_CheckedChanged( object sender, EventArgs e )
        {
            //Actions.actions[StateManager.State.Lobby_AllPlayersReady].setPlaySound( chkOnAllReadyPlaySound.Checked, checkBox1.Checked );
        }

        private void button2_Click( object sender, EventArgs e )
        {
            previewSound( cmbSound.Text, chkLooped.Checked );
        }

        private void comboBox1_SelectedIndexChanged( object sender, EventArgs e )
        {
            var selectedAction = getSelectedAction();
            if ( selectedAction == null )
                return;

            //chkPlaySound.Checked = true;

            var sound = Sound.getSoundFromSoundName( cmbSound.Text );

            selectedAction.setSoundName( sound );
        }

        private void checkBox7_CheckedChanged( object sender, EventArgs e )
        {
            var selectedAction = getSelectedAction();
            if ( selectedAction == null )
                return;

            //chkPlaySound.Checked = true;

            selectedAction.setPlaySoundLooped( chkLooped.Checked );
        }

        private void checkBox1_CheckedChanged( object sender, EventArgs e )
        {
            //chkOnAllReadyPlaySound.Checked = true;

            //Actions.actions[ StateManager.State.Lobby_AllPlayersReady ].setPlaySound( chkOnAllReadyPlaySound.Checked, checkBox1.Checked );
        }

        private void button3_Click( object sender, EventArgs e )
        {
        }

        private void chkMute_CheckedChanged( object sender, EventArgs e )
        {
            Config.setConfigValue( Config.keyGeneralMuteAllSounds, chkMute.Checked );

            if ( chkMute.Checked )
            {
                Actions.stopSound();
            }
        }

        public bool isSoundMuted()
        {
            return chkMute.Checked;
        }

        private void checkBox5_CheckedChanged( object sender, EventArgs e )
        {
            //Actions.actions[ StateManager.State.LoadingMatch_AlmostDone ].setPlaySound( checkBox5.Checked, checkBox2.Checked );
        }

        private void cmbOnStartingGameSound_SelectedIndexChanged( object sender, EventArgs e )
        {
        }

        private void enableActionsGUI()
        {
            chkBringToFront.Enabled = true;
            chkPlaySound.Enabled = true;
            cmbSound.Enabled = true;
            btnPlay.Enabled = true;
            chkLooped.Enabled = true;
        }

        private void lstEvents_SelectedIndexChanged( object sender, EventArgs e )
        {
            var selectedAction = getSelectedAction();
            if ( selectedAction == null )
                return;

            enableActionsGUI();

            chkBringToFront.Checked = selectedAction.getBringGameToFront();

            // Set selected sound
            var soundFile = selectedAction.getSoundName();
            if ( soundFile == SoundsEnum.None )
            {
                cmbSound.Text = "";     // No sound selected
            }
            else
            {
                cmbSound.Text = Sound.getSoundNameFromSound( soundFile );
            }

            chkPlaySound.Checked = selectedAction.getMustPlaySound();
            chkLooped.Checked = selectedAction.getMustPlayLooped();
        }

        private void chkActionsEnabled_CheckedChanged( object sender, EventArgs e )
        {
            Config.setConfigValue( Config.keyGeneralActionsEnabled, chkActionsEnabled.Checked );

            onActionsEnabled( chkActionsEnabled.Checked );
        }

        private void onActionsEnabled(bool b)
        {
            grpActions.Enabled = b;
            Actions.Enabled = b;
        }

        private void grpStats_Enter( object sender, EventArgs e )
        {

        }

        private void btnRefreshStats_Click( object sender, EventArgs e )
        {
            
        }

        public void recalcStats()
        {
            //bool bCustomGames = chkIncludeCustomGames.Checked;

            // Solo escape rate
            (int mySoloEscapeRate, int numMySoloGames) = Stats.getStatsEscapeRateAsSoloSurvivorAsPercentage();
            lblMyEscapeRateValue.Text = formatIntPercentage( mySoloEscapeRate );
            lblNumMySoloGames.Text = formatNumGames( numMySoloGames );

            // SWF escape rate
            (int mySwfEscapeRate, int numMySwfGames) = Stats.getStatsEscapeRateAsSwfSurvivorAsPercentage();
            lblStatsSwfEscapeRateValue.Text = formatIntPercentage( mySwfEscapeRate ) + " ";
            lblNumMySwfGames.Text = formatNumGames( numMySwfGames );

            // My killrate
            ( int myKillratePercentage, double myKillrateXk, int numMyKillerGames) = 
                Stats.getStatsMyKillRateAsKillerAsPercentage();
            lblMyKillrateValue.Text = formatIntPercentage( myKillratePercentage );
            lblNumMyKillerGames.Text = formatNumGames( numMyKillerGames );

            // Add kills in k
            if ( myKillrateXk != Stats.invalidDoubleValue )
                lblMyKillrateValue.Text = lblMyKillrateValue.Text + " (" + myKillrateXk + "k)";

            // Playtime
            int numMySoloAndSwfGames = numMySoloGames + numMySwfGames;
            int allMyGames = numMySoloAndSwfGames + numMyKillerGames;
            if ( allMyGames > 0 )   // avoid division by 0
            {
                int playingSurvivorInPercentage = ( int )Math.Round( ( float )numMySoloAndSwfGames * 100f / ( float )allMyGames, 0 );
                int playingKillerInPercentage = 100 - playingSurvivorInPercentage;

                lblSurvivorKillerPlaytimeValue.Text = formatIntPercentage( playingSurvivorInPercentage ) +
                    " / " + formatIntPercentage( playingKillerInPercentage );
            }

            lblNumMyTotalGames.Text = formatNumGames( allMyGames );

            // My BP
            lblMyBPValue.Text = formatBP(Stats.getStatsSurvivorAverageBP()) + " / " +
                formatBP(Stats.getStatsKillerAverageBP());
            

            // Other survivors escape rate
            int escapeRateOfOthersInSolo = Stats.getStatsEscapeRateOfOtherSurvivorsInMySoloGames();
            lblOthersEscapeRateValue.Text = formatIntPercentage( escapeRateOfOthersInSolo );

            int escapeRateOfOthersInSwf = Stats.getStatsEscapeRateOfOtherSurvivorsDuringMySwfGamesAsPercentage();
            lblOthersSwfEscapeRateValue.Text = formatIntPercentage( escapeRateOfOthersInSwf );

            (int killrate, double killrateXk) = Stats.getStatsOtherKillRateAsKillerAsPercentage();
            lblOthersKillrateValue.Text = formatIntPercentage( killrate );

            // Add kills in k
            if ( killrateXk != Stats.invalidDoubleValue )
                lblOthersKillrateValue.Text = lblOthersKillrateValue.Text + " (" + killrateXk + "k)";

            // BP
            lblOthersBPValue.Text = formatBP(Stats.getStatsOtherSurvivorAverageBP());
            lblOthersBPValue.Text = lblOthersBPValue.Text + " / " + formatBP( Stats.getStatsOtherKillersAverageBP() );

            string formatIntPercentage( int count )
            {
                if ( count == Stats.invalidIntValue )
                    return "-";

                return count.ToString() + "%";
            }

            string formatBP( int count )
            {
                if ( count == Stats.invalidIntValue || count == EndscoreBpDigitGfx.INVALID_BP_AMOUNT )
                    return "-";

                return count.ToString();
            }

            string formatNumGames( int count )
            {
                //if(count == 1)
                //    return count.ToString() + " game";

                return count.ToString();
            }

            //string formatDoublePercentage( double count )
            //{
            //    if ( count == Stats.invalidDoubleValue )
            //        return "-";

            //    return count.ToString() + "%";
            //}
        }

        private void btnStopSound_Click( object sender, EventArgs e )
        {
            Actions.stopSound();
        }

        public bool isAddNewGameResultsEnabled()
        {
            return chkStatsEnabled.Checked;
        }

        private void chkOnTop_CheckedChanged( object sender, EventArgs e )
        {
            this.TopMost = chkOnTop.Checked;

            Config.setConfigValue( Config.keyGeneralAlwaysOnTopOn, chkOnTop.Checked );

        }

        private void chkStatsEnabled_CheckedChanged( object sender, EventArgs e )
        {
            Config.setConfigValue( Config.keyGeneralNewGameResultsEnabled, chkStatsEnabled.Checked );
        }

        private void chkPlaySoundOnStatsSave_CheckedChanged( object sender, EventArgs e )
        {
            Config.setConfigValue( Config.keyGeneralSoundOnNewStats, chkPlaySoundOnStatsSave.Checked );
        }
    }
}
