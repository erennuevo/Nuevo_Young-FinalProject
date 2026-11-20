using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Nuevo_Young_FinalProject2
{
    internal class Leaderboard
    {
        private XmlDocument doc;
        private XmlElement rootNode;
        private string leaderboardPath = "Leaderboard.xml";

        public Leaderboard()
        {
            doc = new XmlDocument();

            // Create the XML Doc the first time the game runs.
            if (!File.Exists(leaderboardPath))
            {
                CreateLeaderboardFile();
            }

            try
            {
                doc.Load(leaderboardPath);
            }
            catch (XmlException)
            {
                CreateLeaderboardFile();
                doc.Load(leaderboardPath);
            }

            rootNode = doc.DocumentElement;
        }

        // Create an XML Doc with an empty root node.
        private void CreateLeaderboardFile()
        {
            XmlDocument newDoc = new XmlDocument();
            newDoc.AppendChild(newDoc.CreateXmlDeclaration("1.0", "UTF-8", null));
            newDoc.AppendChild(newDoc.CreateElement("Leaderboard"));
            newDoc.Save(leaderboardPath);
        }

        // Save a winning name and time to the XML Doc. Empty names become "Guest".
        public void AddEntry(string name, double seconds)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = "Guest";

            XmlElement entryTag = doc.CreateElement("Entry");

            XmlElement nameTag = doc.CreateElement("Name");
            nameTag.InnerText = name.Trim();
            entryTag.AppendChild(nameTag);

            XmlElement secondsTag = doc.CreateElement("Seconds");
            secondsTag.InnerText = seconds.ToString("F2", CultureInfo.InvariantCulture);
            entryTag.AppendChild(secondsTag);

            rootNode.AppendChild(entryTag);
            doc.Save(leaderboardPath);
        }

        // Get the fastest entries to display, e.g. "1. Eren - 31.80s".
        public List<string> GetTopScores(int count)
        {
            doc.Load(leaderboardPath);
            rootNode = doc.DocumentElement;

            List<(string Name, double Seconds)> entries = new List<(string, double)>();

            foreach (XmlNode child in rootNode.ChildNodes)
            {
                string name = child["Name"].InnerText;
                double seconds = double.Parse(child["Seconds"].InnerText, CultureInfo.InvariantCulture);
                entries.Add((name, seconds));
            }

            return entries
                .OrderBy(e => e.Seconds)
                .Take(count)
                .Select((e, i) => $"{i + 1}. {e.Name} - {e.Seconds:F2}s")
                .ToList();
        }
    }
}