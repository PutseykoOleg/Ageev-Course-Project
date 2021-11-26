namespace TextLibrary
{
    // Интерфейс, описывающий стираемый объект (можно применить не только к единицам языка)
    public interface IErasable {
        void Erase();

        bool IsEmpty();
    }
}
