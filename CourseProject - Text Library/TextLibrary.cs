using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextLibrary
{
    // Класс, определяющий обработчик событий
    public class ErrorHandler
    {
        // Форма для взаимодействия
        public static MainForm form { private get; set; }

        // Делегат для события Error
        public delegate void Handler(string message);

        // Событие, обозначающее ошибку
        public event Handler Error;

        public ErrorHandler()
        {
            // Добавление обработчика события
            Error += ShowMessage;
        }

        // Установка сообщения об ошибке
        public void SetError(string message)
        {
            Error?.Invoke(message);
        }

        // Обработчик события
        private void ShowMessage(string message)
        {
            // Демонстрация ошибки в диалоговом окне
            form.ShowError("Ошибка: " + message);
        }
    }

    // Абстрактный класс, определяющий единицу языка
    public abstract class LanguageUnit
    {
        // Защищенное поле, хранящее корректное значение единицы языка
        protected string _instance;

        // Публичное абстрактное поле, через которое происходит обращение и обработка хранимого значения
        public abstract string instance { get; set; }
    }

    // Интерфейс, описывающий стираемый объект
    public interface IErasable {
        void Erase();
    }

    // Класс, определяющий методы работы с файлом и представляемыми библиотекой классами единиц языка
    public class FileHandler
    {
        // Метод, считывающий слово, предложение или текст целиком из файла в указанный объект
        public static async void ReadFromFile(string path, LanguageUnit obj, int indexOfSentence = 0, int indexOfWord = 0)
        {
            ErrorHandler errorHandler = new ErrorHandler();
            string strText = "";

            try
            {
                // Считывание целого текста из файла
                using(StreamReader sr = new StreamReader(path))
                {
                    strText = sr.ReadToEnd();
                }
            }
            catch(Exception e)
            {
                // Показать ошибку в консоли
                Console.WriteLine(e.Message);
                return;
            }

            Text text = new Text(strText);

            int newIndexOfSentence = 0;
            int countOfWords = 0;

            if(indexOfSentence < 0)
            {
                countOfWords += text.containedSentences[newIndexOfSentence].containedWords.Count;
                while (indexOfWord > countOfWords)
                {
                    newIndexOfSentence++;
                }
            } else
            {
                newIndexOfSentence = indexOfSentence;
            }

            // Возвращение соответствующей части текста
            if(obj is Word)
            {
                if(text.containedSentences.Count >= newIndexOfSentence)
                {
                    Sentence sentence = text.containedSentences[newIndexOfSentence];

                    if(sentence.containedWords.Count >= indexOfWord + 1)
                    {
                        obj.instance = sentence.containedWords[indexOfWord].instance;
                    }
                    else
                    {
                        errorHandler.SetError("Ошибка обработки считанного предложения");
                    }
                } else
                {
                    errorHandler.SetError("Считанный текст не содержит предложений");
                }
            } 
            else if(obj is Sentence)
            {
                if (text.containedSentences.Count >= indexOfSentence + 1)
                {
                    obj.instance = text.containedSentences[indexOfSentence].instance;
                }
                else
                {
                    errorHandler.SetError("Считанный текст не содержит предложений");
                }
            }
            else if (obj is Text)
            {
                obj.instance = strText;
            }
        }

        public static async void WriteToFile(string path, LanguageUnit obj)
        {
            if(obj is Word || obj is Sentence || obj is Text)
            {
                try
                {
                    using(StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
                    {
                        sw.Write(" " + obj.instance);
                    }
                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }

    /** Класс, определяющий методы работы с текстом и его обработку (содержит логику определения текста)
     *  
     *  Т.к. текст может быть пустым, то в списке наследования присутствует интерфейс IErasable
     */
    public class Text : LanguageUnit, IErasable
    {
        // Содержащиеся корректные предложения в тексте
        public List<Sentence> containedSentences { get; private set; } = new List<Sentence>();

        // Публичное поле, через которое происходит обращение к приватному
        public override string instance
        {
            // При получении значения возвращается хранимый текст
            get => _instance;

            /** При установке значения, новое проверяется на принадлежность единице языка "текст"
             * В случае успешной проверки, записывается в приватное поле для хранения
             * 
             * Признаки текста:
             * - Последовательность символов может быть пустой.
             * - Последовательность символов может содержать сколько угодно предложений (в т.ч. простых).
             * - Текст корректен тогда, когда корректно каждое его предложение.
             * - Текст корректен тогда, когда пуст.
             */
            set
            {
                /* ==== Проверка каждого отдельного предложения на корректность ==== */

                bool isCorrect = true;
                List<Sentence> newContainedSentences = new List<Sentence>();

                // Разделение предложений с сохранением признака конца предложения и проход по каждому
                Regex regex = new Regex(@"([^(\.|!|\?)]+(\.|!|\?))");

                foreach (Match match in regex.Matches(value))
                {
                    // Удаление лишних пробелов в начале и в конце предложения
                    string strSentence = match.Value.Trim();

                    // Создание предложения для отображения ошибок его обработки
                    Sentence sentence = new Sentence(strSentence);

                    if (Sentence.isCorrect(strSentence))
                    {
                        // Если предложение корректно, то добавление его в список содержащихся предложений
                        newContainedSentences.Add(sentence);
                    }
                    else
                    {
                        // Если нет, то выход из цикла, чтобы не тратить ресурсы на проверку остальных предложений
                        isCorrect = false;
                    }
                }

                // Установка нового значение, если текст пустой или нет, но при этом корректный
                if (value == "" || isCorrect)
                {
                    _instance = value;
                    containedSentences = newContainedSentences;
                } 
            }
        }

        // Конструктор класса
        public Text(string value)
        {
            // Присвоение нового значения, если оно корректно
            this.instance = value;
        }

        public static bool isCorrect(string value)
        {
            /* ==== Проверка каждого отдельного предложения на корректность ==== */

            bool isCorrect = true;
            char[] endSymbols = { '.', '!', '?' };

            foreach (string sentence in Regex.Split(value, @"(.+(\.|!|\?))").Where(
                sentence => !string.IsNullOrEmpty(sentence) && !endSymbols.Contains(sentence[0])
            ).ToList())
            {
                isCorrect &= Sentence.isCorrect(sentence);
            }

            return value == "" || isCorrect;
        }

        public void Erase()
        {
            this._instance = "";
        }
    }
    
    /** Класс, определяющий методы работы с предложением и его обработку (содержит логику определения предложения)
     * 
     *  Предложение по определению не может быть пустым, поэтому в списке наследования нет интерфейса IErasable.
     */
    public class Sentence : LanguageUnit
    {
        // Содержащиеся корректные слова в предложении
        public List<Word> containedWords { get; private set; } = new List<Word>();

        // Публичное поле, через которое происходит обращение к приватному
        public override string instance
        {
            // При получении значения возвращается хранимое предложение
            get => _instance;

            /** При установке значения, новое проверяется на принадлежность единице языка "предложение"
             * В случае успешной проверки, записывается в приватное поле для хранения
             * 
             * Признаки предложения:
             * - Последовательность символов не должна быть пустой (содержит одно или несколько слов).
             * - Последовательность символов начинается с заглавной буквы, кавычки (', ") или цифры.
             * - Последовательность символов заканчивается точкой, вопросительным знаком или восклицательным знаком.
             * - Предложение является корректным, если выполняются все вышеуказанные условия и каждое слово предложения является корректным.
             */
            set
            {
                ErrorHandler errorHandler = new ErrorHandler();
                
                // Если не содержит признак начала предложения (заглавная буква, кавычки или цифра), то предполагается,
                // что передается некорректное предложение
                if (!new Regex("^([A-ZА-ЯЁ]|\'|\"|[0-9])").IsMatch(value))
                {
                    errorHandler.SetError("Неверное начало предложения - \"" + value + "\"");
                }
                // Если не содержит признак конца предложения, то предполагается,
                // что передается некорректное предложение
                else if (!new Regex(@"(\.|!|\?)$").IsMatch(value))
                {
                    errorHandler.SetError("Неверный конец предложения - \"" + value + "\"");
                }

                /* Проверка каждого отдельного слова преддложения на корректность */
                bool isCorrect = true;
                char[] charsToTrimSentences = { ' ', '.', '!', '?' };
                List<Word> newContainedWords = new List<Word>();

                // Разделение слов и проход по каждому
                foreach (string strWord in value.Trim(charsToTrimSentences).Split(' '))
                {
                    // Символы, которые могут обособлять слова
                    char[] charsToTrimWords = { '.', '!', '?', ',', '-', '\"', '\'', '(', ')' };

                    // Удаление обособления
                    string trimmedWord = strWord.Trim(charsToTrimWords);

                    // Создание слова для отображения ошибок его обработки
                    Word word = new Word(strWord);

                    if(Word.isCorrect(trimmedWord))
                    {
                        // Если слово корректно, то добавление его в список содержащихся слов
                        newContainedWords.Add(word);
                    } else
                    {
                        isCorrect = false;
                    }
                }

                // Установка нового значения
                if (isCorrect)
                {
                    _instance = value;
                    containedWords = newContainedWords;
                }
            }
        }

        // Конструктор класса
        public Sentence(string value)
        {
            this.instance = value;
        }

        public static bool isCorrect(string value)
        {
            bool isCorrect = 
                new Regex("^([A-ZА-ЯЁ]|\'|\"|[0-9])").IsMatch(value) &&
                new Regex(@"(\.|!|\?)$").IsMatch(value);

            if (isCorrect)
            {
                foreach (string word in value.Split(' '))
                {
                    char[] charsToTrimWords = { '.', '!', '?', ',', '-', '\"', '\'', '(', ')' };
                    string trimmedWord = word.Trim(charsToTrimWords);

                    isCorrect &= Word.isCorrect(trimmedWord);
                }
            }

            Console.WriteLine("Sentence " + value + ": " + isCorrect);

            return isCorrect;
        }
    }

    /** Класс, определяющий методы работы со словом и его обработку (содержит логику определения слова)
     *  
     *  Слово по определению не может быть пустым, поэтому в списке наследования нет интерфейса IErasable.
     */
    public class Word : LanguageUnit
    {
        // Публичное поле, через которое происходит обращение к приватному
        public override string instance
        {
            // При получении значения возвращается хранимое слово
            get => _instance;

            /** При установке значения, новое проверяется на принадлежность единице языка "слово"
             * В случае успешной проверки, записывается в приватное поле для хранения
             * 
             * Признаки слова:
             * - Последовательность символов должна быть не пустой.
             * - Последовательность символов содержит только буквы.
             * - Последовательность символов может содержать как строчные, так и заглавные буквы.
             * - Слово является корректным тогда и только тогда, когда выполняются все вышеуказанные условия.
             */
            set
            {
                ErrorHandler errorHandler = new ErrorHandler();

                // Если содержит признак конца предложения, то предполагается, что происходит попытка присвоения
                // предложения слову. На признак начала предложения проверка не нужна
                if (new Regex(@"\.|!|\?").IsMatch(value))
                {
                    errorHandler.SetError("Попытка присвоить предложение слову - \"" + value + "\"");
                }
                // Если содержит пробел, табуляцию и тд, то предполагается, что происходит попытка присвоения словосочетания слову
                else if (new Regex(@"\s").IsMatch(value))
                {
                    errorHandler.SetError("Попытка присвоить словосочетание слову - \"" + value + "\"");
                }
                // Если содержит какой-либо символ кроме букв, то предполагается, что передается некорректное слово
                else if(!new Regex(@"^[A-Za-zА-ЯЁа-яё]+$").IsMatch(value)) // u
                {
                    errorHandler.SetError("Попытка присвоить некорректное слово - \"" + value + "\"");
                } else
                {
                    _instance = value;
                }
            }
        }

        // Конструктор класса
        public Word(string value)
        {
            this.instance = value;
        }

        public static bool isCorrect(string value)
        {
            return 
                !new Regex(@"\.|!|\?").IsMatch(value) &&
                !new Regex(@"\s").IsMatch(value) &&
                new Regex(@"^[A-Za-zА-ЯЁа-яё]+$").IsMatch(value);
        }
    }
}
