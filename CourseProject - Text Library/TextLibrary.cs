﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextLibrary
{
    // Класс, определяющий обработчик ошибки
    public class ErrorHandler
    {
        /**
         * Форма для взаимодействия
         * 
         * Отношение агрегации (ErrorHandler - MainForm)
         */
        public static MainForm form { private get; set; } = null;

        // Необходимость логирования (включение или выключение обработчика)
        public static bool log { get; set; } = true;

        public static string currentError { get; private set; } = null;

        // Делегат для события Error
        public delegate void Handler(string message);

        // Событие, обозначающее ошибку
        public event Handler Error;

        public ErrorHandler()
        {
            // Добавление непосредственного обработчика события
            Error += ShowMessage;
        }

        // Установка сообщения об ошибке
        public void SetError(string message)
        {
            currentError = message;

            if (log)
            {
                // Вызов непосредственного обработчика событий
                Error?.Invoke(message);
            }
        }

        public static void UnsetError()
        {
            currentError = null;
        }

        public static bool HasError()
        {
            return !String.IsNullOrEmpty(currentError);
        }

        // Непосредственный обработчик события
        private void ShowMessage(string message)
        {
            if (form != null)
            {
                // Демонстрация ошибки в форме
                form.ShowError("Ошибка: " + message);
            }
        }
    }

    // Абстрактный класс, определяющий единицу языка (его необходимую структуру)
    public abstract class LanguageUnit
    {
        // Защищенное поле, хранящее корректное значение единицы языка
        protected string _instance = null;

        // Публичное абстрактное поле, через которое происходит обращение и обработка хранимого значения
        public abstract string instance { get; set; }
    }

    // Интерфейс, описывающий стираемый объект (можно применить не только к единицам языка)
    public interface IErasable {
        void Erase();

        bool IsEmpty();
    }

    // Интерфейс, описывающий  объект (можно применить не только к единицам языка)
    // Интерфейс, описывающий исправляемый объект (можно применить не только к единицам языка)
    public interface ICorrectable
    {
        void SetWithCorrecting(string value);
    }

    // Класс, определяющий методы работы с файлом и представляемыми библиотекой классами единиц языка
    public class FileHandler
    {

        /**
         * Метод, считывающий слово, предложение или текст целиком из файла в указанный объект
         * 
         * path - путь до файла считывания
         * targetObj - целевой объект считывания, в случае успешного чтения ему будет присвоено соответствующее значение
         * index - индекс целевого объекта считывания (в случае если уелевой объект - текс, индекс будет проигнорирован),
         *         необходим, т.к. чтение каждый раз происходит сначала файла.
         * 
         */
        public static async void ReadFromFile(string path, LanguageUnit targetObj, int index = 0)
        {
            // Обработчик ошибок
            ErrorHandler errorHandler = new ErrorHandler();
            // Считанный текст
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
                // Если вохзникла ошибка чтения файла, вывести ее и выйти из метода
                errorHandler.SetError("Ошибка чтения файла \"" + path + "\": " + e.Message);
                return;
            }

            // Создание объекта текста от считанного вместе с проверкой ошибок обработки единиц языка
            Text text = new Text(strText);

            // Возвращение соответствующей части текста
            if(targetObj is Word)
            {
                if (!String.IsNullOrEmpty(strText))
                {
                    // Общее количество слов в считанных предложениях
                    int countOfWords = 0;

                    // Проход по каждому считанному предложению
                    for (int i = 0; i < text.containedSentences.Count; i++)
                    {
                        // Считанное предложение
                        Sentence sentence = text.containedSentences[i];
                        countOfWords += sentence.containedWords.Count;

                        // Если индекс нужного слова находится в текущем предложении
                        if (index < countOfWords)
                        {
                            // Проход по всем словам текущего предложения
                            for (int j = 0; j < sentence.containedWords.Count; j++)
                            {
                                // Считанное слово
                                Word word = sentence.containedWords[j];

                                // Если слово найдено, то присвоить его переданному значению
                                if (index == countOfWords - sentence.containedWords.Count + j)
                                {
                                    // Отношение ассоциации (FileHandler - Word)
                                    targetObj.instance = word.instance;
                                }
                            }
                        }
                    }

                    // Если переданный индекс больше суммы слов во всех предложениях, то вызвать ошибку
                    if (index >= countOfWords && !ErrorHandler.HasError())
                    {
                        errorHandler.SetError("Достигнут конец файла, слово не было считано");
                    }
                } else
                {
                    errorHandler.SetError("Считанный из файла текст не содержит предложений (нет слов для считывания)");
                }
            } 
            else if(targetObj is Sentence)
            {
                if (!String.IsNullOrEmpty(strText))
                {
                    // Проход по каждому считанному предложению
                    for (int i = 0; i < text.containedSentences.Count; i++)
                    {
                        // Считанное предложение
                        Sentence sentence = text.containedSentences[i];

                        // Если предложение найдено, то присвоить его переданному значению
                        if (index == i)
                        {
                            // Отношение ассоциации (FileHandler - Sentence)
                            targetObj.instance = sentence.instance;
                        }
                    }

                    // Если переданный индекс больше суммы слов во всех предложениях, то вызвать ошибку
                    if (index >= text.containedSentences.Count && !ErrorHandler.HasError())
                    {
                        errorHandler.SetError("Достигнут конец файла, предложение не было считано");
                    }
                }
                else
                {
                    errorHandler.SetError("Считанный из файла текст не содержит предложений");
                }
            }
            else if (targetObj is Text)
            {
                // Если текст не пуст, то присвоить его переданному значению 
                if (!String.IsNullOrEmpty(strText))
                {
                    // Отношение ассоциации (FileHandler - Text)
                    targetObj.instance = text.instance;
                } else
                {
                    errorHandler.SetError("Нет текста для считывания, файл \"" + path + "\" пуст");
                }
            }
        }

        public static async void WriteToFile(string path, LanguageUnit targetObj)
        {
            if(!ErrorHandler.HasError() && (targetObj is Word || targetObj is Sentence || targetObj is Text))
            {
                try
                {
                    using(StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
                    {
                        sw.Write(targetObj.instance);
                    }
                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }

    /** 
     * Класс, определяющий методы работы с текстом и его обработку (содержит логику определения текста)
     * 
     * Отношение наследования (Text - LanguageUnit)
     * Отношение реализации (Text - IErasable)
     */
    public class Text : LanguageUnit, IErasable, ICorrectable
    {
        /** 
         * Содержащиеся корректные предложения в тексте
         * 
         * Отношение композиции (Text - Sentence)
         */
        public List<Sentence> containedSentences { get; private set; } = new List<Sentence>();

        // Публичное поле, через которое происходит обращение к приватному
        public override string instance
        {
            // При получении значения возвращается хранимый текст
            get => _instance;

            /** 
             * При установке значения, новое проверяется на принадлежность единице языка "текст"
             * В случае успешной проверки, записывается в приватное поле для хранения
             * 
             * Признаки текста:
             * - Последовательность символов может быть пустой.
             * - Последовательность символов может содержать сколько угодно предложений (в т.ч. простых).
             * - Текст корректен тогда, когда корректно каждое его предложение.
             */
        set
            {
                // Список найденных корректных предложений в тексте
                List<Sentence> newContainedSentences = new List<Sentence>();
                // Корректность предложения
                bool isCorrect = true;

                /** 
                 * Разделение предложений с сохранением признака конца предложения и проход по каждому
                 * 
                 * Для разделения предложений, происходит их поиск при помощи регулярного выражения
                 * Т.к. нам важно сохранить все, что мы описываем в регулярном выражении в качестве найденного предложения,
                 * оборачиваем все в скобки. Т.е. происходит поиск без контекста. (В регулярном выражении "a(b)c" a и c - контекст, т.е.
                 * в итоге Match.Value созранится только b, причем только та у которой слева была "a", а справа "c".)
                 * 
                 * [^(\.|!|\?)]+ - означает наличие [^(\.|!|\?)] 1 или более раз
                 * [^(\.|!|\?)] - означает "не (\.|!|\?)"
                 * (\.|!|\?) - означает наличие какого-либо символа из следующих: ".", "!", "?"
                 * 
                 * Итого это регулярное выражение описывает строки вида:
                 * - "aaa."
                 * - "aaa!."
                 * - "Aaa ?!."
                 * - и тд.
                 * И исключает строки вида:
                 * - "Aaa. Bbb."
                 * - "aaa!.bbb."
                 * - и тд.
                 * 
                 * Т.е. происходит выбор потенциальных одиночных предложений (в состав которых не входят другие предложения)
                 * 
                 * Match - результат совпадения
                 */
                foreach (Match match in new Regex(@"([^(\.|!|\?)]+(\.|!|\?)+)").Matches(value))
                {
                    // Удаление лишних пробелов в начале и в конце предложения
                    string strSentence = match.Value.Trim();

                    /** 
                     * Создание предложения для вызова ошибок его обработки, если они есть (ошибки возникают при присвоении значения)
                     * Здесь происходит проверка корректности содержащихся в предложении слов и вызов ошибок их обработки, если они есть
                     */
                    Sentence sentence = new Sentence(strSentence);

                    // Если значение было установлено успешно (если предложение корректно)
                    if (!sentence.IsEmpty())
                    {
                        // Добавление его в список содержащихся предложений
                        newContainedSentences.Add(sentence);
                    }
                    else
                    {
                        // Если нет, установка соответствующего флага
                        isCorrect = false;
                    }
                }

                // Установка нового значения в зависимости от корректности текста
                if (value == "" || isCorrect)
                {
                    _instance = value;
                    containedSentences = newContainedSentences;
                } else
                {
                    Erase();
                }
            }
        }

        // Конструктор по умолчанию
        public Text()
        {
            // Выключение логирования ошибок
            ErrorHandler.log = false;
            // Установка пустого значения
            Erase();
            // Включение логирования ошибок
            ErrorHandler.log = true;
        }

        // Конструктор класса
        public Text(string value)
        {
            /** 
             * Присвоение значения ("value" если оно корректно и "" - если нет)
             * 
             * Вместе с этим вызываются ошибки, если они есть
             */
            this.instance = value;
        }

        /** 
         * Метод проверки на содержание значения
         * 
         * Отражает корректность текущего значения - текст корректен, если не пуст
         */
        public bool IsEmpty()
        {
            return String.IsNullOrEmpty(_instance);
        }

        // Метод стирающий значение текста
        public void Erase()
        {
            this._instance = null;
            containedSentences.Clear();
        }

        // Метод устанавливающий в качестве нового значения откорректированное переданное
        public void SetWithCorrecting(string value)
        {
            // Исправленное значение
            string correctValue = value.Trim();
            // Список содержащихся предложений в исправленном тексте
            List<Sentence> newContainedSentences = new List<Sentence>();

            // Добавление символа конца предложения (если его нет) в конец текста
            if(!new Regex(@"(\.|!|\?)$").IsMatch(correctValue))
            {
                correctValue = correctValue + '.';
            }

            // Исправление каждого предложения в тексте
            correctValue = Regex.Replace(correctValue, @"[^(\.|!|\?)]+(\.|!|\?)+", (Match match) =>
            {
                // Исправление текущего предложения
                Sentence sentence = new Sentence();
                sentence.SetWithCorrecting(match.Value);

                // Если успешно найдено и исправлено
                if(!sentence.IsEmpty())
                {
                    // Добавить исправленное предложение в список содержащихся
                    newContainedSentences.Add(sentence);
                    // Замена предложения в исходном тексте на исправленное
                    return sentence.instance;
                }

                // Если нет, то удалить предложение их исходного текста
                return "";
            });

            // Добавление пробела после символов окончания строки
            correctValue = Regex.Replace(correctValue, @"(\.|!|\?)+([^\s])", (Match match) => match.Groups[1].Value + " " + match.Groups[2].Value);

            // Удаление пустого предложения вначале строки, если такое возникло
            correctValue = Regex.Replace(correctValue, @"^(\.|!|\?)+\s*", (Match match) => "");

            // Если в тексте есть хотя бы одно исправленное предложение, т.е. если он не пуст
            if (!String.IsNullOrEmpty(correctValue))
            {
                // Установка нового значения
                _instance = correctValue;
                containedSentences = newContainedSentences;
            } else
            {
                Erase();
            }
        }
    }

    /** 
     * Класс, определяющий методы работы с предложением и его обработку (содержит логику определения предложения)
     * 
     * Отношение наследования (Sentence - LanguageUnit)
     * Отношение реализации (Sentence - IErasable)
     */
    public class Sentence : LanguageUnit, IErasable, ICorrectable
    {
        /** Содержащиеся корректные слова в предложении
         * 
         * Отношение композиции (Sentence - Word)
         */
        public List<Word> containedWords { get; private set; } = new List<Word>();

        // Публичное поле, через которое происходит обращение к приватному (_instance)
        public override string instance
        {
            // При получении значения возвращается хранимое предложение
            get => _instance;

            /** 
             * При установке значения, новое проверяется на принадлежность единице языка "предложение"
             * В случае успешной проверки, записывается в приватное поле для хранения
             * 
             * Признаки предложения:
             * - Последовательность символов не должна быть пустой (содержит одно или несколько слов).
             * - Последовательность символов начинается с заглавной буквы, кавычки (', ") или цифры.
             * - Последовательность символов заканчивается точкой, вопросительным знаком или восклицательным знаком.
             * - Последовательность символов содержит логическое окончание, т.е. не содержит незавершенных конструкций вида:
             *   - "Aaa,."
             *   - "Aaa (."
             *   - и тд.
             * - Предложение является корректным, если выполняются все вышеуказанные условия и каждое слово предложения является корректным.
             */
        set
            {
                // Обработчик ошибок
                ErrorHandler errorHandler = new ErrorHandler();
                // Список найденный корректных слов в предложении
                List<Word> newContainedWords = new List<Word>();
                // Корректность предложения
                bool isCorrect = true;

                /** 
                 * Если не содержит признак начала предложения (заглавная буква, кавычки или цифра), то предполагается,
                 * что передается некорректное предложение
                 * 
                 * ^ в начале - означает, что вся эта конструкция находится в начале строки
                 * [A-ZА-ЯЁ] - означает любую заглавную букву русского или английского алфавита
                 * \' - означает '
                 * \" - означает "
                 * [0-9] - означает любую цифру
                 * 
                 * Итого все это выражение означает что в начале строки должна быть либо заглавная буква, либо кавычка, либо цифра.
                 */
                if (!new Regex("^([A-ZА-ЯЁ]|\'|\"|[0-9])").IsMatch(value))
                {
                    errorHandler.SetError("Неверное начало предложения - \"" + value + "\"");
                    isCorrect = false;
                }
                /** 
                 * Если не содержит признак конца предложения или содержит нелогическое его окончание, то предполагается,
                 * что передается некорректное предложение
                 * 
                 * ^ в начале - означает что вся эта конструкция прижата к началу строки
                 * $ в конце - означает что вся эта конструкция прижата к концу строки
                 * 
                 * .* - означает любой символ 
                 *    Т.е. в начале предложения может идти что угодно, далее это будет проверяться непосредственно в классе предложения и слова
                 *     
                 * В [^(((,|\-|\(|\[|{|\s)(?=(\.|!|\?)))+)?]+ знак "(...)+" означает наличие "..." 1 или более раз
                 *    В [^(((,|\-|\(|\[|{|\s)(?=(\.|!|\?)))+)?] "[^...]" означает "не ..."
                 *    В (((,|\-|\(|\[|{|\s)(?=(\.|!|\?)))+)? знак "(...)?" в конце означает что "..." может присутствовать, а может и нет
                 *    В ((,|\-|\(|\[|{|\s)(?=(\.|!|\?)))+ знак "(...)+" означает наличие "..." 1 или более раз
                 *    В (,|\-|\(|\[|{|\s)(?=(\.|!|\?)) первые скобки "(,|\-|\(|\[|{|\s)" означают какой-либо символ из ",", "-", "(", "[", "{", " ", "    "
                 *    В (,|\-|\(|\[|{|\s)(?=(\.|!|\?)) вторые скобки "(?=(\.|!|\?))" означают что первые присутствуют только если за ними идут либо ".", либо "!", либо "?"
                 *    
                 *    Т.е. перед окончанием предложения не может быть ",", "-", "(", "[", "{", " ", "    "
                 *    
                 * (\.|!|\?)+ - означает наличие какого-либо символа из следующих: ".", "!", "?" 1 или более раз
                 * 
                 *     Т.е. в конце предложения должны быть знаки ".", "!" или "?"
                 * 
                 * Итого это регулярное выражение описывает строки вида:
                 * - "aaaa bbb."
                 * - "aaaa bbb!"
                 * - "Aaaa bbb?"
                 * - "Baaa bbb?!..."
                 * - "aaaa bbb?!.!."
                 * - и тд.
                 * И исключает строки вида:
                 * - "aaaa bbb"
                 * - "aaaa bbb ."
                 * - "aaaa bbb,"
                 * - "aaaa bbb,."
                 * - "aaaa bbb -."
                 * - "Aaaa bbb (."
                 * - "Aaaa bbb [ !"
                 * - и тд.
                 * 
                 * x(?=y)
                 * 
                 * aaa b aaa c
                 */
                else if (!new Regex(@"(.*[^(((,|\-|\(|\[|{|\s)(?=(\.|!|\?)))+)]+(\.|!|\?)+)$").IsMatch(value))
                {
                    errorHandler.SetError("Неверный конец предложения - \"" + value + "\"");
                    isCorrect = false;
                }

                // Если предложение пока (некорректные слова делаю предложение так же некорректным) корректно, то производится поиск слов в нем
                if (isCorrect)
                {
                    /** 
                     * Проверка каждого отдельного слова преддложения на корректность 
                     *
                     * Сейчас у нас есть предложение вида "Aaaa bbb!." (может и любого другого корректного).
                     * Но оно может содержать пробелы после окончания или перед началом.
                     * Поэтому вызываем метод Trim, а после Split для выделения отдельных слов и прохождения по каждому.
                     * 
                     * Разделение слов происходит пробелом, тем самым выделяя слова вида:
                     * - "aaa"
                     * - "aaa,"
                     * - "(aaa)"
                     * - "aaa-bbb"
                     * - " aaa   "
                     * - "aaa." (последнее слово предложения)
                     * - и др.
                     */
                    foreach (string strWord in value.Trim().Split(' '))
                    {
                        // Удаление лишних символов по краям слова
                        char[] charsToTrimWords = { ' ', '.', '!', '?', ',', '-', '\"', '\'', '(', ')' };
                        string trimmedWord = strWord.Trim(charsToTrimWords);

                        // Исключение пустых слов, которые могли возникнуть при разделении
                        if (!String.IsNullOrEmpty(trimmedWord))
                        {
                            // Создание слова для вызова ошибок его обработки, если они есть (ошибки возникают при присвоении значения)
                            Word word = new Word(trimmedWord);

                            // Если значение было установлено успешно (если слово корректно)
                            if (!word.IsEmpty())
                            {
                                // Добавление его в список содержащихся слов данного предложения
                                newContainedWords.Add(word);
                            }
                            else
                            {
                                // Если не корректно, устанавливаем соответствующий флаг
                                isCorrect = false;
                            }
                        }
                    }
                }

                // Установка нового значения в зависимости от корректности предложения
                if (isCorrect)
                {
                    _instance = value;
                    containedWords = newContainedWords;
                } else
                {
                    _instance = null;
                    containedWords.Clear();
                }
            }
        }

        // Конструктор по умолчанию
        public Sentence()
        {
            // Выключение логирования ошибок
            ErrorHandler.log = false;
            // Установка пустого значения
            Erase();
            // Включение логирования ошибок
            ErrorHandler.log = true;
        }

        // Конструктор класса
        public Sentence(string value)
        {
            /** 
             * Присвоение значения ("value" если оно корректно и "null" - если нет)
             * 
             * Вместе с этим вызываются ошибки, если они есть
             */
            this.instance = value;
        }

        /** 
         * Метод проверки на содержание значения
         * 
         * Отражает корректность текущего значения - предложение корректно, если не пусто
         */
        public bool IsEmpty()
        {
            return String.IsNullOrEmpty(_instance);
        }

        // Метод стирающий значение предложения
        public void Erase()
        {
            this._instance = null;
            containedWords.Clear();
        }

        // Метод устанавливающий в качестве нового значения откорректированное переданное
        public void SetWithCorrecting(string value)
        {
            // Исправленное значение, инициализируем, сразу убирая лишние пробелы по краям, чтобы потом не тратить ресурсы на это
            string correctValue = value.Trim();

            // Содержащиеся в исправленном предложении слова
            List<Word> newContainedWords = new List<Word>();

            /** 
             * Если не содержит признак начала предложения (заглавная буква, кавычки или цифра), то предполагается,
             * что передается некорректное предложение
             */
            if (!new Regex("^([A-ZА-ЯЁ]|\'|\"|[0-9])").IsMatch(correctValue))
            {
                /** 
                 * Если первая буква не заглавная, заменить ее заглавной
                 * 
                 * Это единственное что мы можем исправить в текущем признаке, т.к. добавлять что-то в предложение нельзя,
                 * а заменять или удалять можно
                 */
                if (new Regex("^[a-zа-яё]").IsMatch(correctValue))
                {
                    correctValue = correctValue.Substring(0, 1).ToUpper() + (correctValue.Length > 1 ? correctValue.Substring(1) : "");
                }
                // Если мы не можем исправить предложение - стираем его
                else
                {
                    correctValue = "";
                }
            }

            /** 
             * Если не содержит признак конца предложения или содержит нелогическое его окончание, то предполагается,
             * что передается некорректное предложение
             * 
             * Проверяем сразу после предыдущей проверки (без else), т.к. они не взаимосвязаны (нет перекрытия ошибки)
             */
            if (!new Regex(@"(.*[^(((,|\-|\(|\[|{|\s)(?=(\.|!|\?)))+)]+(\.|!|\?)+)$").IsMatch(correctValue))
            {
                /**
                 * Если предложение без заключающего знака препинания - добавить точку (по умолчанию)
                 */
                if (!new Regex(@"(\.|!|\?)$").IsMatch(correctValue))
                {
                    correctValue = correctValue + '.';
                }

                /**
                 * Сразу проверяем дальше
                 * 
                 * Если перед заключающим знаком препинания есть какой-либо ненужный символ (нелогичное окончание предложения),
                 * убрать этот символ
                 * 
                 * Делаем это во вложенном цикле, чтобы удалить все такие символы
                 */
                while (new Regex(@"(,|\-|\(|\[|{|\s)(\.|!|\?)$").IsMatch(correctValue))
                {
                    correctValue = correctValue.Substring(0, correctValue.Length - 2) + correctValue[correctValue.Length - 1];
                }
            }

            // Если в значении несколько предложений, то оставить только последнее
            char[] endOfSentence = { '.', '!', '?' };
            correctValue = correctValue.Substring(0, correctValue.IndexOfAny(endOfSentence) + 1);

            // Добавление пробелов после тех знаков, после которых это необходимо
            correctValue = Regex.Replace(correctValue, @"(,|\)|\]|})([^\s])", (Match match) => match.Groups[1].Value + " " + match.Groups[2].Value);

            // Добавление пробелов перед теми знаками, перед которыми это необходимо
            correctValue = Regex.Replace(correctValue, @"([^\s])(\(|\[|{)", (Match match) => match.Groups[1].Value + " " + match.Groups[2].Value);

            // Замена нескольких идущих подряд отдельных символов пробелом
            correctValue = Regex.Replace(correctValue, @"\s+(\(|\)|\[|\]|{|}|'|""|,|\.|!|\?)*\s+", (Match match) => " ");

            // Исправление слов
            correctValue = Regex.Replace(correctValue, @"[^(\s|\(|\)|\[|\]|{|}|'|""|,|\.|!|\?)]+", (Match match) => {
                // Создание слова без ошибок
                Word word = new Word();
                word.SetWithCorrecting(match.Value);

                // Если значение было установлено успешно (если слово корректно)
                if (!word.IsEmpty())
                {
                    // Добавление слова в список содержащихся слов данного предложения
                    newContainedWords.Add(word);

                    // Замена соответствующего слова в исходной строке
                    return word.instance;
                }

                // Удаление слова из исходного строки
                return "";
            });

            // Если непосредственное содрежимое предложения отсутствует, удалить его
            if (new Regex(@"^(\.|!|\?)+$").IsMatch(correctValue)) correctValue = "";

            // Установление верного значения
            if (!String.IsNullOrEmpty(correctValue))
            {
                _instance = correctValue;
                containedWords = newContainedWords;
            } else
            {
                Erase();
            }
        }
    }

    /** 
     * Класс, определяющий методы работы со словом и его обработку (содержит логику определения слова)
     * 
     * Отношение наследования (Word - LanguageUnit)
     * Отношение реализации (Word - IErasable)
     */
    public class Word : LanguageUnit, IErasable, ICorrectable
    {
        // Публичное поле, через которое происходит обращение к приватному
        public override string instance
        {
            // При получении значения возвращается хранимое слово
            get => _instance;

            /** 
             * При установке значения, новое проверяется на принадлежность единице языка "слово"
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
                // Обработчик ошибок
                ErrorHandler errorHandler = new ErrorHandler();

                /**
                 * Проверка слова на корректность
                 * 
                 * ^ в начале - означает что вся эта конструкция прижата к началу строки
                 * $ в конце - означает что вся эта конструкция прижата к концу строки
                 * 
                 * В ([A-Za-zА-ЯЁа-яё]+\-)? знак "(...)?" означает, что "..." может присутствовать, а может и нет
                 * 
                 *     [A-Za-zА-ЯЁа-яё]+\- - означает наличие любой буквы русского или английского алфавита 1 или более раз, после которых идет "-"
                 *     
                 *     Т.е. начало слова может выглядеть так: "aaa-" (слова-приложения)
                 *     
                 * [A-Za-zА-ЯЁа-яё]+ - означает наличие любой буквы русского или английского алфавита 1 или более раз
                 * 
                 *     Т.е. конец слова может выглядеть так: "bbb"
                 *     
                 * Тем самым описаны слова вида:
                 * - "Abc"
                 * - "AbC"
                 * - "Абв"
                 * - "Abc-def"
                 * - "Abc-абв"
                 */
                if (!new Regex(@"^([A-Za-zА-ЯЁа-яё]+\-)?[A-Za-zА-ЯЁа-яё]+$").IsMatch(value))
                {
                    errorHandler.SetError("Некорректное слово - \"" + value + "\"");
                    Erase();
                } else
                {
                    // Установление верного значения
                    _instance = value;
                }
            }
        }

        // Конструктор по умолчанию
        public Word()
        {
            // Выключение логирования ошибок
            ErrorHandler.log = false;
            // Установка пустого значения
            Erase();
            // Включение логирования ошибок
            ErrorHandler.log = true;
        }

        // Конструктор класса
        public Word(string value)
        {
            this.instance = value;
        }

        /** 
         * Метод проверки на содержание значения
         * 
         * Отражает корректность текущего значения - слово корректно, если не пусто
         */
        public bool IsEmpty()
        {
            return String.IsNullOrEmpty(_instance);
        }

        // Метод стирания значения
        public void Erase()
        {
            _instance = null;
        }

        // Метод устанавливающий в качестве нового значения откорректированное переданное
        public void SetWithCorrecting(string value)
        {
            // Исправленное значение, инициализируем, сразу убирая лишние пробелы по краям, чтобы потом не тратитть ресурсы на это
            string correctValue = value.Trim();

            /**
             * Проверка слова на корректность
             */
            if (!new Regex(@"^([A-Za-zА-ЯЁа-яё]+\-)?[A-Za-zА-ЯЁа-яё]+$").IsMatch(correctValue))
            {
                // Если значение - слово-приложение
                if(new Regex(@"^.+\-.+$").IsMatch(correctValue))
                {
                    // Ищем составные слова
                    MatchCollection matches = new Regex(@"([^\-]+)").Matches(correctValue);

                    // Убрать лишние символы и вместе с тем оставить только два первых составных слова
                    correctValue = GetOnlyLettersFromString(matches[0].Value) + "-" + GetOnlyLettersFromString(matches[1].Value);
                } 
                // Если значение - просто набор символов
                else {
                    correctValue = GetOnlyLettersFromString(correctValue);
                }
            }

            // Установление верного значения
            if (!String.IsNullOrEmpty(correctValue))
            {
                _instance = correctValue;
            } else
            {
                Erase();
            }
        }

        // Метод, удаляющий из строки все символы отличные от букв
        private string GetOnlyLettersFromString(string value)
        {
            string result = "";

            foreach(char ch in value)
            {
                if (Char.IsLetter(ch))
                {
                    result += ch;
                }
            }

            return result;
        }
    }
}
