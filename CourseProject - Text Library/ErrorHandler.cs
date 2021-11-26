using System;

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
}
