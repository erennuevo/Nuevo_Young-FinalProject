using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Nuevo_Young_FinalProject2
{
    public partial class Form1 : Form
    {
        private Word wordList;
        private WordleApp game;
        private int currentColumn;
        private Label[,] gridLabels;
        public Form1()
        {
            InitializeComponent();
            BuildWordsXml();

            gridLabels = new Label[6, 5]
            {
                { label1,  label2,  label3,  label4,  label5 },
                { label6,  label7,  label8,  label9,  label10 },
                { label11, label12, label13, label14, label15 },
                { label16, label17, label18, label19, label20 },
                { label21, label22, label23, label24, label25 },
                { label26, label27, label28, label29, label30 }
            };

            wordList = new Word();
            StartNewGame();
        }

        // When running the form, populate the XML Doc with words from the txt file (only if XML Doc is empty).
        // Words from the txt file were copy pasted from https://eslforums.com/5-letter-words/.
        private void BuildWordsXml()
        {
            string rawTextPath = "rawWords.txt";
            string xmlPath = "Words.xml";

            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            XmlElement rootNode = doc.DocumentElement;

            if (rootNode.HasChildNodes)
            {
                return;
            }

            string[] rawWords = File.ReadAllText(rawTextPath)
                .Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in rawWords)
            {
                if (word.Length == 5)
                {
                    XmlElement wordTag = doc.CreateElement("Word");
                    XmlText wordValue = doc.CreateTextNode(word.ToUpper());
                    wordTag.AppendChild(wordValue);
                    rootNode.AppendChild(wordTag);
                }
            }

            doc.Save(xmlPath);
        }

        private void StartNewGame()
        {
            string secretWord = wordList.GetRandomWord();
            game = new WordleApp(secretWord);
            currentColumn = 0;

            // Clear all 30 labels
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    gridLabels[row, col].Text = "";
                    gridLabels[row, col].BackColor = Color.White;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (game.isGameOver)
                return;

            // Typing A-Z
            if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z)
            {
                if (currentColumn < 5)
                {
                    gridLabels[game.currentRow, currentColumn].Text = e.KeyCode.ToString();
                    currentColumn++;
                }
            }
            // Pressing Backspace
            else if (e.KeyCode == Keys.Back)
            {
                if (currentColumn > 0)
                {
                    currentColumn--;
                    gridLabels[game.currentRow, currentColumn].Text = "";
                }
            }
            // Pressing Enter
            else if (e.KeyCode == Keys.Enter)
            {
                SubmitGuess();
            }
        }

        private void SubmitGuess()
        {
            if (currentColumn < 5)
            {
                MessageBox.Show("Not enough letters.");
                return;
            }

            // Combine the 5 labels in the current row into one word.
            string guess = "";
            for (int c = 0; c < 5; c++)
            {
                guess += gridLabels[game.currentRow, c].Text;
            }

            // Check if the word exists in Words.xml.
            if (!wordList.IsValidWord(guess))
            {
                MessageBox.Show("Invalid word.");
                return;
            }

            int rowToColor = game.currentRow;
            LetterState[] results = game.EvaluateGuess(guess);

            // Color the 5 labels in that row.
            for (int c = 0; c < 5; c++)
            {
                switch (results[c])
                {
                    case LetterState.Green:
                        gridLabels[rowToColor, c].BackColor = Color.SeaGreen;
                        break;
                    case LetterState.Yellow:
                        gridLabels[rowToColor, c].BackColor = Color.Gold;
                        break;
                    case LetterState.Grey:
                        gridLabels[rowToColor, c].BackColor = Color.LightGray;
                        break;
                }
            }

            currentColumn = 0;

            if (game.hasWon)
            {
                MessageBox.Show("You win!");
            }
            else if (game.isGameOver)
            {
                MessageBox.Show($"Game over! The word was {game.targetWord}.");
            }
        }
    }
}
