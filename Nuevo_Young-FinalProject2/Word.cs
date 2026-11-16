using System;
using System.Collections.Generic;
using System.Xml;

namespace Nuevo_Young_FinalProject2
{
    internal class Word
    {
        private XmlDocument doc;
        private XmlElement rootNode;
        private string wordsPath = "Words.xml";
        private List<string> words;

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

                foreach (XmlNode child in rootChildren)
                {
                    words.Add(child.InnerText);
                }
            }
        }

        // Get a random word to guess from the word list.
        public string GetRandomWord()
        {
            Random rand = new Random();
            return words[rand.Next(words.Count)];
        }

        // Check if a user's guess is a valid word.
        public bool IsValidWord(string guess)
        {
            XmlNode validWord = rootNode.SelectSingleNode($"Word[text()='{guess.ToUpper()}']");
            if (validWord != null)
            {
                return true;
            }
            return false;
        }
    }
}
