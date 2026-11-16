namespace Nuevo_Young_FinalProject2
{
    public enum LetterState
    {
        Empty,
        Grey,
        Yellow,
        Green
    }

    public class WordleApp
    {
        public string targetWord;
        public int currentRow;
        public bool isGameOver;
        public bool hasWon;

        public WordleApp(string targetWord)
        {
            this.targetWord = targetWord.ToUpper();
            currentRow = 0;
            isGameOver = false;
            hasWon = false;
        }

        public LetterState[] EvaluateGuess(string guess)
        {
            guess = guess.ToUpper();
            LetterState[] guessResult = new LetterState[5];
            bool[] lettersEvaluated = new bool[5];

            // Find correct letters in the guess.
            for (int i = 0; i < 5; i++)
            {
                if (guess[i] == targetWord[i])
                {
                    guessResult[i] = LetterState.Green;
                    lettersEvaluated[i] = true;
                }
            }

            for (int i = 0; i < 5; i++)
            {
                if (guessResult[i] == LetterState.Green)
                    continue;

                // Loop through the target word to find letters in the guess that
                // are in but in another position.
                bool foundElsewhere = false;
                for (int j = 0; j < 5; j++)
                {
                    if (!lettersEvaluated[j] && guess[i] == targetWord[j])
                    {
                        foundElsewhere = true;
                        lettersEvaluated[j] = true;
                        break;
                    }
                }

                if (foundElsewhere)
                {
                    guessResult[i] = LetterState.Yellow;
                } 
                else
                {
                    guessResult[i] = LetterState.Grey;
                }
            }

            // Update game progress.
            if (guess == targetWord)
            {
                hasWon = true;
                isGameOver = true;
            }
            else if (currentRow >= 5)
            {
                isGameOver = true;
            }
            else
            {
                currentRow++;
            }

            return guessResult;
        }
    }
}
