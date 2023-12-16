namespace SyndiMessage.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class BindingDeclarationAttribute : Attribute
{
    public bool Durable { get; private set; }
    public bool Exclusive { get; private set; }
    public bool AutoDelete { get; private set; }
    public bool AutoAck { get; private set; }
    public BindingDeclarationAttribute(bool durable = true, bool exclusive = false, bool autoDelete = false, bool autoAck = false)
    {
        Durable = durable;
        Exclusive = exclusive;
        AutoDelete = autoDelete;
        AutoAck = autoAck;
    }
}
