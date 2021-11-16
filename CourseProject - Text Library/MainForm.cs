using TextLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextLibrary
{
    public partial class MainForm : Form
    {
        private const string PATH_TO_FILE = "./text.txt";

        private Text text = new Text("");

        private int indexOfReadedSentence = 0;
        private int indexOfReadedWord = 0;

        public MainForm()
        {
            InitializeComponent();
        }

        public void ShowError(string message)
        {
            errorMessage.Text = message;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            textArea.Text = text.instance;

            listOfSentences.ReadOnly = true;
            listOfWords.ReadOnly = true;

            ErrorHandler.form = this;
        }

        private void textArea_TextChanged(object sender, EventArgs e)
        {
            listOfSentences.Text = "";
            listOfWords.Text = "";
            errorMessage.Text = "";

            text.instance = textArea.Text;
            int countOfWords = 0;

            for (int i = 0; i < text.containedSentences.Count; i++)
            {
                Sentence sentence = text.containedSentences[i];

                listOfSentences.Text += (i + 1) + ") " + sentence.instance + Environment.NewLine;

                foreach (Word word in sentence.containedWords)
                {
                    listOfWords.Text += (++countOfWords) + ") " + word.instance + Environment.NewLine;
                }
            }
        }

        private void readTextButton_Click(object sender, EventArgs e)
        {
            Text text = new Text("");

            FileHandler.ReadFromFile(PATH_TO_FILE, text);

            if (TextLibrary.Text.isCorrect(text.instance))
            {
                textArea.Text += text.instance + " ";
            }
        }

        private void readSentenceButton_Click(object sender, EventArgs e)
        {
            Text text = new Text("");

            FileHandler.ReadFromFile(PATH_TO_FILE, text);

            if (TextLibrary.Text.isCorrect(text.instance) && indexOfReadedSentence < text.containedSentences.Count)
            {
                textArea.Text += text.containedSentences[indexOfReadedSentence].instance + " ";
                indexOfReadedSentence++;
            }
        }

        private void readWordButton_Click(object sender, EventArgs e)
        {
            Text text = new Text("");

            FileHandler.ReadFromFile(PATH_TO_FILE, text);

            indexOfReadedSentence += indexOfReadedSentence < text.containedSentences.Count && 
                indexOfReadedWord < text.containedSentences[indexOfReadedSentence].containedWords.Count ? 0 : 1;
            
            if (
                TextLibrary.Text.isCorrect(text.instance) && 
                indexOfReadedSentence < text.containedSentences.Count &&
                indexOfReadedWord < text.containedSentences[indexOfReadedSentence].containedWords.Count
            )
            {
                textArea.Text += text.containedSentences[indexOfReadedSentence].containedWords[indexOfReadedWord].instance + " ";
                indexOfReadedWord++;
            }
        }

        private void writeButton_Click(object sender, EventArgs e)
        {
            FileHandler.WriteToFile(PATH_TO_FILE, new Text(textArea.Text));
        }
    }
}
