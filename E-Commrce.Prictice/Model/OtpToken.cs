namespace E_Commrce.Prictice.Model
{
    public class OtpToken
    {
        public int Id { get; set; }
        public string Email {  get; set; }
        public string Code {  get; set; }
        public DateTime ExpiryTime {  get; set; }
    }
}
