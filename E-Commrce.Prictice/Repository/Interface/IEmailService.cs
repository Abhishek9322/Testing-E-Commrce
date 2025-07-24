namespace E_Commrce.Prictice.Repository.Interface
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string bodyHtml);
    }
}
