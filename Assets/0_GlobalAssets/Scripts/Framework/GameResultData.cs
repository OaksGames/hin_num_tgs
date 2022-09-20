using System;
using System.Collections.Generic;
using System.Linq;
using Tpix;
using Tpix.Utilities;
using UnityEngine.Assertions;

namespace Tpix.UserData
{
    public enum QAResult { None, Correct, Wrong, NoAttempt, Skip }

// ------------------------------------------------------------------------
// Name	 :	GameResultData
// Desc	 :	Summary of the result of a game session.
// Usage :  1. Create an instance : ResultData = new GameResultData( );
//          2. Set the key the game recieves from GameInputData
//          3. Call SetStartTime
//          4. When game ends
//                  a. Call SetEndTime
//                  b. Set the question results via SetResult
//                  c. If there is any custom json data, call SetCustomData
//          5. Pass the result to framework via FindInterfacesOfType< IGameOutputReciever >( ).OnOutput( ResultData );
// ------------------------------------------------------------------------

    public class GameResultData : IJSon
    {
        public string           Key                     { get; private set; } = string.Empty;               
        public DateTime?        LastAttemptStartTS      { get; private set; } = null;                       // When game starts set this.
        public DateTime?        LastAttemptEndTS        { get; private set; } = null;                       // When game ends set this.
        public float            WinPerc                 { get; private set; } = 0f;                         // When game ends set this.
        public string           CustomData              { get; private set; } = string.Empty;               // When game ends set this. ( if any )
        public Dictionary< string, QAResult > Result    { get; private set; } = new Dictionary< string, QAResult>( ); // Key is question key, when game ends set this.
    
        public float            AttemptDuration 
        {
            get
            {
                if( LastAttemptStartTS.HasValue && LastAttemptEndTS.HasValue )
                    return (float) ( LastAttemptEndTS.Value - LastAttemptStartTS.Value ).TotalSeconds;
                else
                    return -1f;
            }
        }
    
        // ------------------------------------------------------------------------
        // Name	:	SetKey
        // Desc	:	Game key. Eg. S03CH01AN01 for a simple game. For composite games,
        //          key will be like S03CH01AN01_S03CH01AN02_S03CH01AN03
        // ------------------------------------------------------------------------

        public void SetKey( string key ) => Key = key;

        // ------------------------------------------------------------------------
        // Name	 :	SetStartTime
        // Desc	 :	Marks the start time of the game.
        // Usage :  Call SetStartTime( System.DateTime.Now ); at game start.
        // ------------------------------------------------------------------------

        public void SetStartTime( DateTime timeStamp ) => LastAttemptStartTS = timeStamp;

        // ------------------------------------------------------------------------
        // Name	 :	SetEndTime
        // Desc	 :	Marks the end time of the game.
        // Usage :  Call SetEndTime( System.DateTime.Now ); at game end.
        // ------------------------------------------------------------------------

        public void SetEndTime( DateTime timeStamp ) =>  LastAttemptEndTS = timeStamp;

        // ------------------------------------------------------------------------
        // Name	:	SetWinPercentage
        // Desc	:	This is called by SetResult function ( private function )
        // ------------------------------------------------------------------------

        private void SetWinPercentage( float perc ) => WinPerc = perc;

        // ------------------------------------------------------------------------
        // Name	 :	SetResult
        // Desc	 :	Sets the result dictionary.
        // Usage :  Create a dictionary of type Dictionary< string, QAResult >. 
        //          var qResDict = new Dictionary< string, QAResult >( );
        //          Add results
        //              qResDict.Add( "QKey0", QAResult.Correct );
        //              qResDict.Add( "QKey1", QAResult.Wrong ); etc...
        //          Then SetResult( qResDict );
        // ------------------------------------------------------------------------

        public void SetResult( Dictionary< string, QAResult > result ) 
        {
            // Makes sure all results are valid.
            Assert.IsTrue( result.Values.Where( v => v == QAResult.None ).Count( ) == 0, "Forgot to set any result ?" );
            // Create a copy.
            Result = new Dictionary< string, QAResult >( result );
            // Calculate win percentage.
            var nQs = Result.Keys.Count( );
            var nCorrectAns = Result.Values.Where( v => v == QAResult.Correct ).Count( );
            SetWinPercentage( (float) nCorrectAns / (float) nQs );
        }

        // ------------------------------------------------------------------------
        // Name	:	SetCustomData
        // Desc	:	Custom json data if any are there pertaining to this game.
        // ------------------------------------------------------------------------

        public void SetCustomData( string cData ) => CustomData = cData;

        // ------------------------------------------------------------------------
        // Name	:	IsCleared
        // Desc	:	WinPerc > 0.3 to clear a game.
        // ------------------------------------------------------------------------

        public bool IsCleared( ) => WinPerc > 0.3f;

        // ------------------------------------------------------------------------
        // Name	:	IsPlayedThisWeek
        // Desc	:	Check if the game is played this week.
        // ------------------------------------------------------------------------

        public bool IsPlayedThisWeek( ) => LastAttemptStartTS > DateTime.Now.StartTimeOfWeek( DayOfWeek.Sunday );

        // ------------------------------------------------------------------------
        // Name	:	-
        // Desc	:	-
        // ------------------------------------------------------------------------

        public string ToJSon( ) => JsonFileUtility.ToJson( this );

        // ------------------------------------------------------------------------
        // Name	:	MergeResults
        // Desc	:	Given a set of result, merge them to create one result.
        // ------------------------------------------------------------------------

        public static GameResultData MergeResults( List< GameResultData > results, string key )
        {
            // If there is only one result, return the first( and only ) element.
            if( results.Count == 1 ) return results[ 0 ];

            var minStartTime = results.Select( x => x.LastAttemptStartTS.Value ).Min( );
            var maxEndTime   = results.Select( x => x.LastAttemptEndTS.Value ).Max( );
            var qaResults    = results.Select( x => x.Result ).ToList( );

            // Merge the QA results.
            var combinedQAResultDict = new Dictionary< string, QAResult >( );

            for( int i = 0; i < qaResults.Count; i++ )
                foreach( var kv in qaResults[ i ] )
                    combinedQAResultDict.Add( kv.Key, kv.Value );
 
            GameResultData combinedResult = new GameResultData( );
            combinedResult.SetKey( key );
            combinedResult.SetStartTime( minStartTime );
            combinedResult.SetEndTime( maxEndTime );
            combinedResult.SetResult( combinedQAResultDict );

            return combinedResult;
        }
    }
}