using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
        private Panel keyboardPanel;
        private Dictionary<char, Button> keyboardButtons;
        private Dictionary<char, LetterState> letterStates;
        private Label feedbackLabel;
        private Button newGameButton;

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
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
            BuildKeyboard();
            BuildFeedbackLabel();
            BuildNewGameButton();
            ArrangeGameLayout();
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
            feedbackLabel.Text = "";

            // Clear all 30 labels
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    gridLabels[row, col].Text = "";
                    gridLabels[row, col].BackColor = Color.White;
                }
            }

            // Reset keyboard
            foreach (char letter in letterStates.Keys.ToList())
            {
                letterStates[letter] = LetterState.Empty;

                keyboardButtons[letter].BackColor = Color.LightGray;
                keyboardButtons[letter].ForeColor = Color.Black;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (game.isGameOver)
                return;

            // Typing A-Z
            if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z)
            {
                AddLetter(e.KeyCode.ToString()[0]);
            }

            // Pressing Backspace
            else if (e.KeyCode == Keys.Back)
            {
                DeleteLetter();
            }

            // Pressing Enter
            else if (e.KeyCode == Keys.Enter)
            {
                SubmitGuess();
            }

            e.SuppressKeyPress = true;
        }

        private void SubmitGuess()
        {
            if (currentColumn < 5)
            {
                ShowFeedback("Not enough letters!");
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
                ShowFeedback("Invalid word.");
                return;
            }

            int rowToColor = game.currentRow;
            LetterState[] results = game.EvaluateGuess(guess);
            UpdateKeyboardColors(guess, results);

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
                ShowFeedback("You win!");
            }
            else if (game.isGameOver)
            {
                ShowFeedback($"Game over! The word was {game.targetWord}.");
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        // Build Keyboard
        private void BuildKeyboard()
        {
            keyboardPanel = new Panel();
            keyboardPanel.Size = new Size(370, 145);

            this.Controls.Add(keyboardPanel);

            keyboardButtons = new Dictionary<char, Button>();
            letterStates = new Dictionary<char, LetterState>();

            string[] rows =
            {
                "QWERTYUIOP",
                "ASDFGHJKL",
                "ZXCVBNM"
            };

            int buttonWidth = 32;
            int buttonHeight = 38;
            int gap = 4;

            for (int r = 0; r < rows.Length; r++)
            {
                int rowWidth = rows[r].Length * (buttonWidth + gap) - gap;

                // Center each keyboard row
                int startX = (keyboardPanel.Width - rowWidth) / 2;
                int y = r * (buttonHeight + gap);

                for (int c = 0; c < rows[r].Length; c++)
                {
                    char letter = rows[r][c];

                    Button key = new Button();

                    key.Text = letter.ToString();
                    key.Size = new Size(buttonWidth, buttonHeight);
                    key.Location = new Point(
                        startX + c * (buttonWidth + gap), y);

                    key.BackColor = Color.LightGray;
                    key.FlatStyle = FlatStyle.Flat;
                    key.FlatAppearance.BorderSize = 0;
                    key.Font = new Font("Arial", 10, FontStyle.Bold);
                    key.Tag = letter;

                    key.Click += KeyboardLetter_Click;

                    keyboardPanel.Controls.Add(key);

                    keyboardButtons.Add(letter, key);
                    letterStates.Add(letter, LetterState.Empty);
                    key.TabStop = false;
                }
            }

            // Enter button
            Button enterButton = new Button();
            enterButton.Text = "ENTER";
            enterButton.Size = new Size(55, buttonHeight);
            enterButton.Location = new Point(0, 84);
            enterButton.Font = new Font("Arial", 8, FontStyle.Bold);
            enterButton.Click += (s, e) => SubmitGuess();
            enterButton.TabStop = false;

            // Backspace button
            Button backButton = new Button();
            backButton.Text = "DEL";
            backButton.Size = new Size(55, buttonHeight);
            backButton.Location = new Point(315, 84);
            backButton.Font = new Font("Arial", 8, FontStyle.Bold);
            backButton.Click += (s, e) => DeleteLetter();
            backButton.TabStop = false;

            keyboardPanel.Controls.Add(enterButton);
            keyboardPanel.Controls.Add(backButton);
        }

        // Clicking the on-screen keyboard will call AddLetter()
        private void KeyboardLetter_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            char letter = (char)clickedButton.Tag;
            AddLetter(letter);
        }

        private void AddLetter(char letter)
        {
            if (game.isGameOver || currentColumn >= 5)
                return;

            gridLabels[game.currentRow, currentColumn].Text =
                letter.ToString();

            currentColumn++;
        }

        // Backspace functionality
        private void DeleteLetter()
        {
            if (game.isGameOver || currentColumn <= 0)
                return;

            currentColumn--;
            gridLabels[game.currentRow, currentColumn].Text = "";
        }

        // Update keyboard colors after every guess
        private void UpdateKeyboardColors(string guess, LetterState[] results)
        {
            for (int i = 0; i < 5; i++)
            {
                char letter = guess[i];

                LetterState newState = results[i];

                LetterState oldState = letterStates[letter];

                // Green has the highest priorit -> Yellow -> Gray
                if (newState == LetterState.Green ||
                    (newState == LetterState.Yellow &&
                     oldState != LetterState.Green) ||
                    (newState == LetterState.Grey &&
                     oldState == LetterState.Empty))
                {
                    letterStates[letter] = newState;
                }

                Button key = keyboardButtons[letter];

                switch (letterStates[letter])
                {
                    case LetterState.Green:
                        key.BackColor = Color.SeaGreen;
                        key.ForeColor = Color.White;
                        break;

                    case LetterState.Yellow:
                        key.BackColor = Color.Gold;
                        key.ForeColor = Color.Black;
                        break;

                    case LetterState.Grey:
                        key.BackColor = Color.DimGray;
                        key.ForeColor = Color.White;
                        break;
                }
            }
        }

        // Add a feedback label
        private void BuildFeedbackLabel()
        {
            feedbackLabel = new Label();

            feedbackLabel.Text = "";
            feedbackLabel.Size = new Size(370, 30);
            feedbackLabel.Location = new Point(35, 0);

            feedbackLabel.Font = new Font("Arial", 11, FontStyle.Bold);
            feedbackLabel.ForeColor = Color.Black;
            feedbackLabel.TextAlign = ContentAlignment.MiddleCenter;

            this.Controls.Add(feedbackLabel);
        }

        // Displaying feedback
        private void ShowFeedback(string message)
        {
            feedbackLabel.Text = message;
        }

        // Automatically adjusts the format
        private void ArrangeGameLayout()
        {
            int minX = int.MaxValue;
            int maxX = 0;
            int boardBottom = 0;

            // Find the boundaries of the existing board
            foreach (Label label in gridLabels)
            {
                minX = Math.Min(minX, label.Left);
                maxX = Math.Max(maxX, label.Right);
                boardBottom = Math.Max(boardBottom, label.Bottom);
            }

            // Center the board horizontally
            int boardWidth = maxX - minX;
            int offsetX = (ClientSize.Width - boardWidth) / 2 - minX;

            foreach (Label label in gridLabels)
            {
                label.Left += offsetX;
                label.Top += 65;
            }

            boardBottom += 65;

            // Position the keyboard below the board
            keyboardPanel.Location = new Point(
                (ClientSize.Width - keyboardPanel.Width) / 2,
                boardBottom + 20
            );

            // Position feedback below the New Game button
            feedbackLabel.Location = new Point(
                (ClientSize.Width - feedbackLabel.Width) / 2,
                45
            );

            // Position New Game button above feedback
            newGameButton.Location = new Point(
                (ClientSize.Width - newGameButton.Width) / 2,
                5
            );

            // Adjust form height
            ClientSize = new Size(
                ClientSize.Width,
                keyboardPanel.Bottom + 20
            );
        }

        // Make new game
        private void BuildNewGameButton()
        {
            newGameButton = new Button();

            newGameButton.Text = "NEW GAME";
            newGameButton.Size = new Size(110, 40);
            newGameButton.Location = new Point(0, 0);

            newGameButton.Font = new Font("Arial", 9, FontStyle.Bold);
            newGameButton.TextAlign = ContentAlignment.MiddleCenter;

            newGameButton.BackColor = Color.FromArgb(83, 141, 78);
            newGameButton.ForeColor = Color.White;

            newGameButton.FlatStyle = FlatStyle.Flat;
            newGameButton.FlatAppearance.BorderSize = 0;
            newGameButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(65, 115, 60);
            newGameButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 95, 45);

            newGameButton.UseVisualStyleBackColor = false;
            newGameButton.Cursor = Cursors.Hand;

            newGameButton.Click += (s, e) => StartNewGame();

            this.Controls.Add(newGameButton);
            newGameButton.BringToFront();
            newGameButton.TabStop = false;
        }
    }
}
