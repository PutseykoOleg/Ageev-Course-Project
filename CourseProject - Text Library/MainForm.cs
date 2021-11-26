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
        // Путь до файла считывания/записи
        private const string PATH_TO_FILE = "./text.txt";

        /** Текст, содержащийся в поле ввода
         * 
         * Отношение композиции (MainForm - Text)
         */
        private Text text = new Text();

        // Индексы считанных предложений и слов
        private int indexOfReadedSentence = 0;
        private int indexOfReadedWord = 0;

        // Конструктор класса
        public MainForm()
        {
            // Инициализация формы
            InitializeComponent();
        }

        // Метод демонстрации ошибки
        public void ShowError(string message)
        {
            // Вывод сообщения в форму
            errorMessage.Text = message;

            // Подсветка кнопки исправления текста
            correctButton.BackColor = Color.FromArgb(255, 192, 192);
            correctButton.ForeColor = Color.FromArgb(192, 0, 0);
        }

        // Метод удаляющий ошибку
        public void HideError()
        {
            // Удаление сообщения
            errorMessage.Text = "";

            // Подсветка кнопки исправления текста
            correctButton.BackColor = SystemColors.Control;
            correctButton.ForeColor = SystemColors.ControlText;
        }

        // Метод, вызывающийся при загрузке формы
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Установка списков предложений и слов только для чтения
            listOfSentences.ReadOnly = true;
            listOfWords.ReadOnly = true;

            // Линковка обработчика ошибок и созданной формы
            ErrorHandler.form = this;
        }

        // Метод, вызывающийся при изменении текста пользователем
        private void textArea_TextChanged(object sender, EventArgs e)
        {
            // Обнуление списков предложений и слов
            listOfSentences.Text = "";
            listOfWords.Text = "";

            // Удаление ошибки
            ErrorHandler.UnsetError();
            HideError();

            // Инициализация объекта текста и вместе с тем вызов ошибок, если они имеются
            text.instance = textArea.Text;
            // Общее количество слов
            int countOfWords = 0;

            // Проход по каждому найденному предложению
            for (int i = 0; i < text.containedSentences.Count; i++)
            {
                // Текущее предложение
                Sentence sentence = text.containedSentences[i];

                // Вывод найденного предложения
                listOfSentences.Text += (i + 1) + ") " + sentence.instance + Environment.NewLine;

                // Проход по каждому найденному слову
                foreach (Word word in sentence.containedWords)
                {
                    // Вывод найденного слова
                    listOfWords.Text += (++countOfWords) + ") " + word.instance + Environment.NewLine;
                }
            }
        }

        // Метод, вызывающийся при нажатии на кнопку считывания текста
        private void readTextButton_Click(object sender, EventArgs e)
        {
            // Целевой объект считывания
            Text text = new Text();

            // Считывание текста из файла
            FileHandler.ReadFromFile(PATH_TO_FILE, text);

            // Если текст не пуст, т.е. корректен и был успешно считан, вывод его в поле ввода
            if (!text.IsEmpty())
            {
                // Добавление считанного текста в конец к введенному
                textArea.Text += text.instance;
            }
        }

        // Метод, вызывающийся при нажатии на кнопку считывания предложения
        private void readSentenceButton_Click(object sender, EventArgs e)
        {
            // Целевой объект считывания
            Sentence sentense = new Sentence();

            // Считывание предложения из файла
            FileHandler.ReadFromFile(PATH_TO_FILE, sentense, indexOfReadedSentence++);

            // Если предложение не пусто, т.е. корректно и было успешно считано, вывод его в поле ввода
            if (!sentense.IsEmpty())
            {
                // Добавление считанного предложения в конец к введенному тексту
                textArea.Text += sentense.instance;
            }
        }

        // Метод, вызывающийся при нажатии на кнопку считывания слова
        private void readWordButton_Click(object sender, EventArgs e)
        {
            // Целевой объект считывания
            Word word = new Word();

            // Считывание слова из файла
            FileHandler.ReadFromFile(PATH_TO_FILE, word, indexOfReadedWord++);

            // Если слово не пусто, т.е. корректно и было успешно считано, вывод его в поле ввода
            if (!word.IsEmpty())
            {
                // Добавление считанного слова в конец к введенному тексту
                textArea.Text += word.instance;
            }
        }

        // Метод, вызывающийся при нажатии на кнопку записи введенного текста в файл
        private void writeButton_Click(object sender, EventArgs e)
        {
            // Запись введенного текста в файл
            FileHandler.WriteToFile(PATH_TO_FILE, text);
        }

        private void correctButton_Click(object sender, EventArgs e)
        {
            // Выключаем логи, чтобы при обновлении текстового поля не выскачили
            //ErrorHandler.log = false;

            Text text = new Text();
            text.SetWithCorrecting(textArea.Text);

            textArea.Text = !text.IsEmpty() ? text.instance : "";

            //ErrorHandler.log = true;

            // Убираем ошибку
            ErrorHandler.UnsetError();
            HideError();
        }
    }
}
