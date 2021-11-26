namespace TextLibrary
{
    // Абстрактный класс, определяющий единицу языка (его необходимую структуру)
    public abstract class LanguageUnit
    {
        // Защищенное поле, хранящее корректное значение единицы языка
        protected string _instance = null;

        // Публичное абстрактное поле, через которое происходит обращение и обработка хранимого значения
        public abstract string instance { get; set; }
    }
}
