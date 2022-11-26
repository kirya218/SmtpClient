namespace SMTP.Interfaces
{
    public interface ICommands
    {
        string CommandHelo();
        string CommandEhlo();
        string CommandMailFrom(string messageClient);
        string CommandRcptTo(string messageClient);
        string CommandData(string messageClient);
    }
}
