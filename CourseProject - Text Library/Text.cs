using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TextLibrary
{
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
            if (!new Regex(@"(\.|!|\?)$").IsMatch(correctValue))
            {
                correctValue = correctValue + '.';
            }

            // Очистка содержащихся предложений
            containedSentences.Clear();
            // Исправление каждого предложения в тексте
            correctValue = Regex.Replace(correctValue, @"[^(\.|!|\?)]*(\.|!|\?)+", CorrectSentence);

            // Добавление пробела после символов окончания строки
            correctValue = Regex.Replace(correctValue, @"((\.|!|\?)+)([^(\s|\.|!|\?)]+)", (Match match) => match.Groups[1].Value + " " + match.Groups[3].Value);

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

        private string CorrectSentence(Match match) {
            // Исправление текущего предложения
            Sentence sentence = new Sentence();
            sentence.SetWithCorrecting(match.Value);

            // Если успешно найдено и исправлено
            if (!sentence.IsEmpty())
            {
                // Добавить исправленное предложение в список содержащихся
                containedSentences.Add(sentence);
                // Замена предложения в исходном тексте на исправленное
                return sentence.instance;
            }

            // Если нет, то удалить предложение их исходного текста
            return "";
        }
    }
}
