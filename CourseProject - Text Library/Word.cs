using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextLibrary
{

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
