
using System.Collections.Generic;

namespace Tpix.ResourceData
{
    [System.Serializable]
    public class GameInputData
    {
        public string           Key;
        public List< int >      Mechanics;
        public List< string >   QuestionKeys;
        public bool             IsRandom;

        public int NQuestions => QuestionKeys.Count;
    }
}