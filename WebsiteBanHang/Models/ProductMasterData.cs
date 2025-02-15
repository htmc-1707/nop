using System.Web.Mvc;

namespace WebsiteBanHang.Models
{
    public partial class ProductMasterData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }

        [AllowHtml]
        public string Description { get; set; }

        public string Images { get; set; }
        public string Details { get; set; }
    }
}
