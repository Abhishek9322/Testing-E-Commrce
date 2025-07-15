namespace E_Commrce.Prictice.Iinterface
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string bodyHtml);
    }
}
