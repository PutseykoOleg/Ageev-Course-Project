namespace TextLibrary
{
    // Интерфейс, описывающий  объект (можно применить не только к единицам языка)
    // Интерфейс, описывающий исправляемый объект (можно применить не только к единицам языка)
    public interface ICorrectable
    {
        void SetWithCorrecting(string value);
    }
}
