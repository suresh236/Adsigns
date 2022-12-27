
namespace Vestcom.PaperSign.ACME.Entities
{
    public class SignLayout
    {
        public int SIGN_LAYOUT_ID { get; set; }
        public int SignSizeId { get; set; }
        public int SignHeaderId { get; set; }
        public string SL_SIGN_SIZE { get; set; }
        public string SL_SIGN_HEADING { get; set; }
        public string SL_LAYOUT_NO { get; set; }
        public string SL_PAPER_TYPE { get; set; }
    }
}
