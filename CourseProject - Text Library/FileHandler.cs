using System;
using System.IO;

namespace TextLibrary
{
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
}
