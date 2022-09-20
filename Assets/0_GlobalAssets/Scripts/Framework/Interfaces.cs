
using Tpix.ResourceData;
using Tpix.UserData;

namespace Tpix
{
    public interface IJSon
    {
        string ToJSon( );
    }

    public interface IGameInputReciever
    {
        void OnInput( GameInputData data );
        void TerminateGame( );
    }

    public interface IGameOutputReciever
    {
        void OnOutput( GameResultData data );
        void OnTerminate(GameResultData data);
    }

    public interface IProgressReciever
    {
        void OnProgressUpdate( float perc );
    }

    public interface IOAKSGame
    {
        void StartGame(GameInputData data);
        void CleanUp( );
    }


}