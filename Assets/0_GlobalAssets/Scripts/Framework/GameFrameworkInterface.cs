using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using Tpix.ResourceData;
using Tpix.UserData;
using Tpix.Utilities;
using UnityEngine;
using Utility = Tpix.Utilities.Utility;

namespace Tpix
{
    public class GameFrameworkInterface : Singleton< GameFrameworkInterface >, IGameInputReciever
    {
        private IProgressReciever       ProgressReciever;
        private IGameOutputReciever     OutputReciever;
        private GameResultData          Result;
        private GameInputData           InputData;
        private bool                    IsGameActive = false;
        private Dictionary< string, QAResult >  QuestionResults;
        private IOAKSGame               Game;

        // ------------------------------------------------------------------------
        // Name :   OnInput
        // Desc :   This is the entry point into the game.
        // ------------------------------------------------------------------------

        public void OnInput( GameInputData data )
        {
            InputData = data;
            
            // Find the progress recievers in the scene and verify there is only 1 reciever.
            var progressRecievers = Utility.FindInterfacesOfType< IProgressReciever >( );
            Assert.IsTrue( progressRecievers.Count == 1 );
            ProgressReciever = progressRecievers[ 0 ];

            // Find the output recievers in the scene and verify there is only 1 reciever.
            var outputRecievers   = Utility.FindInterfacesOfType< IGameOutputReciever >( );
            Assert.IsTrue( outputRecievers.Count == 1 );
            OutputReciever = outputRecievers[ 0 ];

            // Find the games in the scene and verify there is only 1 game running at a time.
            var games   = Utility.FindInterfacesOfType< IOAKSGame >( );
            Assert.IsTrue( games.Count == 1 );
            Game = games[ 0 ];
            
            Result = new GameResultData( );
            Result.SetKey( data.Key );
            // Mark the start time.
            Result.SetStartTime( DateTime.Now );
            // Initialize results.
            InitializeResults( );
            // Everything is ready to start the game.
            IsGameActive = true;
            // Reset the progress bar.
            ProgressReciever.OnProgressUpdate( 0f );
            // Start game.
            Game.StartGame( InputData );
        }
        
        // ------------------------------------------------------------------------
        // Name :   CleanUp
        // Desc :   -
        // ------------------------------------------------------------------------

        public void CleanUp( )
        {
            IsGameActive = false;
            ProgressReciever = null;
            OutputReciever = null;
            Game = null;
            InputData = null;
            QuestionResults.Clear( );
            Result = null;
        }

        // ------------------------------------------------------------------------
        // Name :   InitializeResults
        // Desc :   -
        // ------------------------------------------------------------------------

        private void InitializeResults( )
        {
            QuestionResults = new Dictionary< string, QAResult >( );
            InputData.QuestionKeys.ForEach( key => QuestionResults.Add( key, QAResult.NoAttempt ) );
        }

        // ------------------------------------------------------------------------
        // Name :   ReplaceQuestionKeys
        // Desc :   -
        // ------------------------------------------------------------------------

        public void ReplaceQuestionKeys( List< string > questionKeys ) 
        {
            InputData.QuestionKeys = questionKeys.ToList( );
            InitializeResults( );
        }

        // ------------------------------------------------------------------------
        // Name :   AddResult
        // Desc :   Add / modify question result.
        // Call :   GameFrameworkInterface.Instance.AddResult( "Q0", QAResult.Wrong )
        // ------------------------------------------------------------------------

        public void AddResult( string qKey, QAResult result )
        {
            if( ! IsGameActive ) { ShowGameNotActiveError( ); return; }

            Assert.IsTrue( InputData.QuestionKeys.Contains( qKey ), $"Question key is invalid : {qKey}" );
            QuestionResults[ qKey ] = result;
        }
        
        // ------------------------------------------------------------------------
        // Name :   UpdateProgress
        // Desc :   Call this when the game needs to update progress.
        // ------------------------------------------------------------------------

        public void UpdateProgress( float perc )
        {
            if( ! IsGameActive ) { ShowGameNotActiveError( ); return; }

            ProgressReciever.OnProgressUpdate( perc );
        }

        // ------------------------------------------------------------------------
        // Name :   SetCustomJson
        // Desc :   -
        // ------------------------------------------------------------------------

        public void SetCustomJson( string jsonStr )
        {
            if( ! IsGameActive ) { ShowGameNotActiveError( ); return; }

            Result.SetCustomData( jsonStr );
        }

        // ------------------------------------------------------------------------
        // Name :   SendResultToFramework
        // Desc :   When the game is over, call this function.
        // ------------------------------------------------------------------------

        public void SendResultToFramework( bool success = true )
        {
            if( ! IsGameActive ) { ShowGameNotActiveError( ); return; }

            Result.SetEndTime( DateTime.Now );
            Result.SetResult( QuestionResults );
            QuestionResults.Clear( );

            if( success )
                OutputReciever.OnOutput( Result );
            else
                OutputReciever.OnTerminate( Result );

            CleanUp( );
        }

        // ------------------------------------------------------------------------
        // Name :   TerminateGame
        // Desc :   Terminate the game and send the results back to framework.
        //          ( This will be called by the framework, if user pause and quit the game )
        // ------------------------------------------------------------------------

        public void TerminateGame( )
        {
            Game.CleanUp( );
            SendResultToFramework( false );
        }

        // ------------------------------------------------------------------------
        // Name :   ShowGameNotActiveError
        // Desc :   -
        // ------------------------------------------------------------------------

        void ShowGameNotActiveError( ) => Debug.LogError( "Game is not active" );
    }
}