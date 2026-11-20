using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Nuevo_Young_FinalProject2
{
    internal class Word
    {
        private XmlDocument doc;
        private XmlElement rootNode;
        private string wordsPath = "Words.xml";
        private List<string> words;
        private static Random rand = new Random();
        private HashSet<string> validWords;

        public Word()
        {
            doc = new XmlDocument();
            doc.Load(wordsPath);
            rootNode = doc.DocumentElement;
            CreateWordList();
        }

        // Turn the XML file into a list of strings.
        public void CreateWordList()
        {
            if (words == null)
            {
                words = new List<string>();
                XmlNodeList rootChildren = rootNode.ChildNodes;
                validWords = new HashSet<string>();

                foreach (XmlNode child in rootChildren)
                {
                    if (child is XmlElement && child.Name == "Word")
                    {
                        string word = child.InnerText.Trim().ToUpper();

                        if (validWords.Add(word))
                        {
                            words.Add(word);
                        }
                    }
                }
            }
        }

        // Get a random word to guess from the word list.
        public string GetRandomWord()
        {
            if (words.Count == 0)
                throw new InvalidOperationException("No valid words found.");

            return words[rand.Next(words.Count)];
        }

        // Check if a user's guess is a valid word.
        public bool IsValidWord(string guess)
        {
            guess = guess.Trim().ToUpper();

            if (guess.Length != 5 || !guess.All(char.IsLetter))
                return false;

            return validWords.Contains(guess);
        }
    }
}
